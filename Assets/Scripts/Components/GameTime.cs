using System;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct GameTime : IComponentData
    {
        public float Seconds;
    }
}