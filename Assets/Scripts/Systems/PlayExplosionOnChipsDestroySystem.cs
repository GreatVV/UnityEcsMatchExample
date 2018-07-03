using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UndergroundMatch3.Systems
{
    public class PlayExplosionOnChipsDestroySystem : ComponentSystem
    {
        [Inject] private SystemsUtils.DyingDeadChips _deadChips;
        //[Inject] private SystemsUtils.ScoreScore _score;

        private GameObject _explosion;

        public void Setup(ConfigurationAsset configurationAsset)
        {
            _explosion = configurationAsset.ExplosionPrefab;
        }

        protected override void OnUpdate()
        {
            if (_deadChips.Length > 0)
            {
                var entity = _deadChips.EntityArray[0];
                var position = EntityManager.GetComponentData<Position>(entity).Value;
                Object.Instantiate(_explosion, position, Quaternion.identity);
            }
        }
    }
}