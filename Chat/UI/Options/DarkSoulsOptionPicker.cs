using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;


namespace Grawly.Chat {

	/// <summary>
	/// The Option Picker that should be used for the Dark Souls chat controller.
	/// </summary>
	public class DarkSoulsOptionPicker : ChatOptionPicker {

		#region MAIN CALLS
		/// <summary>
		/// Presents the option picker to display a set of options.
		/// </summary>
		/// <param name="optionPickerParams">The parameters that encapsulate the different kinds of options I may want to present.</param>
		public override void PresentOptions(ChatOptionPickerParams optionPickerParams) {

			// Save a reference to the parameters.
			this.optionPickerParams = optionPickerParams;

			// Also play a sound effect.
			AudioController.instance?.PlaySFX(SFXType.PromptOpen, scale: 1f);

			// If the visuals are not active, turn them on.
			// (This would happen just if I'm working on stuff in scene and don't want them in my way.)
			if (this.allVisuals.activeInHierarchy == false) {
				this.allVisuals.gameObject.SetActive(true);
			}

			// If the chat box is the standard box, call the function that tweens it out of the way.
			/*(optionPickerParams.ChatController.TextBox as DarkSoulsChatController)?.SetOptionPickerState(
				isPickingOption: true,
				currentSpeakerPosition: optionPickerParams.textBoxPosition ?? ChatSpeakerPositionType.None);*/

			// Create a sequence.
			// Sequence seq = DOTween.Sequence();

			// Grab a list of the option items that will be built. This method also turns off the irrelevent ones.
			List<ChatOptionItem> activatedOptionItems = this.DetermineRequiredOptionItems(
				optionPickerParams: optionPickerParams,
				optionItems: this.optionItems);

			// Go through each available option and add a callback that builds it with the parameters.
			foreach (ChatOptionItem optionItem in activatedOptionItems) {
				// HAHAHAHAHAHAHAHAHAHAHAHAHAHAHA
				int indexOfCurrentOptionItem = activatedOptionItems.IndexOf(optionItem);
				optionItem.Build(optionItemParams: optionPickerParams.optionItemParams.ElementAt(indexOfCurrentOptionItem));
			}

			this.HighlightDefaultOption(optionPickerParams: optionPickerParams, defaultOptionItem: activatedOptionItems.First());

		}
		/// <summary>
		/// Dismisses the options after one has been selected.
		/// </summary>
		protected override void DismissOptions() {

			// Dismiss each option item.
			foreach (ChatOptionItem optionItem in this.optionItems) {
				optionItem.Dismiss();
			}

			this.allVisuals.gameObject.SetActive(false);
		}
		/// <summary>
		/// Highlights the default option after the options have been presented.
		/// </summary>
		/// <param name="optionPickerParams">The parameters that encapsulate the different kinds of options I may want to present.</param>
		protected override void HighlightDefaultOption(ChatOptionPickerParams optionPickerParams, ChatOptionItem defaultOptionItem) {

			// Just highlight the first option.
			defaultOptionItem.Highlight();

			// Attempt to select it if its a standard option item.
			try {
				EventSystem.current.SetSelectedGameObject((defaultOptionItem as DarkSoulsOptionItem).Selectable.gameObject);
			} catch (System.Exception e) {

			}
		}
		/// <summary>
		/// Gets called when an option has been picked.
		/// Should normally return control over to the ChatController.
		/// </summary>
		/// <param name="optionPickerParams">The parameters that define how this picker should have been set up.</param>
		/// <param name="optionItemParams">The parameters that built the option item that was selected.</param>
		/// <param name="optionItem">The actual ChatOptionItem that was picked out.</param>
		protected override void PickedOption(ChatOptionPickerParams optionPickerParams, ChatOptionItemParams optionItemParams, ChatOptionItem optionItem) {
			// Dismiss the options.
			this.DismissOptions();

			// Also play a sound effect.
			AudioController.instance?.PlaySFX(SFXType.Select, scale: 1f);

			// Log out what was picked.
			Debug.LogWarning("PICKED: " + optionItemParams.optionLabelText + " -- IF I WANT MORE FUNCTIONALITY MAKE SURE TO REFACTOR THIS LATER");

			// If the item picked sets the toggle, do so.
			if (optionItemParams.IsToggleOption == true) {
				optionPickerParams.ChatController.SetToggle(value: optionItemParams.toggleToSet.Value);
			}

			// If the item picked also performs a jump, do that as well.
			if (optionItemParams.IsJumpOption == true) {
				optionPickerParams.ChatController.JumpToLabel(label: optionItemParams.labelToJumpTo);
			} else {
				// If it's not a jump, just evaluate the next directive.
				optionPickerParams.ChatController.EvaluateNextDirective();
			}

		}
		#endregion

		#region STATE HELPERS
		/// <summary>
		/// Figures out which OptionItems in the scene need to be turned on/turned off.
		/// Returns a list of the ones that can be used.
		/// Also disabled the ones not used automatically.
		/// </summary>
		/// <param name="optionPickerParams">The parameters used in creating this option picker.</param>
		/// <param name="optionItems">The option items being considered for building.</param>
		/// <returns>A list of activated ChatOptionItems ready for building.</returns>
		private List<ChatOptionItem> DetermineRequiredOptionItems(ChatOptionPickerParams optionPickerParams, List<ChatOptionItem> optionItems) {

			// Set all of the options off.
			optionItems.ForEach(i => i.SetActive(false));

			// Go through the parameters that were passed in and figure out how many will be needed.
			switch (optionPickerParams.optionItemParams.Count) {
				case 2:
					// Activate the middle two options and return them in a new list.
					optionItems.ElementAt(0).SetActive(true);
					optionItems.ElementAt(1).SetActive(true);
					return new List<ChatOptionItem>() { optionItems[0], optionItems[1] };
				case 3:
					// Activate the bottom three options and return them in a new list.
					optionItems.ElementAt(0).SetActive(true);
					optionItems.ElementAt(1).SetActive(true);
					optionItems.ElementAt(2).SetActive(true);
					return new List<ChatOptionItem>() { optionItems[0], optionItems[1], optionItems[2] };
				/*case 4:
					// Reactivate the items and return the original list.
					optionItems.ForEach(i => i.SetActive(true));
					return optionItems;*/
				default:
					throw new System.Exception("Number of elements in OptionItemParams list is not between 2 and 4! Can't proceed as is.");
			}
		}
		#endregion

	}


}