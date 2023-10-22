using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Chat;
using Grawly.UI;

namespace Grawly.Toggles.Audio {

	/// <summary>
	/// Adjusts the master volume for the entire application.
	/// </summary>
	[System.Serializable, Title("Dialogue Volume")]
	public class DialogueVolume : StandardFloatToggle, GTGameBootHandler {

		#region MAIN FUNCTIONALITY
		/// <summary>
		/// Find the ChatController's text box, then find the audio source.
		/// </summary>
		private void Execute() {
			try {
				(ChatControllerDX.StandardInstance.TextBox as StandardChatTextBox).ChatBoxAudioSource.volume = this.GetToggleFloat();
			} catch (System.Exception e) {
				Debug.LogError("Error changing text box volume. It may not be in the scene?");
			}
		}
		#endregion

		/// <summary>
		/// Upon horizontal shifting, find the chat controller and tell the chat box's audio source to adjust its volume.
		/// </summary>
		/// <param name="moveDir"></param>
		public override void OnHorizontalMenuMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMenuMove(moveDir);
			this.Execute();
		}
		public void OnGameInitialize() {
			this.Execute();
			// THE CHAT CONTROLLER IS RESPONSIBLE FOR THIS NOW BECAUSE
			// IT NEEDS TO LOAD AND ITS NOT GUARANTEED TO BE READY WHEN THIS GETS CALLED.
		}

		#region FIELDS - METADATA
		public override string ToggleName => "Dialogue Blip Volume";
		public override string ToggleDescription => "Scales the volume of the entire application.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Audio;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

		/*public override void Shift(ToggleShiftType shiftType) {
			throw new System.NotImplementedException();
		}*/
	}


}