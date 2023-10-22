using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

namespace Grawly.Chat {

	/// <summary>
	/// The "standard" way in which to read out text.
	/// </summary>
	public class StandardChatTextBox : ChatTextBox {

		#region FIELDS - STATE
		/// <summary>
		/// The current Text Box Params being used for this text box.
		/// Handy for when I need to remember shit.
		/// </summary>
		private ChatTextBoxParams currentParams;
		#endregion

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time to take when tweening the chat box in.
		/// </summary>
		[Title("Tween Timing")]
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private float chatBoxTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the chat box out.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private float chatBoxTweenOutTime = 0.2f;
		/// <summary>
		/// The amount of time to take when transporting the chat box to another speaker position on the screen.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private float chatBoxTransportTweenTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the option picker to the "pick option" state.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private float chatBoxOptionInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the option picker out of the "pick option" state.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private float chatBoxOptionOutTime = 0.2f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The type of easing to use when tweening the chat box in.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private Ease chatBoxEaseInType = Ease.OutCirc;
		/// <summary>
		/// The type of easing to use when tweening the chat box in.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private Ease chatBoxEaseOutType = Ease.OutCirc;
		/// <summary>
		/// The easing to use when transporting the chat box to a speaker position type.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private Ease chatBoxTransportEaseType = Ease.OutCirc;
		/// <summary>
		/// The ease type to use when presenting the option picker and lowering this chat box.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private Ease chatBoxOptionTweenInEaseType = Ease.OutBack;
		/// <summary>
		/// The ease type to use when dismissing the option picker and raising this chat box.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private Ease chatBoxOptionTweenOutEaseType = Ease.OutBack;
		#endregion

		#region FIELDS - TOGGLES : ROTATION
		/// <summary>
		/// The amount of rotation along the z axis the chat box should be in when in the option picker state.
		/// </summary>
		[Title("Rotation")]
		[SerializeField, TabGroup("Text Box", "Toggles")]
		private float chatBoxOptionStateRotation = 0f;
		#endregion

		#region FIELDS - POSITIONS
		/// <summary>
		/// The position of the ChatBox when it is displayed.
		/// </summary>
		[Title("Positions")]
		[SerializeField, TabGroup("Text Box", "Positions")]
		private Vector2 mainPivotActivePosition = new Vector2();
		/// <summary>
		/// The position of the ChatBox when it is hidden.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Positions")]
		private Vector2 mainPivotHidingPosition = new Vector2();
		/// <summary>
		/// The position the chat box should be in when an option is being picked out.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Positions")]
		private Vector2 mainPivotOptionStatePosition = new Vector2();
		/// <summary>
		/// The position the chat box should be when it is being used by the top right speaker.
		/// If I make any more of these specific positions I'll probably just make a dictionary.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Positions")]
		private Vector2 mainPivotTopRightSpeakerPosition = new Vector2();
		/// <summary>
		/// The position to move the chat box to when the chat box is at the top right speaker of the screen.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Positions")]
		private Vector2 mainPivotTopRightOptionStatePosition = new Vector2();
		#endregion

		#region FIELDS - SCENE REFERENCES : MISC COMPONENTS
		/// <summary>
		/// The component that manages how arrows are displayed on the chat box.
		/// </summary>
		[Title("Arrow Component")]
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private StandardChatBoxArrowAssistant arrowAssistant;
		#endregion

		#region FIELDS - SCENE REFERENCES : RECT TRANSFORMS
		/// <summary>
		/// The main recttransform for all the visuals.
		/// </summary>
		[Title("Rect Transforms")]
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private RectTransform allVisualsMainRectTransform;
		/// <summary>
		/// The CHILD rect transform of the All Visuals object.
		/// I make a distinction between this and the ChatBoxMainPivot because
		/// other elements such as the Name Tag and Advance Button are children of this one.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private RectTransform allVisualsChildRectTransform;
		/// <summary>
		/// The RectTransform for the main pivot of the text box.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private RectTransform chatBoxMainPivotRectTransform;
		/// <summary>
		/// The RectTransform for the child pivot of the text box.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private RectTransform chatBoxChildPivotRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image for the front image of the chat box.
		/// </summary>
		[Title("Images")]
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private Image chatBoxBackingFrontImage;
		/// <summary>
		/// The image to fade in/out when working with system text.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private Image chatBoxCheckerImage;
		/// <summary>
		/// The image for the dropshadow image of the chat box.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private Image chatBoxBackingDropshadowImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : TEXT
		/// <summary>
		/// The SuperTextMesh for the chat box.
		/// </summary>
		[Title("Text")]
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private SuperTextMesh chatBoxTextLabel;
		#endregion

