using System;
using Unity.Entities;

[Serializable]
public struct Generator : IComponentData
{
    public ChipColor ChipColor;
}