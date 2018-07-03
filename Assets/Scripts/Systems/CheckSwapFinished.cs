using UndergroundMatch3.Components;
using Unity.Entities;

namespace UndergroundMatch3.Systems
{
    public class CheckSwapFinished : ComponentSystem
    {
        [Inject] private SystemsUtils.NotFinishedSwaps _notFinishedSwaps;

        protected override void OnUpdate()
        {
            for (int i = 0; i < _notFinishedSwaps.Length; i++)
            {
                var swap = _notFinishedSwaps.Swaps[i];
                var first = swap.First;
                var second = swap.Second;
                if (!EntityManager.HasComponent<TargetPosition>(first) &&
                    !EntityManager.HasComponent<TargetPosition>(second))
                {
                    PostUpdateCommands.CreateEntity();
                    PostUpdateCommands.AddComponent(new AnalyzeField());
                }
            }
        }
    }
}