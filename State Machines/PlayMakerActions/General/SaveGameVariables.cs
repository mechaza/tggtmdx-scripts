using System.Collections;
using System.Collections.Generic;
using Grawly;
using HutongGames.PlayMaker;
using System.IO;
namespace Grawly.PlayMakerActions {

	/// <summary>
	/// Saves the variables currently in the GameController into the specified slot.
	/// </summary>
	[ActionCategory("Grawly - General"), Tooltip("Saves the variables currently in the GameController into the specified slot.")]
	public class SaveGameVariables : FsmStateAction  {

		#region FIELDS
		/// <summary>
		/// The slot to save the variables in.
		/// </summary>
		[Tooltip("The slot to save the variables in.")]
		public FsmInt slotToSaveIn;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			// Tell the Save
			FileStream file = SaveController.SaveGameVariables(
				variables: GameController.Instance.Variables, 
				incrementSaveCount: true,
				slot: this.slotToSaveIn.Value);
			base.Finish();
		}
		#endregion

	}


}