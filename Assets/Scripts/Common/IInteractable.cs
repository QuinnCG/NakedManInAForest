using UnityEngine;

namespace Quinn
{
	public interface IInteractable
	{
		public InteractionType InteractionType { get; }
		public Vector2 InteractPoint { get; }
	}
}
