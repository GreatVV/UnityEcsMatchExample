using System.Collections.Generic;
using UndergroundMatch3.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace UndergroundMatch3.Data.Steps
{
    public class CreateSlotsStep : ICreationPipelineStep
    {
        private readonly Dictionary<int2, Entity> _slotCache;
        private readonly Vector3 _centerPosition;

        public CreateSlotsStep(Dictionary<int2, Entity> slotCache, SceneConfiguration sceneConfiguration)
        {
            _slotCache = slotCache;
            _centerPosition = sceneConfiguration.Center.position;
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

                    var slotDescription = levelDescription.GetSlotDescription(position);
                    if (slotDescription.Generator)
                    {
                        entityManager.AddComponentData(slot, new Generator());
                    }
                }
            }
        }
    }
}