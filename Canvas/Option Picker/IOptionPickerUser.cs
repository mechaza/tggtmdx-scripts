using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly {

	/// <summary>
	/// Anything that needs to use the OptionPicker must implement this.
	/// </summary>
	public interface IOptionPickerUser {

		/// <summary>
		/// Gets called when the option picker sends over the decision that was made.
		/// </summary>
		/// <param name="buttonId">The ID of the button that was picked. This will usually be Yes/No.</param>
		/// <param name="lastSelectedGameObject">The last game object that was selected by the event system. This is good in case I want to actually re-select something or leave it unselected.</param>
		void OptionPicked(int buttonId, GameObject lastSelectedGameObject);

	}


}