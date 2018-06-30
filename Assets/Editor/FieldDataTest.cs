using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[TestFixture]
public class FieldDataTest
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

    [Test]
    public void PositionToIdTest()
    {
        var levelDescription = new LevelDescription();
        levelDescription.Width = 2;
        levelDescription.Height = 2;
        levelDescription.ColorCount = 5;
        levelDescription.Time = 60;

        var game = new GameObject().AddComponent<Game>();
        game.Level = ScriptableObject.CreateInstance<LevelDescriptionAsset>();
        game.Level.Value = levelDescription;

        game.Center = new GameObject("Center").transform;
        game.Center.position = new Vector3(1,1,0);

        Assert.AreEqual(new int2(0,0), game.GetIndex(new Vector3(0.5f, 0.5f)));
        Assert.AreEqual(new int2(1, 0), game.GetIndex(new Vector3(1.5f, 0.5f)));
        Assert.AreEqual(new int2(0, 1), game.GetIndex(new Vector3(0.5f, 1.5f)));
        Assert.AreEqual(new int2(1, 1), game.GetIndex(new Vector3(1.5f, 1.5f)));
    }

    private GameObject CreateChipPrefab(ChipColor color = 0)
    {
        var go = new GameObject("Tile",
            typeof(GameObjectEntity),
            typeof(ChipComponent),
            typeof(PositionComponent),
            typeof(TransformMatrixComponent),
            typeof(MeshInstanceRendererComponent),
            typeof(SlotComponent));

        go.GetComponent<ChipComponent>().UpdateColor(color);

        return go;
    }

    [Test]
    public void CreateSlots()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var levelDescription = new LevelDescription()
        {
            Width = 5,
            Height = 5
        };

        var creationPipeline = new CreateSlotsStep();
        creationPipeline.Apply(levelDescription, entityManager);

        //check
        var entities = entityManager.GetAllEntities();
        var slotEntityCount = 0;
        for (int i = 0; i < entities.Length; i++)
        {
            if (entityManager.HasComponent<Slot>(entities[i]))
            {
                slotEntityCount++;
            }
        }
        entities.Dispose();

        Assert.AreEqual(25, slotEntityCount);
    }

    [Test]
    public void CreateChips()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var levelDescription = new LevelDescription()
        {
            Width = 5,
            Height = 5
        };

        var createSlots = new CreateSlotsStep();
        createSlots.Apply(levelDescription, entityManager);

        var createChips = new CreateChipsStep(new[] {CreateChipPrefab()}, Vector3.zero);
        createChips.Apply(levelDescription, entityManager);

        //check
        var entities = entityManager.GetAllEntities();
        var countChips = 0;
        for (int i = 0; i < entities.Length; i++)
        {
            if (entityManager.HasComponent<Chip>(entities[i]))
            {
                countChips++;
            }
        }
        entities.Dispose();

        Assert.AreEqual(25, countChips);
    }

    [Test]
    public void CreateChipsAndFindSimpleLineCombinations()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var levelDescription = new LevelDescription()
        {
            Width = 3,
            Height = 3,
            SlotChipDescriptions = new List<SlotChipDescription>()
            {
                new SlotChipDescription()
                {
                    Position = new int2(0,0),
                    Color = ChipColor.Red,
                },
                new SlotChipDescription()
                {
                    Position = new int2(1,0),
                    Color = ChipColor.Red,
                },
                new SlotChipDescription()
                {
                    Position = new int2(2,0),
                    Color = ChipColor.Red,
                },

                new SlotChipDescription()
                {
                    Position = new int2(0,1),
                    Color = ChipColor.Blue,
                },
                new SlotChipDescription()
                {
                    Position = new int2(1,1),
                    Color = ChipColor.Blue,
                },
                new SlotChipDescription()
                {
                    Position = new int2(2,1),
                    Color = ChipColor.Blue,
                },

                new SlotChipDescription()
                {
                    Position = new int2(0,2),
                    Color = ChipColor.Yellow,
                },
                new SlotChipDescription()
                {
                    Position = new int2(1,2),
                    Color = ChipColor.Yellow,
                },
                new SlotChipDescription()
                {
                    Position = new int2(2,2),
                    Color = ChipColor.Yellow,
                },

            }
        };

        var createSlots = new CreateSlotsStep();
        createSlots.Apply(levelDescription, entityManager);
        var slots = new NativeArray<Entity>(9, Allocator.Temp);
        FillWithComponents<Slot>(entityManager, ref slots);

        var createChips = new CreateChipsStep(new[]
        {
            CreateChipPrefab(0),
            CreateChipPrefab((ChipColor) 1),
            CreateChipPrefab((ChipColor)2),
            CreateChipPrefab((ChipColor)3),
            CreateChipPrefab((ChipColor)4),
        }, Vector3.zero);
        createChips.Apply(levelDescription, entityManager);

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var combinationList = new NativeList<Entity>(64, Allocator.Temp);
        var visited = new NativeHashMap<int2, bool>(64, Allocator.Temp);

        FindCombinationsSystem.Find(entityManager, ref slots, new int2(0,0), ref visited, ref combinationList);
        Assert.AreEqual(3, combinationList.Length);

        combinationList.Clear();
        FindCombinationsSystem.Find(entityManager, ref slots, new int2(0,1), ref visited, ref combinationList);
        Assert.AreEqual(3, combinationList.Length);

        combinationList.Clear();
        FindCombinationsSystem.Find(entityManager, ref slots, new int2(0,2), ref visited, ref combinationList);
        Assert.AreEqual(3, combinationList.Length);

        slots.Dispose();
        combinationList.Dispose();
    }

    [Test]
    public void CreateChipsAndFindOneBigCombination()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var levelDescription = new LevelDescription()
        {
            Width = 3,
            Height = 3,
            SlotChipDescriptions = new List<SlotChipDescription>()
            {
                new SlotChipDescription()
                {
                    Position = new int2(0,0),
                    Color = ChipColor.Red,
                },
                new SlotChipDescription()
                {
                    Position = new int2(1,0),
                    Color = ChipColor.Red,
                },
                new SlotChipDescription()
                {
                    Position = new int2(2,0),
                    Color = ChipColor.Red,
                },

                new SlotChipDescription()
                {
                    Position = new int2(0,1),
                    Color = ChipColor.Red,
                },
                new SlotChipDescription()
                {
                    Position = new int2(1,1),
                    Color = ChipColor.Red,
                },
                new SlotChipDescription()
                {
                    Position = new int2(2,1),
                    Color = ChipColor.Red,
                },

                new SlotChipDescription()
                {
                    Position = new int2(0,2),
                    Color = ChipColor.Red,
                },
                new SlotChipDescription()
                {
                    Position = new int2(1,2),
                    Color = ChipColor.Red,
                },
                new SlotChipDescription()
                {
                    Position = new int2(2,2),
                    Color = ChipColor.Red,
                },

            }
        };

        var createSlots = new CreateSlotsStep();
        createSlots.Apply(levelDescription, entityManager);
        var slots = new NativeArray<Entity>(9, Allocator.Temp);
        FillWithComponents<Slot>(entityManager, ref slots);

        var createChips = new CreateChipsStep(new[]
        {
            CreateChipPrefab(0),
            CreateChipPrefab((ChipColor) 1),
            CreateChipPrefab((ChipColor)2),
            CreateChipPrefab((ChipColor)3),
            CreateChipPrefab((ChipColor)4),
        }, Vector3.zero);
        createChips.Apply(levelDescription, entityManager);

        var combinationList = new NativeList<Entity>(64, Allocator.Temp);

        var visited = new NativeHashMap<int2, bool>(64, Allocator.Temp);

        FindCombinationsSystem.Find(entityManager, ref slots, new int2(0,0), ref visited, ref combinationList);
        Assert.AreEqual(9, combinationList.Length);

        visited.Clear();
        combinationList.Clear();
        FindCombinationsSystem.Find(entityManager, ref slots, new int2(0,1), ref visited, ref combinationList);
        Assert.AreEqual(9, combinationList.Length);

        visited.Clear();
        combinationList.Clear();
        FindCombinationsSystem.Find(entityManager, ref slots, new int2(0,2),ref visited,  ref combinationList);
        Assert.AreEqual(9, combinationList.Length);

        slots.Dispose();
        visited.Dispose();
        combinationList.Dispose();
    }

    private void FillWithComponents<T>(EntityManager entityManager, ref NativeArray<Entity> slots)
    {
        var i = 0;
        var allEntities = entityManager.GetAllEntities(Allocator.Temp);
        for (int j = 0; j < allEntities.Length; j++)
        {
            if (entityManager.HasComponent<Slot>(allEntities[j]))
            {
                slots[i] = allEntities[j];
                i++;
            }
        }

        allEntities.Dispose();
    }
}