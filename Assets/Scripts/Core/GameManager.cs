using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quinn.Core
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var instance = Addressables.InstantiateAsync("GameManager.prefab")
				.WaitForCompletion();

			DontDestroyOnLoad(instance);
		}

		private async void Awake()
		{
			await UnityServices.InitializeAsync();
			AnalyticsService.Instance.StartDataCollection();
		}
	}
}
