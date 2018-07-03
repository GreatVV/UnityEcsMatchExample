using System.Collections.Generic;
using UndergroundMatch3.Components;
using UndergroundMatch3.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace UndergroundMatch3.Systems
{
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

        public void Setup(ConfigurationAsset configuration, Dictionary<int2, Entity> slotCache, LevelDescription levelDescription)
        {
            _slotCache = slotCache;
            _levelDescription = levelDescription;
            _chipPrefabs = configuration.ChipPrefabs;
        }

        protected override void OnUpdate()
        {
            for (int i = 0; i < _slotGenerators.Length; i++)
            {
                var colorsCount = Mathf.Min(_chipPrefabs.Length, _levelDescription.ColorCount);
                var color = Random.Range(0, colorsCount);
                var slot = _slotGenerators.Entities[i];

                var slotPosition = _slotGenerators.Positions[i].Value;

                var targetY = FallSystem.GetNextEmptyRow(EntityManager, _slotCache, slotPosition);
                float3 position = EntityManager.GetComponentData<Position>(slot).Value;
                if (targetY > 0)
                {
                    targetY-=1;
                    var oneAbove = _slotCache[new int2(slotPosition.x,targetY)];
                    var chipOneAbove = EntityManager.GetComponentData<ChipReference>(oneAbove).Value;
                    position = EntityManager.GetComponentData<Position>(chipOneAbove).Value + new float3(0, 1, 0);
                }



                var chip = EntityManager.Instantiate(_chipPrefabs[color].gameObject);
                EntityManager.AddComponentData(chip, new SlotReference()
                {
                    Value = slot
                });
                EntityManager.SetComponentData(chip, new Position()
                {
                    Value = position
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
}