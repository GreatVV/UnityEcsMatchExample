using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ShowSelectionSystem : ComponentSystem
{
    private GameObject _selectionInstance;


    public void Setup(GameObject selectionPrefab)
    {
        _selectionInstance = Object.Instantiate(selectionPrefab);
        _selectionInstance.gameObject.SetActive(false);
    }

    public struct Selection
    {
        public int Length;
        public ComponentDataArray<Selected> Selected;
        public ComponentDataArray<Position> Position;
    }

    public struct Moving
    {
        public int Length;
        public ComponentDataArray<TargetPosition> TargetPosition;
    }

    [Inject] private Selection _selection;
    [Inject] private Moving _moving;

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