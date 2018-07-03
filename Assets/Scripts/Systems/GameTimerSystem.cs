using TMPro;
using UndergroundMatch3.Components;
using UndergroundMatch3.UI.Screens;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Analytics;

namespace UndergroundMatch3.Systems
{
    public class GameTimerSystem : ComponentSystem
    {
        private struct GameTimerGroup
        {
            public int Length;
            public EntityArray Entities;
            public ComponentDataArray<GameTime> TimeLeft;
            public SubtractiveComponent<Pause> Pause;
            public SubtractiveComponent<TimeOver> TimeOver;
        }

        [Inject] private GameTimerGroup _group;
        private TextMeshProUGUI _timeLeft;
        private GameOverScreen _gameOverScreen;

        public void Setup(GameScreen gameScreen, GameOverScreen gameOverScreen)
        {
            _timeLeft = gameScreen.Timer;
            _gameOverScreen = gameOverScreen;
        }


        protected override void OnUpdate()
        {
            var time = Time.deltaTime;
            for (int i = 0; i < _group.Length; i++)
            {
                var timeLeft = _group.TimeLeft[i];
                timeLeft.Seconds -= time;
                _group.TimeLeft[i] = timeLeft;

                _timeLeft.text = ((int) timeLeft.Seconds).ToString();

                if (timeLeft.Seconds < 0)
                {
                    PostUpdateCommands.AddComponent(_group.Entities[i], new Pause());
                    PostUpdateCommands.AddComponent(_group.Entities[i], new TimeOver());
                    _gameOverScreen.Show(true);
                }
            }
        }
    }
}