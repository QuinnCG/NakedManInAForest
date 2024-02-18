using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public class SFXPlayer : MonoBehaviour
	{
		[SerializeField]
		private float SoundInterval = 0.1f;

		private float _nextPlayTime;

		public void PlaySound(string name)
		{
			if (Time.time < _nextPlayTime) return;
			_nextPlayTime = Time.time + SoundInterval;

			if (!name.StartsWith("event:/"))
			{
				name = $"event:/SFX/{name}";
			}

			RuntimeManager.PlayOneShot(name, transform.position);
		}

		public void PlaySoundAttached(string name)
		{
			if (Time.time < _nextPlayTime) return;
			_nextPlayTime = Time.time + SoundInterval;

			if (!name.StartsWith("event:/"))
			{
				name = $"event:/SFX/{name}";
			}

			RuntimeManager.PlayOneShotAttached(name, gameObject);
		}
	}
}
