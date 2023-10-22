using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Menus.Input {

	/// <summary>
	/// A regular space for entering a series of letters.
	/// </summary>
	public class StandardEntryField : EntryField {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject containing all the visauls.
		/// Probably will delete this later. I'm mostly just turning it on/off for prototyping.
		/// </summary>
		[TabGroup("Entry Field", "Scene References"), Title("Standard"), SerializeField]
		private GameObject allVisuals;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// asdflogkisajgas;dfasdlfg
			this.ResetEntryField();
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Takes in a string for the current highlighted entry letter.
		/// </summary>
		/// <param name="input">The string that was just entered from a text box.</param>
		public override void ReadInput(string input) {
			// Record the letter.
			this.CurrentEntryFieldLetter.RecordLetter(letter: input);
			// Refresh the entry letters.
			this.RefreshEntryLetterHighlights();
		}
		/// <summary>
		/// Backspace. Should be obvious.
		/// </summary>
		public override void Backspace() {
			// Clear the previous letter. This basically operates as a backspace.
			this.PreviousEntryFieldLetter.ClearLetter();
			// Refresh the entry letters.
			this.RefreshEntryLetterHighlights();
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Resets the entry field to its default visuals.
		/// </summary>
		public override void ResetEntryField() {
			Debug.LogWarning("NEED TO ACTUALLY ADD THIS PROPERLY. JUST CALLING DISMISS FOR NOW.");
			this.DismissEntryField();
		}
		/// <summary>
		/// Presents the entry field.
		/// </summary>
		public override void PresentEntryField() {
			this.allVisuals.SetActive(true);
		}
		/// <summary>
		/// Dismisses the entry field.
		/// </summary>
		public override void DismissEntryField() {
			this.allVisuals.SetActive(false);
		}
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Highlights the current entry letter accepting input.
		/// </summary>
		protected override void HighlightEntryLetter(int index) {
			// Highlight the entry letter at... the given index.
			this.EntryLetters[index].Highlight();
		}
		/// <summary>
		/// Dehighlights all entry letters.
		/// </summary>
		protected override void DehighlightAllLetters() {
			// Go through each entry letter and dehighlight it.
			this.EntryLetters.ForEach(e => e.Dehighlight());
		}
		#endregion

	}


}