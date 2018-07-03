using System;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct ChipReference : IComponentData
    {
        public Entity Value;

        public ChipReference(Entity value)
        {
            Value = value;
        }
    }
}