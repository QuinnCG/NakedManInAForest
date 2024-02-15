using FMODUnity;
using Quinn.WorldGeneration;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn.Player
{
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(InteractionManager))]
	public class PlayerController : MonoBehaviour, IFacing
	{
		[SerializeField, Required]
		private AnimationClip IdleAnim;

		[field: SerializeField, Required]
		public AnimationClip MoveAnim { get; private set; }

		[SerializeField]
		private EventReference StartMoveSound;

		private PlayableAnimator _animator;
		private InputReader _input;
		private Movement _movement;
		private InteractionManager _interaction;

		private bool _wasMoving;

		public Vector2 Direction => _direction;

		private Vector2 _direction = Vector2.right;

		private void Awake()
		{
			_animator = GetComponentInChildren<PlayableAnimator>();
			_input = GetComponent<InputReader>();
			_movement = GetComponent<Movement>();
			_interaction = GetComponent<InteractionManager>();

			_input.Move.performed += _ => OnStartMove();
			_input.Interact.performed += _ => OnInteract();
			_input.Dash.performed += _ => OnDash();
		}

		private void Start()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			const int maxAttempts = 10000;
			int attempts = 0;

			while (!WorldGenerator.Instance.IsGround(transform.position))
			{
				transform.position += (Vector3)UnityEngine.Random.insideUnitCircle * 1f;

				attempts++;
				if (attempts > maxAttempts)
				{
					throw new Exception($"Failed to find ground for the player after {maxAttempts} attempts!");
				}
			}
		}

		private void Update()
		{
			if (!_interaction.IsInteracting)
			{
				UpdateMove();
				_animator.Play(_movement.IsMoving ? MoveAnim : IdleAnim);
			}
		}

		private void OnStartMove()
		{
			_interaction.CancelInteraction();

			if (!_wasMoving)
			{
				RuntimeManager.PlayOneShot(StartMoveSound, transform.position);
			}

			_wasMoving = _movement.Velocity.sqrMagnitude > 0f;
		}

		private void UpdateMove()
		{
			var moveDir = _input.Move.ReadValue<Vector2>().normalized;
			if (moveDir.sqrMagnitude > 0f) _direction = moveDir;
			_movement.Move(moveDir);
		}

		private void OnInteract()
		{
			_interaction.InteractWithNearest();
		}

		private void OnDash()
		{
			_movement.Dash();
		}
	}
}
