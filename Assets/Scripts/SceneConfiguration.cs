using System;
using Unity.Mathematics;
using UnityEngine;

namespace UndergroundMatch3
{
    [Serializable]
    public class SceneConfiguration
    {
        public Camera Camera;
        public Transform Center;
        public Transform DeathPosition;
        public LevelDescriptionAsset Level;

        public int2 GetIndex(Vector3 worldPosition)
        {
            var localPoint = worldPosition - Center.transform.position;
            localPoint.x = localPoint.x + Level.Value.Width / 2f ;
            localPoint.y = localPoint.y + Level.Value.Height / 2f ;

            return new int2(Mathf.FloorToInt(localPoint.x), Mathf.FloorToInt(localPoint.y));
        }
    }
}