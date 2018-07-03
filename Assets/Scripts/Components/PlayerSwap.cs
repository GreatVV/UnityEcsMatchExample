using System;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct PlayerSwap : IComponentData
    {
        public Entity First;
        public Entity Second;
    }
}