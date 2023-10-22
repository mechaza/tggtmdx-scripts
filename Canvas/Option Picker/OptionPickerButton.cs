using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Grawly {

	/// <summary>
	/// This is the button that is used for when an option needs to be picked out.
	/// </summary>
	public class OptionPickerButton : MonoBehaviour, ISubmitHandler, ISelectHandler, IDeselectHandler {

		#region FIELDS - STATE
		/// <summary>
		/// This is the number that this button passes back to the option picker when selected.
		/// </summary>
		[SerializeField, TabGroup("Options", "State")]
		private int buttonId = 0;
		/// <summary>
		/// This is what gets shown on the button when it's displayed. Usually just defaults to Yes or No.
		/// </summary>
		private string buttonString = "";
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// This is the image that stays behind the text label to indicate that it is the currently highlighted option.
		/// </summary>
		[SerializeField, TabGroup("Options", "Scene References")]
		private Image selectionBackgroundImage;
		/// <summary>
		/// This is the text mesh that shows the button.
		/// </summary>
		[SerializeField, TabGroup("Options", "Scene References")]
		private SuperTextMesh buttonLabel;
		#endregion


		#region PREPARATION
		/// <summary>
		/// Preps this option button to be used.
		/// </summary>
		/// <param name="buttonString">The string that should be on display for this button.</param>
		/// <param name="buttonNumber">The ID that gets passed back to the Option Picker when this button is hit. </param>
		public void Prepare(string buttonString, int buttonId) {
			this.buttonString = buttonString;
			this.buttonId = buttonId;

			// This is kinda... shoehorning in the logic but it looks nicer when one of the buttons "looks" activated by default.
			//
			//	ahhhhhhh it works but... whatever
			//
			if (this.buttonId == 0) {
				this.Dehighlight();
			} else {
				this.Highlight();
				
			}
		}
		#endregion

		#region GRPAHICS
		private void Highlight() {
			this.selectionBackgroundImage.gameObject.SetActive(true);
			this.buttonLabel.Text = "<w=default><c=white>" + this.buttonString;
		}
		private void Dehighlight() {
			this.selectionBackgroundImage.gameObject.SetActive(false);
			this.buttonLabel.Text = "<c=black>" + this.buttonString;
		}
		#endregion

		#region EVENT HANDLERS
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(type: SFXType.Hover, scale: 1f);
			this.Dehighlight();
			// this.selectionBackgroundImage.gameObject.SetActive(false);
			// this.buttonLabel.Text = "<c=black>" + this.buttonString;
		}
		public void OnSelect(BaseEventData eventData) {
			this.Highlight();
			// this.selectionBackgroundImage.gameObject.SetActive(true);
			// this.buttonLabel.Text = "<w=default><c=white>" + this.buttonString;
		}
		public void OnSubmit(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(type: SFXType.Select, scale: 1f);
			// Okay, it's safe to assume that I won't need this button again so:
			// Tell the event system to nullify the current selected object.
			EventSystem.current.SetSelectedGameObject(null);
			// Make sure to keep it highlighted though!
			this.Highlight();
			// Tell the option picker this was the button that was hit.
			OptionPicker.instance.ButtonHit(buttonId: this.buttonId);
		}
		#endregion

	}


}