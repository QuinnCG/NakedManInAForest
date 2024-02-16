using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn.Player
{
	[RequireComponent(typeof(PlayerController))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(InventoryManager))]
	public class InteractionManager : MonoBehaviour
	{
		[SerializeField]
		private float PollRadius = 1.5f;

		[SerializeField]
		private LayerMask InteractionLayer;

		[SerializeField]
		private EventReference PickUpSound;

		[SerializeField]
		private InteractionAnimation[] InteractionAnimations;

		public bool IsInteracting { get; private set; }

		private PlayerController _player;
		private PlayableAnimator _animator;
		private Movement _movement;
		private InventoryManager _inventory;

		private IInteractable _interactable;
		private IEnumerator _interactSequence;

		private void Awake()
		{
			_player = GetComponent<PlayerController>();
			_animator = GetComponentInChildren<PlayableAnimator>();
			_movement = GetComponent<Movement>();
			_inventory = GetComponent<InventoryManager>();
		}

		public void InteractWithNearest()
		{
			if (_interactSequence != null) return;

			// Physics cast to find nearby colliders.
			Vector2 origin = transform.position;
			int layer = InteractionLayer.value;

			var colliders = Physics2D.OverlapCircleAll(origin, PollRadius, layer);

			// Find and sort by nearest.
			var interactables = new List<IInteractable>();

			foreach (var collider in colliders)
			{
				if (collider.gameObject == gameObject)
				{
					continue;
				}

				if (collider.gameObject.GetComponent(typeof(IInteractable)) is IInteractable interactable)
				{
					interactables.Add(interactable);
				}
			}

			interactables.OrderBy(x => Vector2.Distance(transform.position, x.InteractPoint));

			// If collider is interactable, begin interacting.
			while (interactables.Count > 0)
			{
				var nearest = interactables[0];
				interactables.RemoveAt(0);

				bool isPickUp = nearest.InteractionType == InteractionType.PickUp;
				bool isAttack = nearest.InteractionType == InteractionType.Attack;

				bool isCorrectInteractionType = false;

				if (_inventory.IsEquipped)
				{
					isCorrectInteractionType = _inventory.HeldItem.InteractionType == nearest.InteractionType;
				}

				if (isPickUp || isAttack || isCorrectInteractionType)
				{
					_interactable = nearest;

					_interactSequence = InteractSequence();
					StartCoroutine(_interactSequence);

					break;
				}
			}
		}

		public void CancelInteraction()
		{
			if (_interactSequence != null)
			{
				StopCoroutine(_interactSequence);

				IsInteracting = false;
				_animator.Speed = 1f;
				_interactSequence = null;
			}
		}

		public void InteractAnim()
		{
			if (_interactSequence != null && _interactable != null)
			{
				_interactable.Interact(player: gameObject);
				EventReference sound = PickUpSound;

				if (_inventory.IsEquipped)
				{
					if (_interactable.InteractionType != InteractionType.PickUp)
					{
						sound = _inventory.HeldItem.InteractSound;
					}
				}

				RuntimeManager.PlayOneShot(sound, (_interactable as MonoBehaviour).transform.position);
			}
		}

		private IEnumerator InteractSequence()
		{
			IsInteracting = true;

			Vector2 target = _interactable.InteractPoint;
			float dst = float.PositiveInfinity;

			// Move to interactable.
			_animator.Play(_player.MoveAnim);

			while (dst > _interactable.InteractDistance)
			{
				target = _interactable.InteractPoint;
				dst = Vector2.Distance(transform.position, target);

				_movement.FaceDirection(Mathf.Sign(target.x - transform.position.x));
				_movement.MoveTowards(target);

				yield return null;
			}

			// Get animation to play.
			AnimationClip interactAnim =
				InteractionAnimations
				.Where(x => x.Type == _interactable.InteractionType)
				.FirstOrDefault()
				.Animation;

			// Change playback speed.
			var item = _inventory.HeldItem;
			if (item != null && item.InteractionType == _interactable.InteractionType)
			{
				_animator.Speed = item.InteractPlaybackFactor;
			}

			// Play animation.
			_animator.Play(interactAnim);

			// Wait for animation to end.
			yield return new WaitForSeconds(interactAnim.length);

			// Cleanup.
			IsInteracting = false;
			_animator.Speed = 1f;
			_interactSequence = null;
		}
	}
}
