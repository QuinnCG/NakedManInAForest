using DG.Tweening;
using Quinn.Player;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(PlayableAnimator))]
	[RequireComponent(typeof(Movement))]
	public class EnemyController : MonoBehaviour, IFacing, IInteractable
	{
		[SerializeField]
		private int HitPoints = 8;

		[SerializeField]
		private float AttackRadius = 0.5f,DamageRadius = 1.5f;

		[SerializeField, Required]
		private AnimationClip IdleAnim, MoveAnim;

		[SerializeField, Required]
		private AnimationClip AttackAnim;

		[SerializeField, Required]
		private AnimationClip HurtAnim, DeathAnim;

		private SpriteRenderer _renderer;
		private PlayableAnimator _animator;
		private Movement _movement;

		public Vector2 Direction => _dir.normalized;

		public InteractionType InteractionType => InteractionType.Attack;

		public Vector2 InteractPoint => transform.position;

		public float InteractDistance => 1f;

		private Vector2 _dir = Vector2.right;

		private int _hitPoints;

		private bool _isAttacking;
		private bool _isHurting;
		private bool _isDead;

		private IEnumerator _attackSequence, _hurtSequence;

		private void Awake()
		{
			_renderer = GetComponent<SpriteRenderer>();
			_animator = GetComponent<PlayableAnimator>();
			_movement = GetComponent<Movement>();

			_hitPoints = HitPoints;
		}

		private void Update()
		{
			if (!_isAttacking && !_isHurting && !_isDead)
			{
				var anim = _movement.IsMoving ? MoveAnim : IdleAnim;
				_animator.Play(anim);

				var target = PlayerController.Instance.transform.position;
				_dir = (target - transform.position).normalized;
				_movement.MoveTowards(target);

				if (Vector2.Distance(transform.position, target) < AttackRadius)
				{
					StartAttack();
				}
			}
		}

		public void Interact(GameObject player)
		{
			if (_isDead) return;

			_hitPoints--;

			if (_hitPoints <= 0)
			{
				StartDeath();
			}
			else
			{
				StartHurt();
			}
		}

		public void DamageTarget()
		{
			Vector2 target = PlayerController.Instance.transform.position;
			float dst = Vector2.Distance(transform.position, target);

			if (dst < DamageRadius)
			{
				PlayerController.Instance.TakeDamage();
			}
		}

		private void StartAttack()
		{
			if (!_isAttacking)
			{
				_isAttacking = true;

				_attackSequence = AttackSequence();
				StartCoroutine(_attackSequence);
			}
		}

		private void StopAttack()
		{
			if (_isAttacking)
			{
				_isAttacking = false;
				StopCoroutine(_attackSequence);

				_attackSequence = null;
			}
		}

		private void StartHurt()
		{
			if (!_isHurting)
			{
				_isHurting = true;

				_hurtSequence = HurtSequence();
				StartCoroutine(_hurtSequence);
			}
		}

		private void StopHurt()
		{
			if (_isHurting)
			{
				_isHurting = false;

				StopCoroutine(_hurtSequence);
				_hurtSequence = null;
			}
		}

		private void StartDeath()
		{
			if (!_isDead)
			{
				_isDead = true;
				StartCoroutine(DeathSequence());
			}
		}

		private IEnumerator AttackSequence()
		{
			_animator.Play(AttackAnim);
			yield return new WaitForSeconds(AttackAnim.length);

			StopAttack();
		}

		private IEnumerator HurtSequence()
		{
			_animator.Play(HurtAnim);
			yield return new WaitForSeconds(HurtAnim.length);

			StopHurt();
		}

		private IEnumerator DeathSequence()
		{
			_animator.Play(DeathAnim);
			yield return new WaitForSeconds(DeathAnim.length);

			yield return new WaitForSeconds(3f);
			yield return _renderer.DOFade(0f, 3f).WaitForCompletion();

			Destroy(gameObject);
		}
	}
}
