using UndergroundMatch3.Components;
using Unity.Collections;
using Unity.Entities;

namespace UndergroundMatch3.Systems
{
    public static class SystemsUtils
    {
        public struct FinishedSwaps
        {
            public int Length;
            public EntityArray Entities;
            public ComponentDataArray<PlayerSwap> Swaps;
            public ComponentDataArray<SwapSuccess> SwapsSuccess;
        }

        public struct NotFinishedSwaps
        {
            public int Length;
            public EntityArray Entities;
            public ComponentDataArray<PlayerSwap> Swaps;
            public SubtractiveComponent<SwapSuccess> SwapsSuccess;
        }

        public struct MarkedForDestroy
        {
            public int Length;
            public EntityArray Entities;
            [ReadOnly]
            public ComponentDataArray<DestroyMarker> Destroy;
        }

        public struct MovingChips
        {
            public int Length;
            [ReadOnly]
            public ComponentDataArray<TargetPosition> TargetPositions;
        }

        public struct ScoreScore
        {
            public int Length;
            public ComponentDataArray<Score> Score;
        }

        public struct DyingDeadChips
        {
            public int Length;
            public ComponentDataArray<Dying> Dying;
            public ComponentDataArray<DestroyMarker> Destroy;
            public EntityArray EntityArray;
        }
    }
}