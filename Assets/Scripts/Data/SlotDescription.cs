using System;
using Unity.Mathematics;

namespace UndergroundMatch3.Data
{
    [Serializable]
    public class SlotDescription
    {
        public int2 Position;
        public bool Generator;
        public bool Blocked;
    }
}