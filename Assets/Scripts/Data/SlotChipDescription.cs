using System;
using Unity.Mathematics;

namespace UndergroundMatch3.Data
{
    [Serializable]
    public class SlotChipDescription
    {
        public int2 Position;
        public ChipType ChipType;
        public ChipColor Color = ChipColor.Random;
    }
}