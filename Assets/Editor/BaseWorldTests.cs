using NUnit.Framework;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BaseWorldTests
{
    [SetUp]
    public void EcsSetup()
    {
        World.DisposeAllWorlds();
        World w = new World("Test World");
        World.Active = w;
    }

    [TearDown]
    public void TearDown()
    {
        World.Active.Dispose();
    }

    protected GameObjectEntity CreateChipPrefab(ChipColor color = 0)
    {
        var go = new GameObject("Tile",
            typeof(GameObjectEntity),
            typeof(ChipComponent),
            typeof(PositionComponent),
            typeof(TransformMatrixComponent),
            typeof(MeshInstanceRendererComponent),
            typeof(SlotComponent));

        go.GetComponent<ChipComponent>().UpdateColor(color);

        return go.GetComponent<GameObjectEntity>();
    }
}