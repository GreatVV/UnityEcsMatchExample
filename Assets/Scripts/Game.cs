using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class Game : MonoBehaviour
{
	public Camera Camera;
	public LevelDescriptionAsset Level;
	public GameObjectEntity[] ChipPrefabs;
	public Transform Center;
	private EntityManager _entityManager;

	public LevelDescription LevelDescription { get; set; }

	public readonly Dictionary<int2, Entity> SlotCache = new Dictionary<int2, Entity>();

	public void Start()
	{
		LevelDescription = Level.Value;
		_entityManager = World.Active.GetOrCreateManager<EntityManager>();
		ProcessLevelDescription();
		World.Active.GetOrCreateManager<UserControlSystem>().Setup(this);
		World.Active.GetOrCreateManager<FindCombinationsSystem>().Setup(SlotCache);
		World.Active.GetOrCreateManager<GeneratorSystem>().Setup(ChipPrefabs, SlotCache, LevelDescription);
		World.Active.GetOrCreateManager<FallSystem>().Setup(SlotCache, LevelDescription);

		_entityManager.CreateEntity(typeof(AnalyzeField));
	}

	public void ProcessLevelDescription()
	{
		var steps = new List<ICreationPipelineStep>();
		steps.Add(new CreateSlotsStep(SlotCache, Center.position));
		steps.Add(new CreateChipsStep(ChipPrefabs));

		foreach (var creationPipelineStep in steps)
		{
			creationPipelineStep.Apply(LevelDescription, _entityManager);
		}
	}

	public int2 GetIndex(Vector3 worldPosition)
	{
		var localPoint = worldPosition - Center.transform.position;
		localPoint.x = localPoint.x + Level.Value.Width / 2f ;
		localPoint.y = localPoint.y + Level.Value.Height / 2f ;

		return new int2(Mathf.FloorToInt(localPoint.x), Mathf.FloorToInt(localPoint.y));
	}

}