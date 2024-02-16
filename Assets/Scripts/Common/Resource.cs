using DG.Tweening;
using FMODUnity;
using Quinn.Player;
using Quinn.WorldGeneration;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn
{
	public class Resource : MonoBehaviour, IInteractable
	{
		[field: SerializeField]
		public InteractionType InteractionType { get; private set; }

		[SerializeField]
		private float MaxDistance = 0.3f;

		[SerializeField]
		private int HitPoints = 16;

		[SerializeField, MinValue(0f), MaxValue(1f)]
		private float SpawnChance = 0.35f;

		[SerializeField]
		private ResourceHealthStage[] HealthStages;

		[SerializeField]
		private ResourceHarvestSpawnEntry[] SpawnEntries;

		[SerializeField, BoxGroup("Sounds")]
		private EventReference ItemHitGroundSound, ItemHitWaterSound;

		[SerializeField, BoxGroup("Sounds")]
		private EventReference HitSound, DestroySound;

		private SpriteRenderer _renderer;
		private int _hitPoints;

		private void Awake()
		{
			_renderer = GetComponent<SpriteRenderer>();
			_hitPoints = HitPoints;

			transform.rotation = Quaternion.Euler(0f, Random.value < 0.5f ? -180f : 0f, 0f);
		}

		private void OnDestroy()
		{
			transform.DOKill();
		}

		public Vector2 InteractPoint => transform.position;
		public float InteractDistance => MaxDistance;

		public void Interact(GameObject player)
		{
			var inventory = player.GetComponent<InventoryManager>();
			var heldItem = inventory.HeldItem;

			if (heldItem != null && heldItem.InteractionType == InteractionType)
			{
				_hitPoints--;
				OnDamage();

				if (_hitPoints == 0)
				{
					OnKill();
				}
			}
		}

		private void OnDamage()
		{
			if (!HitSound.IsNull)
			{
				RuntimeManager.PlayOneShot(HitSound, transform.position);
			}

			if (Random.value < SpawnChance)
			{
				SpawnRandomItem();
			}

			transform.DOScale(0.85f, 0.1f).onComplete += () =>
			{
				transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
			};

			float percent = (float)_hitPoints / HitPoints;
			foreach (var stage in HealthStages)
			{
				if (percent < stage.HealthPercent)
				{
					_renderer.sprite = stage.Sprite;
					break;
				}
			}
		}

		private void OnKill()
		{
			if (!DestroySound.IsNull)
			{
				RuntimeManager.PlayOneShot(DestroySound, transform.position);
			}

			for (int i = 0; i < 3; i++)
			{
				SpawnRandomItem();
			}

			Destroy(gameObject);
		}

		private void SpawnRandomItem()
		{
			int index = Random.Range(0, SpawnEntries.Length - 1);
			var entry = SpawnEntries[index];

			var target = transform.position;
			target += Random.insideUnitSphere * 2f;

			const string key = "PhysicalItem.prefab";
			Addressables.InstantiateAsync(key, transform.position, Quaternion.identity).Completed += handle =>
			{
				var instance = handle.Result;

				var collider = instance.GetComponent<Collider2D>();
				collider.enabled = false;

				instance.GetComponent<PhysicalItem>().Set(new Slot()
				{
					Item = entry.Item,
					Count = Random.Range(entry.MinCount, entry.MaxCount),
					RemainingUses = entry.Item.MaxUses
				});

				instance.transform.DOJump(target, Random.Range(1f, 2f), 1, Random.Range(0.5f, 1f))
				.SetEase(Ease.Linear)
				.onComplete += () =>
				{
					if (!WorldGenerator.Instance.IsGround(instance.transform.position))
					{
						Destroy(instance);

						if (!ItemHitWaterSound.IsNull)
						{
							RuntimeManager.PlayOneShot(ItemHitWaterSound, instance.transform.position);
						}
					}
					else
					{
						collider.enabled = true;

						if (!ItemHitGroundSound.IsNull)
						{
							RuntimeManager.PlayOneShot(ItemHitGroundSound, instance.transform.position);
						}
					}
				};
			};
		}
	}
}
