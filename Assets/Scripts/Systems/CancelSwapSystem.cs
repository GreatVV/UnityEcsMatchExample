using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CancelSwapSystem : ComponentSystem
{
    public struct SwapFinishedData
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<PlayerSwap> Swaps;
        public ComponentDataArray<SwapSuccess> SwapsSuccess;
    }

    [Inject] private SwapFinishedData _swaps;

    protected override void OnUpdate()
    {
        for (int i = 0; i < _swaps.Length; i++)
        {
            var playerSwap = _swaps.Swaps[i];
            if (_swaps.SwapsSuccess[i].Value == SwapResult.Fail)
            {
                var slot1 = EntityManager.GetComponentData<SlotReference>(playerSwap.First).Value;
                var slot2 = EntityManager.GetComponentData<SlotReference>(playerSwap.Second).Value;

                PostUpdateCommands.SetComponent(playerSwap.First, new SlotReference(slot2));
                PostUpdateCommands.SetComponent(slot1, new ChipReference(playerSwap.Second));
                if (!EntityManager.HasComponent<TargetPosition>(playerSwap.First))
                {
                    PostUpdateCommands.AddComponent(playerSwap.First,
                        new TargetPosition(EntityManager.GetComponentData<Position>(slot2).Value));
                }
                else
                {
                    PostUpdateCommands.SetComponent(playerSwap.First,
                        new TargetPosition(EntityManager.GetComponentData<Position>(slot2).Value));
                }

                PostUpdateCommands.SetComponent(playerSwap.Second, new SlotReference(slot1));
                PostUpdateCommands.SetComponent(slot2, new ChipReference(playerSwap.First));
                if (!EntityManager.HasComponent<TargetPosition>(playerSwap.Second))
                {
                    PostUpdateCommands.AddComponent(playerSwap.Second,
                        new TargetPosition(EntityManager.GetComponentData<Position>(slot1).Value));
                }
                else
                {
                    PostUpdateCommands.SetComponent(playerSwap.Second,
                        new TargetPosition(EntityManager.GetComponentData<Position>(slot1).Value));
                }
            }
            PostUpdateCommands.AddComponent(_swaps.Entities[i], new DestroyMarker());
        }
    }
}