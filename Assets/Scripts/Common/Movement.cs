using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class Movement : Locomotion
	{
		[field: SerializeField, Unit(Units.MetersPerSecond)]
		public float MoveSpeed { get; set; } = 5f;

		public bool IsMoving { get; private set; }

		private IFacing _direction;

		protected override void Awake()
		{
			base.Awake();

			if (!TryGetComponent(out _direction))
			{
				Debug.LogError($"{nameof(Movement)} must be on the same gameObject as a component that implements {nameof(IFacing)}!");
			}
		}

		public void Move(Vector2 direction)
		{
			IsMoving = direction != Vector2.zero && Velocity.sqrMagnitude > 0f;

			direction.Normalize();
			AddVelocity(MoveSpeed * direction);

			// Flip sprite.
			FaceDirection(_direction.Direction.x);
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
	}
}
