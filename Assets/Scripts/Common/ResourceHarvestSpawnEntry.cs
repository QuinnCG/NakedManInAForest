using Sirenix.OdinInspector;

namespace Quinn
{
	[System.Serializable]
	public class ResourceHarvestSpawnEntry
	{
		public Item Item;

		[HorizontalGroup]
		public int MinCount = 1, MaxCount = 4;
	}
}
