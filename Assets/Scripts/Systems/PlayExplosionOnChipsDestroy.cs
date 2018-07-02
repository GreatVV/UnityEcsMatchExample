using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayExplosionOnChipsDestroy : ComponentSystem
{
    public struct DestroyChip
    {
        public int Length;
        [ReadOnly]
        public ComponentDataArray<Chip> Chip;
        [ReadOnly]
        public ComponentDataArray<DestroyMarker> _;
        [ReadOnly]
        public ComponentDataArray<Position> Positions;
    }

    [Inject] private DestroyChip _deadChips;
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
            var position = _deadChips.Positions[0].Value;
            Object.Instantiate(_explosion, position, Quaternion.identity);
        }
    }
}