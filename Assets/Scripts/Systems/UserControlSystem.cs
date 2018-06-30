using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class UserControlSystem : ComponentSystem
{
    public struct Chips
    {
        public int Length;
        public ComponentDataArray<SlotReference> Slots;
    }

    public struct SelectedTilesData
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly] public ComponentDataArray<Selected> Selected;
    }

    public struct MovingChips
    {
        public int Length;
        [ReadOnly] public ComponentDataArray<TargetPosition> TargetPosition;
    }

    [Inject] private SelectedTilesData _selected;
    [Inject] private MovingChips _movingChips;
    [Inject] private Chips _chips;
    private Game _game;

    public void Setup(Game game)
    {
        _game = game;
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_movingChips.Length > 0)
            {
                return;
            }

            var worldPosition = _game.Camera.ScreenToWorldPoint(Input.mousePosition);
            var clickPosition = _game.GetIndex(worldPosition);
            var slot = _game.SlotCache[clickPosition];
            var position = EntityManager.GetComponentData<SlotPosition>(slot).Value;

            var chip = EntityManager.GetComponentData<ChipReference>(slot).Value;
            if (EntityManager.HasComponent(chip, ComponentType.Create<Selected>()))
            {
                PostUpdateCommands.RemoveComponent<Selected>(chip);
            }
            else
            {
                if (_selected.Length == 0)
                {
                    PostUpdateCommands.AddComponent(chip, new Selected()
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

                        PostUpdateCommands.AddComponent(chip, new Selected()
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
        }
    }
}
