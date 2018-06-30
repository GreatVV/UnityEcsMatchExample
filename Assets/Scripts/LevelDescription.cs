using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class LevelDescription
{
	public int Width;
	public int Height;
	public int ColorCount = 5;
	public int Time = 60;

	public List<SlotChipDescription> SlotChipDescriptions = new List<SlotChipDescription>();

	public SlotChipDescription GetChipDescription(int2 slotPosition)
	{
		//todo replace with better data structure
		foreach (var x in SlotChipDescriptions)
		{
			if (x.Position.x == slotPosition.x && x.Position.y == slotPosition.y) return x;
		}

		return null;
	}
}