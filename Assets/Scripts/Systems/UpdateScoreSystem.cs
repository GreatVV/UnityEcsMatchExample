using UndergroundMatch3.Components;
using Unity.Entities;

namespace UndergroundMatch3.Systems
{
    public class UpdateScoreSystem : ComponentSystem
    {
        [Inject] private SystemsUtils.DyingDeadChips _deadChips;
        [Inject] private SystemsUtils.ScoreScore _score;

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
}