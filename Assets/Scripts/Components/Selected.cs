using System;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct Selected : IComponentData
    {
        public int Number;
    }
}