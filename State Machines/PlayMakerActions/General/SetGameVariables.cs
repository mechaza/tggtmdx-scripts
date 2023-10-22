using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace Grawly.PlayMakerActions {

	/// <summary>
	/// Assigns the specified variables to the GameController.
	/// </summary>
	[ActionCategory("Grawly - General"), Tooltip("Assigns the specified variables to the GameController.")]
	public class SetGameVariables : FsmStateAction {

		#region FIELDS
		/// <summary>
		/// The template to load up.
		/// </summary>
		[ObjectType(typeof(GameVariablesTemplate)), Tooltip("The variables assign to the GameController.")]
		public FsmObject gameVariablesTemplate;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			// whomp whomp
			GameController.Instance?.SetVariables(template: ((GameVariablesTemplate)this.gameVariablesTemplate.Value));
			base.Finish();
		}
		#endregion

	}


}