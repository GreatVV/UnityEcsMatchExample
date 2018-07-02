using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MoveDeadChipsToScore : ComponentSystem
{
    public struct DeadChips
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly]
        public ComponentDataArray<Dying> Dying;

        public ComponentDataArray<Position> Positions;
        public SubtractiveComponent<TargetPosition> TargetPosition;
    }

    [Inject] public DeadChips _deadChips;
    private Vector3 _targetPosition;
    private float _speed;
    private float _acceleration;

    public void Setup(Vector3 targetPosition, float speed, float acceleration)
    {
        _targetPosition = targetPosition;
        _speed = speed;
        _acceleration = acceleration;
    }

    protected override void OnUpdate()
    {
        for (int i = 0; i < _deadChips.Length; i++)
        {
            PostUpdateCommands.SetComponent(_deadChips.Entities[i], new ChipSpeed()
            {
                Value = _speed
            });
            PostUpdateCommands.SetComponent(_deadChips.Entities[i], new ChipAcceleration()
            {
                Value = _acceleration
            });
            PostUpdateCommands.AddComponent(_deadChips.Entities[i], new TargetPosition(_targetPosition));
        }
    }
}