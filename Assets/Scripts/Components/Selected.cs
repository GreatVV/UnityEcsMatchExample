using System;
using System.ComponentModel;
using Unity.Entities;

[Serializable]
public struct Selected : IComponentData
{
    public int Number;
}