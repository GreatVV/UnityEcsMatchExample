using UndergroundMatch3.Components;
using Unity.Entities;

namespace UndergroundMatch3.Data.Steps
{
    public class CreateScoreStep : ICreationPipelineStep
    {
        public void Apply(LevelDescription levelDescription, EntityManager entityManager)
        {
            entityManager.CreateEntity(typeof(Score));
        }
    }
}