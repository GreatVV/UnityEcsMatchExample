using UnityEngine;

namespace UndergroundMatch3.UI.Screens
{
	public class Screen : MonoBehaviour
	{
		public void Show(bool state)
		{
			gameObject.SetActive(state);
		}
	}
}
