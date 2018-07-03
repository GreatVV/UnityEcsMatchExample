using UndergroundMatch3.Data;
using Unity.Entities;

namespace UndergroundMatch3.Components
{
    public class ChipComponent : ComponentDataWrapper<Chip>
    {
        public void UpdateColor(ChipColor color)
        {
            Value = new Chip()
            {
                Color = color
            };
        }
    }
}