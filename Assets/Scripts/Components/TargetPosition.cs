using System;
using Unity.Entities;
using Unity.Mathematics;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct TargetPosition : IComponentData
    {
        public float3 Value;

        public TargetPosition(float3 value)
        {
            Value = value;
        }
    }
}