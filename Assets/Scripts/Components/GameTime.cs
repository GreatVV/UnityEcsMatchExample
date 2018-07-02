using System;
using Unity.Entities;

[Serializable]
public struct GameTime : IComponentData
{
    public float Seconds;
}