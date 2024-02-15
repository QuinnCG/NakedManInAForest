using DG.Tweening;
using FMODUnity;
using Game;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Quinn.Player
{
	public class InventoryManager : MonoBehaviour
	{
		[SerializeField, Required]
		private SpriteRenderer HeldItemSprite;

		[field: SerializeField]
		public int SlotCount { get; private set; } = 6;

		[SerializeField]
		private EventReference EquipSound, DequipSound;

		public Item HeldItem { get; private set; }

		public bool IsInventoryOpen { get; private set; }

		public event Action OnChanged;
		public event Action<int> OnSelect;
		public event Action<bool> OnToggleInventory;

		private InputReader _input;

		private int _selectedSlot = -1;
		private Slot[] _inventory;

		private void Awake()
		{
			_inventory = new Slot[SlotCount];
			HeldItemSprite.sprite = null;

			_input = GetComponent<InputReader>();
			_input.Inventory.performed += _ => ToggleInventory();
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

				HeldItemSprite.sprite = null;
				Jiggle();
				RuntimeManager.PlayOneShot(DequipSound, transform.position);

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

					HeldItemSprite.sprite = slot.Item.Sprite;
					Jiggle();
					RuntimeManager.PlayOneShot(EquipSound, transform.position);

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

		private void Jiggle()
		{
			transform.DOKill();
			transform.localScale = Vector3.one;

			var tween = DOTween.Sequence(transform);
			tween.Append(transform.DOScaleY(0.8f, 0.2f).SetEase(Ease.OutBounce));
			tween.Append(transform.DOScaleY(1f, 0.2f));

			DOTween.To(() => transform.localScale.y, value =>
			{
				var scale = transform.localScale;
				scale.y = value;

				float sign = Mathf.Sign(scale.x);
				float abs = Mathf.Abs(scale.x);

				abs = value;
				scale.x = abs * sign;

				transform.localScale = scale;
			}, 0.9f, 0.08f)
				.SetTarget(transform)
				.SetEase(Ease.Linear)
				.SetLoops(2, LoopType.Yoyo);
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

			const int maxAttempts = 1000;
			int attempts = 0;

			while (remaining > 0 && !IsFull())
			{
				attempts++;

				if (attempts >= maxAttempts)
				{
					throw new Exception($"Failed to add item to inventory after {maxAttempts} attempts!");
				}


				// Find valid slot to add too or fail.
				int existingSlotIndex = -1;

				for (int i = 0; i < SlotCount; i++)
				{
					var s = GetAt(i);
					if (s == null) continue;

					if (s.Item != null && s.Item == item && s.Count < item.MaxStack)
					{
						existingSlotIndex = i;
					}
				}


				// Add to existing slot.
				if (existingSlotIndex > -1)
				{
					var slot = _inventory[existingSlotIndex];

					int stack = slot.Count;
					int emptyStackSpace = item.MaxStack - stack;
					int toAdd = Mathf.Min(emptyStackSpace, remaining);

					// Average uses.
					if (uses > -1 && item.MaxUses > -1)
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

		private void ToggleInventory()
		{
			if (IsInventoryOpen)
			{
				IsInventoryOpen = false;

				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;

				OnToggleInventory?.Invoke(false);
			}
			else
			{
				IsInventoryOpen = true;

				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;

				OnToggleInventory?.Invoke(true);
			}
		}
	}
}
