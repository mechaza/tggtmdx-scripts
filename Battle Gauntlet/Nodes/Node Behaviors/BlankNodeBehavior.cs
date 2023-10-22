using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Gauntlet.Nodes {

	/// <summary>
	/// Doesn't do shit.
	/// </summary>
	public class BlankNodeBehavior : GauntletNodeBehavior, IOnEnterNode {

		#region INTERFACE IMPLEMENTATION - IONENTERNODE
		public GauntletReaction OnEnterNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				GauntletMenuController.instance.NodeTitle.SetVisualsActive(false);
				reactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region INSPECTOR JUNK
		private string inspectorDescription = "Doesn't do shit.";
		protected override string InspectorDescription {
			get {
				return this.inspectorDescription;
			}
		}
		#endregion
	}


}