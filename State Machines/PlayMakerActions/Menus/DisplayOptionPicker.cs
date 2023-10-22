using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Grawly.PlayMakerActions {

	/// <summary>
	/// Displays the option picker and asks the player for a prompt.
	/// </summary>
	[ActionCategory("Grawly - Menus"), HutongGames.PlayMaker.Tooltip("Displays the option picker and asks the player for a prompt. When done, will send an event called 'Button [index]'.")]
	public class DisplayOptionPicker : FsmStateAction, IOptionPickerUser {

		#region FIELDS
		/// <summary>
		/// The prompt to display to the player.
		/// </summary>
		[HutongGames.PlayMaker.Tooltip("The prompt to display to the player.")]
		public FsmString prompt;
		/// <summary>
		/// The first option to display.
		/// </summary>
		[HutongGames.PlayMaker.Tooltip("The first option to display.")]
		public FsmString optionOne;
		/// <summary>
		/// The second option to display.
		/// </summary>
		[HutongGames.PlayMaker.Tooltip("The second option to display.")]
		public FsmString optionTwo;
		/// <summary>
		/// Should Unity's event system re-select the game object that was previously being selected when a selection is made?
		/// </summary>
		[HutongGames.PlayMaker.Tooltip("Should Unity's event system re-select the game object that was previously being selected when a selection is made?")]
		public FsmBool reselectGameObjectOnSubmit;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			// Tell the option picker to display the prompt.
			OptionPicker.instance.Display(prompt: this.prompt.Value, options: new List<string>() { this.optionOne.Value, this.optionTwo.Value }, sourceOfCall: this);
			base.Finish();
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IOPTIONPICKERUSER
		public void OptionPicked(int buttonId, GameObject lastSelectedGameObject) {
			// If told to do so, reselect the game object that was previously being selected by the EventSystem.
			if (this.reselectGameObjectOnSubmit.Value == true) {
				EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
			}
			// Tell the fsm that the specified button was picked.
			base.Fsm.FsmComponent.SendEvent("Button" + buttonId);
		}
		#endregion

	}


}