using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Quinn.Player
{
	[RequireComponent(typeof(PlayerController))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(InventoryManager))]
	public class InteractionManager : MonoBehaviour
	{
		[System.Serializable]
		public class InteractionAnimation
		{
			[HorizontalGroup, LabelText("")]
			public InteractionType Type;

			[HorizontalGroup, LabelText("")]
			public AnimationClip Animation;
		}

		[SerializeField]
		private float PollRadius = 1.5f;

		[SerializeField]
		private LayerMask InteractionLayer;

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

		public void PollNearbyInteractables()
		{
			// Physics cast to find nearby colliders.
			Vector2 origin = transform.position;
			int layer = InteractionLayer.value;

			var colliders = Physics2D.OverlapCircleAll(origin, PollRadius, layer);

			GameObject bestTarget = null;
			float bestDst = float.PositiveInfinity;

			// Find closest collider.
			foreach (var collider in colliders)
			{
				float dst = Vector2.Distance(origin, collider.transform.position);

				if (dst < bestDst)
				{
					bestTarget = collider.gameObject;
					bestDst = dst;
				}
			}

			// If collider is interactable, begin interacting.
			if (bestTarget != null)
			{
				if (bestTarget.GetComponent(typeof(IInteractable)) is IInteractable i)
				{
					_interactable = i;

					_interactSequence = InteractSequence();
					StartCoroutine(_interactSequence);
				}
			}
		}

		public void CancelInteraction()
		{
			if (_interactSequence != null)
			{
				StopCoroutine(_interactSequence);
				IsInteracting = false;
			}
		}

		public void Interact()
		{
			if (_interactSequence != null)
			{
				_interactable.Interact(gameObject);
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
				dst = Vector2.Distance(transform.position, target);

				_movement.MoveTowards(target);
				yield return null;
			}

			// Get animation to play.
			AnimationClip interactAnim =
				InteractionAnimations
				.Where(x => x.Type == _interactable.InteractionType)
				.First()
				.Animation;

			// Play animation.
			_animator.Play(interactAnim);
			_animator.Speed = _inventory.HeldItem.InteractPlaybackFactor;

			// Wait for animation to end.
			yield return new WaitForSeconds(interactAnim.length);

			// Cleanup.
			_animator.Speed = 1f;
			IsInteracting = false;
		}
	}
}
