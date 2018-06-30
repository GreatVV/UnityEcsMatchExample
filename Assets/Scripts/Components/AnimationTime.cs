using System;
using Unity.Entities;

[Serializable]
public struct AnimationTime : IComponentData
{
    public float Value;
}