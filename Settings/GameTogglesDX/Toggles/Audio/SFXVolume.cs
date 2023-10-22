using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.UI;

namespace Grawly.Toggles.Audio {

	/// <summary>
	/// Adjusts the master volume for the entire application.
	/// </summary>
	[System.Serializable, Title("SFX Volume")]
	public class SFXVolume : StandardFloatToggle, GTGameBootHandler {

		#region MAIN FUNCTIONALITY
		/// <summary>
		/// Just change the sfx volume.
		/// I'll call this from any kinds of events, like a horizontal event or something. Idk. I'm high.
		/// </summary>
		private void Execute() {
			AudioController.instance?.SetSFXVolume(volume: this.GetToggleFloat());
		}
		#endregion

		public override void OnHorizontalMenuMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMenuMove(moveDir);
			this.Execute();
		}

		public void OnGameInitialize() {
			this.Execute();
		}

		#region FIELDS - METADATA
		public override string ToggleName => "SFX Volume";
		public override string ToggleDescription => "The volume to play sound effects at.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Audio;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

		/*public override void Shift(ToggleShiftType shiftType) {
			throw new System.NotImplementedException();
		}*/
	}


}