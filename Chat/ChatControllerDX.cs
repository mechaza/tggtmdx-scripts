using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using System.Linq;

namespace Grawly.Chat {

	/// <summary>
	/// The NEW chat controller to use for all my bullshit. Abstract just in case. Might help me to think about it better that way.
	/// </summary>
	public abstract class ChatControllerDX : SerializedMonoBehaviour {

		#region INSTANCES
		/// <summary>
		/// The current ChatControllerDX in use.
		/// Note that this may become a problem if I reference it too heavily and I use more than one chat at a time.
		/// But who cares.
		/// </summary>
		public static ChatControllerDX Instance { get; protected set; }
		/// <summary>
		/// The Instance of the STANDARD chat controller.
		/// </summary>
		public static ChatControllerDX StandardInstance { get; protected set; }
		/// <summary>
		/// The Instance of the Dark Souls chat controller.
		/// </summary>
		public static ChatControllerDX DarkSoulsInstance { get; protected set; }
		#endregion

		#region FIELDS - STATE
		/// <summary>
		/// The params that were used to call on this chat.
		/// </summary>
		protected ChatControllerParams chatParams;
		/// <summary>
		/// The runtime version of the script being read out.
		/// </summary>
		protected RuntimeChatScript currentRuntimeScript;
		#endregion

		#region FIELDS - SCENE REFERENCES : PRIMARY COMPONENTS
		/// <summary>
		/// A reference to the text box that shows. The text.
		/// </summary>
		[Title("Primary Components")]
		[SerializeField, TabGroup("Controller", "Scene References"), ShowInInspector]
		public ChatTextBox TextBox { get; protected set; }
		/// <summary>
		/// The chat bust ups that are part of the scene.
		/// </summary>
		[Title("Bust Ups")]
		[SerializeField, TabGroup("Controller", "Scene References"), ShowInInspector]
		public List<ChatBustUp> ChatBustUps { get; protected set; } = new List<ChatBustUp>();
		/// <summary>
		/// A list of different potential option pickers to use for picking options.
		/// This is a list because I may have multiple kinds of pickers to choose from, with different styles.
		/// </summary>
		[Title("Option Pickers")]
		[SerializeField, TabGroup("Controller", "Scene References"), ShowInInspector]
		public List<ChatOptionPicker> OptionPickers { get; protected set; } = new List<ChatOptionPicker>();
		/// <summary>
		/// Big thumbnails that can be used to display pictures for the player. Handy in dialogue.
		/// </summary>
		[Title("Chat Pictures")]
		[SerializeField, TabGroup("Controller", "Scene References"), ShowInInspector]
		public List<ChatPicture> ChatPictures { get; protected set; } = new List<ChatPicture>();
		/// <summary>
		/// The borders that surround this chat controller.
		/// </summary>
		[Title("Chat Borders")]
		[SerializeField, TabGroup("Controller", "Scene References"), ShowInInspector]
		public ChatBorders ChatBorders { get; protected set; }
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets the bust up at the specified position type.
		/// </summary>
		/// <param name="positionType">The position of the desired bust up.</param>
		/// <returns>The bust up associated with the given position.</returns>
		public ChatBustUp GetBustUp(ChatSpeakerPositionType positionType) {
			// Will return null if the bust up is not defined.
			return this.ChatBustUps.FirstOrDefault(b => b.SpeakerPositionType == positionType);
		}
		/// <summary>
		/// Gets the bust up for the specific speaker shorthand.
		/// May be null if the shorthand does not exist.
		/// </summary>
		/// <param name="speakerShorthand">The Shorthand that the RuntimeChatSpeaker uses.</param>
		/// <returns>The bust up that has the RuntimeChatSpeaker which identifies with the same shorthand.</returns>
		public ChatBustUp GetBustUp(string speakerShorthand) {
			// Go through each bust up, and look at the current speaker template. Find the RuntimeChatSpeaker that contains the same template.
			return this.ChatBustUps.FirstOrDefault(b => b.currentChatSpeaker == this.GetRuntimeChatSpeaker(speakerShorthand: speakerShorthand).ChatSpeaker);
		}
		/// <summary>
		/// Gets the position of the speaker currently on the screen via their shorthand.
		/// Returns None if the speaker is not present.
		/// </summary>
		/// <param name="speakerShorthand">The shorthand for the speaker.</param>
		/// <returns>The position on the screen the speaker lays.</returns>
		public ChatSpeakerPositionType GetSpeakerPosition(string speakerShorthand) {

			// Don't bother getting the position for a blank speaker.
			if (string.IsNullOrEmpty(value: speakerShorthand)) {
				return ChatSpeakerPositionType.None;
			}
			
			try {
				// Find the bust up via its shorthand, then just return the position.
				return this.GetBustUp(speakerShorthand: speakerShorthand).SpeakerPositionType;
			} catch (System.Exception e) {
				Debug.LogWarning("Couldn't find position for speaker with shorthand '" + speakerShorthand + "'. Reason: " + e.Message);
				return ChatSpeakerPositionType.None;
			}
		}
		/// <summary>
		/// Gets the RuntimeChatSpeaker associated with the given shorthand.
		/// </summary>
		/// <param name="speakerShorthand">The shorthand associated with the given speaker.</param>
		public RuntimeChatSpeaker GetRuntimeChatSpeaker(string speakerShorthand) {
			// Call upon the runtime script for this.
			return this.currentRuntimeScript.GetRuntimeChatSpeaker(speakerShorthand: speakerShorthand);
		}
		#endregion

