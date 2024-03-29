using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Quinn.WorldGeneration
{
	public class WorldGenerator : MonoBehaviour
	{
		public static WorldGenerator Instance { get; private set; }

		[SerializeField, Required]
		private Tilemap GroundTilemap, BarrierTilemap;

		[SerializeField, Required]
		private TileBase GrassTile;

		[Space, SerializeField]
		private float Scale = 1f;

		[SerializeField]
		private int Size = 100;

		[SerializeField]
		private float GroundTheshold = 0.2f;

		[SerializeField]
		private float Diameter = 95f;

		[SerializeField]
		private Sprite[] GrassFoliageSprites;

		[SerializeField]
		private ResourceSpawnEntry[] ResourceSpawns;

		private readonly List<GameObject> _resources = new();

		private void Awake()
		{
			Instance = this;
			Generate();
		}

		public bool IsGround(Vector2 position)
		{
			var pos = new Vector3Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
			var tile = GroundTilemap.GetTile(pos);

			return tile != null;
		}

		[Button("Regenerate")]
		public void Generate()
		{
			GroundTilemap.ClearAllTiles();
			BarrierTilemap.ClearAllTiles();

			ClearAll();

			int half = Size / 2;
			float scale = Scale;

			int seed = Random.Range(0, 10000);
			float offset = seed / 1000f;

			for (int x = -half; x < half; x++)
			{
				for (int y = -half; y < half; y++)
				{
					float value = Mathf.PerlinNoise(((float)x / Size * scale) + offset, ((float)y / Size * scale) + offset);

					float dst = Vector2Int.Distance(new Vector2Int(0, 0), new Vector2Int(x, y));
					value = dst > (Diameter / 2f) ? 0f : value;

					if (value > GroundTheshold)
					{
						SetGround(x, y);
					}
					else
					{
						SetBarrier(x, y);
					}
				}
			}

			Refresh();
		}

		[Button("Clear")]
		public void ClearAll()
		{
			foreach (var resource in _resources)
			{
#if UNITY_EDITOR
				DestroyImmediate(resource);
#else
				Destroy(resource);
#endif
			}

			_resources.Clear();

			GroundTilemap.ClearAllTiles();
			BarrierTilemap.ClearAllTiles();

			Refresh();
		}

		private void SetGround(int x, int y)
		{
			GroundTilemap.SetTile(new Vector3Int(x, y), GrassTile);

			Vector2 pos = GroundTilemap.transform.position;
			pos += new Vector2(x, y);
			pos += Vector2.one * 0.5f;

			foreach (var spawn in ResourceSpawns)
			{
				float chance = Random.value;

				if (chance < spawn.Chance)
				{
					var instance = Instantiate(spawn.Prefab, pos, Quaternion.identity, transform);
					_resources.Add(instance);

					return;
				}
			}

			//var sprite = GrassFoliageSprites[Random.Range(0, GrassFoliageSprites.Length)];
			//var foliage = new GameObject("Foliage");
			//var r = foliage.AddComponent<SpriteRenderer>();
			//r.sprite = sprite;
			//r.sortingLayerName = "Ground";
			//foliage.transform.parent = transform;
			//foliage.transform.position = pos;
		}

		private void SetBarrier(int x, int y)
		{
			BarrierTilemap.SetTile(new Vector3Int(x, y), GrassTile);
		}

		private void Refresh()
		{
			GroundTilemap.RefreshAllTiles();
			BarrierTilemap.RefreshAllTiles();
		}
	}
}
