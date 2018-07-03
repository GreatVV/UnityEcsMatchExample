using TMPro;
using UndergroundMatch3.Components;
using UndergroundMatch3.UI.Screens;
using Unity.Entities;

namespace UndergroundMatch3.Systems
{
    public class SyncScoreSystem : ComponentSystem
    {
        private TextMeshProUGUI _scoreLabel;

        public void Setup(GameScreen gameScreen)
        {
            _scoreLabel = gameScreen.ScoreLabel;
        }

        public struct ScoreScore
        {
            public int Length;
            public ComponentDataArray<Score> Score;
        }
        [Inject] private ScoreScore _score;

        private int _prevScore = -1;

        protected override void OnUpdate()
        {
            if (_score.Length > 0)
            {
                var value = _score.Score[0].Value;
                if (value != _prevScore)
                {
                    _scoreLabel.text = value.ToString();
                }
            }
        }
    }
}