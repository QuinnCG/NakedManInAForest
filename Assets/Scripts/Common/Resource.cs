using DG.Tweening;
using FMODUnity;
using Quinn.Player;
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

		[SerializeField]
		private EventReference HitSound, DestroySound;

		[SerializeField]
		private ResourceHarvestSpawnEntry[] SpawnEntries;

		private int _hitPoints;

		private void Awake()
		{
			_hitPoints = HitPoints;
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

			if (Random.value < 0.3f)
			{
				SpawnRandomItem();
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
			target += Random.insideUnitSphere * 3f;

			const string key = "PhysicalItem.prefab";
			Addressables.InstantiateAsync(key, transform.position, Quaternion.identity).Completed += handle =>
			{
				var instance = handle.Result;

				instance.transform.DOJump(target, Random.Range(1f, 2f), 1, Random.Range(8f, 16f))
				.SetEase(Ease.Linear)
				.SetSpeedBased();
			};
		}
	}
}
