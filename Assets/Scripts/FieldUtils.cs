using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public static class FieldUtils
{
    public static float3 GetPosition(int x, int y, int width, int height, float3 center)
    {
        return new float3(
            center.x - width / 2f + x +0.5f,
            center.y - height / 2f + y + 0.5f,
            center.z
        );
    }

    public static Neighbours GetNeighbour(int x, int y, int width, int height)
    {
        var mask = Neighbours.None;
        if (x > 0)
        {
            mask |= Neighbours.Left;
        }

        if (y > 0)
        {
            mask |= Neighbours.Bottom;
        }

        if (x < width - 1)
        {
            mask |= Neighbours.Right;
        }

        if (y < height - 1)
        {
            mask |= Neighbours.Top;
        }

        return mask;
    }

    public static int2 NeighbourToInt2(Neighbours neighbours)
    {
        switch (neighbours)
        {
            case Neighbours.None:
                return new int2();
                break;
            case Neighbours.Top:
                return new int2(0,1);
                break;
            case Neighbours.TopRight:
                return new int2(1,1);
                break;
            case Neighbours.Right:
                return new int2(1,0);
                break;
            case Neighbours.BottomRight:
                return new int2(1,-1);
                break;
            case Neighbours.Bottom:
                return new int2(0,-1);
                break;
            case Neighbours.BottomLeft:
                return new int2(-1,-1);
                break;
            case Neighbours.Left:
                return new int2(-1,0);
                break;
            case Neighbours.TopLeft:
                return new int2(-1,1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(neighbours), neighbours, null);
        }
    }

    public static bool NextToEachOther(int2 first, int2 second)
    {
        if (first.x == second.x)
        {
            if (first.y == second.y - 1 || first.y == second.y + 1)
            {
                return true;
            }
        }
        else
        {
            if (first.y == second.y)
            {
                if (first.x == second.x - 1 || first.x == second.x + 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void MoveChipToSlot(EntityManager entityManager, Entity chip, Entity slot)
    {
        var previousSlot = entityManager.GetComponentData<SlotReference>(chip).Value;
        entityManager.RemoveComponent<ChipReference>(previousSlot);
        entityManager.SetComponentData(chip, new SlotReference() {Value = slot});
        entityManager.AddComponentData(slot, new ChipReference() {Value = chip});
        if (entityManager.HasComponent<TargetPosition>(chip))
        {
            entityManager.SetComponentData(chip, new TargetPosition()
                {
                    Value = entityManager.GetComponentData<Position>(slot).Value
                }
            );
            entityManager.SetComponentData(chip, new AnimationTime());
        }
        else
        {
            entityManager.AddComponentData(chip, new TargetPosition()
                {
                    Value = entityManager.GetComponentData<Position>(slot).Value
                }
            );
            entityManager.AddComponentData(chip, new AnimationTime());
        }
    }
}