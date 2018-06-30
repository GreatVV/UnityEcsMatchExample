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
        public ComponentDataArray<AnimationTime> AnimationTime;
    }

    [Inject] private MovingChips _movingChips;
    private float _animationTime;

    public void Setup(float animationTime)
    {
        _animationTime = animationTime;
    }

    protected override void OnUpdate()
    {
        var dt = Time.deltaTime;
        for (int i = 0; i < _movingChips.Length; i++)
        {
            var target = _movingChips.TargetPosition[i];
            var current = _movingChips.CurrentPosition[i];
            if (math.abs(target.Value.x - current.Value.x) < 0.01f && math.abs(target.Value.y - current.Value.y) < 0.01f)
            {
                current.Value = target.Value;

                var movingTilesEntity = _movingChips.Entities[i];
                PostUpdateCommands.RemoveComponent<TargetPosition>(movingTilesEntity);
                PostUpdateCommands.RemoveComponent<AnimationTime>(movingTilesEntity);
                PostUpdateCommands.CreateEntity();
                PostUpdateCommands.AddComponent(new SwapFinished());
            }
            else
            {
                var animationTimeComponent = _movingChips.AnimationTime[i];
                var animationTime = math.clamp(animationTimeComponent.Value / _animationTime, 0, 1);
                current.Value = math.lerp(current.Value, target.Value, animationTime);
                animationTimeComponent.Value += dt;
                _movingChips.AnimationTime[i] = animationTimeComponent;
            }

            _movingChips.CurrentPosition[i] = current;
        }
    }
}