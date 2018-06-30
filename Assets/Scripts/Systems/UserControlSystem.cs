using System.Runtime.Serialization.Formatters;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class UserControlSystem : ComponentSystem
{
    private Camera Camera => _game.Camera;
    private Game _game;

    public struct NotSelectedTilesData
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly] public ComponentDataArray<SlotReference> Slots;
        [ReadOnly] public ComponentDataArray<Chip> Tiles;
    }

    public struct SelectedTilesData
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly] public ComponentDataArray<Chip> Tiles;
        [ReadOnly] public ComponentDataArray<Selected> Selected;
    }

    public struct MovingTiles
    {
        public int Length;
        [ReadOnly] public ComponentDataArray<TargetPosition> TargetPosition;
    }

    [Inject] private NotSelectedTilesData _nonSelected;
    [Inject] private SelectedTilesData _selected;
    [Inject] private MovingTiles _movingTiles;

    protected override void OnCreateManager(int capacity)
    {
    }

    public void Setup(Game game)
    {
        _game = game;
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_movingTiles.Length > 0)
            {
                return;
            }

            var worldPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
            var clickPosition = _game.GetIndex(worldPosition);
            for (int i = 0; i < _nonSelected.Length; i++)
            {
                var position = EntityManager.GetComponentData<SlotPosition>(_nonSelected.Slots[i].Value).Value;
                if (position.x == clickPosition.x && position.y == clickPosition.y)
                {
                    var entity = _nonSelected.Entities[i];
                    if (EntityManager.HasComponent(entity, ComponentType.Create<Selected>()))
                    {
                        EntityManager.RemoveComponent<Selected>(entity);
                    }
                    else
                    {
                        if (_selected.Length == 0)
                        {
                            EntityManager.AddComponentData(entity, new Selected()
                            {
                                Number = 1
                            });
                        }
                        else
                        {
                            //check if next to previous slot;
                            var previousSelected = _selected.Entities[0];
                            var previousSelectedSlot = EntityManager.GetComponentData<SlotReference>(previousSelected).Value;
                            var previousSelectedPosition = EntityManager.GetComponentData<SlotPosition>(previousSelectedSlot);

                            if (FieldUtils.NextToEachOther(previousSelectedPosition.Value, position))
                            {

                                EntityManager.AddComponentData(entity, new Selected()
                                {
                                    Number = 2
                                });
                            }
                            else
                            {
                                PostUpdateCommands.RemoveComponent<Selected>(previousSelected);
                            }
                        }
                    }
                    break;
                }
            }
        }
    }
}