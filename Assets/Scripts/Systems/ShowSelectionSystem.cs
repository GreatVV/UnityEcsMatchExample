using System.Runtime.CompilerServices;
using UndergroundMatch3.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace UndergroundMatch3.Systems
{
    public class ShowSelectionSystem : ComponentSystem
    {
        private GameObject _selectionInstance;

        public void Setup(ConfigurationAsset configuration)
        {
            _selectionInstance = Object.Instantiate(configuration.SelectionPrefab);
            _selectionInstance.gameObject.SetActive(false);
        }

        public struct Selection
        {
            public int Length;
            public ComponentDataArray<Selected> Selected;
            public ComponentDataArray<Position> Position;
        }


        [Inject] private Selection _selection;
        [Inject] private SystemsUtils.MovingChips _moving;

        protected override void OnUpdate()
        {
            if (_selection.Length == 1 && _moving.Length == 0)
            {
                _selectionInstance.gameObject.SetActive(true);
                _selectionInstance.transform.position = _selection.Position[0].Value;
            }
            else
            {
                _selectionInstance.gameObject.SetActive(false);
            }
        }
    }
}