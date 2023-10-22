using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Grawly.Chat;
using DG.Tweening;
using Grawly.Dungeon.Legacy;
using Sirenix.OdinInspector;
using Grawly.Story;
using Grawly.Toggles;
using Grawly.Toggles.Audio;

namespace Grawly.Dungeon {
	
	/// <summary>
	/// Defines an area in which 
	/// </summary>
	[RequireComponent(typeof(BoxCollider))]
	public class DungeonEnemySpawnArea : MonoBehaviour, IPlayerCollisionHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The amount of time the DungeonPlayer has spent inside the spawn area.
		/// </summary>
		private float CurrentTimeInside { get; set; } = 0f;
		/// <summary>
		/// The amount of time the DungeonPlayer has traveled inside the spawn area.
		/// </summary>
		private float CurrentTravelDistanceInside { get; set; } = 0f;
		/// <summary>
		/// The last known position of the player.
		/// Helps with calculating distance traveled.
		/// </summary>
		private Vector3 LastKnownPlayerPosition { get; set; } = new Vector3();
		#endregion

		#region FIELDS - TOGGLES : ACTIVATION
		/// <summary>
		/// The amount of time to take until spawning an enemy.
		/// This is temporary.
		/// </summary>
		[SerializeField, TabGroup("Spawn", "Toggles")]
		private float timeToSpawn = 10f;
		/// <summary>
		/// The minimum amount of distance that must be traveled in order to spawn an enemy.
		/// </summary>
		[SerializeField, TabGroup("Spawn", "Toggles")]
		private float minimumTravelDistance = 5f;
		#endregion
		
		#region FIELDS - TOGGLES : BATTLES
		/// <summary>
		/// The list of BattleTemplates to pick from when using this spawn area.
		/// </summary>
		[Title("Battles")]
		[SerializeField, TabGroup("Spawn", "Toggles")]
		private List<BattleTemplate> battleTemplates = new List<BattleTemplate>();
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The BoxCollider attached to this spawn area.
		/// </summary>
		private BoxCollider BoxCollider { get; set; }
		#endregion

		#region FIELDS - RESOURCES
		/// <summary>
		/// A list of prefabs to pick from when spawning an enemy.
		/// </summary>
		[SerializeField, TabGroup("Spawn", "Resources")]
		private List<GameObject> dungeonEnemyPrefabs = new List<GameObject>();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			this.BoxCollider = this.GetComponent<BoxCollider>();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the spawn area.
		/// </summary>
		private void ResetState() {
			// Zero out the time inside/distance traveled inside.
			this.CurrentTimeInside = 0f;
			this.CurrentTravelDistanceInside = 0f;
			// this.LastKnownPlayerPosition = Vector3.zero;
		}
		#endregion

		#region STATUS CHECKING
		/// <summary>
		/// Checks whether or not an enemy can be spawned in this area.
		/// </summary>
		/// <param name="timeInsideArea">The amount of time spent inside this spawn area in particular.</param>
		/// <param name="distanceTraveledInArea">The distance traveled while inside of this spawn area.</param>
		/// <param name="timeSinceLastBattle">The amount of time since the last battle.</param>
		/// <returns>Whether or not an enemy can be spawned in this area.</returns>
		private bool ReadyToSpawn(float timeInsideArea, float distanceTraveledInArea) {
			
			// Check if enough time has passed inside of this area.
			bool enoughTimePassedInside = timeInsideArea >= this.timeToSpawn;

			// Check if enough distance has been traveled inside the area.
			bool enoughDistanceTraveledInside = distanceTraveledInArea >= this.minimumTravelDistance;

			// If all of these checks are okay, an enemy can be spawned.
			return enoughTimePassedInside && enoughDistanceTraveledInside;

		}
		#endregion
		
