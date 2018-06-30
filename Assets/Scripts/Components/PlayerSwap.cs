using System;
using Unity.Entities;

[Serializable]
public struct PlayerSwap : IComponentData
{
    public Entity First;
    public Entity Second;
}