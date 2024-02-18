using DG.Tweening;
using FMODUnity;
using Quinn.Player;
using Quinn.WorldGeneration;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Quinn.AI
{
	public class EnemySpawner : MonoBehaviour
	{
		[SerializeField, HorizontalGroup(GroupID = "Spawn Interval")]
		private float MinSpawnInterval = 35f, MaxSpawnInterval = 80f;

		[SerializeField, HorizontalGroup(GroupID = "Spawn Count")]
		private int MinSpawnCount = 1, MaxSpawnCount = 3;

		[SerializeField]
		private float SpawnRadius = 4f;

		[SerializeField]
		private GameObject PortalPrefab;

		[SerializeField, FoldoutGroup("Sound")]
		private EventReference PortalAppearSound;
		[SerializeField, FoldoutGroup("Sound")]
		private EventReference EnemySpawnSound;
		[SerializeField, FoldoutGroup("Sound")]
		private EventReference PortalDisappearSound;

		[SerializeField]
		private GameObject[] EnemyPrefabs;

		private void Start()
		{
			StartCoroutine(SpawnLoop());
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.H))
			{
				StartCoroutine(SpawnSequence());
			}
#endif
		}

		private IEnumerator SpawnLoop()
		{
			// Buffer.
			yield return new WaitForSeconds(20f);

			// Spawn rate factor.
			float factor = 1f;
			float maxCountFactor = 1f;

			while (true)
			{
				yield return new WaitForSeconds(Random.Range(MinSpawnInterval, MaxSpawnInterval) * factor);

				for (int i = 0; i < Random.Range(MinSpawnCount, Mathf.RoundToInt((float)MaxSpawnCount * MaxSpawnCount)); i++)
				{
					StartCoroutine(SpawnSequence());
				}

				factor -= 0.02f;
				factor = Mathf.Max(factor, 0.25f);

				maxCountFactor += 0.3f;
				maxCountFactor = Mathf.Min(maxCountFactor, 6f);
			}
		}

		private IEnumerator SpawnSequence()
		{
			Unity.Services.Analytics.AnalyticsService.Instance.RecordEvent("enemySpawned");

			// Get spawn position.
			Vector2 pos = GetSpawnPosition();

			// Spawn portal.
			GameObject portal = CreatePortal(pos);
			if (!PortalAppearSound.IsNull)
			{
				RuntimeManager.PlayOneShot(PortalAppearSound, pos);
			}
			portal.transform.DOScale(0f, 0.545f).SetEase(Ease.OutCubic).From();

			// Spawn enemy.
			yield return new WaitForSeconds(Random.Range(1f, 3f));
			GameObject enemy = SpawnEnemy(pos);
			enemy.transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutElastic).From();
			if (!EnemySpawnSound.IsNull)
			{
				RuntimeManager.PlayOneShot(EnemySpawnSound, pos);
			}

			// Despawn portal.
			yield return new WaitForSeconds(0.5f);
			if (!PortalDisappearSound.IsNull)
			{
				RuntimeManager.PlayOneShot(PortalDisappearSound, pos);
			}

			yield return portal.transform.DOScale(0f, 0.2f).SetEase(Ease.InCubic).WaitForCompletion();
			Destroy(portal);
		}

		private Vector2 GetSpawnPosition()
		{
			Vector2 playerPos = PlayerController.Instance.transform.position;
			playerPos = new Vector2(Mathf.Floor(playerPos.x), Mathf.Floor(playerPos.y));

			Vector2 pos;
			bool isValid;

			int attempts = 0;
			const int maxAttempts = 10000;

			do
			{
				pos = playerPos + (Random.insideUnitCircle * SpawnRadius) + (Vector2.one * 0.5f);
				isValid = WorldGenerator.Instance.IsGround(pos);

				attempts++;
				if (attempts > maxAttempts)
				{
					return playerPos;
				}
			}
			while (!isValid);

			return pos;
		}

		private GameObject CreatePortal(Vector2 position)
		{
			return Instantiate(PortalPrefab, position, Quaternion.identity, transform);
		}

		private GameObject SpawnEnemy(Vector2 position)
		{
			GameObject enemy = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)];
			Instantiate(enemy, position, Quaternion.identity, transform);

			return enemy;
		}
	}
}
