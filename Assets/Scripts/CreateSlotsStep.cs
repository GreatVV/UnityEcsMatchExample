using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CreateSlotsStep : ICreationPipelineStep
{
    private readonly Dictionary<int2, Entity> _slotCache;
    private readonly Vector3 _centerPosition;

    public CreateSlotsStep(Dictionary<int2, Entity> slotCache, Vector3 centerPosition)
    {
        _slotCache = slotCache;
        _centerPosition = centerPosition;
    }

    public void Apply(LevelDescription levelDescription, EntityManager entityManager)
    {
        var slotArchitype = entityManager.CreateArchetype(typeof(SlotPosition), typeof(Slot), typeof(Position));

        for (int x = 0; x < levelDescription.Width; x++)
        {
            for (int y = 0; y < levelDescription.Height; y++)
            {
                var slot = entityManager.CreateEntity(slotArchitype);
                var position = new int2(x, y);
                entityManager.SetComponentData(slot, new SlotPosition() {Value = position});
                entityManager.SetComponentData(slot, new Slot());
                entityManager.SetComponentData(slot, new Position() {Value = FieldUtils.GetPosition(x,y, levelDescription.Width, levelDescription.Height, _centerPosition )});
                _slotCache[position] = slot;
            }
        }
    }
}

public class CreateChipsStep : ICreationPipelineStep
{
    private readonly GameObject[] _chipsPrefabs;

    public CreateChipsStep(GameObject[] chipsPrefabs)
    {
        _chipsPrefabs = chipsPrefabs;
    }

    public void Apply(LevelDescription levelDescription, EntityManager entityManager)
    {
        var entities = entityManager.GetAllEntities();
        for (int i = 0; i < entities.Length; i++)
        {
            if (entityManager.HasComponent<Slot>(entities[i]))
            {
                var slotPosition = entityManager.GetComponentData<SlotPosition>(entities[i]);
                var x = slotPosition.Value.x;
                var y = slotPosition.Value.y;
                var position = entityManager.GetComponentData<Position>(entities[i]).Value;
                var slot = entities[i];
                entityManager.AddComponentData(slot, new PossibleNeighbours()
                {
                    Value = FieldUtils.GetNeighbour(x,y, levelDescription.Width, levelDescription.Height)
                });

                var chipDescription = levelDescription.GetChipDescription(slotPosition.Value);
                if (chipDescription.ChipType == ChipType.None)
                {
                    continue;
                }

                var colorsCount = Mathf.Min(_chipsPrefabs.Length, levelDescription.ColorCount);
                var color = (chipDescription.Color == ChipColor.Random) ? (ChipColor) Random.Range(0, colorsCount) : chipDescription.Color;

				var chip = CreateChip(entityManager, slot, position, color );
                entityManager.AddComponentData(slot, new ChipReference()
				{
					Value = chip
				});
            }
        }
        entities.Dispose();

    }

    public Entity CreateChip(EntityManager entityManager, Entity slot, float3 position, ChipColor chipColor)
    {
        var chip = entityManager.Instantiate(_chipsPrefabs[(int) chipColor]);
        entityManager.AddComponentData(chip, new SlotReference()
        {
            Value = slot
        });
        entityManager.SetComponentData(chip, new Position()
        {
            Value = position
        });
        return chip;
    }
}