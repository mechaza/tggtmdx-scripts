using System.Collections;
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
	public class ConfirmInputBoxLetter : InputBoxLetter, ISelectHandler, IDeselectHandler, ICancelHandler, ISubmitHandler {

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

			// If nothing has been input, don't allow entry.
			if (NameEntryController.instance.EntryField.EnteredString == "") {
				AudioController.instance?.PlaySFX(type: SFXType.Invalid);
			} else {
				AudioController.instance?.PlaySFX(type: SFXType.Select);
				NameEntryController.instance.ConfirmName(inputText: NameEntryController.instance.EntryField.EnteredString);
			}
			
			
			// Pass the entry field the letter that was just hit.
			// NameEntryController.Instance.EntryField.ReadInput(input: this.Letter);
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
			// Whomp whomp
			NameEntryController.instance.EntryField.Backspace();
		}
		#endregion

	}

}