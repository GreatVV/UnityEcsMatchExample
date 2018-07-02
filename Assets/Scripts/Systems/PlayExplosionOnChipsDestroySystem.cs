using System;
using System.Security.Policy;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayExplosionOnChipsDestroySystem : ComponentSystem
{
    [Inject] private UpdateScoreSystem.DyingDeadChips _deadChips;
    [Inject] private SyncScoreSystem.ScoreScore _score;

    private GameObject _explosion;

    public void Setup(GameObject explosion)
    {
        _explosion = explosion;
    }

    protected override void OnUpdate()
    {
        if (_deadChips.Length > 0)
        {
            Debug.Log("Play");
            var entity = _deadChips.EntityArray[0];
            var position = EntityManager.GetComponentData<Position>(entity).Value;
            Object.Instantiate(_explosion, position, Quaternion.identity);
        }
    }
}