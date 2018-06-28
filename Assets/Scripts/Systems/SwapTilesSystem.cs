using Unity.Entities;
using Unity.Transforms;

public class SwapTilesSystem : ComponentSystem
{
    public struct SelectedTilesData
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<SelectedData> Selected;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<SlotData> SlotData;
    }

    [Inject] private SelectedTilesData _selected;

    protected override void OnUpdate()
    {
        if (_selected.Length == 2)
        {
            var slotId = _selected.SlotData[0];
            _selected.SlotData[0] = _selected.SlotData[1];
            _selected.SlotData[1] = slotId;

            var position1 = _selected.Position[0].Value;
            var position2 = _selected.Position[1].Value;

            PostUpdateCommands.AddComponent(_selected.Entities[0], new TargetPosition()
            {
                Value = position2
            });

            PostUpdateCommands.AddComponent(_selected.Entities[1], new TargetPosition()
            {
                Value = position1
            });
        }

        if (_selected.Length >= 2)
        {
            for (int i = _selected.Length - 1; i >= 0; i--)
            {
                PostUpdateCommands.RemoveComponent<SelectedData>(_selected.Entities[i]);
            }
        }
    }
}