using UnityEngine;
using Sirenix.OdinInspector;

namespace Quinn.Player
{
	[System.Serializable]
	public class Recipe
	{
		public bool HasA = true, HasB = true, HasC;

		[Space, HorizontalGroup(GroupID = "A"), ShowIf(nameof(HasA))]
		public Item A;
		[HorizontalGroup(GroupID = "A"), ShowIf(nameof(HasA))]
		public int ACount = 1;

		[HorizontalGroup(GroupID = "B"), ShowIf(nameof(HasB))]
		public Item B;
		[HorizontalGroup(GroupID = "B"), ShowIf(nameof(HasB))]
		public int BCount;

		[HorizontalGroup(GroupID = "C"), ShowIf(nameof(HasC))]
		public Item C;
		[HorizontalGroup(GroupID = "C"), ShowIf(nameof(HasC))]
		public int CCount;

		[HorizontalGroup(GroupID = "Result")]
		public Item Result;
		[HorizontalGroup(GroupID = "Result")]
		public int ResultCount = 1;
	}
}
