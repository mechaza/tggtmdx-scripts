using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle.WorldEnemies;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Grawly.Battle.BattleArena.Setup {

	/// <summary>
	/// This is what handles the setting up of the battle arena.
	/// Usually StandardSetupArena is what's used but you never know.
	/// </summary>
	public abstract class BattleArenaSetup {

		#region MAIN CALLS
		/// <summary>
		/// Sets up the battle arena based on the template passed in.
		/// </summary>
		/// <param name="battleArenaController">The BattleArenaControllerDX which contains references to many of the things the setup may need to access.</param>
		/// <param name="arenaPosition">The position that the arena is intended to be placed.</param>
		/// <param name="battleTemplate">The template to use when setting up the arena.</param>
		/// <returns>A list of world enemies that will be used in the battle. They should be set in stone at this point.</returns>
		public abstract List<WorldEnemyDX> SetupBattleArena(BattleArenaControllerDX battleArenaController, BattleArenaDXPosition arenaPosition, BattleTemplate battleTemplate);
		/// <summary>
		/// Removes the list of enemies from the battle.
		/// </summary>
		/// <param name="enemiesToRemove">The enemies to remove from this battle.</param>
		/// <param name="battleArenaController">The current BattleArenaController.</param>
		public abstract void RemoveEnemiesFromBattle(List<Enemy> enemiesToRemove, BattleArenaControllerDX battleArenaController);
		#endregion

	}

}