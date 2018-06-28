using System;
using Unity.Entities;

[Serializable]
public struct Tile : IComponentData
{
    public int Color;
}