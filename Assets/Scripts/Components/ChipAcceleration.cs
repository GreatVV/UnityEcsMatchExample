using System;
using Unity.Entities;

[Serializable]
public struct ChipAcceleration : IComponentData
{
    public float Value;
}