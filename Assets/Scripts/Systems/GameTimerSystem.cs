using System.Runtime.InteropServices;
using TMPro;
using Unity.Entities;
using UnityEngine;

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
    private GameObject _gameOverScreen;

    public void Setup(TextMeshProUGUI timeLeft, GameObject gameOverScreen)
    {
        _timeLeft = timeLeft;
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
                _gameOverScreen.SetActive(true);
            }
        }
    }
}