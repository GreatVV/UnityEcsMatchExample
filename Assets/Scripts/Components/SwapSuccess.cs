using System;
using Unity.Entities;

[Serializable]
public struct SwapSuccess : IComponentData
{
    public SwapResult Value;
}

public enum SwapResult
{
    Success = 0,
    Fail = 1
}