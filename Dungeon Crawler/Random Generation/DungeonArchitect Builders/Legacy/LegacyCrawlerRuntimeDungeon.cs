using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI.Legacy;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler.Legacy {
	
	/// <summary>
	/// Provides access to generating a crawler dungeon at runtime.
	/// </summary>
	public class LegacyCrawlerRuntimeDungeon : MonoBehaviour {
		public static LegacyCrawlerRuntimeDungeon Instance { get; private set; }

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should the dungeon be generated on start?
		/// </summary>
		[SerializeField, Title("Toggles")]
		private bool generateOnStart = false;
		#endregion
		
		#region FIELDS - STATE : PARAMETERS
		/// <summary>
		/// The params that are currently in use for this dungeon session.
		/// </summary>
		private LegacyCrawlerSessionParams CurrentSessionParams { get; set; }
		/// <summary>
		/// The params used to build the current floor.
		/// </summary>
		private LegacyCrawlerFloorParams CurrentFloorParams { get; set; }
		#endregion

		#region FIELDS - STATE : ROUTINES
		/// <summary>
		/// The currently executing transition routine, if one is running.
		/// This is needed in the event there's a hard reset and I need to exit.
		/// </summary>
		private Coroutine CurrentTransitionRoutine { get; set; }
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
			// this.CurrentRuntimeDungeon = this.GetComponent<RuntimeDungeon>();
		}
		private void Start() {
			
			// If generating on start, prepare and build.
			if (this.generateOnStart == true) {
				this.PrepareSession();
				this.BuildFloor(floorNumber: 1);
			}
			
		}
		private void OnDestroy() {
			// STOP the transition routine on destroy.
			StopCoroutine(this.CurrentTransitionRoutine);
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Prepares the dungeon session with the default parameters.
		/// Generates a seed on its own.
		/// </summary>
		public void PrepareSession() {
			// Generate a seed and cascade down.
			this.PrepareSession(seed: Random.Range(minInclusive: 0, maxExclusive: 100000));
		}
		/// <summary>
		/// Prepares the dungeon session with default parameters.
		/// </summary>
		/// <param name="seed">The seed to use for this session.</param>
		public void PrepareSession(int seed) {
			throw new System.NotImplementedException("There is no more DunGen.");
			// Create a new parameters object with the default info and prepare with that.
			/*this.PrepareSession(sessionParams: new CrawlerSessionParams() {
				CurrentSeed = seed,
				CurrentPlayer = CrawlerPlayer.Instance,
				CurrentRuntimeDungeon = this.GetComponent<RuntimeDungeon>()
			});*/
			
		}
		/// <summary>
		/// Prepares the dungeon with the parameters passed in.
		/// </summary>
		/// <param name="sessionParams">The parameters to use in preparing the dungeon.</param>
		private void PrepareSession(LegacyCrawlerSessionParams sessionParams) {
			
			// Save the session params.
			this.CurrentSessionParams = sessionParams;
			
			// Set the seed on the generator.
			throw new NotImplementedException("There is no more DunGen.");
			// sessionParams.CurrentRuntimeDungeon.Generator.Seed = sessionParams.CurrentSeed;
			
			// Add a callback for the post processing step.
			throw new System.NotImplementedException("There is no more DunGen.");
			// sessionParams.CurrentRuntimeDungeon.Generator.RegisterPostProcessStep(postProcessCallback: this.OnGenerationComplete);
			
		}
		#endregion

		#region DUNGEON GENERATION - BUILDING
		/// <summary>
		/// Builds the "next" floor by incrementing by the given amount.
		/// </summary>
		/// <param name="incrementAmount">The amount to increment the floor count by.</param>
		/// <param name="startingTileType">The type of tile to warp to on build.</param>
		public void BuildNextFloor(int incrementAmount, CrawlerTileType startingTileType) {

			// Calculate the next floor.
			int nextFloor = this.CurrentFloorParams.FloorNumber + incrementAmount;
			
			// Build it.
			this.BuildFloor(floorNumber: nextFloor, startingTileType: startingTileType);

		}
		/// <summary>
		/// Builds the floor of the given number and warps to the specified starting tile.
		/// </summary>
		/// <param name="floorNumber">The floor to build.</param>
		/// <param name="startingTileType">The tile to start on.</param>
		private void BuildFloor(int floorNumber, CrawlerTileType startingTileType = CrawlerTileType.Start) {
			
			// Create a new set of floor params with a new floor count.
			LegacyCrawlerFloorParams floorParams = new LegacyCrawlerFloorParams() {
				FloorNumber = floorNumber,
				StartingTileType = startingTileType
			};
			
			// Use these new params to build the floor.
			this.BuildFloor(floorParams: floorParams);
			
		}
		/// <summary>
		/// Builds the floor with the given floor and session params.
		/// </summary>
		/// <param name="floorParams">The params for the desired floor.</param>
		private void BuildFloor(LegacyCrawlerFloorParams floorParams) {
			
			// Start the build routine and save a reference to it.
			this.CurrentTransitionRoutine = GameController.Instance.StartCoroutine(
				routine: this.BuildFloorRoutine (
					floorParams: floorParams, 
					sessionParams: this.CurrentSessionParams
				));

		}
		/// <summary>
		/// Builds the floor with the given floor and session params.
		/// </summary>
		/// <param name="floorParams">The params for the desired floor.</param>
		/// <param name="sessionParams">The parameters for the entire session.</param>
		private IEnumerator BuildFloorRoutine(LegacyCrawlerFloorParams floorParams, LegacyCrawlerSessionParams sessionParams) {
			
			// Tell the player to wait.
			sessionParams.CurrentPlayer.SetState(CrawlerPlayerState.Wait);
			
			// Fade out.
			Flasher.FadeOut();
			
			// Wait a second.
			yield return new WaitForSeconds(1f);
			
			// Save the floor params.
			this.CurrentFloorParams = floorParams;
			
			// Set the seed to be the session seed plus the floor number.
			
			// sessionParams.CurrentRuntimeDungeon.Generator.Seed = sessionParams.CurrentSeed + floorParams.FloorNumber;

			// Build the floor.
			throw new NotImplementedException("There is no more DunGen.");
			// sessionParams.CurrentRuntimeDungeon.Generate();

			// The callback that relocates the player was registered on prepare,
			// so that will get run when its done generating.
			
		}
		#endregion

		/*#region DUNGEON GENERATION - MANIPULATION
		/// <summary>
		/// Relocates the player to the tile of the specified type.
		/// </summary>
		/// <param name="targetTileType">The type of tile to relocate to.</param>
		/// <param name="crawlerPlayer">The crawler player to relocate.</param>
		private void RelocatePlayer(CrawlerTileType targetTileType, CrawlerPlayer crawlerPlayer) {

			// The target tile type should be either start or goal.
			Debug.Assert(targetTileType == CrawlerTileType.Start || targetTileType == CrawlerTileType.Goal);
			
			// Find the stairs tile to relocate to.
			StairsTile stairsTile = (targetTileType == CrawlerTileType.Start)
				? (StairsTile)StartTile.Instance
				: (StairsTile)GoalTile.Instance;
			
			// Use the target transform on the stairs tile to set the position/rotation.
			crawlerPlayer.transform.SetPositionAndRotation(
				position: stairsTile.TargetTransform.position,
				rotation: stairsTile.TargetTransform.rotation);
			
		}
		#endregion*/

	}

}