using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Dungeon;
using System.Linq;
using Grawly.DungeonCrawler;
using Sirenix.Serialization;
using UnityEngine.Events;

namespace Grawly.Chat {

	/// <summary>
	/// A way to rapidly prototype scripts from the scene.
	/// </summary>
	public class SceneChatScriptDX : SerializedMonoBehaviour, IChatScript {

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// Should the player be freed on chat close?
		/// </summary>
		[SerializeField, TabGroup("General", "Toggles"), Title("General")]
		private bool freePlayerOnChatClose = true;
		#endregion
		
		#region FIELDS - TOGGLES : SCRIPT
		/// <summary>
		/// Should a text asset be used?
		/// </summary>
		[SerializeField, TabGroup("General", "Script")]
		private bool useTextAsset = false;
		/// <summary>
		/// The text asset to use for this script, if set to do so.
		/// </summary>
		[SerializeField, ShowIf("useTextAsset"),TabGroup("General", "Script")]
		private TextAsset textAsset;
		#endregion
		
		#region FIELDS - SPEAKERS
		/// <summary>
		/// The chat speakers to be used at runtime, with their shorthand already defined.
		/// </summary>
		[OdinSerialize, HideIf("useTextAsset"), TabGroup("General", "Script"), ListDrawerSettings(DraggableItems = false)]
		private List<RuntimeChatSpeaker> runtimeSpeakers = new List<RuntimeChatSpeaker>();
		#endregion

		#region FIELDS - DIRECTIVES
		/// <summary>
		/// The ChatDirectives to use for this script.
		/// </summary>
		[OdinSerialize, HideIf("useTextAsset"), TabGroup("General", "Script"), ListDrawerSettings(Expanded = true)]
		private List<ChatDirective> chatDirectives = new List<ChatDirective>();
		#endregion

		#region FIELDS - EVENTS
		/// <summary>
		/// An event to invoke when the chat is closed.
		/// </summary>
		[Title("Events")]
		[SerializeField, TabGroup("General", "Toggles")]
		private UnityEvent onChatCloseEvent;
		#endregion
		
		#region INTERFACE IMPLEMENTATION - ICHATSCRIPT
		/// <summary>
		/// The speakers being used for this script.
		/// </summary>
		public List<RuntimeChatSpeaker> RuntimeChatSpeakers {
			get {
				
				// Create a list of spekers that should be available by default.
				List<RuntimeChatSpeaker> standardSpeakers = new List<RuntimeChatSpeaker>() {
					new RuntimeChatSpeaker(chatSpeaker: DataController.GetChatSpeaker("dorothy"), speakerShorthand: "d"),
					new RuntimeChatSpeaker(chatSpeaker: DataController.GetChatSpeaker("blanche"), speakerShorthand: "b"),
					new RuntimeChatSpeaker(chatSpeaker: DataController.GetChatSpeaker("rose"), speakerShorthand: "r"),
					new RuntimeChatSpeaker(chatSpeaker: DataController.GetChatSpeaker("sophia"), speakerShorthand: "s"),
					new RuntimeChatSpeaker(chatSpeaker: DataController.GetChatSpeaker(""), speakerShorthand: ""),
				};
				// Concat these speakers with the standards.
				return this.runtimeSpeakers.Concat(standardSpeakers).ToList();				
				
			}
		}
		/// <summary>
		/// The ChatDirectives for this script.
		/// </summary>
		public List<ChatDirective> Directives {
			get {
				return this.chatDirectives
					.SelectMany(c => {
						if (c is RawDirective) {
							// If there is a raw directive, transform it into its own list.
							return RawDirectiveParser.GetDirectives(text: (c as RawDirective).rawText);
						} else {
							// If this directive is not raw, return a new list with just the single element.
							return new List<ChatDirective>() { c };
						}
					})
					.ToList();
			}
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Runs this script.
		/// </summary>
		public void RunChatScript() {

			// If the chat prompt is shown, make sure to show it again on chat close. Also save the prompt text.
			bool displayActionPromptOnChatClose = CrawlerActionPrompt.Instance != null && CrawlerActionPrompt.Instance.IsDisplayed == true;
			string promptText = CrawlerActionPrompt.Instance?.CurrentPrompt ?? "";
			CrawlerActionPrompt.Instance?.Dismiss();
			
			
			// Create a new callback to run upon close.
			ChatClosedCallback callback = delegate(string str, int num, bool toggle) {
				if (this.freePlayerOnChatClose == true) {
					DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Free);
					CrawlerPlayer.Instance?.SetState(CrawlerPlayerState.Free);
				}
				
				if (displayActionPromptOnChatClose == true) {
					Debug.LogWarning("Prompt type will always be none. Fix this later.");
					CrawlerActionPrompt.Instance.Display(promptText: promptText);
				} else {
					Debug.Log("Not showing the action prompt.");
				}
				
				// If there is an on-close event, invoke that.
				this.onChatCloseEvent?.Invoke();
				
			};
			
			
			// Run the script with this callback.
			this.RunChatScript(chatClosedCallback: callback);
			
			
		}
		/// <summary>
		/// Runs this sript in the text asset form.
		/// </summary>
		/// <param name="chatClosedCallback">The chat closed callback to run.</param>
		private void RunChatScript(ChatClosedCallback chatClosedCallback) {
			
			// Tell the players to wait.
			DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Wait);
			CrawlerPlayer.Instance?.SetState(CrawlerPlayerState.Wait);
			
