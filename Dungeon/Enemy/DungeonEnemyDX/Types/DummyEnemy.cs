using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Invector.vCharacterController;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Grawly.Dungeon.Enemy {
	
	/// <summary>
	/// An enemy that doesn't actually do anything.
	/// </summary>
	public class DummyEnemy : DungeonEnemyDX {

		#region PROPERTIES - STATE
		/// <summary>
		/// The current state of this dummy enemy.
		/// They're basically always going to be idle.
		/// </summary>
		public override DungeonEnemyStateType CurrentState => DungeonEnemyStateType.Idle;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The advantage type to use when this dummy enemy is struck.
		/// </summary>
		[SerializeField, TabGroup("Enemy", "Toggles")]
		private BattleAdvantageType battleAdvantageType = BattleAdvantageType.Normal;
		/// <summary>
		/// Should a custom battle template be used,
		/// instead of relying on the DungeonController(??) to supply it?
		/// </summary>
		[SerializeField, TabGroup("Enemy", "Toggles")]
		private bool useCustomBattleTemplate = false;
		/// <summary>
		/// The BattleTemplate to use, if a custom one is being supplied.
		/// </summary>
		[SerializeField, TabGroup("Enemy", "Toggles"), ShowIf("useCustomBattleTemplate")]
		private BattleTemplate customBattleTemplate;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The BattleTemplate that should be used when this enemy is struck.
		/// </summary>
		private BattleTemplate CurrentBattleTemplate {
			get {
				if (useCustomBattleTemplate == true) {
					return this.customBattleTemplate;
				} else {
					throw new NotImplementedException("IMPLEMENT THIS FEATURE");
				}
			}
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IPLAYERATTACKHANDLER
		/// <summary>
		/// Should the DungeonPlayer be locked upon invokation of this handler?
		/// The case here should be Yes because upon attacking, a battle should be initated.
		/// </summary>
		public override bool LockDungeonPlayer => true;
		/// <summary>
		/// The function that should be called when a dungeon player attacks this enemy.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who initated the attack.</param>
		public override void OnDungeonPlayerAttack(DungeonPlayer dungeonPlayer) {
			Debug.Log("ATTACK CONNECTED");
			BattleController.Instance.StartBattle(
				battleTemplate: this.CurrentBattleTemplate, 
				battleAdvantageType: this.battleAdvantageType, 
				onBattleComplete: () => {
					Destroy(this.gameObject);
				});
		}
		#endregion
		
	}
}