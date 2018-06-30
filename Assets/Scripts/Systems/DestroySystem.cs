using Unity.Collections;
using Unity.Entities;


public class DestroySystem : ComponentSystem
{
    public struct SwapFinishedData
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly]
        public ComponentDataArray<DestroyData> Destroy;
    }

    [Inject] private SwapFinishedData _destroyTargets;

    protected override void OnUpdate()
    {
        for (int i = 0; i < _destroyTargets.Length; i++)
        {
            PostUpdateCommands.DestroyEntity(_destroyTargets.Entities[i]);
        }
    }
}