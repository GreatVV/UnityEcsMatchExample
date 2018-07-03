using System;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    [Serializable]
    public struct PossibleNeighbours : IComponentData
    {
        public Neighbours Value;
    }
}