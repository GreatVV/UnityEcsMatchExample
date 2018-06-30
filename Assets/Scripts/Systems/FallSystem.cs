using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(DestroySystem))]
[UpdateAfter(typeof(FindCombinationsSystem))]
public class FallSystem : ComponentSystem
{
    private Dictionary<int2, Entity> _slotCache;
    private LevelDescription _levelDescription;

    public struct AnalyzeFieldFlag
    {
        public int Length;
        [ReadOnly] public ComponentDataArray<AnalyzeField> AnalyzeField;
    }

    public struct Swap
    {
        public int Length;
        [ReadOnly] public ComponentDataArray<PlayerSwap> PlayerSwap;
    }

    [Inject] private AnalyzeFieldFlag _analyzeFieldFlag;
    [Inject] private Swap _swap;

    public void Setup(Dictionary<int2, Entity> slotCache, LevelDescription levelDescription)
    {
        _slotCache = slotCache;
        _levelDescription = levelDescription;
    }

    protected override void OnUpdate()
    {
        if (_analyzeFieldFlag.Length == 0 && _swap.Length > 0)
        {
            return;
        }

        for (int column = 0; column < _levelDescription.Width; column++)
        {
            for (int row = 1; row < _levelDescription.Height; row++)
            {
                var position = new int2(column, row);
                var slot = _slotCache[position];
                if (EntityManager.HasComponent<ChipReference>(slot))
                {
                    MoveDown(slot, position);
                }
            }
        }
    }

    public static int GetNextEmptyRow(EntityManager entityManager, Dictionary<int2, Entity> slotsCache, int2 position)
    {
        position.y -= 1;

        var slot = slotsCache[position];
        while (position.y >= 0 && !entityManager.HasComponent<ChipReference>(slot))
        {
            position.y -= 1;
            if (slotsCache.ContainsKey(position))
            {
                slot = slotsCache[position];
            }
        }

        return position.y + 1;
    }

    void MoveDown(Entity slot, int2 position)
    {
        var nextRowPos = GetNextEmptyRow(EntityManager, _slotCache, position);
        if (nextRowPos != position.y)
        {
            var newPosition = new int2(position.x, nextRowPos);
            var chip = EntityManager.GetComponentData<ChipReference>(slot).Value;
            FieldUtils.MoveChipToSlot(EntityManager, chip, _slotCache[newPosition]);
        }
        else
        {
            var chip = EntityManager.GetComponentData<ChipReference>(slot).Value;
            if (!EntityManager.HasComponent<TargetPosition>(chip))
            {
                PostUpdateCommands.AddComponent(chip,
                    new TargetPosition(EntityManager.GetComponentData<Position>(slot).Value));
                PostUpdateCommands.AddComponent(chip, new AnimationTime());
            }
        }
    }
}