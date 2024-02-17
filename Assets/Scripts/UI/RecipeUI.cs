using FMODUnity;
using Quinn.Player;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class RecipeUI : MonoBehaviour
	{
		[SerializeField, Required]
		private Button Button;

		[SerializeField, Required]
		private Image SlotA, SlotB, SlotC, ResultSlot;

		[SerializeField, Required]
		private Transform PlusA, PlusB, Equal;

		[SerializeField]
		private Recipe Recipe;

		[SerializeField]
		private EventReference CraftSound, DropSound;

		private void Awake()
		{
			static void Set(Image image, int count)
			{
				var textMesh = image.gameObject.GetComponentInChildren<TextMeshProUGUI>();
				textMesh.text = $"{count}x";
			}

			if (Recipe.Result == null)
			{
				return;
			}

			if (Recipe.HasA)
			{
				SlotA.sprite = Recipe.A.Sprite;
				Set(SlotA, Recipe.ACount);
			}
			else
			{
				Destroy(SlotA.gameObject);
			}

			if (Recipe.HasB)
			{
				SlotB.sprite = Recipe.B.Sprite;
				Set(SlotB, Recipe.BCount);
			}
			else
			{
				Destroy(SlotB.gameObject);
				Destroy(PlusA.gameObject);
				Destroy(PlusB.gameObject);
			}

			if (Recipe.HasC)
			{
				SlotC.sprite = Recipe.C.Sprite;
				Set(SlotC, Recipe.CCount);
			}
			else
			{
				Destroy(SlotC.gameObject);
				Destroy(PlusB.gameObject);
			}

			ResultSlot.sprite = Recipe.Result.Sprite;
		}

		public void OnClick()
		{
			if (Recipe.Result == null) return;
			if (!HasRequiredItems()) return;

			var inv = InventoryManager.Instance;

			int space = inv.CalculateAvailableSpace(Recipe.A);
			int toAdd = Recipe.ResultCount;

			if (Recipe.HasA)
			{
				inv.Remove(Recipe.A, Recipe.ACount);
			}

			if (Recipe.HasB)
			{
				inv.Remove(Recipe.B, Recipe.BCount);
			}

			if (Recipe.HasC)
			{
				inv.Remove(Recipe.C, Recipe.CCount);
			}

			if (toAdd > space)
			{
				int toDrop = space - toAdd;
				Drop(toDrop);
			}

			Add(toAdd);
		}

		private bool HasRequiredItems()
		{
			var inv = InventoryManager.Instance;

			if (Recipe.HasA && inv.GetCount(Recipe.A) < Recipe.ACount)
			{
				return false;
			}

			if (Recipe.HasB && inv.GetCount(Recipe.B) < Recipe.BCount)
			{
				return false;
			}

			if (Recipe.HasC && inv.GetCount(Recipe.C) < Recipe.CCount)
			{
				return false;
			}

			return true;
		}

		private void Add(int amount)
		{
			if (!CraftSound.IsNull)
			{
				RuntimeManager.PlayOneShot(CraftSound);
			}

			InventoryManager.Instance.Add(Recipe.Result, amount);
		}

		private void Drop(int amount)
		{
			const string key = "PhysicalItem.prefab";
			Vector2 pos = PlayerController.Instance.transform.position;

			Addressables.InstantiateAsync(key, pos, Quaternion.identity).Completed += handle =>
			{
				var physicalItem = handle.Result.GetComponent<PhysicalItem>();

				physicalItem.Set(new Slot()
				{
					Item = Recipe.Result,
					Count = amount
				});

				if (!DropSound.IsNull)
				{
					RuntimeManager.PlayOneShot(DropSound);
				}
			};
		}
	}
}
