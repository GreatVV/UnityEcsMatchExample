using UndergroundMatch3.Components;
using Unity.Entities;
using Unity.Transforms;

namespace UndergroundMatch3.Systems
{
    public class CancelSwapSystem : ComponentSystem
    {
        [Inject] private SystemsUtils.FinishedSwaps _finishedSwaps;

        protected override void OnUpdate()
        {
            for (int i = 0; i < _finishedSwaps.Length; i++)
            {
                var playerSwap = _finishedSwaps.Swaps[i];
                var pc = PostUpdateCommands;
                var em = EntityManager;

                if (_finishedSwaps.SwapsSuccess[i].Value == SwapResult.Fail)
                {
                    var slot1 = em.GetComponentData<SlotReference>(playerSwap.First).Value;
                    var slot2 = em.GetComponentData<SlotReference>(playerSwap.Second).Value;

                    pc.SetComponent(playerSwap.First, new SlotReference(slot2));
                    pc.SetComponent(slot1, new ChipReference(playerSwap.Second));
                    if (!em.HasComponent<TargetPosition>(playerSwap.First))
                    {
                        pc.AddComponent(playerSwap.First,
                            new TargetPosition(em.GetComponentData<Position>(slot2).Value));
                    }
                    else
                    {
                        pc.SetComponent(playerSwap.First,
                            new TargetPosition(em.GetComponentData<Position>(slot2).Value));
                    }

                    pc.SetComponent(playerSwap.Second, new SlotReference(slot1));
                    pc.SetComponent(slot2, new ChipReference(playerSwap.First));
                    if (!em.HasComponent<TargetPosition>(playerSwap.Second))
                    {
                        pc.AddComponent(playerSwap.Second,
                            new TargetPosition(em.GetComponentData<Position>(slot1).Value));
                    }
                    else
                    {
                        pc.SetComponent(playerSwap.Second,
                            new TargetPosition(em.GetComponentData<Position>(slot1).Value));
                    }
                }
                pc.AddComponent(_finishedSwaps.Entities[i], new DestroyMarker());
            }
        }
    }
}