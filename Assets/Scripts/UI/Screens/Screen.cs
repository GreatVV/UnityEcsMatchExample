using UnityEngine;

namespace UndergroundMatch3.UI.Screens
{
	public class Screen : MonoBehaviour
	{
		public virtual void Show(bool state)
		{
			gameObject.SetActive(state);
		}
	}
}