			// If using the text asset, open that. If not, just use this.
			if (this.useTextAsset == true) {
				ChatControllerDX.GlobalOpen(textAsset: this.textAsset, chatClosedCallback: chatClosedCallback);
			} else {
				ChatControllerDX.GlobalOpen(chatScript: this, chatClosedCallback: chatClosedCallback);
			}
	
		}
		#endregion
		
		/*#region ODIN BUTTONS
		/// <summary>
		/// Quickly adds a raw directive to the end of the list.
		/// </summary>
		[TabGroup("General", "Script"), HideIf("useTextAsset"), ButtonGroup("General/Quick Add"), LabelText("Raw")]
		private void AddRawDirective() {
			this.chatDirectives.Add(new RawDirective());
		}
		/// <summary>
		/// Quickly adds a jump directive to the end of the list.
		/// Button gets accessed in the inspector.
		/// </summary>
		[TabGroup("General", "Script"),HideIf("useTextAsset"),  ButtonGroup("General/Quick Add"), LabelText("GameObject")]
		private void AddToggleGameObjectDirective() {
			this.chatDirectives.Add(new ToggleGameObjectDirective());
		}
		/// <summary>
		/// Quickly adds a dialogue directive to the end of the list.
		/// Button gets accessed in the inspector.
		/// </summary>
		[TabGroup("General", "Script"), HideIf("useTextAsset"),  ButtonGroup("General/Quick Add"), LabelText("Dialogue")]
		private void AddDialogueDirective() {
			this.chatDirectives.Add(new DialogueDirective());
		}
		/// <summary>
		/// Quickly adds a show directive to the end of the list.
		/// Button gets accessed in the inspector.
		/// </summary>
		[TabGroup("General", "Script"), HideIf("useTextAsset"), ButtonGroup("General/Quick Add"), LabelText("Show")]
		private void AddShowDirective() {
			this.chatDirectives.Add(new ShowDirective());
		}
		/// <summary>
		/// Quickly adds a label directive to the end of the list.
		/// Button gets accessed in the inspector.
		/// </summary>
		[TabGroup("General", "Script"), HideIf("useTextAsset"),  ButtonGroup("General/Quick Add"),LabelText("Label")]
		private void AddLabelDirective() {
			this.chatDirectives.Add(new LabelDirective());
		}
		/// <summary>
		/// Quickly adds a jump directive to the end of the list.
		/// Button gets accessed in the inspector.
		/// </summary>
		[TabGroup("General", "Script"), HideIf("useTextAsset"), ButtonGroup("General/Quick Add"), LabelText("Jump")]
		private void AddJumpDirective() {
			this.chatDirectives.Add(new JumpDirective());
		}
		/// <summary>
		/// Tests the script out during run time.
		/// </summary>
		[TabGroup("General", "Script"),  LabelText("Test"), HideInEditorMode, PropertySpace(10)]
		private void TestScript() {
			if (this.useTextAsset == true) {
				ChatControllerDX.GlobalOpen(textAsset: this.textAsset, chatClosedCallback: delegate { });
			} else {
				ChatControllerDX.GlobalOpen(chatScript: this, chatClosedCallback: delegate { });
			}
			
		}
		#endregion*/

	}


}