using DG.Tweening;
using FMODUnity;
using Quinn.UI;
using Quinn.WorldGeneration;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quinn.Player
{
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(InteractionManager))]
	public class PlayerController : MonoBehaviour, IFacing
	{
		public static PlayerController Instance { get; private set; }

		[SerializeField]
		private int HitPoints = 20;

		[SerializeField, Required]
		private AnimationClip IdleAnim, DeathAnim;

		[field: SerializeField, Required]
		public AnimationClip MoveAnim { get; private set; }

		[SerializeField]
		private EventReference StartMoveSound, HurtSound, DeathSound;

		[SerializeField, Required]
		private StudioEventEmitter Music;

		public bool IsDead { get; private set; }
		public Vector2 Direction => _direction;
		public float HealthPercent => (float)_hitPoints / HitPoints;

		private PlayableAnimator _animator;
		private InputReader _input;
		private Movement _movement;
		private InteractionManager _interaction;

		private bool _wasMoving;
		private Vector2 _direction = Vector2.right;

		private int _hitPoints;

		private float _nextRegenTime;

		private Material _mat;

		private void Awake()
		{
			Instance = this;

			_mat = GetComponentInChildren<SpriteRenderer>().material;

			_animator = GetComponentInChildren<PlayableAnimator>();
			_input = GetComponent<InputReader>();
			_movement = GetComponent<Movement>();
			_interaction = GetComponent<InteractionManager>();

			_input.Move.performed += _ => OnStartMove();
			_input.Interact.performed += _ => OnInteract();
			_input.Dash.performed += _ => OnDash();

			_hitPoints = HitPoints;
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
			if (IsDead) return;

			if (Time.time > _nextRegenTime)
			{
				_nextRegenTime = Time.time + 30f;
				_hitPoints = Mathf.Min(_hitPoints + 1, HitPoints);
			}

			if (!_interaction.IsInteracting)
			{
				UpdateMove();
				_animator.Play(_movement.IsMoving ? MoveAnim : IdleAnim);
			}

			if (!_interaction.IsInteracting && _input.Interact.IsPressed())
			{
				_interaction.InteractWithNearest();
			}
		}

		public void TakeDamage()
		{
			if (IsDead) return;

			int damage = 11;
			damage = Mathf.Max(1, damage - InventoryManager.Instance.GetDamageReduction());

			_hitPoints -= damage;
			_hitPoints = Mathf.Max(0, _hitPoints);

			if (!HurtSound.IsNull)
			{
				RuntimeManager.PlayOneShot(HurtSound);
			}

			if (_hitPoints == 0)
			{
				if (!DeathSound.IsNull)
				{
					RuntimeManager.PlayOneShot(DeathSound);
				}

				IsDead = true;
				StartCoroutine(DeathSequence());
			}
			else
			{
				DOTween.To(() => _mat.GetFloat("_Hurt"), x => _mat.SetFloat("_Hurt", x), 1f, 0.1f).onComplete += () =>
				{
					DOTween.To(() => _mat.GetFloat("_Hurt"), x => _mat.SetFloat("_Hurt", x), 0f, 0.1f);
				};
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

		private IEnumerator DeathSequence()
		{
			Unity.Services.Analytics.AnalyticsService.Instance.RecordEvent("playerDeath");

			Music.Stop();

			_animator.Play(DeathAnim);
			yield return new WaitForSeconds(DeathAnim.length);

			yield return new WaitForSeconds(3f);
			SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
		}
	}
}
