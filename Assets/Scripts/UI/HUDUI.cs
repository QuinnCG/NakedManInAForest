using DG.Tweening;
using FMODUnity;
using Quinn.Player;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class HUDUI : MonoBehaviour
	{
		[SerializeField, Required]
		private InventoryManager Inventory;

		[SerializeField, Required]
		private GameObject CraftingMenu;

		[SerializeField, Required]
		private Transform Hotbar;

		[SerializeField, Required]
		private Slider HealthBar;

		[SerializeField]
		private EventReference InventoryOpenSound, InventoryCloseSound;

		private SlotUI[] Slots;

		private void Awake()
		{
			CraftingMenu.SetActive(false);

			Inventory.OnChanged += UpdateSlots;
			Inventory.OnSelect += OnSelectSlot;

			Slots = new SlotUI[Inventory.SlotCount];
			for (int i = 0; i < Hotbar.childCount; i++)
			{
				Slots[i] = Hotbar.GetChild(i).GetComponent<SlotUI>();
			}

			Inventory.OnToggleInventory += OnToggleInventory;
		}

		private void Update()
		{
			float percent = PlayerController.Instance.HealthPercent;
			HealthBar.value = percent;
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

		private void OnToggleInventory(bool open)
		{
			CraftingMenu.SetActive(open);

			if (open)
			{
				CraftingMenu.transform.DOKill();
				CraftingMenu.transform.DOScale(0.95f, 0.4f).From().SetEase(Ease.OutElastic);

				if (!InventoryOpenSound.IsNull)
				{
					RuntimeManager.PlayOneShot(InventoryOpenSound);
				}
			}
			else
			{
				if (!InventoryCloseSound.IsNull)
				{
					RuntimeManager.PlayOneShot(InventoryCloseSound);
				}
			}
		}
	}
}
