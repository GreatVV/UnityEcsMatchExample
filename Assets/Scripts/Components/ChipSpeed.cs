using System;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct ChipSpeed : IComponentData
    {
        public float Value;
    }
}