using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.WorldGeneration
{
	[System.Serializable]
	public class ResourceSpawnEntry
	{
		[AssetList(Path = "Prefabs/Resources")]
		public GameObject Prefab;

		[MinValue(0f), MaxValue(1f)]
		public float Chance = 0.1f;
	}
}
