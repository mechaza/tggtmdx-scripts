using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Grawly.Battle.DropParsers {

	/// <summary>
	/// Gauntlets will parse drops differently than Dungeons (former is delayed, latter is immediate) so I thought I might as well make this a component of the BattleTemplate as well.
	/// </summary>
	public abstract class BattleDropParser {

		/// <summary>
		/// The function that parses the results of the battle and decides on what to do from there.
		/// </summary>
		/// <param name="battleTemplate">The template that was used to create the battle.</param>
		/// <param name="battleParams">The BattleParams that were used to make this battle..</param>
		/// <param name="gameVariables">The GameVariables that contain things like the players and inventory and shit.</param>
		public abstract BattleResultsSet ParseBattleDrops(BattleTemplate battleTemplate, BattleParams battleParams, GameVariables gameVariables);

	}

}