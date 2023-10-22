using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.BattleMenu;
using Grawly.Battle.WorldEnemies;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.Serialization;

namespace Grawly.Battle.BattleArena {

	/// <summary>
	/// Controls the WorldEnemyDX's as a whole. 
	/// </summary>
	public class BattleArenaControllerDX : MonoBehaviour {

		public static BattleArenaControllerDX instance;

		#region FIELDS - STATE : WORLD ENEMIES
		/// <summary>
		/// A list of enemies currently active in battle.
		/// This is what the BattleController reads when it needs to figure out what enemies are left.
		/// </summary>
		public List<WorldEnemyDX> ActiveWorldEnemyDXs { get; private set; }
		#endregion

		#region FIELDS - SCENE REFERENCES : ENEMIES
		/// <summary>
		/// The WorldEnemyDX's to use by default in this arena. These are usually just the WorldEnemyDX sprites.
		/// </summary>
		[FormerlySerializedAs("defaultWorldEnemyDXs")]
		[SerializeField, TabGroup("Scene References", "Default World Enemies")]
		private List<WorldEnemyDX> defaultEnemySprites = new List<WorldEnemyDX>();
		/// <summary>
		/// The WorldEnemyDX's to use by default in this arena. These are usually just the WorldEnemyDX sprites.
		/// Accessible to any classes within the same namespace.
		/// </summary>
		internal List<WorldEnemyDX> DefaultEnemySprites {
			get {
				return this.defaultEnemySprites;
			}
		}
		/// <summary>
		/// The list of transforms where 3D enemies should be placed, if any are in battle.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Default World Enemies")]
		private List<Transform> default3DEnemyPositions = new List<Transform>();
		/// <summary>
		/// The list of transforms where 3D enemies should be placed, if any are in battle.
		/// </summary>
		internal List<Transform> Default3DEnemyPositions {
			get {
				return this.default3DEnemyPositions;
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES : ANCHORS
		/// <summary>
		/// A reference to the game object that contains all the visuals for the battle arena.
		/// Typically gets repositioned from a battle setup.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Anchors")]
		internal GameObject battleArenaGameObject;
		/// <summary>
		/// The game object that basically just acts as an anchor for where the fight cloud should be.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Anchors")]
		private GameObject fightCloudAnchorGameObject;
		/// <summary>
		/// The position where the fight cloud for the all out attack should be spawned.
		/// </summary>
		public Vector3 FightCloudPosition {
			get {
				return this.fightCloudAnchorGameObject.transform.position;
			}
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Preps the arena for use by taking in the battle template which contains the enemies/proceedures for setting things up.
		/// </summary>
		/// <param name="battleTemplate">The BattleTemplate that will be used for setting up this battle.</param>
		/// <param name="battleParams">The routine set containing the arena setup behavior itself.</param>
		public void PrepareBattleArena(BattleTemplate battleTemplate, BattleParams battleParams) {
			
			// Run the arena setup stored in the template and save the references to the enemies that are going to be used in this battle.
			this.ActiveWorldEnemyDXs = battleParams.ArenaSetup.SetupBattleArena(
				battleArenaController: this, 
				arenaPosition: BattleArenaDXPosition.instance,
				battleTemplate: battleTemplate);
			
		}
		#endregion

		#region REMOVAL
		/// <summary>
		/// Removes the specified world enemies from the battle arena.
		/// </summary>
		/// <param name="enemiesToRemove">A list of enemies to totally remove from the arena.</param>
		/// <param name="battleParams">The parameters that contain routines specific to this battle..</param>
		public void RemoveWorldEnemies(List<Enemy> enemiesToRemove, BattleParams battleParams) {
			battleParams.ArenaSetup.RemoveEnemiesFromBattle(
				enemiesToRemove: enemiesToRemove, 
				battleArenaController: this);
		}
		#endregion

	}


}