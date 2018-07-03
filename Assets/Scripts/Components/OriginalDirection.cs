using System;
using Unity.Entities;
using Unity.Mathematics;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct OriginalDirection : IComponentData
    {
        public float3 Value;
    }
}