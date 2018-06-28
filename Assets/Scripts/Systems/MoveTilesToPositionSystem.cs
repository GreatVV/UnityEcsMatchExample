using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveTilesToPositionSystem : ComponentSystem
{
    public struct MovingTiles
    {
        public int Length;
        public EntityArray EntityArray;
        [ReadOnly]
        public ComponentDataArray<TargetPosition> TargetPosition;
        public ComponentDataArray<Position> CurrentPosition;
    }

    [Inject] private MovingTiles _movingTiles;

    protected override void OnUpdate()
    {
        var dt = Time.deltaTime;
        for (int i = 0; i < _movingTiles.Length; i++)
        {
            var target = _movingTiles.TargetPosition[i];
            var current = _movingTiles.CurrentPosition[i];
            if (math.abs(target.Value.x - current.Value.x) < 0.01f && math.abs(target.Value.y - current.Value.y) < 0.01f)
            {

                current.Value = target.Value;
                PostUpdateCommands.RemoveComponent<TargetPosition>(_movingTiles.EntityArray[i]);
            }
            else
            {
                current.Value = math.lerp(current.Value, target.Value, dt);
            }
            _movingTiles.CurrentPosition[i] = current;
        }
    }
}