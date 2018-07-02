using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SwapChipsSystem : ComponentSystem
{
    public struct SelectedChips
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<Selected> Selected;
        [ReadOnly]
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<SlotReference> SlotReference;
        public SubtractiveComponent<TargetPosition> TargetPosition;
    }

    [Inject] private SelectedChips _selectedChips;

    protected override void OnUpdate()
    {
        if (_selectedChips.Length == 2)
        {
            var firstChip = _selectedChips.Entities[0];
            var secondChip = _selectedChips.Entities[1];

            PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent(
                new PlayerSwap()
                {
                    First = firstChip,
                    Second = secondChip
                });

            var tempSlotReference = _selectedChips.SlotReference[0];
            _selectedChips.SlotReference[0] = _selectedChips.SlotReference[1];
            _selectedChips.SlotReference[1] = tempSlotReference;

            var firstSlot = _selectedChips.SlotReference[0].Value;
            EntityManager.SetComponentData(firstSlot, new ChipReference(firstChip));

            var secondSlot = _selectedChips.SlotReference[1].Value;
            EntityManager.SetComponentData(secondSlot, new ChipReference(secondChip));

            var position1 = _selectedChips.Position[0].Value;
            var position2 = _selectedChips.Position[1].Value;


            PostUpdateCommands.AddComponent(firstChip, new TargetPosition(position2));
            PostUpdateCommands.AddComponent(secondChip, new TargetPosition(position1));
        }

        if (_selectedChips.Length >= 2)
        {
            for (int i = _selectedChips.Length - 1; i >= 0; i--)
            {
                PostUpdateCommands.RemoveComponent<Selected>(_selectedChips.Entities[i]);
            }
        }
    }
}