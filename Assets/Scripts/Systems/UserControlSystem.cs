using System.Runtime.Serialization.Formatters;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

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
        [ReadOnly]
        public SubtractiveComponent<Dying> Dying;
    }

    public struct TimeUp
    {
        public int Length;
        [ReadOnly] public ComponentDataArray<TimeOver> _;
    }

    [Inject] private SelectedTilesData _selected;
    [Inject] private MovingChips _movingChips;
    [Inject] private Chips _chips;
    [Inject] private TimeUp _timeUp;
    private Game _game;

    public void Setup(Game game)
    {
        _game = game;
    }

    private Vector3 _startTouchPosition;
    private float _startTouchTime;

    private float _swapMinDistance = 0.5f;


    protected override void OnUpdate()
    {
        if (_timeUp.Length > 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _startTouchPosition = Input.mousePosition;
            _startTouchTime = Time.realtimeSinceStartup;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_movingChips.Length > 0)
            {
                return;
            }

            var startWorldPosition = _game.Camera.ScreenToWorldPoint(_startTouchPosition);
            var worldPosition = _game.Camera.ScreenToWorldPoint(Input.mousePosition);

            var swapDirection = worldPosition - startWorldPosition;

            if (swapDirection.magnitude < _swapMinDistance)
            {
                var clickPosition = _game.GetIndex(startWorldPosition);
                if (_game.SlotCache.ContainsKey(clickPosition))
                {
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
                            var previousSelectedSlot =
                                EntityManager.GetComponentData<SlotReference>(previousSelected).Value;
                            var previousSelectedPosition =
                                EntityManager.GetComponentData<SlotPosition>(previousSelectedSlot);

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
                                PostUpdateCommands.AddComponent(chip, new Selected()
                                {
                                    Number = 1
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                var clickSlotPosition = _game.GetIndex(startWorldPosition);
                if (_game.SlotCache.ContainsKey(clickSlotPosition))
                {
                    int2 otherClickPosition;
                    if (Mathf.Abs(swapDirection.x) > Mathf.Abs(swapDirection.y))
                    {
                        //horizontal swap
                        if (swapDirection.x > 0)
                        {
                            otherClickPosition = clickSlotPosition + new int2(1, 0);

                        }
                        else
                        {
                            otherClickPosition = clickSlotPosition - new int2(1, 0);
                        }
                    }
                    else
                    {
                        //vertical swap
                        if (swapDirection.y > 0)
                        {
                            otherClickPosition = clickSlotPosition + new int2(0, 1);

                        }
                        else
                        {
                            otherClickPosition = clickSlotPosition - new int2(0, 1);
                        }
                    }

                    if (_game.SlotCache.ContainsKey(otherClickPosition))
                    {
                        for (int i = 0; i < _selected.Length; i++)
                        {
                            PostUpdateCommands.RemoveComponent<Selected>(_selected.Entities[i]);
                        }

                        var firstChip =
                            EntityManager.GetComponentData<ChipReference>(_game.SlotCache[clickSlotPosition]).Value;

                        var secondChip =
                            EntityManager.GetComponentData<ChipReference>(_game.SlotCache[otherClickPosition]).Value;

                        PostUpdateCommands.AddComponent(firstChip, new Selected()
                        {
                            Number = 1
                        });
                        PostUpdateCommands.AddComponent(secondChip, new Selected()
                        {
                            Number = 2
                        });

                    }
                }
            }
        }
    }
}