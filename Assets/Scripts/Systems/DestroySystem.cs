using UndergroundMatch3.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Experimental.PlayerLoop;

namespace UndergroundMatch3.Systems
{
    [UpdateAfter(typeof(PostLateUpdate))]
    public class DestroySystem : ComponentSystem
    {
        [Inject] private SystemsUtils.MarkedForDestroy _markedForDestroy;

        protected override void OnUpdate()
        {
            for (int i = 0; i < _markedForDestroy.Length; i++)
            {
                PostUpdateCommands.DestroyEntity(_markedForDestroy.Entities[i]);
            }
        }
    }
}