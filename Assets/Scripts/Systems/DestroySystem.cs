using Unity.Collections;
using Unity.Entities;
using UnityEngine.Experimental.PlayerLoop;


[UpdateAfter(typeof(PostLateUpdate))]
public class DestroySystem : ComponentSystem
{
    public struct DestroyTarget
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly]
        public ComponentDataArray<DestroyMarker> Destroy;
    }

    [Inject] private DestroyTarget _destroyTargets;

    protected override void OnUpdate()
    {
        for (int i = 0; i < _destroyTargets.Length; i++)
        {
            PostUpdateCommands.DestroyEntity(_destroyTargets.Entities[i]);
        }
    }
}