using System;
using Unity.Mathematics;

[Serializable]
public class SlotChipDescription
{
    public int2 Position;
    public ChipType ChipType;
    public ChipColor Color = ChipColor.Random;
}