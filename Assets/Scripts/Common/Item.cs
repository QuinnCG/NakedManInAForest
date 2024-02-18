using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	[CreateAssetMenu]
	public class Item : ScriptableObject
	{
		public string DisplayName = "Item Name";

		[Multiline]
		public string Description = "This is the item's description.\nIt can be multi-line.";

		public Sprite Sprite;

		[ValueDropdown(nameof(GetMaxStackSizes))]
		public int MaxStack = 16;

		[ValueDropdown(nameof(GetMaxUses))]
		public int MaxUses = -1;

		[Space, HideIf(nameof(IsWearable))]
		public bool IsEquippable;

		[ShowIf(nameof(IsEquippable))]
		public InteractionType InteractionType;

		[ShowIf(nameof(IsEquippable))]
		public float InteractPlaybackFactor = 1f;

		[ShowIf(nameof(IsEquippable))]
		public int Damage = 1;

		[ShowIf(nameof(IsEquippable))]
		public EventReference InteractSound;

		[Space, HideIf(nameof(IsEquippable))]
		public bool IsWearable;

		[ShowIf(nameof(IsWearable))]
		public AttireType AttireType;

		[ShowIf(nameof(IsWearable))]
		public float DamageReduction = 0f;

		private static IEnumerable<int> GetMaxStackSizes()
		{
			return new int[]
			{
				1, 8, 16, 32
			};
		}

		private static IEnumerable<int> GetMaxUses()
		{
			return new int[]
			{
				-1, 1, 8, 16, 32, 64, 128, 256, 512, 1024
			};
		}
	}
}
