using System.Collections.Generic;
using UndergroundMatch3.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace UndergroundMatch3.Systems
{
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
            [ReadOnly] public SubtractiveComponent<Dying> Dying;
        }

        public struct TimeUp
        {
            public int Length;
            [ReadOnly] public ComponentDataArray<TimeOver> _;
        }

        [Inject] private SelectedTilesData _selected;
        [Inject] private MovingChips _movingChips;
        [Inject] private TimeUp _timeUp;
        [Inject] private Chips _chips;

        private SceneConfiguration _sceneConfiguration;
        private Dictionary<int2, Entity> _slotCache;

        public void Setup(SceneConfiguration sceneConfiguration, Dictionary<int2, Entity> slotCache)
        {
            _sceneConfiguration = sceneConfiguration;
            _slotCache = slotCache;
        }

        private Vector3 _startTouchPosition;
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
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_movingChips.Length > 0)
                {
                    return;
                }

                var startWorldPosition = _sceneConfiguration.Camera.ScreenToWorldPoint(_startTouchPosition);
                var worldPosition = _sceneConfiguration.Camera.ScreenToWorldPoint(Input.mousePosition);

                var swapDirection = worldPosition - startWorldPosition;

                var pm = PostUpdateCommands;
                var em = EntityManager;
                if (swapDirection.magnitude < _swapMinDistance)
                {
                    var clickPosition = _sceneConfiguration.GetIndex(startWorldPosition);
                    if (_slotCache.ContainsKey(clickPosition))
                    {
                        var slot = _slotCache[clickPosition];
                        var position = em.GetComponentData<SlotPosition>(slot).Value;

                        var chip = em.GetComponentData<ChipReference>(slot).Value;
                        if (em.HasComponent(chip, ComponentType.Create<Selected>()))
                        {
                            pm.RemoveComponent<Selected>(chip);
                        }
                        else
                        {
                            if (_selected.Length == 0)
                            {
                                pm.AddComponent(chip, new Selected());
                            }
                            else
                            {
                                //check if next to previous slot;
                                var previousSelected = _selected.Entities[0];
                                var previousSelectedSlot =
                                    em.GetComponentData<SlotReference>(previousSelected).Value;
                                var previousSelectedPosition =
                                    em.GetComponentData<SlotPosition>(previousSelectedSlot);

                                if (FieldUtils.NextToEachOther(previousSelectedPosition.Value, position))
                                {
                                    pm.AddComponent(chip, new Selected());
                                }
                                else
                                {
                                    pm.RemoveComponent<Selected>(previousSelected);
                                    pm.AddComponent(chip, new Selected());
                                }
                            }
                        }
                    }
                }
                else
                {
                    var clickSlotPosition = _sceneConfiguration.GetIndex(startWorldPosition);
                    if (_slotCache.ContainsKey(clickSlotPosition))
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

                        if (_slotCache.ContainsKey(otherClickPosition))
                        {
                            for (int i = 0; i < _selected.Length; i++)
                            {
                                pm.RemoveComponent<Selected>(_selected.Entities[i]);
                            }

                            var firstChip =
                                em.GetComponentData<ChipReference>(_slotCache[clickSlotPosition]).Value;

                            var secondChip =
                                em.GetComponentData<ChipReference>(_slotCache[otherClickPosition]).Value;

                            pm.AddComponent(firstChip, new Selected());
                            pm.AddComponent(secondChip, new Selected());

                        }
                    }
                }
            }
        }
    }
}