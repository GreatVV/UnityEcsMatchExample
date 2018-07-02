using System.Runtime.Serialization.Formatters;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveChipsToPositionSystem : ComponentSystem
{
    public struct MovingChips
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly]
        public ComponentDataArray<TargetPosition> TargetPosition;
        public ComponentDataArray<Position> CurrentPosition;
        public ComponentDataArray<ChipSpeed> ChipSpeed;
        public ComponentDataArray<ChipAcceleration> ChipAcceleration;
        public SubtractiveComponent<DestroyMarker> Destroy;
    }

    [Inject] private MovingChips _movingChips;

    protected override void OnUpdate()
    {
        var dt = Time.deltaTime;

        for (int i = 0; i < _movingChips.Length; i++)
        {
            var target = _movingChips.TargetPosition[i];
            var current = _movingChips.CurrentPosition[i];
            var direction = target.Value - current.Value;
            var e = _movingChips.Entities[i];
            if (math.lengthSquared(direction) < 0.0001f || (EntityManager.HasComponent<OriginalDirection>(e) && math.dot(EntityManager.GetComponentData<OriginalDirection>(e).Value, math.normalize(direction) ) <= 0))
            {
                current.Value = target.Value;
                PostUpdateCommands.RemoveComponent<TargetPosition>(e);
                if (EntityManager.HasComponent<OriginalDirection>(e))
                {
                    PostUpdateCommands.RemoveComponent<OriginalDirection>(e);
                }

                if (EntityManager.HasComponent<Dying>(e))
                {
                    PostUpdateCommands.AddComponent(e, new DestroyMarker());
                }
            }
            else
            {
                if (!EntityManager.HasComponent<OriginalDirection>(e))
                {
                    PostUpdateCommands.AddComponent(e, new OriginalDirection()
                    {
                        Value = math.normalize(direction)
                    });
                }

                var chipSpeed = _movingChips.ChipSpeed[i];
                current.Value += math.normalize(direction) * chipSpeed.Value * dt;
                chipSpeed.Value += _movingChips.ChipAcceleration[i].Value * dt;
                _movingChips.ChipSpeed[i] = chipSpeed;

            }

            _movingChips.CurrentPosition[i] = current;
        }
    }
}