		#region SETTERS
		/// <summary>
		/// Sets the toggle value on the runtime script.
		/// </summary>
		/// <param name="value">The value to set on the runtime toggle.</param>
		public void SetToggle(bool value) {
			this.currentRuntimeScript.SetBoolVariable(toggle: value);
		}
		/// <summary>
		/// Gets the toggle currently set in the script.
		/// </summary>
		/// <returns></returns>
		public bool GetToggle() {
			return this.currentRuntimeScript.toggle;
		}
		#endregion

		#region SCRIPT READING
		/// <summary>
		/// Just reads a simple line.
		/// </summary>
		/// <param name="scriptLine"></param>
		/// <param name="simpleClosedCallback"></param>
		public static void GlobalOpen(string scriptLine, Action simpleClosedCallback) {
			// Just create a closed callback that invokes the action.
			GlobalOpen(
				scriptLine: scriptLine,
				chatClosedCallback: ((str, num, toggle) => {
					simpleClosedCallback.Invoke();
				}));
		}
		/// <summary>
		/// Just reads a simple line.
		/// </summary>
		/// <param name="scriptLine"></param>
		/// <param name="chatClosedCallback"></param>
		public static void GlobalOpen(string scriptLine, ChatClosedCallback chatClosedCallback) {
			GlobalOpen(
				chatScript: new PlainChatScript(rawText: scriptLine), 
				chatClosedCallback: chatClosedCallback);
		}
		/// <summary>
		/// Displays a simple prompt with two options.
		/// </summary>
		/// <param name="promptLine">The text to prompt with.</param>
		/// <param name="optionOne">The text for the first option.</param>
		/// <param name="optionTwo">The text for the second option.</param>
		/// <param name="chatClosedCallback">The callback to run when the chat is closed.</param>
		public static void GlobalOpen(string promptLine, string optionOne, string optionTwo, ChatClosedCallback chatClosedCallback) {
			
			// Concatinate the options into one string that can be parsed.
			string optionStr = "option1: " + optionOne + "; option2: " + optionTwo;
			string fullScriptStr = promptLine + "\n" + optionStr;
			PlainChatScript plainChatScript = new PlainChatScript(fullScriptStr);
			
			GlobalOpen(
				chatScript: plainChatScript,
				chatClosedCallback: chatClosedCallback);
			
		}
		/// <summary>
		/// Just reads some simple lines.
		/// </summary>
		/// <param name="scriptLines"></param>
		/// <param name="chatClosedCallback"></param>
		public static void GlobalOpen(List<string> scriptLines, ChatClosedCallback chatClosedCallback) {
			GlobalOpen(
				chatScript: new PlainChatScript(rawTextList: scriptLines),
				chatClosedCallback: chatClosedCallback);
		}
		/// <summary>
		/// This is the function that gets called from a bolt macro.
		/// </summary>
		/// <param name="textAsset">The text asset containing the script.</param>
		/// <param name="sourceOfCall">The source of the call. Gets told when the chat is closed.</param>
		public static void GlobalOpen(TextAsset textAsset, GameObject sourceOfCall) {
			GlobalOpen(
				textAsset: textAsset, 
				sourceOfCall: sourceOfCall, 
				controllerType: ChatControllerType.Standard);
		}
		/// <summary>
		/// Opens a chat script from a text asset with a closed callback.
		/// </summary>
		/// <param name="textAsset">The text asset containing the chat script.</param>
		/// <param name="chatClosedCallback">The callback to run when finished.</param>
		public static void GlobalOpen(TextAsset textAsset, ChatClosedCallback chatClosedCallback, ChatControllerType controllerType = ChatControllerType.Standard) {
			GlobalOpen(
				textAsset: textAsset, 
				chatClosedCallback: chatClosedCallback, 
				controllerType: controllerType, 
				chatOpenedCallback: delegate {
					
				});
		}
		/// <summary>
		/// This is the function that gets called from a bolt macro.
		/// </summary>
		/// <param name="textAsset">The text asset containing the script.</param>
		/// <param name="sourceOfCall">The source of the call. Gets told when the chat is closed.</param>
		/// <param name="controllerType">The type of controller to use. Relevant for the DarkSouls gag.</param>
		public static void GlobalOpen(TextAsset textAsset, GameObject sourceOfCall, ChatControllerType controllerType) {
			GlobalOpen(
				textAsset: textAsset,
				chatClosedCallback: delegate(string str, int num, bool toggle) {
					Unity.VisualScripting.CustomEvent.Trigger(target: sourceOfCall, name: "CHAT CLOSED", toggle);
				},
				chatOpenedCallback: delegate { },
				controllerType: controllerType);
			/*GlobalOpen(
				textAsset: textAsset,
				chatClosedCallback: delegate {
					// When the chat gets closed, send an event.
					Bolt.CustomEvent.Trigger(target: sourceOfCall, name: "CHAT CLOSED");
				},
				chatOpenedCallback: delegate { },
				controllerType: controllerType);*/
		}
		/// <summary>
		/// This is the function that gets called from a bolt macro.
		/// </summary>
		/// <param name="chatScript">The script to use.</param>
		/// <param name="sourceOfCall">The source of the call. Gets told when the chat is closed.</param>
		public static void GlobalOpen(IChatScript chatScript, GameObject sourceOfCall) {
			GlobalOpen(
				chatScript: chatScript,
				chatClosedCallback: delegate {
					// When the chat gets closed, send an event.
				
					Unity.VisualScripting.CustomEvent.Trigger(target: sourceOfCall, name: "CHAT CLOSED");
				},
				chatOpenedCallback: delegate { });
		}
		/// <summary>
		/// Opens the ChatControllerDX from anywhere.
		/// </summary>
		/// <param name="chatScript"></param>
		/// <param name="chatClosedCallback"></param>
		public static void GlobalOpen(IChatScript chatScript, ChatClosedCallback chatClosedCallback) {
			Instance.Open(chatScript: chatScript, chatClosedCallback: chatClosedCallback);
		}
		/// <summary>
		/// Opens the ChatControllerDX from anywhere.
		/// THIS one is mostly for compatability. I hate the ChatOpened callback.
		/// </summary>
		/// <param name="chatScript"></param>
		/// <param name="chatClosedCallback"></param>
		public static void GlobalOpen(IChatScript chatScript, ChatOpenedCallback chatOpenedCallback, ChatClosedCallback chatClosedCallback) {
			Instance.Open(
				chatScript: chatScript,
				chatParams: new ChatControllerParams() {
					chatClosedCallback = chatClosedCallback,
					chatOpenedCallback = chatOpenedCallback
				});
		}
		/// <summary>
		/// Opens the ChatControllerDX from anywhere.
		/// THIS one is mostly for compatability. I hate the ChatOpened callback.
		/// </summary>
		/// <param name="chatScript"></param>
		/// <param name="chatClosedCallback"></param>
		public static void GlobalOpen(TextAsset textAsset, ChatOpenedCallback chatOpenedCallback, ChatClosedCallback chatClosedCallback) {
			/*Instance.Open(
				chatScript: new PlainChatScript(rawText: textAsset.text),
				chatParams: new ChatControllerParams() {
					chatClosedCallback = chatClosedCallback,
					chatOpenedCallback = chatOpenedCallback
				});*/
			GlobalOpen(
				textAsset: textAsset,
				chatOpenedCallback: chatOpenedCallback,
				chatClosedCallback: chatClosedCallback,
				controllerType: ChatControllerType.Standard);
		}
		/// <summary>
		/// Opens the ChatControllerDX from anywhere.
		/// THIS one is mostly for compatability. I hate the ChatOpened callback.
		/// </summary>
		/// <param name="chatScript"></param>
		/// <param name="chatClosedCallback"></param>
		public static void GlobalOpen(TextAsset textAsset, ChatOpenedCallback chatOpenedCallback, ChatClosedCallback chatClosedCallback, ChatControllerType controllerType) {

			switch (controllerType) {
				case ChatControllerType.Standard:
					StandardInstance.Open(
						chatScript: new PlainChatScript(rawText: textAsset.text),
						chatParams: new ChatControllerParams() {
							chatClosedCallback = chatClosedCallback,
							chatOpenedCallback = chatOpenedCallback
						});
					break;
				case ChatControllerType.DarkSouls:
					DarkSoulsInstance.Open(
						chatScript: new PlainChatScript(rawText: textAsset.text),
						chatParams: new ChatControllerParams() {
							chatClosedCallback = chatClosedCallback,
							chatOpenedCallback = chatOpenedCallback
						});
					break;
			}

			/*Instance.Open(
				chatScript: new PlainChatScript(rawText: textAsset.text),
				chatParams: new ChatControllerParams() {
					chatClosedCallback = chatClosedCallback,
					chatOpenedCallback = chatOpenedCallback
				});*/

		}
		/// <summary>
		/// Opens the chat controller with the specified script and callback upon close.
		/// </summary>
		/// <param name="chatScript">The script to open.</param>
		/// <param name="chatClosedCallback">The callback to run upon completion.</param>
		public void Open(IChatScript chatScript, ChatClosedCallback chatClosedCallback) {
			// Just call the version that accepts params, but make sure to assign the callback.
			this.Open(
				chatScript: chatScript,
				chatParams: new ChatControllerParams() {
					chatClosedCallback = chatClosedCallback
				});
		}
		/// <summary>
		/// Opens the ChatController with a script to be formed from a textasset.
		/// </summary>
		/// <param name="textAsset">The TextAsset to use in generating the script.</param>
		/// <param name="chatParams">Any additional parameters that may be required to run the script.</param>
		public abstract void Open(TextAsset textAsset, ChatControllerParams chatParams);
		/// <summary>
		/// Opens the ChatController with a chat script all ready to go.
		/// </summary>
		/// <param name="chatScript">The script to use.</param>
		/// <param name="chatParams">Any additional parameters that may be required to run the script.</param>
		public abstract void Open(IChatScript chatScript, ChatControllerParams chatParams);
		/// <summary>
		/// Jumps to the specified label in the script.
		/// </summary>
		/// <param name="label">The label to jump to.</param>
		public void JumpToLabel(string label) {
			// Tell the runtime script to jump.
			this.currentRuntimeScript.JumpToLabel(label: label);
			// Go to the next directive.
			this.EvaluateNextDirective();
		}
		/// <summary>
		/// Closes out the chat.
		/// </summary>
		/// <param name="runCloseCallback">Should the chat closed callback be run? Relevant if I need to hard reset a scene and DON'T want it to run.</param>
		public void Close(bool runCloseCallback = true) {
			// Call the version of this function that accepts the parameters.
			this.Close(chatParams: this.chatParams, runCloseCallback: runCloseCallback);
		}
		/// <summary>
		/// Closes out the chat.
		/// </summary>
		/// <param name="chatParams">The parameters that were passed in when the chat was made.</param>
		/// <param name="runCloseCallback">Should the chat closed callback be run? Relevant if I need to hard reset a scene and DON'T want it to run.</param>
		protected abstract void Close(ChatControllerParams chatParams, bool runCloseCallback = true);
		#endregion

