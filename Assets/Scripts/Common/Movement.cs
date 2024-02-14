using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Quinn
{
	public class Movement : Locomotion
	{
		[field: SerializeField, Unit(Units.MetersPerSecond)]
		public float MoveSpeed { get; set; } = 5f;

		[SerializeField]
		private float DashDuration = 0.5f;

		[SerializeField]
		private float DashSpeed = 10f;

		[SerializeField]
		private float DashCooldown = 0.15f;

		[SerializeField]
		private EventReference DashSound;

		public bool IsMoving { get; private set; }
		public bool IsDashing { get; private set; }

		private IFacing _direction;
		private readonly Timer _dashDuration = new(), _dashCooldown = new();

		private EventInstance _dashSound;

		protected override void Awake()
		{
			base.Awake();

			if (!TryGetComponent(out _direction))
			{
				throw new System.NullReferenceException($"{nameof(Movement)} must be on the same gameObject as a component that implements {nameof(IFacing)}!");
			}
		}

		private void Update()
		{
			if (IsDashing)
			{
				SetVelocity(_direction.Direction.normalized * DashSpeed);

				if (_dashDuration.IsDone)
				{
					IsDashing = false;
					_dashCooldown.Reset(DashCooldown);
					
					if (_dashSound.isValid())
					{
						_dashSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					}
				}
			}
		}

		public void FaceDirection(float x)
		{
			if (x > 0f)
			{
				transform.localScale = Vector3.one;
			}
			else if (x < 0f)
			{
				transform.localScale = new Vector3(-1f, 1f, 1f);
			}
		}

		public void Move(Vector2 direction)
		{
			if (IsDashing) return;

			IsMoving = direction != Vector2.zero && Velocity.sqrMagnitude > 0f;

			direction.Normalize();
			AddVelocity(MoveSpeed * direction);

			// Flip sprite.
			FaceDirection(_direction.Direction.x);
		}

		public void MoveTowards(Vector2 target)
		{
			Vector2 dir = target - (Vector2)transform.position;
			dir.Normalize();

			Move(dir);
		}

		public void Dash()
		{
			if (!IsDashing && _dashCooldown.IsDone)
			{
				IsDashing = true;
				_dashDuration.Reset(DashDuration);

				if (!DashSound.IsNull)
				{
					if (_dashSound.isValid())
					{
						_dashSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					}

					_dashSound = RuntimeManager.CreateInstance(DashSound);
					_dashSound.start();
				}
			}
		}
	}
}
