using UndergroundMatch3.Components;
using Unity.Entities;

namespace UndergroundMatch3.Data.Steps
{
    public class CreateTimeStep : ICreationPipelineStep
    {
        public void Apply(LevelDescription levelDescription, EntityManager entityManager)
        {
            var time = entityManager.CreateEntity();
            entityManager.AddComponentData(time, new GameTime() {Seconds = levelDescription.Time});
        }
    }
}