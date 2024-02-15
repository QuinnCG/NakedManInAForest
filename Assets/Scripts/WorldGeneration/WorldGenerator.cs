using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

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
		private void Generate()
		{
			GroundTilemap.ClearAllTiles();
			BarrierTilemap.ClearAllTiles();

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

		private void SetGround(int x, int y)
		{
			GroundTilemap.SetTile(new Vector3Int(x, y), GrassTile);
		}

		private void SetBarrier(int x, int y)
		{
			BarrierTilemap.SetTile(new Vector3Int(x, y), GrassTile);
		}

		private void Refresh()
		{
			GroundTilemap.RefreshAllTiles();
		}
	}
}
