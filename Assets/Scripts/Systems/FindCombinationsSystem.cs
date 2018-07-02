using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(SwapChipsSystem))]
public class FindCombinationsSystem : ComponentSystem
{
    public struct AllSlots
    {
        public int Length;
        public EntityArray EntityArray;
        [ReadOnly]
        public ComponentDataArray<ChipReference> ChipReference;
        [ReadOnly]
        public ComponentDataArray<SlotPosition> Positions;
    }

    public struct SwapFinishedData
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<PlayerSwap> PlayerSwap;
        public SubtractiveComponent<SwapSuccess> SwapSuccess;
    }

    public struct AnalyzeFieldFlag
    {
        public int Length;
        public ComponentDataArray<AnalyzeField> AnalyzeField;
        public EntityArray Entities;
    }

    [Inject] private SwapFinishedData _swaps;
    [Inject] private AllSlots _allSlots;
    [Inject] private AnalyzeFieldFlag _analyzeFieldFlag;

    private Dictionary<int2, Entity> _slotCache;

    public void Setup(Dictionary<int2, Entity> slotCache)
    {
        _slotCache = slotCache;
    }

    protected override void OnUpdate()
    {
        if (_analyzeFieldFlag.Length == 0)
        {
            return;
        }

        for (int i = 0; i < _analyzeFieldFlag.Length; i++)
        {
            PostUpdateCommands.AddComponent(_analyzeFieldFlag.Entities[i], new DestroyData());
        }

        var visited = new NativeHashMap<int2, bool>(_slotCache.Count, Allocator.Temp);

        var anyCombination = false;

        for (int i = 0; i < _allSlots.Length; i++)
        {
            var position = _allSlots.Positions[i];
            var solution = new NativeList<Entity>(_slotCache.Count, Allocator.Temp);
            Find(EntityManager, _slotCache, position.Value, ref visited, ref solution);

            if (solution.Length >= 3)
            {
                anyCombination = true;
//                Debug.Log("Combinations");
//                for (int j = 0; j < solution.Length; j++)
//                {
//                    Debug.Log(EntityManager.GetComponentData<SlotPosition>(solution[j]).Value);
//                }

                for (int j = solution.Length - 1; j >= 0; j--)
                {
                    var slot = solution[j];
                    var chip = EntityManager.GetComponentData<ChipReference>(slot).Value;
                    PostUpdateCommands.DestroyEntity(chip);
                    PostUpdateCommands.RemoveComponent<ChipReference>(slot);
                }

            }

            solution.Dispose();
        }

        for (int i = 0; i < _swaps.Length; i++)
        {
            PostUpdateCommands.AddComponent(_swaps.Entities[i], new SwapSuccess()
            {
                Value = anyCombination ? SwapResult.Success : SwapResult.Fail
            });
        }

        visited.Dispose();
    }

    public static void Visit(EntityManager entityManager, int2 position, ref NativeHashMap<int2, bool> visited, Dictionary<int2, Entity> positions, ref NativeList<Entity> list)
    {
        bool visitedSlot;
        if (visited.TryGetValue(position, out visitedSlot))
        {
            return;
        }

        Entity slot;
        if (!positions.TryGetValue(position, out slot))
        {
            return;
        }
        visited.TryAdd(position, true);
        if (!entityManager.HasComponent<ChipReference>(slot))
        {
            return;
        }


        var neighbours = entityManager.GetComponentData<PossibleNeighbours>(slot).Value;
        list.Add(slot);

        if (IsSameColor(entityManager, Neighbours.Top, neighbours, position, slot, positions))
                 {
                     Visit(entityManager, position + FieldUtils.NeighbourToInt2(Neighbours.Top), ref visited, positions, ref list);
                 }

        if (IsSameColor(entityManager, Neighbours.Bottom, neighbours, position, slot, positions))
        {
            Visit(entityManager, position + FieldUtils.NeighbourToInt2(Neighbours.Bottom), ref visited, positions, ref list);
        }

        if (IsSameColor(entityManager, Neighbours.Left, neighbours, position, slot, positions))
        {
            Visit(entityManager, position + FieldUtils.NeighbourToInt2(Neighbours.Left), ref visited, positions, ref list);
        }

        if (IsSameColor(entityManager, Neighbours.Right, neighbours, position, slot, positions))
        {
            Visit(entityManager, position + FieldUtils.NeighbourToInt2(Neighbours.Right), ref visited, positions, ref list);
        }
    }

    private static bool IsSameColor(EntityManager entityManager, Neighbours target, Neighbours neighbours, int2 position, Entity slot, Dictionary<int2, Entity> positions)
    {
        if ((target & neighbours) == 0)
        {
            return false;
        }

        position += FieldUtils.NeighbourToInt2(target);
        Entity entity;
        if (positions.TryGetValue(position, out entity))
        {
            var currentColor = entityManager.GetComponentData<Chip>(entityManager.GetComponentData<ChipReference>(slot).Value).Color;
            if (!entityManager.HasComponent<ChipReference>(entity))
            {
                return false;
            }

            var newColor = entityManager.GetComponentData<Chip>(entityManager.GetComponentData<ChipReference>(entity).Value).Color;
            if (newColor == currentColor)
            {
                return true;
            }
        }

        return false;

    }

    public static void Find(EntityManager entityManager, Dictionary<int2, Entity> positions,  int2 targetSlotPosition, ref NativeHashMap<int2, bool> visited, ref NativeList<Entity> solution)
    {
        bool status;

        if (!visited.TryGetValue(targetSlotPosition, out status))
        {
            Visit(entityManager, targetSlotPosition, ref visited, positions, ref solution);
        }
    }
}