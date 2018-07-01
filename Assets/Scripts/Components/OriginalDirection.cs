using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct OriginalDirection : IComponentData
{
    public float3 Value;
}