using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine;

[TestFixture]
public class FieldDataTest
{
    [Test]
    public void FirstTest()
    {
        var levelDescription = new LevelDescription();
        levelDescription.Width = 8;
        levelDescription.Height = 8;
        levelDescription.ColorCount = 5;
        levelDescription.Time = 60;

        var entityManager = new EntityManager();
        //Game.CreateSlots(levelDescription, entityManager);

        Assert.AreEqual(64, entityManager.GetAllEntities().Length);

    }

}
