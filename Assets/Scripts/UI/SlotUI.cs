using Quinn.Player;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class SlotUI : MonoBehaviour
	{
		[SerializeField, Required]
		private Image Icon;

		[SerializeField, Required]
		private Image Background;

		[SerializeField, Required]
		private TextMeshProUGUI Count;

		[SerializeField, Required]
		private Sprite SelectedSprite;

		[SerializeField]
		private int Index = -1;

		private Sprite _normalBackground;

		private void Awake()
		{
			_normalBackground = Background.sprite;
		}

		public void Set(Slot slot)
		{
			if (slot == null || slot.Item == null)
			{
				Count.gameObject.SetActive(false);
				Icon.gameObject.SetActive(false);
			}
			else
			{
				Icon.gameObject.SetActive(true);
				Icon.sprite = slot.Item.Sprite;
				
				if (slot.Item.MaxStack > 1)
				{
					Count.gameObject.SetActive(true);
					Count.text = $"{slot.Count}x";
				}
				else
				{
					Count.gameObject.SetActive(false);
				}
			}
		}

		public void SetSelected(bool isSelected)
		{
			if (isSelected)
			{
				Background.sprite = SelectedSprite;
			}
			else
			{
				Background.sprite = _normalBackground;
			}
		}

		public void OnClick()
		{
			var slot = InventoryManager.Instance.GetAt(Index);

			if (slot != null && slot.Item != null)
			{
				InventoryManager.Instance.Spawn(slot.Item, slot.Count);
				InventoryManager.Instance.RemoveAt(Index);
			}
		}
	}
}
