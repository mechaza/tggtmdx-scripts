using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

namespace Grawly.Chat {

	/// <summary>
	/// A class that represents a way to pick out different options during a chat.
	/// </summary>
	public abstract class ChatOptionPicker : SerializedMonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The parameters that were passed into this option picker when it was built.
		/// </summary>
		protected ChatOptionPickerParams optionPickerParams;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the visuals for the option picker.
		/// </summary>
		[SerializeField, TabGroup("Option Picker", "Scene References"), ShowInInspector, Title("All Visuals")]
		protected GameObject allVisuals;
		/// <summary>
		/// The option items that should be used for the picker.
		/// </summary>
		[SerializeField, TabGroup("Option Picker", "Scene References"), ShowInInspector, Title("Option Items")]
		protected List<ChatOptionItem> optionItems = new List<ChatOptionItem>();
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Presents the option picker to display a set of options.
		/// </summary>
		/// <param name="optionPickerParams">The parameters that encapsulate the different kinds of options I may want to present.</param>
		public abstract void PresentOptions(ChatOptionPickerParams optionPickerParams);
		/// <summary>
		/// Highlights the default option after the options have been presented.
		/// </summary>
		/// <param name="optionPickerParams">The parameters that encapsulate the different kinds of options I may want to present.</param>
		protected abstract void HighlightDefaultOption(ChatOptionPickerParams optionPickerParams, ChatOptionItem defaultOptionItem);
		/// <summary>
		/// Gets called when an option has been picked.
		/// Should normally return control over to the ChatController.
		/// This version of the method is used by the ChatOptionItem but just calls the protected version.
		/// </summary>
		/// <param name="optionItemParams">The parameters that built the option item that was selected.</param>
		/// <param name="optionItem">The actual ChatOptionItem that was picked out.</param>
		public void PickedOption(ChatOptionItemParams optionItemParams, ChatOptionItem optionItem) {
			this.PickedOption(
				optionPickerParams: this.optionPickerParams,
				optionItemParams: optionItemParams,
				optionItem: optionItem);
		}
		/// <summary>
		/// Gets called when an option has been picked.
		/// Should normally return control over to the ChatController.
		/// </summary>
		/// <param name="optionPickerParams">The parameters that define how this picker should have been set up.</param>
		/// <param name="optionItemParams">The parameters that built the option item that was selected.</param>
		/// <param name="optionItem">The actual ChatOptionItem that was picked out.</param>
		protected abstract void PickedOption(ChatOptionPickerParams optionPickerParams, ChatOptionItemParams optionItemParams, ChatOptionItem optionItem);
		/// <summary>
		/// Dismisses the options after one has been selected.
		/// </summary>
		protected abstract void DismissOptions();
		#endregion

	}


}