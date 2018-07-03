using System;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct ChipAcceleration : IComponentData
    {
        public float Value;
    }
}