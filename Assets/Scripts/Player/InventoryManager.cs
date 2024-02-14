using System;
using UnityEngine;

namespace Quinn.Player
{
	public class InventoryManager : MonoBehaviour
	{
		[SerializeField]
		private Item EmptyHand;

		[field: SerializeField]
		public int SlotCount { get; private set; } = 6;

		public Item HeldItem
		{
			get
			{
				return _heldItem == null ? EmptyHand : _heldItem;
			}
			set => _heldItem = value;
		}

		public event Action OnChanged;
		public event Action<int> OnSelect;

		private int _selectedSlot = -1;
		private Item _heldItem;
		private Slot[] _inventory;

		private void Awake()
		{
			_inventory = new Slot[SlotCount];
		}

		private void Start()
		{
			Reconstruct();
		}

		private void Update()
		{
			void UpdateForIndex(int index)
			{
				if (Input.GetKeyDown(KeyCode.Alpha1 + index))
				{
					Select(index);
				}
			}

			UpdateForIndex(0);
			UpdateForIndex(1);
			UpdateForIndex(2);
			UpdateForIndex(3);
			UpdateForIndex(4);
			UpdateForIndex(5);
		}

		public void Select(int index)
		{
			// Deselect.
			if (index == -1)
			{
				HeldItem = null;
				_selectedSlot = -1;
				OnSelect?.Invoke(-1);

				return;
			}

			// Tried selecting an unselected slot.
			if (_selectedSlot != index)
			{
				// Tried to select empty slot, deselecting.
				if (GetAt(index) == null)
				{
					Select(-1);
					return;
				}

				// Select unselected slot.
				if (GetAt(index).Item.IsEquippable)
				{
					_selectedSlot = index;

					var slot = GetAt(index);
					HeldItem = slot.Item;

					OnSelect(index);
				}
			}
			// Tried selecting already selected slot, deselecting.
			else
			{
				Select(-1);
			}
		}

		public void Set(int i, Slot slot)
		{
			_inventory[i] = slot;
		}

		public Slot GetAt(int index)
		{
			return _inventory[index];
		}

		public Slot Get(Item item)
		{
			foreach (var slot in _inventory)
			{
				if (slot == null) continue;
				if (slot.Item == item) return slot;
			}

			return null;
		}

		public int IndexOf(Item item)
		{
			for (int i = 0; i < _inventory.Length; i++)
			{
				var slot = GetAt(i);
				if (slot == null) continue;
				if (slot.Item == item)
				{
					return i;
				}
			}

			return -1;
		}

		public bool Contains(Item item)
		{
			foreach (var slot in _inventory)
			{
				if (slot.Item == item) return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if there is any empty slots or if any slots have extra stack space in them.
		/// </summary>
		public bool IsFull()
		{
			for (int i = 0; i < _inventory.Length; i++)
			{
				if (_inventory[i] == null) return false;
				if (_inventory[i].Count < _inventory[i].Item.MaxStack) return false;
			}

			return true;
		}

		public int GetEmptySlotIndex()
		{
			for (int i = 0; i <= _inventory.Length; i++)
			{
				if (_inventory[i] == null) return i;
			}

			return -1;
		}

		public bool Add(Item item, int count, int uses = -1)
		{
			if (item == null || count <= 0)
			{
				throw new NullReferenceException($"Failed to add item '{item}'!");
			}

			int remaining = count;

			while (remaining > 0 && !IsFull())
			{
				int index = IndexOf(item);

				// Add to existing slot.
				if (index > -1)
				{
					var slot = _inventory[index];

					int stack = slot.Count;
					int emptyStackSpace = item.MaxStack - stack;
					int toAdd = Mathf.Min(emptyStackSpace, remaining);

					// Average uses.
					if (uses > -1)
					{
						float a = (float)slot.RemainingUses / item.MaxUses;
						float b = (float)uses / item.MaxUses;
						float avg = (a + b) / 2f;
						int result = Mathf.FloorToInt(avg * item.MaxUses);

						slot.RemainingUses = result;
					}

					slot.Count += toAdd;
					remaining -= toAdd;
				}
				// Add to empty slot.
				else
				{
					int toAdd = Mathf.Min(count, item.MaxStack);

					int i = GetEmptySlotIndex();
					Set(i, new Slot()
					{
						Item = item,
						Count = toAdd,
						RemainingUses = uses == -1 ? item.MaxUses : uses
					});

					remaining -= toAdd;
				}
			}

			Reconstruct();
			return remaining == 0;
		}

		public bool Remove(Item item, int count)
		{
			if (item == null || count <= 0)
			{
				throw new NullReferenceException($"Failed to remove item '{item}'!");
			}

			int index = IndexOf(item);
			if (index == -1) return false;

			int remaining = count;

			for (int i = 0; i < _inventory.Length; i++)
			{
				var slot = GetAt(i);
				if (slot == null) continue;
				if (slot.Item != item) continue;

				int toRemove = Mathf.Min(count, slot.Count);
				slot.Count -= toRemove;
				remaining -= toRemove;

				if (slot.Count == 0)
				{
					Set(i, null);
				}
			}

			Reconstruct();
			return remaining == 0;
		}

		public void Swap(int a, int b)
		{
			var slot = GetAt(a);
			Set(a, GetAt(b));
			Set(b, slot);

			if (GetAt(a).Item.IsEquippable != GetAt(b).Item.IsEquippable
				&& (_selectedSlot == a || _selectedSlot == b))
			{
				Select(-1);
			}

			Reconstruct();
		}

		private void Reconstruct()
		{
			OnChanged?.Invoke();
		}
	}
}
