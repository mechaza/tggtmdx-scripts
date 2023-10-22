using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Gauntlet;
using HutongGames.PlayMaker;

namespace Grawly.PlayMakerActions {

	/// <summary>
	/// A shoehorned in method for pushing/popping the last selected node.
	/// Specifically good for when I'm handling shit in the pause menu.
	/// </summary>
	[ActionCategory("Grawly - Gauntlet")]
	public class PushPopGauntletNode : FsmStateAction {

		#region EVENTS
		public override void OnEnter() {
			GauntletController.instance?.SetFSMState(GauntletStateType.Free);
			// throw new System.Exception("ahh.. dont ever use this");
			// GauntletController.Instance?.ReselectCurrentNode();
			base.Finish();
		}
		#endregion

	}


}