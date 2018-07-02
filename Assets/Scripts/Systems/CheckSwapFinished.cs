using Unity.Entities;

public class CheckSwapFinished : ComponentSystem
{
    public struct SwapInProgress
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<PlayerSwap> Swaps;
        public SubtractiveComponent<SwapSuccess> SwapsSuccess;
    }

    [Inject] private SwapInProgress _swapInProgress;

    protected override void OnUpdate()
    {
        for (int i = 0; i < _swapInProgress.Length; i++)
        {
            var swap = _swapInProgress.Swaps[i];
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