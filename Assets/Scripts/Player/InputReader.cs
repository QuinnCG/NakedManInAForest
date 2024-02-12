using UnityEngine;
using UnityEngine.InputSystem;

namespace Quinn.Player
{
	public class InputReader : MonoBehaviour
	{
		[field: SerializeField]
		public InputAction Move { get; private set; }

		[field: SerializeField]
		public InputAction Interact { get; private set; }

		[field: SerializeField]
		public InputAction Dash { get; private set; }

		[field: SerializeField]
		public InputAction Inventory { get; private set; }

		private void OnEnable()
		{
			Move.Enable();
			Interact.Enable();
			Dash.Enable(); 
			Inventory.Enable();
		}

		private void OnDisable()
		{
			Move.Disable();
			Interact.Disable();
			Dash.Disable();
			Inventory.Disable();
		}
	}
}
