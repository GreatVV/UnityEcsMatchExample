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
	public GameObject[] ChipPrefabs;
	public Transform Center;
	private EntityManager _entityManager;

	public LevelDescription LevelDescription;
	private float AnimationTime = 0.5f;

	public void Start()
	{
		LevelDescription = Level.Value;
		_entityManager = World.Active.GetOrCreateManager<EntityManager>();
		ProcessLevelDescription();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void InitializeWithScene()
	{
		var game = FindObjectOfType<Game>();
		World.Active.GetOrCreateManager<UserControlSystem>().Setup(game);
		World.Active.GetOrCreateManager<MoveChipsToPositionSystem>().Setup(game.AnimationTime);
	}

	public void ProcessLevelDescription()
	{
		var steps = new List<ICreationPipelineStep>();
		steps.Add(new CreateSlotsStep());
		steps.Add(new CreateChipsStep(ChipPrefabs, Center.position));

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