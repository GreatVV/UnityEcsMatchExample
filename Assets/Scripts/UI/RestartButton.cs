using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UndergroundMatch3.UI
{
    public class RestartButton : MonoBehaviour
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}