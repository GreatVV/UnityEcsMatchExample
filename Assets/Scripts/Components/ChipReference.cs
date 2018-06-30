using System;
using Unity.Entities;

[Serializable]
public struct ChipReference : IComponentData
{
    public Entity Value;
}