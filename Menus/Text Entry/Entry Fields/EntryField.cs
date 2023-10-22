using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Menus.Input {

	/// <summary>
	/// A class that receives input of some kind from an input box.
	/// </summary>
	public abstract class EntryField : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The index of the current entry letter.
		/// </summary>
		public int CurrentIndex {
			get {
				// The current index is the count of used letters. Make sure to clamp it though, obviously.
				return Mathf.Clamp(
					value: this.EntryLetters.TakeWhile(e => e.Used == true).Count(),
					min: 0,
					max: this.maxLetterCount - 1);
			}
		}
		/// <summary>
		/// Is this entry field "maxed out"?
		/// </summary>
		public bool MaxedOut {
			get {
				return this.EntryLetters.Count(e => e.Used) == this.maxLetterCount;
			}
		}
		/// <summary>
		/// The entry letter that was previously input.
		/// </summary>
		protected EntryFieldLetter PreviousEntryFieldLetter {
			get {
				return this.EntryLetters.LastOrDefault(e => e.Used == true) ?? this.EntryLetters.First();
			}
		}
		/// <summary>
		/// The current entry field letter being input to.
		/// </summary>
		protected EntryFieldLetter CurrentEntryFieldLetter {
			get {
				// Find the first entry letter that isn't being used.
				// If they're ALL in use, just return the last one.
				return this.EntryLetters.FirstOrDefault(e => e.Used == false) ?? this.EntryLetters.Last();
			}
		}
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The minimum amount of letters required to accept this field.
		/// </summary>
		[TabGroup("Entry Field", "Toggles"), Title("Count"), SerializeField]
		protected int minLetterCount = 1;
		/// <summary>
		/// The maximum amount of letters allowed for this field.
		/// </summary>
		[TabGroup("Entry Field", "Toggles"), SerializeField]
		protected int maxLetterCount = 8;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The entry letters that make up this field.
		/// </summary>
		[TabGroup("Entry Field", "Scene References"), Title("Base"), SerializeField]
		private List<EntryFieldLetter> entryLetters = new List<EntryFieldLetter>();
		/// <summary>
		/// The entry letters that make up this field.
		/// </summary>
		protected List<EntryFieldLetter> EntryLetters {
			get {
				return this.entryLetters;
			}
		}
		#endregion

		#region PROPERTIES
		/// <summary>
		/// The aggregation of strings that were entered by the player.
		/// </summary>
		public string EnteredString {
			get {
				// If no letters have been used, return an empty string.
				// I actually can't do this on my own.
				if (this.EntryLetters.Count(el => el.Used) == 0) {
					return "";
				}
				return this.EntryLetters
					.TakeWhile(el => el.Used == true)
					.Select(el => el.CurrentLetter)
					.Aggregate((l1, l2) => l1 + l2);
			}
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Wipes the state of the entry field so it starts out fresh.
		/// </summary>
		/// <param name="highlightFirstLetter">Should the first entry letter be highlighted? Good for if I'm clearing in preparation of player input.</param>
		public void Clear(bool highlightFirstLetter = false) {
			// Go through each letter and just clear it out.
			this.EntryLetters.ForEach(el => el.ClearLetter());
			// If looking to highlight the first letter, well, do that.
			if (highlightFirstLetter == true) {
				this.RefreshEntryLetterHighlights();
			}
		}
		/// <summary>
		/// Takes in a string for the current highlighted entry letter.
		/// </summary>
		/// <param name="input">The string that was just entered from a text box.</param>
		public abstract void ReadInput(string input);
		/// <summary>
		/// Backspace. Should be obvious.
		/// </summary>
		public abstract void Backspace();
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Resets the entry field to its default visuals.
		/// </summary>
		public abstract void ResetEntryField();
		/// <summary>
		/// Presents the entry field.
		/// </summary>
		public abstract void PresentEntryField();
		/// <summary>
		/// Dismisses the entry field.
		/// </summary>
		public abstract void DismissEntryField();
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Refreshes the highlights on the entry letters so that only the current letter is highlighted.
		/// </summary>
		protected void RefreshEntryLetterHighlights() {
			// Dehighlight all other letters.
			this.DehighlightAllLetters();
			// Now just highlight the current letter.
			this.HighlightEntryLetter(index: this.CurrentIndex);
		}
		/// <summary>
		/// Highlights the entry letter at the given index.
		/// </summary>
		/// <param name="index">The index of the entry letter to highlight.</param>
		protected abstract void HighlightEntryLetter(int index);
		/// <summary>
		/// Dehighlights all entry letters.
		/// </summary>
		protected abstract void DehighlightAllLetters();
		#endregion

	}

}