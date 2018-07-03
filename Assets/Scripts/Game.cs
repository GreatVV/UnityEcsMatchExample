using System.Collections.Generic;
using TMPro;
using UndergroundMatch3.Components;
using UndergroundMatch3.Data;
using UndergroundMatch3.Data.Steps;
using UndergroundMatch3.Systems;
using UndergroundMatch3.UI.Screens;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace UndergroundMatch3
{
	public class Game : MonoBehaviour
	{
		public GameOverScreen GameOverScreen;
		public GameScreen GameScreen;

		public ConfigurationAsset Configuration;
		public SceneConfiguration SceneConfiguration;
		public LevelDescription LevelDescription { get; set; }

		private EntityManager _entityManager;

		public readonly Dictionary<int2, Entity> SlotCache = new Dictionary<int2, Entity>();

		public void Start()
		{
			LevelDescription = SceneConfiguration.Level.Value;

			var w = World.Active;
			_entityManager = w.GetOrCreateManager<EntityManager>();
			ProcessLevelDescription();
			w.GetOrCreateManager<UserControlSystem>().Setup(SceneConfiguration, SlotCache);
			w.GetOrCreateManager<FindCombinationsSystem>().Setup(SlotCache, LevelDescription);
			w.GetOrCreateManager<GeneratorSystem>().Setup(Configuration, SlotCache, LevelDescription);
			w.GetOrCreateManager<FallSystem>().Setup(SlotCache, LevelDescription);
			w.GetOrCreateManager<GameTimerSystem>().Setup(GameScreen, GameOverScreen);
			w.GetOrCreateManager<MoveDeadChipsToScore>().Setup(SceneConfiguration, Configuration);
			w.GetOrCreateManager<ShowSelectionSystem>().Setup(Configuration);
			w.GetOrCreateManager<SyncScoreSystem>().Setup(GameScreen);
			w.GetOrCreateManager<PlayExplosionOnChipsDestroySystem>().Setup(Configuration);

			_entityManager.CreateEntity(typeof(AnalyzeField));

			var score = _entityManager.CreateEntity();
			_entityManager.AddComponentData(score, new Score() {Value = 0});

			var time = _entityManager.CreateEntity();
			_entityManager.AddComponentData(time, new GameTime() {Seconds = LevelDescription.Time});
		}

		public void ProcessLevelDescription()
		{
			var steps = new List<ICreationPipelineStep>();
			steps.Add(new CreateSlotsStep(SlotCache, SceneConfiguration));
			steps.Add(new CreateChipsStep(Configuration));

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



	}
}