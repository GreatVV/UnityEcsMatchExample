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

	public LevelDescription()
	{

	}

	public LevelDescription(int width, int height, int colorCount, ChipColor defaultColor, ChipType defaultType)
	{
		Width = width;
		Height = height;
		ColorCount = colorCount;
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				var newSlotChipDescription = new SlotChipDescription()
				{
					Position = new int2(i,j),
					Color = defaultColor,
					ChipType = defaultType
				};
				SlotChipDescriptions.Add(newSlotChipDescription);
			}
		}

		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				var newSlotDescription = new SlotDescription()
				{
					Position = new int2(i, j),
					Generator = j == Height - 1
				};
				SlotDescriptions.Add(newSlotDescription);
			}
		}
	}

	public List<SlotDescription> SlotDescriptions = new List<SlotDescription>();

	public List<SlotChipDescription> SlotChipDescriptions = new List<SlotChipDescription>();

	public SlotChipDescription DefaultChipDescription = new SlotChipDescription()
	{
		ChipType = ChipType.Simple,
		Color = ChipColor.Random
	};

	public SlotChipDescription GetChipDescription(int2 slotPosition)
	{
		//todo replace with better data structure
		foreach (var x in SlotChipDescriptions)
		{
			if (x.Position.x == slotPosition.x && x.Position.y == slotPosition.y) return x;
		}

		return DefaultChipDescription;
	}

	public SlotDescription GetSlotDescription(int2 position)
	{
		foreach (var x in SlotDescriptions)
		{
			if (x.Position.x == position.x && x.Position.y == position.y) return x;
		}

		return new SlotDescription();
	}
}