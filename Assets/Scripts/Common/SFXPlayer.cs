using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public class SFXPlayer : MonoBehaviour
	{
		public void PlaySound(string name)
		{
			if (!name.StartsWith("event:/"))
			{
				name = $"event:/SFX/{name}";
			}

			RuntimeManager.PlayOneShot(name, transform.position);
		}

		public void PlaySoundAttached(string name)
		{
			if (!name.StartsWith("event:/"))
			{
				name = $"event:/SFX/{name}";
			}

			RuntimeManager.PlayOneShotAttached(name, gameObject);
		}
	}
}
