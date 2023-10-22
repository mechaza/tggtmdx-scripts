using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Menus.Input {

	/// <summary>
	/// Is used as part of an InputEntryField so that I can keep track of what letters are currently recorded.
	/// </summary>
	public abstract class EntryFieldLetter : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// Has this entry letter been used?
		/// </summary>
		public bool Used {
			get {
				// Return true if the current letter is NOT the empty string.
				return this.CurrentLetter != "" ? true : false;
			}
		}
		/// <summary>
		/// The string this EntryLetter is currently keeping track of.
		/// </summary>
		public string CurrentLetter { get; protected set; } = "";
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// This is what gets called when an EntryLetter accepts a string.
		/// </summary>
		/// <param name="letter">The string containing the letter to remember.</param>
		public void RecordLetter(string letter) {
			this.CurrentLetter = letter;
		}
		/// <summary>
		/// Wipes the state of this entry letter.
		/// </summary>
		public void ClearLetter() {
			this.CurrentLetter = "";
		}
		#endregion

		#region VISUALS
		/// <summary>
		/// Highlights the entry letter to show the player this is what is currently being used.
		/// </summary>
		public abstract void Highlight();
		/// <summary>
		/// Dehighlights the entry letter to show this is not the letter currently accepting input.
		/// </summary>
		public abstract void Dehighlight();
		#endregion

	}


}