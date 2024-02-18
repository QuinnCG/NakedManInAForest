using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Quinn
{
	public class DestroyAfter : MonoBehaviour
	{
		private static bool _firstSpawn = true;

		public float Delay = 1f;

		private void Start()
		{
			if (_firstSpawn)
			{
				_firstSpawn = false;
			}
			else
			{
				Destroy(gameObject);
				return;
			}

			DOVirtual.DelayedCall(Delay, () =>
			{
				GetComponent<TextMeshProUGUI>().DOFade(0f, 1f).onComplete += () =>
				{
					Destroy(gameObject, Delay);
				};
			}, ignoreTimeScale: false);
		}
	}
}
