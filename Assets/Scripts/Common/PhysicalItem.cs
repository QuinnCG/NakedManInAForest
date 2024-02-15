using FMODUnity;
using Quinn.Player;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(CircleCollider2D))]
	public class PhysicalItem : MonoBehaviour, IInteractable
	{
		[SerializeField]
		private EventReference PickUpSound;

		[field: SerializeField]
		public Slot ItemData { get; private set; }

		public InteractionType InteractionType => InteractionType.PickUp;

		public Vector2 InteractPoint => transform.position;

		public float InteractDistance => 0.1f;

		private SpriteRenderer _renderer;
		private CircleCollider2D _collider;

		private void Awake()
		{
			_renderer = GetComponent<SpriteRenderer>();
			_collider = GetComponent<CircleCollider2D>();

			if (ItemData != null && ItemData.Item != null)
			{
				Set(ItemData);
			}
		}

		public void Set(Slot slot)
		{
			ItemData = slot;

			_renderer.sprite = slot.Item.Sprite;
			_collider.radius = _renderer.bounds.size.x;
		}

		public void Interact(GameObject player)
		{
			if (!PickUpSound.IsNull)
			{
				RuntimeManager.PlayOneShot(PickUpSound);
			}

			var inventory = player.GetComponent<InventoryManager>();
			inventory.Add(ItemData.Item, ItemData.Count, ItemData.RemainingUses);

			Destroy(gameObject);
		}
	}
}
