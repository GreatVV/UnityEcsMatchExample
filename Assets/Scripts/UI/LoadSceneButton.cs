using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UndergroundMatch3.UI
{
    public class LoadSceneButton : MonoBehaviour
    {
        public string SceneName;
    
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
            SceneManager.LoadScene(SceneName);
        }
    }
}