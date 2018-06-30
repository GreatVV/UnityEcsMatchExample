using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TargetPosition : IComponentData
{
    public float3 Value;

    public TargetPosition(float3 value)
    {
        Value = value;
    }
}