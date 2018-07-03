using System;
using UndergroundMatch3.Data;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct Chip : IComponentData
    {
        public ChipColor Color;
    }
}