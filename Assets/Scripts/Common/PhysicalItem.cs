using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public class PhysicalItem : MonoBehaviour, IInteractable
	{
		[SerializeField]
		private EventReference PickUpSound;

		public InteractionType InteractionType => InteractionType.PickUp;

		public Vector2 InteractPoint => transform.position;

		public float InteractDistance => 0.1f;

		public void Interact(GameObject player)
		{
			if (!PickUpSound.IsNull)
			{
				RuntimeManager.PlayOneShot(PickUpSound);
			}

			Destroy(gameObject);
		}
	}
}
