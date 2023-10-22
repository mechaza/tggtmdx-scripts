using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Toggles;
using Grawly.Toggles.Audio;

namespace Grawly.Chat {

	/// <summary>
	/// The standard way I'll be reading out the chat junk.
	/// May make assumptions about the specific subclass of components I'll be using
	/// (e.x., StandardChatBox, StandardBustUps, etc.)
	/// </summary>
	public class StandardChatController : ChatControllerDX {

		#region FIELDS - DEBUGGING
		/// <summary>
		/// Should this script boot into debug mode upon launch?
		/// </summary>
		[SerializeField]
		private bool debugMode = false;
		/// <summary>
		/// The script that should be used in debug mode.
		/// </summary>
		[SerializeField, ShowIf("debugMode")]
		private SerializedChatScriptDX debugChatScript;
		/// <summary>
		/// The script that should be used in debug mode.
		/// Text version.
		/// </summary>
		[SerializeField, ShowIf("debugMode")]
		private TextAsset textScript;
		[Button]
		private void OpenDebugScript() {
			this.Open(chatScript: this.debugChatScript, chatClosedCallback: delegate { });
		}
		[Button]
		private void OpenTextScript() {
			this.Open(textAsset: this.textScript, chatParams: new ChatControllerParams() { });
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				StandardInstance = this;
				ResetController.AddToDontDestroy(this.gameObject);
				// DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			// On start, change the volume of the audio source.
			// I don't do this in OnGameControllerInitialize because there is a race condition to make the Chat Controller scene load first.
			(this.TextBox as StandardChatTextBox).ChatBoxAudioSource.volume = ToggleController.GetToggle<DialogueVolume>().GetToggleFloat();
		}
		#endregion

		#region SCRIPT READING
		/// <summary>
		/// Opens the ChatController with a script to be formed from a textasset.
		/// </summary>
		/// <param name="textAsset">The TextAsset to use in generating the script.</param>
		/// <param name="chatParams">Any additional parameters that may be required to run the script.</param>
		public override void Open(TextAsset textAsset, ChatControllerParams chatParams) {
			this.Open(
				chatScript: new PlainChatScript(rawText: textAsset.text),
				chatParams: chatParams);
		}
		/// <summary>
		/// Opens the ChatController with a chat script all ready to go.
		/// </summary>
		/// <param name="chatScript">The script to use.</param>
		/// <param name="chatParams">Any additional parameters that may be required to run the script.</param>
		public override void Open(IChatScript chatScript, ChatControllerParams chatParams) {
			
			//Debug.Log(chatScript.ToString());
			//Debug.Break();
			
			// Overwrite the Instance.
			Instance = this;

			// Save the params
			this.chatParams = chatParams;

			// Run the opened callback. This is more for backwards compatability.
			chatParams.chatOpenedCallback?.Invoke();

			// Create a new runtime script.
			this.currentRuntimeScript = new RuntimeChatScript(chatScript: chatScript);
			// Evaluate the first directive.
			this.currentRuntimeScript.GetNextDirective().EvaluateDirective(chatController: this);
		}
		/// <summary>
		/// Closes out the chat.
		/// </summary>
		/// <param name="chatParams">The parameters that were passed in when the chat was made.</param>
		/// <param name="runCloseCallback">Should the chat closed callback be run? Relevant if I need to hard reset a scene and DON'T want it to run.</param>
		protected override void Close(ChatControllerParams chatParams, bool runCloseCallback = true) {
			// Dismiss the text box.
			this.TextBox.DismissTextBox(textBoxParams: new ChatTextBoxParams() { });
			// Dismiss the bust ups.
			this.ChatBustUps.ForEach(b => {
				b.DismissSpeaker();
			});

			// Wait half a second and then run the callback.
			GameController.Instance?.WaitThenRun(timeToWait: 0.5f, action: delegate {

				// Only run the close callback if specified to do so. It will usually be fine,
				// but sometimes I need to hard reset due to a fatal error and NOT want to run the callback.
				if (runCloseCallback == true) {
					// Call the chat closed callback.
					chatParams.chatClosedCallback?.Invoke(
						str: this.currentRuntimeScript.str,
						num: this.currentRuntimeScript.num,
						toggle: this.currentRuntimeScript.toggle);
				}
				

				// Null out the chat params/current runtime script, just in case.
				this.currentRuntimeScript = null;
				this.chatParams = null;
			});

		}
		#endregion

		#region EVENTS
		/// <summary>
		/// Gets called when the AdvanceButton receives a Submit event of some kind.
		/// </summary>
		public override void AdvanceButtonHit() {
			// Alert the current directive that the advance button was hit.
			this.currentRuntimeScript.CurrentDirective.AdvanceButtonHit(chatController: this);
		}
		#endregion


	}


}