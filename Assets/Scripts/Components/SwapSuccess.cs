using System;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct SwapSuccess : IComponentData
    {
        public SwapResult Value;
    }
}