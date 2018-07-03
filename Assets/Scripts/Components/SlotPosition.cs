using System;
using Unity.Entities;
using Unity.Mathematics;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct SlotPosition : IComponentData
    {
        public int2 Value;
    }
}