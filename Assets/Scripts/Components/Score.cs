using System;
using Unity.Entities;

[Serializable]
public struct Score : IComponentData
{
    public int Value;
}