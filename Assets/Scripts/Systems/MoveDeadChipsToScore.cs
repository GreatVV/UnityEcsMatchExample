using UndergroundMatch3.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace UndergroundMatch3.Systems
{
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

        public void Setup(SceneConfiguration sceneConfiguration, ConfigurationAsset configuration)
        {
            _targetPosition = sceneConfiguration.DeathPosition.position;
            _speed = configuration.Speed;
            _acceleration = configuration.Acceleration;
        }
    }
}