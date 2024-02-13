using UnityEngine;

namespace Quinn
{
	public class PhysicalItem : MonoBehaviour, IInteractable
	{
		public InteractionType InteractionType => InteractionType.PickUp;

		public Vector2 InteractPoint => transform.position;

		public float InteractDistance => 0.1f;

		public void Interact(GameObject player)
		{
			Destroy(gameObject);
		}
	}
}
