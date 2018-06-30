using Unity.Entities;
using Unity.Transforms;

public class SwapChipsSystem : ComponentSystem
{
    public struct SelectedChips
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<Selected> Selected;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<SlotReference> SlotReference;
    }

    [Inject] private SelectedChips _selected;

    protected override void OnUpdate()
    {
        if (_selected.Length == 2)
        {
            var tempSlotReference = _selected.SlotReference[0];
            _selected.SlotReference[0] = _selected.SlotReference[1];
            _selected.SlotReference[1] = tempSlotReference;

            var firstSlot = _selected.SlotReference[0].Value;
            EntityManager.SetComponentData(firstSlot, new ChipReference() {Value = _selected.Entities[0]});

            var secondSlot = _selected.SlotReference[1].Value;
            EntityManager.SetComponentData(secondSlot, new ChipReference() {Value = _selected.Entities[1]});

            var position1 = _selected.Position[0].Value;
            var position2 = _selected.Position[1].Value;

            PostUpdateCommands.AddComponent(_selected.Entities[0], new TargetPosition()
            {
                Value = position2
            });
            PostUpdateCommands.AddComponent(_selected.Entities[0], new AnimationTime());

            PostUpdateCommands.AddComponent(_selected.Entities[1], new TargetPosition()
            {
                Value = position1
            });
            PostUpdateCommands.AddComponent(_selected.Entities[1], new AnimationTime());
        }

        if (_selected.Length >= 2)
        {
            for (int i = _selected.Length - 1; i >= 0; i--)
            {
                PostUpdateCommands.RemoveComponent<Selected>(_selected.Entities[i]);
            }
        }
    }
}