using Unity.Entities;

public class UpdateScoreSystem : ComponentSystem
{
    public struct DyingDeadChips
    {
        public int Length;
        public ComponentDataArray<Dying> Dying;
        public ComponentDataArray<DestroyMarker> Destroy;
        public EntityArray EntityArray;
    }

    public struct ScoreScore
    {
        public int Length;
        public ComponentDataArray<Score> Score;
    }

    [Inject] private DyingDeadChips _deadChips;
    [Inject] private ScoreScore _score;

    protected override void OnUpdate()
    {
        if (_score.Length > 0 && _deadChips.Length > 0)
        {
            var score = _score.Score[0];
            score.Value += _deadChips.Length;
            _score.Score[0] = score;
        }
    }
}