using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon.Enemy {
	
	/// <summary>
	/// The NEW way in which Dungeon Enemies should behave.
	/// Fuck Off This Is My Video Game
	/// </summary>
	public abstract class DungeonEnemyDX : MonoBehaviour, IDungeonPlayerAttackHandler {

		#region PROPERTIES - STATE
		/// <summary>
		/// The current state of this DungeonEnemyDX.
		/// Helpful to infer what they are doing.
		/// </summary>
		public abstract DungeonEnemyStateType CurrentState { get; }
		#endregion

		#region INTERFACE IMPLEMENTATION - IPLAYERATTACKHANDLER
		/// <summary>
		/// Should the DungeonPlayer be locked upon invokation of this handler?
		/// Good for situations where an enemy is attacked and I need to make sure the player isn't freed.
		/// </summary>
		public abstract bool LockDungeonPlayer { get; }
		/// <summary>
		/// The function that should be called when a dungeon player attacks this enemy.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who initated the attack.</param>
		public abstract void OnDungeonPlayerAttack(DungeonPlayer dungeonPlayer);
		#endregion
	

	}
}