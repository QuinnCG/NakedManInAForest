using UnityEngine;

namespace Quinn
{
	public class PhysicalItem : MonoBehaviour, IInteractable
	{
		public InteractionType InteractionType => InteractionType.PickUp;

		public Vector2 InteractPoint => transform.position;
	}
}
