using UnityEngine;

namespace Quinn
{
	public class Resource : MonoBehaviour, IInteractable
	{
		[SerializeField]
		private ResourceType Type;

		public InteractionType InteractionType => Type switch
		{
			ResourceType.Wood => InteractionType.Fell,
			ResourceType.Stone => InteractionType.Mine,
			_ => throw new System.Exception("Invalid ResourceType!")
		};

		public Vector2 InteractPoint => transform.position;
	}
}
