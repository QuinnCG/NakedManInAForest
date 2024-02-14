namespace Quinn.Player
{
	[System.Serializable]
	public class Slot
	{
		public Item Item;
		public int Count = 1;
		public int RemainingUses = -1;
	}
}
