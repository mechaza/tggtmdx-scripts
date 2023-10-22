using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.UI;

namespace Grawly.Toggles.Audio {

	/// <summary>
	/// Adjusts the master volume for the entire application.
	/// </summary>
	[System.Serializable, Title("Music Volume")]
	public class MusicVolume : StandardFloatToggle, GTGameBootHandler {

		#region MAIN FUNCTIONALITY
		/// <summary>
		/// Just changes the static variable on the introloop track class.
		/// I'm a little hands off with IntroLoop.
		/// </summary>
		private void Execute() {
			IntroloopTrack.GRAWLY_MUSIC_VOLUME_OVERRIDE = this.GetToggleFloat();
		}
		#endregion

		/// <summary>
		/// When moving left or right in the menu, update the index (done in base)
		/// and then reassign the Introloop's music override.
		/// </summary>
		/// <param name="moveDir"></param>
		public override void OnHorizontalMenuMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMenuMove(moveDir);
			this.Execute();
		}
		public void OnGameInitialize() {
			this.Execute();
		}

		#region FIELDS - METADATA
		public override string ToggleName => "Music Volume";
		public override string ToggleDescription => "The volume to play music at.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Audio;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

		/*public override void Shift(ToggleShiftType shiftType) {
			throw new System.NotImplementedException();
		}*/
	}


}