using UnityEngine;

namespace Quinn.Player
{
	[RequireComponent(typeof(InputReader))]
	[RequireComponent(typeof(Movement))]
	public class PlayerController : MonoBehaviour, IFacing
	{
		private InputReader _input;
		private Movement _movement;

		public Vector2 Direction => _direction;

		private Vector2 _direction = Vector2.right;

		private void Awake()
		{
			_input = GetComponent<InputReader>();
			_movement = GetComponent<Movement>();
		}

		private void Update()
		{
			var moveDir = _input.Move.ReadValue<Vector2>().normalized;
			if (moveDir.sqrMagnitude > 0f) _direction = moveDir;
			_movement.Move(moveDir);
		}
	}
}
