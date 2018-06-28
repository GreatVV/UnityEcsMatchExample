using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine;

[TestFixture]
public class FieldDataTest
{
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

        Assert.AreEqual(0, game.GetIndex(new Vector3(0.5f, 0.5f)));
        Assert.AreEqual(1, game.GetIndex(new Vector3(1.5f, 0.5f)));
        Assert.AreEqual(2, game.GetIndex(new Vector3(0.5f, 1.5f)));
        Assert.AreEqual(3, game.GetIndex(new Vector3(1.5f, 1.5f)));
    }

}
