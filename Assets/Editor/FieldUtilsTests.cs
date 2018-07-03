using NUnit.Framework;
using UndergroundMatch3;
using UndergroundMatch3.Components;
using Unity.Mathematics;

[TestFixture]
public class FieldUtilsTests
{
    [Test]
    public void GetNeighboursZeroZero()
    {
        var neighbours = FieldUtils.GetNeighbour(0, 0, 2, 2);
        Assert.AreEqual(Neighbours.Right | Neighbours.Top, neighbours);

        neighbours = FieldUtils.GetNeighbour(1, 1, 2, 2);
        Assert.AreEqual(Neighbours.Left | Neighbours.Bottom, neighbours);

        neighbours = FieldUtils.GetNeighbour(1, 1, 3, 3);
        Assert.AreEqual(Neighbours.Left | Neighbours.Bottom | Neighbours.Right | Neighbours.Top, neighbours);
    }

    [Test]
    public void IsPositionNear()
    {
        Assert.IsTrue(FieldUtils.NextToEachOther(new int2(0,0), new int2(0,1) ));
        Assert.IsTrue(FieldUtils.NextToEachOther(new int2(0,0), new int2(1,0) ));
        Assert.IsTrue(FieldUtils.NextToEachOther(new int2(0,0), new int2(-1,0) ));
        Assert.IsTrue(FieldUtils.NextToEachOther(new int2(0,0), new int2(0,-1) ));
        Assert.IsFalse(FieldUtils.NextToEachOther(new int2(0,0), new int2(1,1) ));
        Assert.IsFalse(FieldUtils.NextToEachOther(new int2(0,0), new int2(0,2) ));
    }
}