		#region FIELDS - SCENE REFERENCES : AUDIO
		/// <summary>
		/// The AudioSource for the Chat Box.
		/// I mostly just need a reference for the GameToggle that adjusts the volume.
		/// </summary>
		[Title("Audio")]
		[SerializeField, TabGroup("Text Box", "Scene References")]
		private AudioSource chatBoxAudioSource;
		/// <summary>
		/// The AudioSource for the Chat Box.
		/// I mostly just need a reference for the GameToggle that adjusts the volume.
		/// </summary>
		public AudioSource ChatBoxAudioSource {
			get {
				return this.chatBoxAudioSource;
			}
		}
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
			// Reset the position.
			this.allVisualsMainRectTransform.anchoredPosition = this.mainPivotHidingPosition;
			// Reset the rotation, in case an option was being picked.
			this.allVisualsMainRectTransform.localEulerAngles = Vector3.zero;
			// Change the color of the box back to white, just in case it was reset at some point.
			this.SetBoxRectangleColor(themeType: ChatBoxThemeType.SolidColor, boxRectangleColor: Color.white);
			// Clear out the text.
			this.chatBoxTextLabel.Text = "";
		}
		/// <summary>
		/// Presents the text box onto the screen.
		/// </summary>
		/// <param name="textBoxParams">Any additional parameters that may help with presenting the text box.</param>
		public override void PresentTextBox(ChatTextBoxParams textBoxParams) {
			this.allVisualsMainRectTransform.DOAnchorPos(
				endValue: this.mainPivotActivePosition, 
				duration: this.chatBoxTweenInTime,
				snapping: true)
				.SetEase(ease: this.chatBoxEaseInType);

			// Tell the arrow assistant to also move the arrows to the correct position.
			this.arrowAssistant.SetArrowPosition(currentSpeakerPosition: textBoxParams.speakerPositionType, snapping: true);

		}
		/// <summary>
		/// Dismisses the text box from the screen.
		/// </summary>
		/// <param name="textBoxParams">Any additional parameters that may help with presenting the text box.</param>
		public override void DismissTextBox(ChatTextBoxParams textBoxParams) {
			this.allVisualsMainRectTransform.DOAnchorPos(
				endValue: this.mainPivotHidingPosition,
				duration: this.chatBoxTweenOutTime,
				snapping: true)
				.SetEase(ease: this.chatBoxEaseOutType);

			// Tell the arrow assistant to retract the arrows.
			this.arrowAssistant.SetArrowPosition(currentSpeakerPosition: ChatSpeakerPositionType.None);

			// Clear out the text.
			this.chatBoxTextLabel.Text = "";

		}
		/// <summary>
		/// Transports the chat box to the desired location depending on who is speaking.
		/// </summary>
		/// <param name="speakerPositionType">The position of the current speaker.</param>
		public override void TransportToPosition(ChatSpeakerPositionType speakerPositionType) {

			// If the speaker is the top right, transport to there.
			if (speakerPositionType == ChatSpeakerPositionType.TopRight) {
				this.allVisualsMainRectTransform.DOAnchorPos(
					endValue: this.mainPivotTopRightSpeakerPosition,
					duration: this.chatBoxTransportTweenTime,
					snapping: true)
					.SetEase(ease: this.chatBoxTransportEaseType);
			} else {
				// If the speaker is literally anyone else, just transport to its regular position.
				this.allVisualsMainRectTransform.DOAnchorPos(
					endValue: this.mainPivotActivePosition,
					duration: this.chatBoxTransportTweenTime,
					snapping: true)
					.SetEase(ease: this.chatBoxTransportEaseType);
			}
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

			// Keep track of the text box params. I may need it for state reasons.
			this.currentParams = textBoxParams;

			// Tell the arrow assistant to also move the arrows to the correct position.
			this.arrowAssistant.SetArrowPosition(currentSpeakerPosition: textBoxParams.speakerPositionType, snapping: false);

			// Also set the colors of the arrows. 0.2f is an arbitrary value. I hate video games.
			this.arrowAssistant.SetBoxColor(color: textBoxParams.boxRectangleColor, time: 0.2f);

			// Set the color of the text box.
			this.SetBoxRectangleColor(textBoxParams: textBoxParams);

			// Prepend the voice/color to the dialogue text.
			text = textBoxParams.speakerVoice + textBoxParams.TextColorTag + text;

			// Read out the dialogue text.
			this.chatBoxTextLabel.Text = text;

		}
		/// <summary>
		/// Skips the text being read out to the very end, if it is reading out anything.
		/// </summary>
		public override void SkipToEnd() {
			// Debug.Log("CHAT: SkipToEnd called.");
			this.chatBoxTextLabel.SkipToEnd();
		}
		/// <summary>
		/// Gets called as an event from a SendMessage when the STM on the chat box is done reading out (or, SkipToEnd() forced it to do so)
		/// </summary>
		private void TextFinishedReadingOut() {

			// Debug.Log("CHAT: TextFinishedReadingOut called.");

			// 11/18/18: Fun change! This is explicitly for when I want dialogue that auto advances.
			if (this.currentParams != null && this.currentParams.autoAdvance == true) {
				Debug.Log("Auto advance enabled for this specific box. If I need to, move this to dialogue directive. I don't like how this is set up.");
				GameController.Instance.WaitThenRun(timeToWait: this.currentParams.timeToWaitBeforeAdvance, action: delegate {
					ChatControllerDX.StandardInstance.EvaluateNextDirective();
				});
				// Don't let the code below run. Theoretically, I should never need to call it.
				return;
			}

			// Only do this if the text isn't a blank string.
			if (this.chatBoxTextLabel.Text != "") {
				// Debug.Log("TEXT HAS FINISHED READING OUT. SETTING ADVANCE BUTTON TO BE VISIBLE/SELECTABLE.");
				this.AdvanceButton.SetVisible(status: true);
				this.AdvanceButton.SetSelectable(true);
			}
		}
		#endregion

