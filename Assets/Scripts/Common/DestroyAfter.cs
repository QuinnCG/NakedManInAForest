using UnityEngine;

namespace Quinn
{
	public class DestroyAfter : MonoBehaviour
	{
		public float Delay = 1f;

		private void Start()
		{
			Destroy(gameObject, Delay);
		}
	}
}
