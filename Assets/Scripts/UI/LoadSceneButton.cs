using System.Collections;
using UndergroundMatch3.UI.Screens;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UndergroundMatch3.UI
{
    public class LoadSceneButton : MonoBehaviour
    {
        public string LoadingSceneName;
        public string SceneName;

        public Button Button;

        private bool _loading;

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
            if (!_loading)
            {
                _loading = true;
                StartCoroutine(LoadSceneCoroutine());
            }
        }

        private IEnumerator LoadSceneCoroutine()
        {
            var previousScene = SceneManager.GetActiveScene().name;

            yield return SceneManager.LoadSceneAsync(LoadingSceneName, LoadSceneMode.Additive);

            var loadingScreen = LoadingScreen.Instance;

            loadingScreen.Show(true);

            while (loadingScreen.Director.state == PlayState.Playing)
            {
                yield return null;
            }

            yield return null;

            loadingScreen.LoadScene(SceneName);

            yield return SceneManager.UnloadSceneAsync(previousScene);

            _loading = false;
        }
    }
}