		#region SPAWNING
		/// <summary>
		/// Spawns a totally random enemy at the provided spawn point
		/// </summary>
		/// <param name="spawnPoint">The point to spawn the enemy at.</param>
		private void SpawnRandomEnemy(IDungeonSpawnPoint spawnPoint) {
			// Just cascade down.
			this.SpawnRandomEnemy(
				targetPosition: spawnPoint.SpawnPointPosition, 
				targetRotation: spawnPoint.SpawnPointRotation);
		}
		/// <summary>
		/// Spawns a totally random enemy at the provided position/rotation.
		/// </summary>
		/// <param name="targetPosition">The position to spawn the enemy at.</param>
		/// <param name="targetRotation">The rotation to spawn the enemy at.</param>
		private void SpawnRandomEnemy(Vector3 targetPosition, Quaternion targetRotation) {
			
			// Grab a random enemy prefab/template.
			GameObject enemyPrefab = this.dungeonEnemyPrefabs.Random();
			BattleTemplate battleTemplate = this.battleTemplates.Random();
			
			// Use these to spawn the enemy.
			this.SpawnEnemy(
				enemyPrefab: enemyPrefab, 
				battleTemplate: battleTemplate, 
				targetPosition: targetPosition, 
				targetRotation: targetRotation);
			
		}
		/// <summary>
		/// Instantates an enemy at the specified position and rotation,
		/// then prepares it with the provided BattleTemplate.
		/// </summary>
		/// <param name="enemyPrefab">The prefab to instantiate.</param>
		/// <param name="battleTemplate">The BattleTemplate to prepare the enemy with.</param>
		/// <param name="targetPosition">The position to spawn the enemy prefab at.</param>
		/// <param name="targetRotation">The rotation to put the enemy at when spawning.</param>
		private void SpawnEnemy(GameObject enemyPrefab, BattleTemplate battleTemplate, Vector3 targetPosition, Quaternion targetRotation) {
			
			// Instantiate the enemy and grab a reference to its DungeonEnemy.
			DungeonEnemy spawnedEnemy = GameObject.Instantiate(
				original: enemyPrefab,
				position: targetPosition,
				rotation: targetRotation)
				.GetComponent<DungeonEnemy>();
			
			// Prep the enemy with the template.
			spawnedEnemy.Prepare(battleTemplate: battleTemplate);
			
			// Use the spawn effect.
			spawnedEnemy.SpawnEnemyEffect();
			
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IPLAYERCOLLISIONHANDLER
		/// <summary>
		/// Gets called when the DungeonPlayer enters a special collision.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		public void OnPlayerCollisionEnter(DungeonPlayer dungeonPlayer) {
			
			// Upon entering, reset the state.
			this.ResetState();
			
			// Take note of the DungeonPlayer's position.
			this.LastKnownPlayerPosition = dungeonPlayer.transform.position;
			
		}
		/// <summary>
		/// Gets called when the DungeonPlayer exits a special collision.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		public void OnPlayerCollisionExit(DungeonPlayer dungeonPlayer) {
			
		}
		/// <summary>
		/// Gets called when the DungeonPlayer simply stays in an area.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		public void OnPlayerCollisionStay(DungeonPlayer dungeonPlayer) {
			
			// Only increment the time if the player is actually free.
			if (dungeonPlayer.CurrentState == DungeonPlayerStateType.Free) {
				
				// Increment by fixed delta time.
				this.CurrentTimeInside += Time.fixedDeltaTime;

				// Calculate the distance traveled and update the last known player position.
				Vector3 currentPlayerPosition = dungeonPlayer.transform.position;
				float distanceTraveled = (currentPlayerPosition - this.LastKnownPlayerPosition).magnitude;
				this.CurrentTravelDistanceInside += distanceTraveled;
				this.LastKnownPlayerPosition = currentPlayerPosition;
				
				// Check if the area is ready to spawn...
				bool readyToSpawn = this.ReadyToSpawn(
					timeInsideArea: this.CurrentTimeInside, 
					distanceTraveledInArea: this.CurrentTravelDistanceInside);

				// ...as well as if the wheel itself is ready.
				bool spawnWheelReady = dungeonPlayer.SpawnWheel.HasValidPoints();
				
				// If actually ready and points are available...
				if (readyToSpawn == true && spawnWheelReady == true) {
					// ...grab a random spawn wheel point.
					var spawnWheelPoint = dungeonPlayer.SpawnWheel.GetRandomValidPoint();
					// Use this to spawn a random enemy.
					this.SpawnRandomEnemy(spawnPoint: spawnWheelPoint);
					// Reset the state.
					this.ResetState();
				}
				
			}
			
		}
		#endregion
		
	}
}