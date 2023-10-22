using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Grawly.Battle.Outros {

	/// <summary>
	/// The routine that gets called when exiting from a battle after its completion.
	/// </summary>
	public abstract class BattleOutro {

		/// <summary>
		/// The way in which the battle should be exited if the player wins, or the conditions for completion have been met.
		/// </summary>
		/// <param name="template"></param>
		public abstract void PlayOutro(BattleTemplate template, BattleParams battleParams, BattleResultsSet battleResultsSet);
		/// <summary>
		/// Gets called after Battle Complete after any extrenuous shit has been taken care of.
		/// Typically coming back from the results screen, but sometimes that may not happen.
		/// </summary>
		/// <param name="template"></param>
		public abstract void ReturnToCaller(BattleTemplate template, BattleParams battleParams);

	}

}