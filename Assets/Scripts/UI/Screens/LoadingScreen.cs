using System.Collections;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace UndergroundMatch3.UI.Screens
{
    public class LoadingScreen : Screen
    {
        public static LoadingScreen Instance;

        public PlayableDirector Director;
        public PlayableAsset ShowAnimation;
        public PlayableAsset HideAnimation;

        void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override void Show(bool state)
        {
            if (state)
            {
                gameObject.SetActive(true);
                Director.Play(ShowAnimation);
            }
            else
            {
                Director.Play(HideAnimation);
            }
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(Load(sceneName));
        }

        IEnumerator Load(string sceneName)
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loadSceneAsync.allowSceneActivation = true;
            yield return loadSceneAsync;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            Show(false);

            while (Director.state == PlayState.Playing)
            {
                yield return null;
            }

            yield return SceneManager.UnloadSceneAsync(gameObject.scene.name);
        }
    }
}