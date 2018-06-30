using System;
using Unity.Entities;

[Serializable]
public struct PossibleNeighbours : IComponentData
{
    public Neighbours Value;
}