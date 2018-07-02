using TMPro;
using Unity.Entities;
using UnityEngine.AI;

public class SyncScoreSystem : ComponentSystem
{
    private TextMeshProUGUI _scoreLabel;

    public void Setup(TextMeshProUGUI scoreLabel)
    {
        _scoreLabel = scoreLabel;
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