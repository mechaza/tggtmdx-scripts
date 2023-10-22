using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Grawly.DungeonCrawler.Generation;
using Grawly.UI.Legacy;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// The script that controls misc things about the dungeon crawler.
	/// </summary>
	[RequireComponent(typeof(CrawlerDungeonGenerator))]
	public class CrawlerController : MonoBehaviour {
		
		public static CrawlerController Instance { get; private set; }
		
		#region FIELDS - RESOURCES
		/// <summary>
		/// The dungeon template to be associated with the scene this crawlercontroller is inside of.
		/// </summary>
		[SerializeField]
		private CrawlerDungeonTemplate crawlerDungeonTemplate;
		#endregion
		
		#region FIELDS - STATE : RUNTIME
		/// <summary>
		/// The current state of the crawler during runtime.
		/// </summary>
		private RuntimeCrawlerState RuntimeState { get; set; } = new RuntimeCrawlerState();
		#endregion

		#region PROPERTIES - STATE : GENERAL
		/// <summary>
		/// The number of the current floor.
		/// </summary>
		public int CurrentFloorNumber => this.RuntimeState.CurrentFloorNumber;
		/// <summary>
		/// The current danger level stored in the runtime state.
		/// </summary>
		public DangerLevelType CurrentDangerLevel => this.RuntimeState.CurrentDangerLevel;
		/// <summary>
		/// The floor template associated whatever floor the player is currently on.
		/// </summary>
		public CrawlerFloorTemplate CurrentFloorTemplate => this.CurrentCrawlerTemplate.GetFloorTemplate(this.CurrentFloorNumber);
		/// <summary>
		/// The ID of the crawler dungeon currently being used.
		/// </summary>
		public CrawlerDungeonIDType CurrentCrawlerDungeonIDType => this.CurrentCrawlerTemplate.CrawlerDungeonIDType;
		/// <summary>
		/// The current template being used for this crawler.
		/// </summary>
		public CrawlerDungeonTemplate CurrentCrawlerTemplate {
			get {
				return this.crawlerDungeonTemplate;
			}
		}
		/// <summary>
		/// The progression set associated with whatever dungeon is currently in use.
		/// </summary>
		public CrawlerProgressionSet CurrentProgressionSet {
			get {
				// Probe the variables for the desired progression set.
				return GameController.Instance.Variables.GetCrawlerProgressionSet(crawlerDungeonIDType: this.CurrentCrawlerDungeonIDType);
			}
		}
		#endregion

		#region FIELDS - EVENTS
		/// <summary>
		/// The event that should get fired when the player steps.
		/// Gets invoked in the player's Move() function,
		/// and is run regardless of if a runtime dungeon is present
		/// </summary>
		public UnityEvent PlayerStepEvent { get; private set; } = new UnityEvent();
		/// <summary>
		/// The event that should get fired when a floor is generated.
		/// </summary>
		public UnityEvent FloorGeneratedEvent { get; private set; } = new UnityEvent();
		#endregion
		
		#region PROPERTIES - TOGGLES : ENCOUNTERS
		/// <summary>
		/// The range of steps to take before the danger level is incremented.
		/// </summary>
		private Vector2Int StepRangePerDangerLevel {
			get {
				// Grab the min/max values for the range.
				int minSteps = Mathf.Max(a: 0, b: this.CurrentFloorTemplate.baseStepsPerDangerLevel - this.CurrentFloorTemplate.rangeStepsPerDangerLevel);
				int maxSteps = this.CurrentFloorTemplate.baseStepsPerDangerLevel + this.CurrentFloorTemplate.rangeStepsPerDangerLevel;
				// Return a new vector.
				return new Vector2Int(x: minSteps, y: maxSteps);
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The component that this controller should talk to to help actually create the maps.
		/// </summary>
		private CrawlerDungeonGenerator CrawlerDungeonGenerator { get; set; }
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
			this.CrawlerDungeonGenerator = this.GetComponent<CrawlerDungeonGenerator>();
			this.PlayerStepEvent.AddListener(this.OnPlayerStep);
		}
		private void Start() {
			
			// Reset the state of the crawler.
			this.ResetDangerState();

			// Show the enemy radar with the current danger level.
			EnemyRadar.Instance?.Show(dangerLevelType: this.RuntimeState.CurrentDangerLevel);

			// Prepare the floor.
			this.Prepare(startFloor: this.RuntimeState.CurrentFloorNumber);

		}
		#endregion

		#region STATE RESETS
		/// <summary>
		/// Totally resets the state of this crawler controller.
		/// </summary>
		private void ResetDangerState() {
			// Reset the state variables.
			this.RuntimeState.CurrentDangerLevelStepCount = 0;
			this.RuntimeState.CurrentDangerLevelStepTrigger = this.StepRangePerDangerLevel.Random();
			this.RuntimeState.CurrentDangerLevel = (this.CurrentFloorTemplate.enableEncounters == true) ? DangerLevelType.Low : DangerLevelType.None;
		}
		/// <summary>
		/// Resets the short term state of the crawler.
		/// </summary>
		private void ResetCrawlerState() {
			this.RuntimeState = new RuntimeCrawlerState();
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Prepares the crawler dungeon by processing the provided ID type and inferring the status from there.
		/// </summary>
		/// <param name="startFloor">The floor that should be built upon preparing this crawler.</param>
		public void Prepare(int startFloor = 0) {
			this.Prepare(
				gameVariables: GameController.Instance.Variables, 
				crawlerDungeonTemplate: this.CurrentCrawlerTemplate, 
				startFloor: startFloor);
		}
		/// <summary>
		/// Preps the CrawlerController with the information provided by the progression data.
		/// This is helpful in making sure state can be restored when coming back to the dungeon after leaving.
		/// </summary>
		/// <param name="gameVariables">The variables which contain the current progression.</param>
		/// <param name="crawlerDungeonTemplate">The template containing the information on how to build this dungeon..</param>
		/// <param name="startFloor">The floor that should be built upon preparing this crawler.</param>
		private void Prepare(GameVariables gameVariables, CrawlerDungeonTemplate crawlerDungeonTemplate, int startFloor) {
			// Cascade down.
			this.Prepare(
				gameVariables: gameVariables,
				crawlerDungeonTemplate: crawlerDungeonTemplate, 
				crawlerDungeonGenerator: this.GetComponent<CrawlerDungeonGenerator>(),
				startFloor: startFloor);
		}
		/// <summary>
		/// Preps the CrawlerController with the information provided by the progression data.
		/// This is helpful in making sure state can be restored when coming back to the dungeon after leaving.
		/// </summary>
		/// <param name="gameVariables">The variables which contain the current progression.</param>
		/// <param name="crawlerDungeonTemplate">The template containing the information on how to build this dungeon..</param>
		/// <param name="crawlerDungeonGenerator">The actual component that will build out this floor.</param>
		/// <param name="startFloor">The floor that should be built upon preparing this crawler.</param>
		private void Prepare(GameVariables gameVariables, CrawlerDungeonTemplate crawlerDungeonTemplate, CrawlerDungeonGenerator crawlerDungeonGenerator, int startFloor) {
			
			// Reset the current runtime state.
			this.ResetCrawlerState();
			this.RuntimeState.CurrentFloorNumber = startFloor;
			
			// Pass this information down to the function that handles actually building the floor.
			this.BuildFloor(
				gameVariables: gameVariables,
				crawlerDungeonTemplate: crawlerDungeonTemplate,
				crawlerDungeonGenerator: crawlerDungeonGenerator, 
				floorNumber: startFloor);
			
		}
		#endregion

		#region FLOOR PROGRESSION
		/// <summary>
		/// Takes care of both building the floor and executing any events that need to be invoked as a result.
		/// </summary>
		/// <param name="gameVariables">The variables which contain the current progression.</param>
		/// <param name="crawlerDungeonTemplate">The template containing the information on how to build this dungeon..</param>
		/// <param name="crawlerDungeonGenerator">The actual component that will build out this floor.</param>
		/// <param name="floorNumber">The actual floor that should be built.</param>
		private void BuildFloor(GameVariables gameVariables, CrawlerDungeonTemplate crawlerDungeonTemplate, CrawlerDungeonGenerator crawlerDungeonGenerator, int floorNumber) {
			
			// Tell the generator to build the "current" floor stored in the progression and save the results.
			CrawlerFloorResultSet floorResultSet = crawlerDungeonGenerator.GenerateFloor(
				dungeonTemplate: crawlerDungeonTemplate,
				floorNumber: floorNumber);

			// Grab a list of the tiles that should handle events upon floor generation and invoke them.
			List<IFloorGeneratedHandler> floorGeneratedHandlers = floorResultSet.GetModifiers<IFloorGeneratedHandler>();
			foreach (IFloorGeneratedHandler floorGeneratedHandler in floorGeneratedHandlers) {
				floorGeneratedHandler.OnFloorGenerated(
					crawlerProgressionSet: gameVariables.GetCrawlerProgressionSet(crawlerDungeonTemplate.CrawlerDungeonIDType), 
					floorNumber: floorNumber);
			}

		}
		#endregion

		#region PROCEDURES
		/// <summary>
		/// Increments the current floor number and builds the next floor.
		/// </summary>
		/// <param name="incrementAmount">The number to increment the current floor by.</param>
		public void IncrementFloor(int incrementAmount) {
			// Begin the routine that will transition the floor.
			this.StartCoroutine(this.IncrementFloorRoutine(
				gameVariables: GameController.Instance.Variables,
				crawlerDungeonTemplate: this.CurrentCrawlerTemplate,
				crawlerDungeonGenerator: this.GetComponent<CrawlerDungeonGenerator>(),
				incrementAmount: incrementAmount));
		}
		/// <summary>
		/// The actual routine that handles moving floors.
		/// </summary>
		/// <param name="gameVariables">The variables which contain the current progression.</param>
		/// <param name="crawlerDungeonTemplate">The template containing the information on how to build this dungeon..</param>
		/// <param name="crawlerDungeonGenerator">The actual component that will build out this floor.</param>
		/// <param name="incrementAmount">The number to increment the current floor by.</param>
		/// <returns></returns>
		private IEnumerator IncrementFloorRoutine(GameVariables gameVariables, CrawlerDungeonTemplate crawlerDungeonTemplate, CrawlerDungeonGenerator crawlerDungeonGenerator, int incrementAmount) {
			
			// Adjust the current floor state based on what was passed in.
			this.RuntimeState.CurrentFloorNumber += incrementAmount;
			
			// Send a signal to the GameVariables that this may be the highest floor.
			gameVariables.UpdateHighestCrawlerFloor(
				crawlerDungeonIDType: crawlerDungeonTemplate.CrawlerDungeonIDType,
				currentFloor: this.RuntimeState.CurrentFloorNumber);
			
			// Tell the player to wait.
			CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Wait);
			
			// Fade out.
			Flasher.FadeOut();
			
			// Wait a second.
			yield return new WaitForSeconds(1f);
			
			// Actually build the floor.
			this.BuildFloor(
				gameVariables: gameVariables,
				crawlerDungeonTemplate: crawlerDungeonTemplate,
				crawlerDungeonGenerator: crawlerDungeonGenerator, 
				floorNumber: this.RuntimeState.CurrentFloorNumber);

			//
			//	Remember: the StartTile already teleports the player to the target location.
			//	You don't have to teleport them in this routine; it's taken care of automatically.
			//
			
			// Wait a little longer.
			yield return new WaitForSeconds(1f);

			// Fade in.
			Flasher.FadeIn();

			// Wait!
			yield return new WaitForSeconds(1f);
			
			// Free!
			CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Free);
			
		}
		#endregion
		
		#region UNITYEVENTS : PLAYER
		/// <summary>
		/// When the player steps, increment the step handlers and shit.
		/// </summary>
		private void OnPlayerStep() {
			// Increment the state count.
			this.RuntimeState.CurrentDangerLevelStepCount += 1;

			// If encounters are disabled, just return.
			if (this.CurrentFloorTemplate.enableEncounters == false) {
				return;
			}

			// If the current step count is higher than the threshold...
			if (this.RuntimeState.CurrentDangerLevelStepCount >= this.RuntimeState.CurrentDangerLevelStepTrigger) {
				// ... Get the next danger level type.
				this.RuntimeState.CurrentDangerLevel = this.GetNextDangerLevel(this.RuntimeState.CurrentDangerLevel);

				// Reset the step count and threshold.
				this.RuntimeState.CurrentDangerLevelStepCount = 0;
				this.RuntimeState.CurrentDangerLevelStepTrigger = this.StepRangePerDangerLevel.Random();

				// Update the radar.
				EnemyRadar.Instance?.SetDanger(dangerLevelType: this.RuntimeState.CurrentDangerLevel);
			}

			// If the specified danger level is encounter...
			if (this.RuntimeState.CurrentDangerLevel == DangerLevelType.Encounter) {
				// ...reset the current state...
				this.ResetDangerState();

				// ...and begin the battle.
				BattleController.Instance.StartBattle(battleTemplate: this.GetBattleTemplate());
			}
		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets a battle template for this dungeon crawler scene.
		/// </summary>
		/// <returns>A battle template for this dungeon crawler.</returns>
		private BattleTemplate GetBattleTemplate() {
			// Go into the current floor template and get a random battle.
			return this.CurrentFloorTemplate.GetRandomBattle();
		}
		#endregion
		
		#region HELPERS : DANGER LEVEL AND ENCOUNTERS
		/// <summary>
		/// Rolls the danger level to the next one.
		/// </summary>
		/// <param name="currentDangerLevel"></param>
		private DangerLevelType GetNextDangerLevel(DangerLevelType currentDangerLevel) {
			// Increment the danger level and pass it to set danger level.
			return currentDangerLevel + 1;
		}
		#endregion
		
	}
}