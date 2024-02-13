using UnityEngine;

namespace Quinn.Player
{
	public class AnimatorInteract : MonoBehaviour
	{
		private InteractionManager _interaction;

		private void Awake()
		{
			_interaction = GetComponentInParent<InteractionManager>();
		}

		public void Interact()
		{
			_interaction.Interact();
		}
	}
}
