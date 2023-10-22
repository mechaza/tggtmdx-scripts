using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// A gag Chat Controller stylized like Dark Souls.
	/// </summary>
	public class DarkSoulsChatController : ChatControllerDX {

		#region FIELDS - DEBUGGING
		[SerializeField]
		private SerializedChatScriptDX debugChatScript;
		[SerializeField]
		private TextAsset textScript;
		[Button]
		private void OpenDebugScript() {
			this.Open(chatScript: this.debugChatScript, chatParams: new ChatControllerParams() { });
			// this.Open(chatScript: this.debugChatScript, chatClosedCallback: delegate { });
		}
		[Button]
		private void OpenTextScript() {
			this.Open(textAsset: this.textScript, chatParams: new ChatControllerParams() { });
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (ChatControllerDX.DarkSoulsInstance == null) {
				ChatControllerDX.DarkSoulsInstance = this;
			}
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

			// Overwrite the Instance.
			Instance = this;

			// Save the params
			this.chatParams = chatParams;

			// Run the opened callback. This is more for backwards compatability.
			chatParams.chatOpenedCallback?.Invoke();

			// Show the textbox.
			this.TextBox.PresentTextBox(new ChatTextBoxParams() { });

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

			/*// Dismiss the bust ups.
			this.ChatBustUps.ForEach(b => {
				b.DismissSpeaker();
			});*/

			// Wait half a second and then run the callback.
			GameController.Instance.WaitThenRun(timeToWait: 0.5f, action: delegate {
				// Call the chat closed callback.
				chatParams.chatClosedCallback?.Invoke(
					str: this.currentRuntimeScript.str,
					num: this.currentRuntimeScript.num,
					toggle: this.currentRuntimeScript.toggle);

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