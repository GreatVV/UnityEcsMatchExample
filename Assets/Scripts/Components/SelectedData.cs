using System;
using System.ComponentModel;
using Unity.Entities;

[Serializable]
public struct SelectedData : IComponentData
{
    public int Number;
}