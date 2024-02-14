using Quinn.Player;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quinn.UI
{
	public class HUDUI : MonoBehaviour
	{
		[SerializeField, Required]
		private InventoryManager Inventory;

		[SerializeField, Required]
		private Transform Hotbar;

		private SlotUI[] Slots;

		private void Awake()
		{
			Inventory.OnChanged += UpdateSlots;
			Inventory.OnSelect += OnSelectSlot;

			Slots = new SlotUI[Inventory.SlotCount];
			for (int i = 0; i < Hotbar.childCount; i++)
			{
				Slots[i] = Hotbar.GetChild(i).GetComponent<SlotUI>();
			}
		}

		private void UpdateSlots()
		{
			for (int i = 0; i < Hotbar.childCount; i++)
			{
				GameObject child = Hotbar.GetChild(i).gameObject;
				if (child.TryGetComponent(out SlotUI slotUI))
				{
					slotUI.Set(Inventory.GetAt(i));
				}
			}
		}

		private void OnSelectSlot(int index)
		{
			foreach (var slot in Slots)
			{
				slot.SetSelected(false);
			}

			if (index > -1)
			{
				Slots[index].SetSelected(true);
			}
		}
	}
}
