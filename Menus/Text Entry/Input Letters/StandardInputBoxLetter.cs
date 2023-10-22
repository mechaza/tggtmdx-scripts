﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Grawly.Menus.Input {

	/// <summary>
	/// Nothing fancy. Just a regular letter to be part of an input box.
	/// </summary>
	[RequireComponent(typeof(Selectable))]
	public class StandardInputBoxLetter : InputBoxLetter, ISelectHandler, IDeselectHandler, ICancelHandler, ISubmitHandler {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The selectable attached to this Letter.
		/// </summary>
		private Selectable selectable;
		/// <summary>
		/// The STM that this input letter is referring to.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private SuperTextMesh letterLabel;
		/// <summary>
		/// The image for the highlight square.
		/// </summary>
		[SerializeField]
		private GameObject highlightSquareImage;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			this.selectable = this.transform.GetComponent<Selectable>();
		}
		#endregion

		#region SELECTABLES
		/// <summary>
		/// Allows this letter to be highlighted.
		/// Especially useful if I need to turn on a Selectable component, for example.
		/// </summary>
		public override void EnableLetter() {
			this.selectable.enabled = true;
		}
		/// <summary>
		/// Allows this letter to be highlighted.
		/// Especially useful if I need to turn OFF a Selectable component, for example.
		/// </summary>
		public override void DisableLetter() {
			this.selectable.enabled = false;
			if (EventSystem.current.currentSelectedGameObject == this.gameObject) {
				Debug.LogWarning("THE EVENT SYSTEM IS CURRENTLY SELECTING THIS LETTER WHILE IM TRYING TO DISABLE IT." +
					"Setting currentSelectedGameObject to null.");
				EventSystem.current.SetSelectedGameObject(null);
			}
		}
		#endregion

		#region STATE
		/// <summary>
		/// Toggles the casing of the letter.
		/// I'm too lazy to really write anything better than what I'm about to shit out so get ready.
		/// </summary>
		public void ToggleCase() {
			// Create a placeholder.
			string upper = "";
			try {

				// Try toggling the letter to uppercase.
				upper = this.Letter.ToUpper();

				// If the uppercase version of the letter is this letter,
				// that means... this letter was uppercase to begin with.
				if (upper == this.Letter) {
					// Make it lower if it's upper.
					this.Letter = this.Letter.ToLower();
				} else {
					// If this letter is NOT uppercase... well, make it.
					this.Letter = this.Letter.ToUpper();
				}

				// Set the letter label text.
				this.letterLabel.Text = this.Letter;

				// If this is letter is still being selected, I guess uh. Re-highlight it.
				if (EventSystem.current.currentSelectedGameObject == this.gameObject) {
					this.Highlight();
					// Debug.LogError("Note that there's going to be an issue when toggling happens and this is the letter that's currently selected.");
				}

			} catch (System.Exception e) {
				Debug.LogWarning("Could not toggle letter!");
			}
		}
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Shows this letter being highlighted.
		/// </summary>
		protected override void Highlight() {
			this.highlightSquareImage.gameObject.SetActive(true);
			this.letterLabel.Text = "<c=purpley>" + this.Letter;
		}
		/// <summary>
		/// Dehighlights this letter.
		/// </summary>
		protected override void Dehighlight() {
			this.highlightSquareImage.gameObject.SetActive(false);
			this.letterLabel.Text = this.Letter;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - EVENTS
		/// <summary>
		/// Highlights the input letter.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnSelect(BaseEventData eventData) {
			this.Highlight();
		}
		/// <summary>
		/// Dehighlights the input letter.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(type: SFXType.Hover);
			this.Dehighlight();
		}
		/// <summary>
		/// Adds this letter to the entry field.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnSubmit(BaseEventData eventData) {

			if (this.gameObject.activeInHierarchy == false) {
				Debug.LogError("This object received an event while its GameObject was inactive! Ignoring.");
				return;
			}

			AudioController.instance?.PlaySFX(type: SFXType.Select);
			
			// Need to do something extra before submitting.
			// Create a flag.
			bool toggleCaseFlag = false;
			// If this is the first letter and it's uppercase, set the flag.
			if (NameEntryController.instance.EntryField.CurrentIndex == 0 && this.Letter.ToUpper() == this.Letter) {
				toggleCaseFlag = true;
			}

			// Pass the entry field the letter that was just hit.
			NameEntryController.instance.EntryField.ReadInput(input: this.Letter);
			
			// If maxed out, call LengthLimitReached (this should skip to OK)
			if (NameEntryController.instance.EntryField.MaxedOut == true) {
				Debug.Log("MAXED OUT");
				NameEntryController.instance.InputBox.LengthLimitReached();
				return;
			}

			if (toggleCaseFlag == true) {
				// After that, if the flag was set, toggle the case.
				NameEntryController.instance.InputBox.ToggleCase();
			}

		}
		/// <summary>
		/// Backspace.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnCancel(BaseEventData eventData) {

			if (this.gameObject.activeInHierarchy == false) {
				Debug.LogError("This object received an event while its GameObject was inactive! Ignoring.");
				return;
			}

			Debug.Log("ATTEMPTING TO BACKSPACE");
			AudioController.instance?.PlaySFX(type: SFXType.Close);
			// Whomp whomp
			NameEntryController.instance.EntryField.Backspace();
		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// Updates the stored letter with whatever is in the STM label.
		/// </summary>
		[ShowInInspector]
		private void UpdateStoredLetter() {
			this.Letter = this.letterLabel.Text;
		}
		#endregion

	}

}