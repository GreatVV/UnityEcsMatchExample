using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class UserControlSystem : ComponentSystem
{
    private Camera _camera;
    private Game _game;

    public struct NotSelectedTilesData
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly] public ComponentDataArray<SlotData> Slots;
        [ReadOnly] public ComponentDataArray<Tile> Tiles;
    }

    public struct SelectedTilesData
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly] public ComponentDataArray<Tile> Tiles;
        [ReadOnly] public ComponentDataArray<SelectedData> Selected;
    }

    [Inject] private NotSelectedTilesData _nonSelected;
    [Inject] private SelectedTilesData _selected;

    protected override void OnCreateManager(int capacity)
    {
    }

    public void Setup()
    {
        _game = Object.FindObjectOfType<Game>();
        _camera = Camera.main;
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            var slotId = _game.GetIndex(worldPosition);
            for (int i = 0; i < _nonSelected.Length; i++)
            {
                if (_nonSelected.Slots[i].Id == slotId)
                {
                    var entity = _nonSelected.Entities[i];
                    if (EntityManager.HasComponent(entity, ComponentType.Create<SelectedData>()))
                    {
                        EntityManager.RemoveComponent<SelectedData>(entity);
                    }
                    else
                    {
                        if (_selected.Length == 0)
                        {
                            EntityManager.AddComponentData(entity, new SelectedData()
                            {
                                Number = 1
                            });
                        }
                        else
                        {
                            EntityManager.AddComponentData(entity, new SelectedData()
                            {
                                Number = 2
                            });
                        }
                    }
                    break;
                }
            }
        }
    }
}