using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CreateSlotsStep : ICreationPipelineStep
{
    public void Apply(LevelDescription levelDescription, EntityManager entityManager)
    {
        var slotArchitype = entityManager.CreateArchetype(typeof(SlotPosition), typeof(Slot));

        for (int x = 0; x < levelDescription.Width; x++)
        {
            for (int y = 0; y < levelDescription.Height; y++)
            {
                var slot = entityManager.CreateEntity(slotArchitype);
                entityManager.SetComponentData(slot, new SlotPosition() {Value = new int2(x, y)});
                entityManager.SetComponentData(slot, new Slot());
            }
        }
    }
}

public class CreateChipsStep : ICreationPipelineStep
{
    private readonly GameObject[] _chipsPrefabs;
    private readonly Vector3 _centerPosition;

    public CreateChipsStep(GameObject[] chipsPrefabs, Vector3 centerPosition)
    {
        _chipsPrefabs = chipsPrefabs;
        _centerPosition = centerPosition;
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
                var position = FieldUtils.GetPosition(x, y, levelDescription.Width, levelDescription.Height,_centerPosition);
                var slot = entities[i];
                var chipDescription = levelDescription.GetChipDescription(slotPosition.Value);
                var colorsCount = Mathf.Min(_chipsPrefabs.Length, levelDescription.ColorCount);
                var color = (chipDescription == null || chipDescription.Color == ChipColor.Random) ? (ChipColor) Random.Range(0, colorsCount) : chipDescription.Color;

				var chip = CreateChip(entityManager, slot, position, color );
                entityManager.AddComponentData(slot, new ChipReference()
				{
					Value = chip
				});

                entityManager.AddComponentData(slot, new PossibleNeighbours()
				{
					Value = FieldUtils.GetNeighbour(x,y, levelDescription.Width, levelDescription.Height)
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