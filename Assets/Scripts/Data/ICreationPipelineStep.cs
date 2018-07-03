using Unity.Entities;

namespace UndergroundMatch3.Data
{
    public interface ICreationPipelineStep
    {
        void Apply(LevelDescription levelDescription, EntityManager entityManager);
    }
}