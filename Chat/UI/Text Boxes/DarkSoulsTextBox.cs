using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

namespace Grawly.Chat {

	/// <summary>
	/// A gag text box inspired by Dark Souls.
	/// </summary>
	public class DarkSoulsTextBox : ChatTextBox {

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image for the front image of the chat box.
		/// </summary>
		[Title("Images")]
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private Image chatBoxBackingFrontImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : TEXT
		/// <summary>
		/// The SuperTextMesh for the chat box.
		/// </summary>
		[Title("Text")]
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private SuperTextMesh chatBoxTextLabel;
		#endregion

		#region FIELDS - COMPUTED PROPERTIES
		/// <summary>
		/// Is the text box still reading out text?
		/// </summary>
		public override bool IsStillReadingOut {
			get {
				// Return the Reading variable in the text box.
				return this.chatBoxTextLabel.reading;
			}
		}
		#endregion

		#region UNITY CALLS
		private void Start() {
			// this.chatBoxTextLabel.Text = "";
			this.ResetState();
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Totally resets the chat box to its initial values.
		/// </summary>
		public override void ResetState() {
			this.chatBoxBackingFrontImage.CrossFadeAlpha(0f, duration: 0f, ignoreTimeScale: true);
			// Clear out the text.
			this.chatBoxTextLabel.Text = "";
		}
		/// <summary>
		/// Presents the text box onto the screen.
		/// </summary>
		/// <param name="textBoxParams">Any additional parameters that may help with presenting the text box.</param>
		public override void PresentTextBox(ChatTextBoxParams textBoxParams) {
			this.chatBoxBackingFrontImage.CrossFadeAlpha(1f, duration: 0.5f, ignoreTimeScale: true);
			// this.chatBoxBackingFrontImage.DOKill(complete: true);
			// this.chatBoxBackingFrontImage.DOColor(endValue: Color.white, duration: 0.5f);
		}
		/// <summary>
		/// Dismisses the text box from the screen.
		/// </summary>
		/// <param name="textBoxParams">Any additional parameters that may help with presenting the text box.</param>
		public override void DismissTextBox(ChatTextBoxParams textBoxParams) {
			this.chatBoxBackingFrontImage.CrossFadeAlpha(0f, duration: 0.5f, ignoreTimeScale: true);
			// 	this.chatBoxBackingFrontImage.DOKill(complete: true);
			// this.chatBoxBackingFrontImage.DOColor(endValue: Color.clear, duration: 0.5f);
			// Clear out the text.
			this.chatBoxTextLabel.Text = "";

		}
		/// <summary>
		/// Transports the chat box to the desired location depending on who is speaking.
		/// </summary>
		/// <param name="speakerPositionType">The position of the current speaker.</param>
		public override void TransportToPosition(ChatSpeakerPositionType speakerPositionType) {
			// throw new System.NotImplementedException("This should never be called here.");
		}
		#endregion

		#region READING OUT TEXT
		/// <summary>
		/// Reads out text onto the screen box.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="textBoxParams">Any additional parameters that may help with presenting the text box.</param>
		[Button]
		public override void ReadText(string text, ChatTextBoxParams textBoxParams) {
			// Prepend the voice to the dialogue text.
			text = textBoxParams.speakerVoice + text;
			// Read out the dialogue text.
			this.chatBoxTextLabel.Text = text;

		}
		/// <summary>
		/// Skips the text being read out to the very end, if it is reading out anything.
		/// </summary>
		public override void SkipToEnd() {
			this.chatBoxTextLabel.SkipToEnd();
		}
		/// <summary>
		/// Gets called as an event from a SendMessage when the STM on the chat box is done reading out (or, SkipToEnd() forced it to do so)
		/// </summary>
		private void TextFinishedReadingOut() {
			// Only do this if the text isn't a blank string.
			if (this.chatBoxTextLabel.Text != "") {
				// Debug.Log("TEXT HAS FINISHED READING OUT. SETTING ADVANCE BUTTON TO BE VISIBLE/SELECTABLE.");
				this.AdvanceButton.SetVisible(status: true);
				this.AdvanceButton.SetSelectable(true);
			}
		}
		#endregion

	}


}