		#region EVENTS
		/// <summary>
		/// Gets called when the AdvanceButton receives a Submit event of some kind.
		/// </summary>
		public abstract void AdvanceButtonHit();
		/// <summary>
		/// Evaluate the next directive.
		/// </summary>
		public void EvaluateNextDirective() {
			// Grab the next directive.
			ChatDirective nextDirective = this.currentRuntimeScript.GetNextDirective();
			// Log it out because I'm a fucking idiot.
			// Debug.Log("CHAT: Evaluating directive of type: " + nextDirective.GetType().Name);
			// Evaluate it.
			nextDirective.EvaluateDirective(chatController: this);
			// I'm probably just going to use the runtime script for this.
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Puts focus on the specified speaker by making them Speaking and everyone else Listening.
		/// </summary>
		/// <param name="speakerShorthand">the shorthand for the speaker to focus on.</param>
		public void FocusOnSpeaker(string speakerShorthand, ChatBustUpType bustUpType) {

			// Find the bust up that will be focused on.
			ChatBustUp focusBustUp = this.GetBustUp(speakerShorthand: speakerShorthand);

			this.ChatBustUps                            // Go through all the bust ups where...
				.Where(b => b != focusBustUp)           // ... it is not the one I want to focus on...
				.Where(b => b.ActiveInChat == true)     // ... and it is also active in the chat.
				.ToList()
				.ForEach(b => b.SetListening(bustUpParams: new ChatBustUpParams() { }));

			// Tell the focus bust up to speak.
			focusBustUp?.SetSpeaking(bustUpParams: new ChatBustUpParams() {
				BustUpType = bustUpType
			});
		}
		#endregion

	}


}

