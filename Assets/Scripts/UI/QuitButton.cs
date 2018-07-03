using UnityEngine;
using UnityEngine.UI;

namespace UndergroundMatch3.UI
{
    public class QuitButton : MonoBehaviour
    {
        public Button Button;

        void Start()
        {
            Button.onClick.AddListener(Click);
        }

        private void OnValidate()
        {
            if (!Button)
            {
                Button = GetComponentInChildren<Button>();
            }
        }


        private void Click()
        {
            Application.Quit();
        }
    }
}