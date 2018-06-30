using Unity.Entities;

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