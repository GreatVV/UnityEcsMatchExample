using System.Collections.Generic;
using NUnit.Framework;
using UndergroundMatch3.Data;
using UndergroundMatch3.Data.Steps;
using UndergroundMatch3.Systems;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[TestFixture]
public class FallSystemTests : BaseWorldTests
{
    [Test]
    public void FallTillZero()
    {
         var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var levelDescription = new LevelDescription()
        {
            Width = 1,
            Height = 3,
            SlotChipDescriptions = new List<SlotChipDescription>()
            {
                new SlotChipDescription()
                {
                    Position = new int2(0,2),
                    Color = ChipColor.Red,
                },
            }
        };
        levelDescription.DefaultChipDescription = new SlotChipDescription()
        {
            ChipType = ChipType.None
        };

        var slotCache = new Dictionary<int2, Entity>();
        var sceneConfiguration = CreateTestSceneConfiguration();
        var configuration = CreateTestConfiguration();
        var createSlots = new CreateSlotsStep(slotCache, sceneConfiguration);
        createSlots.Apply(levelDescription, entityManager);

        var createChips = new CreateChipsStep(configuration);
        createChips.Apply(levelDescription, entityManager);

        var nextY = FallSystem.GetNextEmptyRow(entityManager, slotCache, new int2(0, 2));

        Assert.AreEqual(0, nextY);
    }

    [Test]
    public void NoFall()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var levelDescription = new LevelDescription()
        {
            Width = 1,
            Height = 3,
            SlotChipDescriptions = new List<SlotChipDescription>()
            {
                new SlotChipDescription()
                {
                    Position = new int2(0,2),
                    Color = ChipColor.Red,
                },
            }
        };

        var slotCache = new Dictionary<int2, Entity>();
        var sceneConfiguration = CreateTestSceneConfiguration();
        var configuration = CreateTestConfiguration();

        var createSlots = new CreateSlotsStep(slotCache, sceneConfiguration);
        createSlots.Apply(levelDescription, entityManager);

        var createChips = new CreateChipsStep(configuration);
        createChips.Apply(levelDescription, entityManager);

        var nextY = FallSystem.GetNextEmptyRow(entityManager, slotCache, new int2(0, 2));

        Assert.AreEqual(2, nextY);
    }
}