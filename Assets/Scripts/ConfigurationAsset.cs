using Unity.Entities;
using UnityEngine;

namespace UndergroundMatch3
{
    [CreateAssetMenu]
    public class ConfigurationAsset : ScriptableObject
    {
        public GameObjectEntity[] ChipPrefabs;
        public GameObject SelectionPrefab;
        public GameObject ExplosionPrefab;
        public float Speed = 20;
        public float Acceleration = 10;
    }
}