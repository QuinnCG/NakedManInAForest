using UnityEngine;

namespace Quinn.Player
{
	public class InventoryManager : MonoBehaviour
	{
		[SerializeField]
		private Item EmptyHand;

		public Item HeldItem
		{
			get
			{
				return _heldItem == null ? EmptyHand : _heldItem;
			}
			set => _heldItem = value;
		}

		private Item _heldItem;
	}
}
