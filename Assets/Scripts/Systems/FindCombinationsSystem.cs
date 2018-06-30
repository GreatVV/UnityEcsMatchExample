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

    public struct MoveFinished
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<SwapFinished> Swaps;
    }

    [Inject] private AllSlots _allSlots;
    [Inject] private MoveFinished _moveFinished;

    protected override void OnUpdate()
    {
        if (_moveFinished.Length == 0)
        {
            return;
        }

        for (int i = 0; i < _moveFinished.Length; i++)
        {
            PostUpdateCommands.DestroyEntity(_moveFinished.Entities[i]);
        }


        //todo fix size to dynamic based on level data

        var visited = new NativeHashMap<int2, bool>(64, Allocator.Temp);

        NativeArray<Entity> array = new NativeArray<Entity>(_allSlots.EntityArray.Length, Allocator.Temp);
        _allSlots.EntityArray.CopyTo(array, 0);

        for (int i = 0; i < _allSlots.Length; i++)
        {
            var position = _allSlots.Positions[i];
            NativeList<Entity> solution = new NativeList<Entity>(64, Allocator.Temp);
            Find(EntityManager, ref array, position.Value, ref visited, ref solution );

            if (solution.Length >= 3)
            {
                for (int j = solution.Length - 1; j >= 0; j--)
                {
                    var chip = EntityManager.GetComponentData<ChipReference>(solution[j]).Value;
                    if (EntityManager.Exists(chip))
                    {
                        PostUpdateCommands.DestroyEntity(chip);
                    }

                    PostUpdateCommands.RemoveComponent<ChipReference>(solution[j]);
                }
            }
            solution.Dispose();
        }

        array.Dispose();
        visited.Dispose();
    }

    public static void Visit(EntityManager entityManager, int2 position, ref NativeHashMap<int2, bool> visited, ref NativeHashMap<int2, Entity> positions, ref NativeList<Entity> list)
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
        var neighbours = entityManager.GetComponentData<PossibleNeighbours>(slot).Value;
        visited.TryAdd(position, true);
        list.Add(slot);

        if (IsSameColor(entityManager, Neighbours.Top, neighbours, position, slot, ref positions))
                 {
                     Visit(entityManager, position + FieldUtils.NeighbourToInt2(Neighbours.Top), ref visited, ref positions, ref list);
                 }

        if (IsSameColor(entityManager, Neighbours.Bottom, neighbours, position, slot, ref positions))
        {
            Visit(entityManager, position + FieldUtils.NeighbourToInt2(Neighbours.Bottom), ref visited, ref positions, ref list);
        }

        if (IsSameColor(entityManager, Neighbours.Left, neighbours, position, slot, ref positions))
        {
            Visit(entityManager, position + FieldUtils.NeighbourToInt2(Neighbours.Left), ref visited, ref positions, ref list);
        }

        if (IsSameColor(entityManager, Neighbours.Right, neighbours, position, slot, ref positions))
        {
            Visit(entityManager, position + FieldUtils.NeighbourToInt2(Neighbours.Right), ref visited, ref positions, ref list);
        }
    }

    private static bool IsSameColor(EntityManager entityManager, Neighbours target, Neighbours neighbours, int2 position, Entity slot, ref NativeHashMap<int2, Entity> positions)
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
            var newColor = entityManager.GetComponentData<Chip>(entityManager.GetComponentData<ChipReference>(entity).Value).Color;
            if (newColor == currentColor)
            {
                return true;
            }
        }

        return false;

    }

    public static void Find(EntityManager entityManager, ref NativeArray<Entity> slots, int2 targetSlotPosition, ref NativeHashMap<int2, bool> visited, ref NativeList<Entity> solution)
    {
        var positions = new NativeHashMap<int2, Entity>(64, Allocator.Temp);


        //hash positions
        for (int i = 0; i < slots.Length; i++)
        {
            var position = entityManager.GetComponentData<SlotPosition>(slots[i]);
            positions.TryAdd(position.Value, slots[i]);
        }

        bool status;

        if (!visited.TryGetValue(targetSlotPosition, out status))
        {
            Visit(entityManager, targetSlotPosition, ref visited, ref positions, ref solution);
            Debug.Log("Combinations");
            for (int j = 0; j < solution.Length; j++)
            {
                Debug.Log(entityManager.GetComponentData<SlotPosition>(solution[j]).Value);
            }
        }

        positions.Dispose();
    }
}