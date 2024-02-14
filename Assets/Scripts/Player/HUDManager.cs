using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quinn.Player
{
	[RequireComponent(typeof(InventoryManager))]
	public class HUDManager : MonoBehaviour
	{
		[SerializeField, Required]
		private UIDocument Document;

		private InventoryManager _inventory;

		private VisualElement _root;
		private VisualElement _hotbar;

		private void Awake()
		{
			_inventory = GetComponent<InventoryManager>();

			_root = Document.rootVisualElement;
			_hotbar = _root.Q("hotbar");
		}

		private void Start()
		{
			ConstructHotbar();
		}

		private void ConstructHotbar()
		{
			_hotbar.Clear();

			for (int i = 0; i < _inventory.SlotCount; i++)
			{
				_hotbar.Add(CreateSlot());
			}
		}

		private VisualElement CreateSlot()
		{
			var root = new VisualElement();
			root.AddToClassList("slot");

			var count = new Label("64x");
			count.AddToClassList("slot-count");
			root.Add(count);

			return root;
		}
	}
}
