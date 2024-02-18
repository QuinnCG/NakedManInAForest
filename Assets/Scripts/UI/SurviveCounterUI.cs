using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Quinn.UI
{
	public class SurviveCounterUI : MonoBehaviour
	{
		[SerializeField, Required]
		private TextMeshProUGUI Text;

		private void Update()
		{
			string time = (Time.time / 60f).ToString("0.0");
			Text.text = $"Survived: {time}m";
		}
	}
}
