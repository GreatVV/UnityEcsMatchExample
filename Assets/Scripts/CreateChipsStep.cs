using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CreateChipsStep : ICreationPipelineStep
{
    private readonly GameObjectEntity[] _chipsPrefabs;

    public CreateChipsStep(GameObjectEntity[] chipsPrefabs)
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
        var chip = entityManager.Instantiate(_chipsPrefabs[(int) chipColor].gameObject);
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