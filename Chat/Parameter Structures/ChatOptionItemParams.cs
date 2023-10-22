using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Chat {

	/// <summary>
	/// A way for me to encapsulate the parameters I may need to send to a ChatOptionItem when I build it.
	/// </summary>
	public class ChatOptionItemParams {

		#region FIELDS - REFERENCES
		/// <summary>
		/// The ChatOptionPicker that this item belongs to.
		/// </summary>
		public ChatOptionPicker optionPicker;
		#endregion

		#region FIELDS - BUTTON SPECIFIC
		/// <summary>
		/// The text that should be used on the label for this option.
		/// </summary>
		public string optionLabelText = "";
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should this option item jump to the specified label when picked?
		/// </summary>
		public bool IsJumpOption {
			get {
				// Return true if the label to jump to is not empty.
				return this.labelToJumpTo != "";
			}
		}
		/// <summary>
		/// The label to jump to if this button is picked, if it is a jump label.
		/// </summary>
		public string labelToJumpTo = "";
		/// <summary>
		/// Should this option item set the toggle variable in the runtime chat when picked?
		/// </summary>
		public bool IsToggleOption {
			get {
				// Since toggleToSet is nullable, return true is a value exists.
				return this.toggleToSet != null;
			}
		}
		/// <summary>
		/// The value to set the toggle to in the runtime script if this option is picked.
		/// </summary>
		public bool? toggleToSet;
		#endregion

		#region HELPERS
		/// <summary>
		/// Assigns the option picker these params should reference to be the option picker passed in.
		/// </summary>
		/// <param name="optionPicker">The OptionPicker that will be using these params.</param>
		/// <returns>A reference to the params that were just set.</returns>
		public ChatOptionItemParams SetOptionPicker(ChatOptionPicker optionPicker) {
			this.optionPicker = optionPicker;
			return this;
		}
		#endregion

	}

}
