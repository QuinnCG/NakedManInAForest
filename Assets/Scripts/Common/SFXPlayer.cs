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

			var sound = EventReference.Find(name);

			if (sound.IsNull)
			{
				throw new System.NullReferenceException($"Cannot find event by the name: {name}!");
			}

			RuntimeManager.PlayOneShot(sound, transform.position);
		}

		public void PlaySoundAttached(string name)
		{
			if (!name.StartsWith("event:/"))
			{
				name = $"event:/SFX/{name}";
			}

			var sound = EventReference.Find(name);

			if (sound.IsNull)
			{
				throw new System.NullReferenceException($"Cannot find event by the name: {name}!");
			}

			RuntimeManager.PlayOneShotAttached(sound, gameObject);
		}
	}
}
