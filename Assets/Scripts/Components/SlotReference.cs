using System;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct SlotReference : IComponentData
    {
        public Entity Value;

        public SlotReference(Entity value)
        {
            Value = value;
        }
    }
}