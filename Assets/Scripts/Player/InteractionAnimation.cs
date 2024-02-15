using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.Player
{
	[System.Serializable]
	public class InteractionAnimation
	{
		[HorizontalGroup, LabelText("")]
		public InteractionType Type;

		[HorizontalGroup, LabelText("")]
		public AnimationClip Animation;
	}
}
