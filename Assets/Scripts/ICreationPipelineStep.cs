using Unity.Entities;

public interface ICreationPipelineStep
{
    void Apply(LevelDescription levelDescription, EntityManager entityManager);
}