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
		[SerializeField]
		private Recipe Recipe;

		[SerializeField, Required, FoldoutGroup("References")]
		private Button Button;

		[SerializeField, Required, FoldoutGroup("References")]
		private Image SlotA, SlotB, SlotC, ResultSlot;

		[SerializeField, Required, FoldoutGroup("References")]
		private Transform PlusA, PlusB, Equal;

		[SerializeField, FoldoutGroup("References")]
		private EventReference CraftSound, DropSound;

		private void Awake()
		{
			static void Set(Image image, int count)
			{
				var textMesh = image.transform.parent.GetComponentInChildren<TextMeshProUGUI>();
				textMesh.text = $"{count}x";
			}

			if (Recipe.Result == null)
			{
				SlotA.enabled = false;
				SlotB.enabled = false;
				SlotC.enabled = false;
				ResultSlot.enabled = false;

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
				Destroy(SlotB.transform.parent.gameObject);
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
				Destroy(SlotC.transform.parent.gameObject);
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

			var evnt = new Unity.Services.Analytics.CustomEvent("itemCrafted")
			{
				["itemName"] = Recipe.Result.name
			};
			Unity.Services.Analytics.AnalyticsService.Instance.RecordEvent(evnt);
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
			InventoryManager.Instance.Spawn(Recipe.Result, amount);
		}
	}
}
