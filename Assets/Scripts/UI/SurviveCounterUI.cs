using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Quinn.UI
{
	public class SurviveCounterUI : MonoBehaviour
	{
		[SerializeField, Required]
		private TextMeshProUGUI Text;

		private float _startTime;

		private void Start()
		{
			_startTime = Time.time;
		}

		private void Update()
		{
			string time = ((Time.time - _startTime) / 60f).ToString("0.0");
			Text.text = $"Survived: {time} min.";
		}
	}
}
