using NUnit.Framework;
using UndergroundMatch3;
using UndergroundMatch3.Components;
using UndergroundMatch3.Data;
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

    protected SceneConfiguration CreateTestSceneConfiguration()
    {
        return new SceneConfiguration()
        {
            Center = new GameObject("Center").transform
        };
    }

    protected ConfigurationAsset CreateTestConfiguration()
    {
        var configurationAsset = ScriptableObject.CreateInstance<ConfigurationAsset>();

        configurationAsset.ChipPrefabs = new[]
        {
            CreateChipPrefab(0),
            CreateChipPrefab((ChipColor) 1),
            CreateChipPrefab((ChipColor)2),
            CreateChipPrefab((ChipColor)3),
            CreateChipPrefab((ChipColor)4),
        };

        return configurationAsset;
    }
}