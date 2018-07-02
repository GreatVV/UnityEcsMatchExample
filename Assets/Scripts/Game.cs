using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using TMPro;
using Unity.Collections;
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
	public TextMeshProUGUI Timer;
	public TextMeshProUGUI ScoreLabel;
	public GameObject GameOverScreen;
	public Transform DeathPosition;
	public GameObject SelectionPrefab;
	public GameObject ExplosionPrefab;
	public float Speed = 20;
	public float Acceleration = 10;

	public LevelDescription LevelDescription { get; set; }

	public readonly Dictionary<int2, Entity> SlotCache = new Dictionary<int2, Entity>();

	public void Start()
	{
		LevelDescription = Level.Value;

		_entityManager = World.Active.GetOrCreateManager<EntityManager>();
		ProcessLevelDescription();
		World.Active.GetOrCreateManager<UserControlSystem>().Setup(this);
		World.Active.GetOrCreateManager<FindCombinationsSystem>().Setup(SlotCache, LevelDescription);
		World.Active.GetOrCreateManager<GeneratorSystem>().Setup(ChipPrefabs, SlotCache, LevelDescription);
		World.Active.GetOrCreateManager<FallSystem>().Setup(SlotCache, LevelDescription);
		World.Active.GetOrCreateManager<GameTimerSystem>().Setup(Timer, GameOverScreen);
		World.Active.GetOrCreateManager<MoveDeadChipsToScore>().Setup(DeathPosition.position, Speed, Acceleration);
		World.Active.GetOrCreateManager<ShowSelectionSystem>().Setup(SelectionPrefab);
		World.Active.GetOrCreateManager<SyncScoreSystem>().Setup(ScoreLabel);
		World.Active.GetOrCreateManager<PlayExplosionOnChipsDestroy>().Setup(ExplosionPrefab);

		_entityManager.CreateEntity(typeof(AnalyzeField));

		var score = _entityManager.CreateEntity();
		_entityManager.AddComponentData(score, new Score() {Value = 0});

		var time = _entityManager.CreateEntity();
		_entityManager.AddComponentData(time, new GameTime() {Seconds = LevelDescription.Time});
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

	private void OnDestroy()
	{
		var allEntities = _entityManager.GetAllEntities(Allocator.Temp);
		for (int i = allEntities.Length - 1; i >= 0; i--)
		{
			_entityManager.DestroyEntity(allEntities[i]);
		}
		allEntities.Dispose();
	}

	public int2 GetIndex(Vector3 worldPosition)
	{
		var localPoint = worldPosition - Center.transform.position;
		localPoint.x = localPoint.x + Level.Value.Width / 2f ;
		localPoint.y = localPoint.y + Level.Value.Height / 2f ;

		return new int2(Mathf.FloorToInt(localPoint.x), Mathf.FloorToInt(localPoint.y));
	}

}