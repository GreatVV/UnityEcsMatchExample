using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class Game : MonoBehaviour
{
	public LevelDescriptionAsset Level;
	public GameObject[] TilePrefabs;

	public Transform Center;


	void Start()
	{
		var entityManager = World.Active.GetOrCreateManager<EntityManager>();
		CreateSlots(Level.Value, entityManager, TilePrefabs, Center);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void InitializeWithScene()
	{
		World.Active.GetOrCreateManager<UserControlSystem>().Setup();
	}

	public static float3 GetPosition(int x, int y, int width, int height, float3 center)
	{
		return new float3(
			center.x - width / 2f + x +0.5f,
			center.y - height / 2f + y + 0.5f,
			center.z
			);
	}

	public static void CreateSlots(LevelDescription levelDescription, EntityManager entityManager,GameObject[] tilePrefabs, Transform center)
	{
		var slotArchitype = entityManager.CreateArchetype(typeof(SlotPosition), typeof(SlotData));

		for (int x = 0; x < levelDescription.Width; x++)
		{
			for (int y = 0; y < levelDescription.Height; y++)
			{
				var entity = entityManager.CreateEntity(slotArchitype);
				entityManager.SetComponentData(entity, new SlotPosition() {Value = new int2(x, y)});
				var id = y * levelDescription.Width + x;
				entityManager.SetComponentData(entity, new SlotData() {Id = id});

				var tile = entityManager.Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)]);
				entityManager.SetComponentData(tile, new SlotData() {Id = id});
				entityManager.SetComponentData(tile, new Position()
				{
					Value = GetPosition(x, y, levelDescription.Width, levelDescription.Height, center.position)
				});
			}
		}
	}

	public long GetIndex(Vector3 worldPosition)
	{
		var localPoint = worldPosition - Center.transform.position;
		localPoint.x = localPoint.x + Level.Value.Width / 2f ;
		localPoint.y = localPoint.y + Level.Value.Height / 2f ;

		return Mathf.FloorToInt(localPoint.y) * Level.Value.Width + Mathf.FloorToInt(localPoint.x);
	}
}