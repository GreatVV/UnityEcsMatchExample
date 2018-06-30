using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GeneratorSystem : ComponentSystem
{
    public struct SlotGenerators
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<Slot> Slots;
        public ComponentDataArray<Generator> Generators;
        public SubtractiveComponent<ChipReference> ChipReference;
        public ComponentDataArray<SlotPosition> Positions;
    }

    [Inject] private SlotGenerators _slotGenerators;
    private GameObjectEntity[] _chipPrefabs;
    private LevelDescription _levelDescription;
    private Dictionary<int2, Entity> _slotCache;

    public void Setup(GameObjectEntity[] chipPrefabs, Dictionary<int2, Entity> slotCache, LevelDescription levelDescription)
    {
        _slotCache = slotCache;
        _levelDescription = levelDescription;
        _chipPrefabs = chipPrefabs;
    }

    protected override void OnUpdate()
    {
        for (int i = 0; i < _slotGenerators.Length; i++)
        {
            var colorsCount = Mathf.Min(_chipPrefabs.Length, _levelDescription.ColorCount);
            var color = Random.Range(0, colorsCount);
            var slot = _slotGenerators.Entities[i];

            var slotPosition = _slotGenerators.Positions[i].Value;

            var fall = slotPosition.y - FallSystem.GetNextEmptyRow(EntityManager, _slotCache, slotPosition);

            var chip = EntityManager.Instantiate(_chipPrefabs[color].gameObject);
            EntityManager.AddComponentData(chip, new SlotReference()
            {
                Value = slot
            });
            var position = EntityManager.GetComponentData<Position>(slot).Value;
            EntityManager.SetComponentData(chip, new Position()
            {
                Value = position + new int3(0, fall, 0)
            });
            EntityManager.AddComponentData(slot, new ChipReference()
            {
                Value = chip
            });

            break;
        }

        PostUpdateCommands.CreateEntity();
        PostUpdateCommands.AddComponent(new AnalyzeField());
    }
}