		#region SPECIFIC ANIMATIONS : THEMING
		/// <summary>
		/// Sets the color of the text box and the text inside it based on what the params say should be sent.
		/// </summary>
		/// <param name="textBoxParams">The parameters that contain the text box information.</param>
		private void SetBoxRectangleColor(ChatTextBoxParams textBoxParams) {
			// Call the version of this function that takes a color.
			this.SetBoxRectangleColor(
				themeType: textBoxParams.themeType,
				boxRectangleColor: textBoxParams.boxRectangleColor, 
				tweenTime: 0.1f);
		}
		/// <summary>
		/// Sets the color of the rectangle containing the chat text.
		/// </summary>
		/// <param name="themeType">The theme type. This is used to fade the checker in/out.</param>
		/// <param name="boxRectangleColor">The color to set the regular backing image to.</param>
		/// <param name="tweenTime"></param>
		private void SetBoxRectangleColor(ChatBoxThemeType themeType, Color boxRectangleColor, float tweenTime = 0f) {
		
			// Kill any tweens on the images.
			this.chatBoxBackingFrontImage.DOKill(complete: true);
			this.chatBoxCheckerImage.DOKill(complete: true);
			
			// Figure out what color the checker image should be.
			Color checkerRectangleColor = themeType == ChatBoxThemeType.SolidColor ? Color.clear : Color.white;
			
			// Now take care of the theming.
			if (tweenTime == 0f) {
				// If the tween time is zero, make it instantaneous.
				this.chatBoxBackingFrontImage.color = boxRectangleColor;
				this.chatBoxCheckerImage.color = checkerRectangleColor;				
			} else {
				// Otherwise, tween the colors for both the front/checker images.
				this.chatBoxBackingFrontImage.DOColor(endValue: boxRectangleColor, duration: tweenTime);
				this.chatBoxCheckerImage.DOColor(endValue: checkerRectangleColor, duration: tweenTime);
			}
			
		}
		#endregion

		#region SPECIFIC ANIMATIONS : MISC
		/// <summary>
		/// Sets the color/rotation/position of the chat box based on whether or not the option picker is present.
		/// </summary>
		/// <param name="isPickingOption">Is the option picker displayed?</param>
		/// <param name="currentSpeakerPosition">The position of the current speaker. Needed because the way the text rotates depends on it.</param>
		public void SetOptionPickerState(bool isPickingOption, ChatSpeakerPositionType currentSpeakerPosition) {

			if (isPickingOption == true) {

				// Tell the arrow assistant to change the color on the box to gray.
				this.arrowAssistant.SetBoxColor(color: Color.gray, time: 1f);

				// If an option is being picked, move the transform down to the picker position.
				this.allVisualsMainRectTransform.DOAnchorPos(
					endValue: currentSpeakerPosition == ChatSpeakerPositionType.TopRight ? this.mainPivotTopRightOptionStatePosition : this.mainPivotOptionStatePosition, // The end position depends on where the text box is right now.
					duration: this.chatBoxOptionInTime,
					snapping: true)
					.SetEase(ease: this.chatBoxOptionTweenInEaseType);

				// Only rotate if the position is something other than top right.
				if (currentSpeakerPosition != ChatSpeakerPositionType.TopRight) {
					this.allVisualsMainRectTransform.DOLocalRotate(
						endValue: new Vector3(x: 0, y: 0, z: this.chatBoxOptionStateRotation),
						duration: this.chatBoxOptionInTime,
						mode: RotateMode.FastBeyond360)
						.SetEase(ease: this.chatBoxOptionTweenInEaseType);
				}

			} else {

				// Tell the arrow assistant to change the color on the box to white.
				this.arrowAssistant.SetBoxColor(color: Color.white, time: 0.5f);

				// If the picker is being dismissed, move the transform back up.
				this.allVisualsMainRectTransform.DOAnchorPos(
					endValue: currentSpeakerPosition == ChatSpeakerPositionType.TopRight ? this.mainPivotTopRightSpeakerPosition : this.mainPivotActivePosition,  // The end position depends on where the text box is right now.
					duration: this.chatBoxOptionOutTime,
					snapping: true)
					.SetEase(ease: this.chatBoxOptionTweenOutEaseType);

				this.allVisualsMainRectTransform.DOLocalRotate(
					endValue: new Vector3(x: 0, y: 0, z: 0f),
					duration: this.chatBoxOptionOutTime,
					mode: RotateMode.Fast)
					.SetEase(ease: this.chatBoxOptionTweenOutEaseType);


			}
		}
		#endregion

	}


}