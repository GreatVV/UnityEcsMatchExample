using System;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct SlotPosition : IComponentData
{
    public int2 Value;
}