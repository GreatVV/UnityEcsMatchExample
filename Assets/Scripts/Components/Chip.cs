using System;
using Unity.Entities;

[Serializable]
public struct Chip : IComponentData
{
    public ChipColor Color;
}