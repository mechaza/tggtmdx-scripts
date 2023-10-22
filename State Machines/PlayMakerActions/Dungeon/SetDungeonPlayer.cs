using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using Grawly.Dungeon;

namespace Grawly.PlayMakerActions {

	/// <summary>
	/// Sets the status on the DungeonPlayer.
	/// </summary>
	[ActionCategory("Grawly - Dungeon")]
	public class SetDungeonPlayer : FsmStateAction {

		#region FIELDS
		/// <summary>
		/// The state to set the player to.
		/// </summary>
		[ObjectType(typeof(DungeonPlayerStateType))]
		public FsmEnum playerStateType;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			DungeonPlayer.Instance?.SetFSMState(state: ((DungeonPlayerStateType)this.playerStateType.Value));
			base.Finish();
		}
		#endregion

	}


}