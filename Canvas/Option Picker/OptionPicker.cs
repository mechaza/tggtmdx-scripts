using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Grawly {

	/// <summary>
	/// A lil prompt that comes up where I can ask basic yes/no questions.
	/// </summary>
	public class OptionPicker : MonoBehaviour, IOptionPickerUser {

		public static OptionPicker instance;

		#region FIELDS - STATE
		/// <summary>
		/// A reference to whatever object just called this option picker.
		/// </summary>
		private IOptionPickerUser sourceOfCall;
		/// <summary>
		/// The game object that was currently selected before the prompt was called.
		/// May want to remember it just in case because the caller may not want to reselect it.
		/// </summary>
		private GameObject lastSelectedGameObject;
		/// <summary>
		/// Is the option picker currently open?
		/// </summary>
		public bool CurrentlyOpened { get; private set; } = false;
		#endregion

		#region FIELDS - STATE : SCARY
		private System.Action callback1;
		private System.Action callback2;
		private bool reselectOnDone = true;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The ease type for scaling this prompt in.
		/// </summary>
		[SerializeField, TabGroup("Options", "Toggles")]
		private Ease introEaseType = Ease.InOutQuart;
		/// <summary>
		/// The ease type for scaling this prompt in/out.
		/// </summary>
		[SerializeField, TabGroup("Options", "Toggles")]
		private Ease outroEaseType = Ease.InOutQuart;
		/// <summary>
		/// The amount of time it takes to scale the option prompt in/out.
		/// </summary>
		[SerializeField, TabGroup("Options", "Toggles")]
		private float scaleTime = 0.5f;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains everything else referenced. Handy for quick activating/deactivating.
		/// </summary>
		[SerializeField, TabGroup("Options", "Scene References")]
		private GameObject optionPickerObject;
		/// <summary>
		/// The background that shows up against the text to make a nice effect.
		/// </summary>
		[SerializeField, TabGroup("Options", "Scene References")]
		private Image pickerBackground;
		/// <summary>
		/// A lil highlight for the picker background.
		/// </summary>
		[SerializeField, TabGroup("Options", "Scene References")]
		private Image pickerBackgroundHighlight;
		/// <summary>
		/// The text mesh that prompts the user for what they want.
		/// </summary>
		[SerializeField, TabGroup("Options", "Scene References")]
		private SuperTextMesh promptLabel;
		/// <summary>
		/// The buttons that get displayed as choices when the picker shows up.
		/// </summary>
		[SerializeField, TabGroup("Options", "Scene References")]
		private List<OptionPickerButton> optionPickerButtons = new List<OptionPickerButton>();
		#endregion

		#region FIELDS - DEBUGGING
		/// <summary>
		/// The prompt that gets displayed when I run the debug animation.
		/// </summary>
		[SerializeField, TabGroup("Debug", "Debug")]
		private string debugPrompt = "";
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				ResetController.AddToDontDestroy(this.gameObject);
				// DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}
		#endregion

		#region DISPLAYING
		/// <summary>
		/// Displays a very basic option picker with default yes/no prompts.
		/// Reselects the last game object if No was picked.
		/// </summary>
		/// <param name="prompt">The prompt to display.</param>
		/// <param name="onYes">The callback to run if Yes was picked.</param>
		public void Display(string prompt, System.Action onYes) {

			// Grab the currently selected game object.
			GameObject currentSelection = EventSystem.current.currentSelectedGameObject;
			
			// Display a yes or no prompt that just reslesect the old object on No.
			this.Display(
				prompt: prompt, 
				option1: "Yes", 
				option2: "No", 
				callback1: onYes,
				callback2: delegate {
					EventSystem.current.SetSelectedGameObject(currentSelection);
				}, 
				reselectOnDone: false);
		}
		/// <summary>
		/// Displays the option picker with the specified list of options.
		/// </summary>
		/// <param name="options">The labels to use for the different choices.</param>
		/// <param name="sourceOfCall">The source of the call. Needs to know when a choice was made.</param>
		public void Display(string prompt, List<string> options, IOptionPickerUser sourceOfCall) {
			Debug.Log("OPENING OPTION PICKER. IF THERES AN ISSUE WITH THE EVENT SYSTEM, INVESTIGATE HERE MAYBE?");

			// Set the picker to be open.
			this.CurrentlyOpened = true;
			
			// Remember who was the source of the call.
			this.sourceOfCall = sourceOfCall;
			// Remember what the last selected game object was. It needs to be given to the source when done.
			this.lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			// Nullify the current selected object because I can't be having the player hit submit when the picker is opening up.
			EventSystem.current.SetSelectedGameObject(null);

			// Play the sound for the prompt.
			AudioController.instance?.PlaySFX(type: SFXType.PromptOpen, scale: 1f);

			// Prep the buttons for use. For right now I'm just making a basic two-button pick.
			this.optionPickerButtons[0].Prepare(buttonString: options[0], buttonId: 0);
			this.optionPickerButtons[1].Prepare(buttonString: options[1], buttonId: 1);
			// Set the prompt text.
			this.promptLabel.Text = prompt;
			// Play the open animation. This also selects the button when its done.
			this.OpenAnimation();
		}
		/// <summary>
		/// Displays the option picker with a simple yes/no option.
		/// </summary>
		/// <param name="sourceOfCall">The source of the call. Needs to know when a choice was made.</param>
		public void Display(string prompt, IOptionPickerUser sourceOfCall) {
			// Call the version of this method that actually does take the Yes/No params.
			this.Display(prompt: prompt, options: new List<string>() { "Yes", "No" }, sourceOfCall: sourceOfCall);
		}
		/// <summary>
		/// hmm i think i will go apeshit
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="options"></param>
		/// <param name="callback1"></param>
		/// <param name="callback2"></param>
		public void Display(string prompt, string option1, string option2, System.Action callback1, System.Action callback2, bool reselectOnDone = true) {
			this.Display(
				prompt: prompt,
				options: new List<string>(){option1, option2}, 
				callback1: callback1,
				callback2: callback2,
				reselectOnDone: reselectOnDone);
		}
		/// <summary>
		/// hmm i think i will go apeshit
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="options"></param>
		/// <param name="callback1"></param>
		/// <param name="callback2"></param>
		public void Display(string prompt, List<string> options, System.Action callback1, System.Action callback2, bool reselectOnDone = true) {
			this.callback1 = callback1;
			this.callback2 = callback2;
			this.reselectOnDone = reselectOnDone;
			this.Display(prompt, options, this);
		}
		#endregion

		#region CONFIRMING
		/// <summary>
		/// Called from the option picker button and instructs the option picker to start doing its thing.
		/// </summary>
		/// <param name="buttonId">The ID of the button that was picked.</param>
		public void ButtonHit(int buttonId) {
			Debug.Log("Button Hit: " + buttonId);
			// Close out the prompt. This also tells the caller what to do next.
			this.CloseAnimation(buttonId: buttonId, sourceOfCall: this.sourceOfCall, lastSelectedGameObject: this.lastSelectedGameObject);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Plays the animation for when the picker is opening. THIS ALSO SELECTS THE OPTION BUTTON.
		/// </summary>
		private void OpenAnimation() {
			// Turn the visuals on.
			this.optionPickerObject.SetActive(true);
			// Set the scale of the picker object to zero. It needs that.
			this.optionPickerObject.transform.localScale = Vector3.zero;

			// Start up a sequence.
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(new TweenCallback(delegate {
				// Scale the option picker in.
				this.optionPickerObject.transform.DOScale(endValue: 1f, duration: this.scaleTime).SetEase(ease: this.introEaseType);
			}));
			// Wait a moment. Adding a slight delay just in case.
			seq.AppendInterval(this.scaleTime + 0.02f);
			// Select the first button. Second button. whatejver.
			seq.AppendCallback(new TweenCallback(delegate {
				EventSystem.current.SetSelectedGameObject(this.optionPickerButtons[1].gameObject);
			}));
			seq.Play();
		}
		/// <summary>
		/// Plays the animation for when the picker is closing. THIS ALSO TELLS THE CALLER THE CHOICE.
		/// </summary>
		/// <param name="buttonId">The ID of the button that was picked.</param>
		private void CloseAnimation(int buttonId, IOptionPickerUser sourceOfCall, GameObject lastSelectedGameObject) {

			// Set the scale of the picker object to one. It needs that.
			this.optionPickerObject.transform.localScale = Vector3.one;

			// Start up a sequence.
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(new TweenCallback(delegate {
				// Scale the option picker out.
				this.optionPickerObject.transform.DOScale(endValue: 0f, duration: this.scaleTime).SetEase(ease: this.outroEaseType);
			}));
			// Wait a moment. Adding a slight delay just in case.
			seq.AppendInterval(this.scaleTime + 0.02f);
			// Tell the caller what the choice was.
			seq.AppendCallback(new TweenCallback(delegate {
				// Also turn the object picker visuals off.
				this.optionPickerObject.SetActive(false);
				// Set the picker to be closed.
				this.CurrentlyOpened = false;
				if (this.callback1 == null && this.callback2 == null) {
					sourceOfCall.OptionPicked(buttonId: buttonId, lastSelectedGameObject: lastSelectedGameObject);
				} else {
					if (this.reselectOnDone == true) { EventSystem.current.SetSelectedGameObject(lastSelectedGameObject); }
					if (buttonId == 0) { this.callback1.Invoke(); }
					if (buttonId == 1) { this.callback2.Invoke(); }
					this.callback1 = null;
					this.callback2 = null;
				}
			}));
			seq.Play();
		}
		#endregion


		#region DEBUGGING
		/// <summary>
		/// Opens up the picker for debug purposes.
		/// </summary>
		[HideInEditorMode, TabGroup("Debug", "Debug")]
		public void DebugPicker() {
			this.Display(prompt: this.debugPrompt, sourceOfCall: this);
		}
		[HideInEditorMode, TabGroup("Debug", "Debug")]
		public void DebugPickerDX() {
			this.Display(prompt: "answer the question", options: new List<string>() { "sorry", "ten"}, sourceOfCall: this);
		}
		/// <summary>
		/// This is whats called when something is picked.
		/// </summary>
		/// <param name="buttonId"></param>
		/// <param name="lastSelectedGameObject"></param>
		public void OptionPicked(int buttonId, GameObject lastSelectedGameObject) {
			Sequence seq = DOTween.Sequence();
			seq.AppendInterval(2f);
			seq.AppendCallback(new TweenCallback(delegate {
				this.DebugPickerDX();
			}));
		}
		#endregion

	}


}