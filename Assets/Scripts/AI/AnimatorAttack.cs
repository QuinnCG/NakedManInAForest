using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI
{
	public class AnimatorAttack : MonoBehaviour
	{
		[SerializeField, Required]
		private EnemyController Enemy;

		public void Attack()
		{
			Enemy.DamageTarget();
		}
	}
}
