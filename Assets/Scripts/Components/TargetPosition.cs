using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TargetPosition : IComponentData
{
    public float3 Value;
}

[Serializable]
public struct AnimationTime : IComponentData
{
    public float Value;
}

[Serializable]
public struct SwapFinished : IComponentData
{

}