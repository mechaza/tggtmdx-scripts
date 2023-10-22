using System.Collections;
using System.Collections.Generic;
using Grawly.Chat;
using Grawly.Dungeon;
using Grawly.DungeonCrawler;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Grawly.Calendar.Behavior.General {
	
	// [InfoBox("Automatically plays a chat log when the scene is loaded.")]
	[Title("Auto Chat")]
	[System.Serializable]
	[GUIColor(r: 1f, g: 0.9f, b: 0.9f, a: 1f)]
	public class AutoChat : StoryBehavior {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to wait before running the chat.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		[PropertyTooltip("The amount of time to wait before running the chat.")]
		private float timeToWait = 1f;
		/// <summary>
		/// Should the player be freed upon closing the chat?
		/// Note that the player will always be set to Wait when the chat is opened.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		[PropertyTooltip("Should the player be freed upon closing the chat?\nNote that the player will always be set to Wait when the chat is opened.")]
		private bool freePlayerOnClose = true;
		#endregion

		#region FIELDS - CHAT SCRIPTS
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private ChatScriptReferenceType scriptReferenceType = ChatScriptReferenceType.Inline;
		/// <summary>
		/// The chat script to use if using a plain text file.
		/// </summary>
		[SerializeField, ShowIf("UseTextAsset")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private TextAsset chatTextAsset;
		/// <summary>
		/// The kind of chat asset to use if using an asset.
		/// </summary>
		[SerializeField, ShowIf("UseChatAsset")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private ChatScriptAssetType scriptAssetType = ChatScriptAssetType.Simple;
		/// <summary>
		/// The chat script asset to use if set to reference an actual IChatScript.
		/// </summary>
		[FormerlySerializedAs("chatScriptAsset")]
		[SerializeField, ShowIf("UseSimpleChatAsset")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private SimpleChatScriptDX simpleChatAsset;
		/// <summary>
		/// The chat script to use for when a serialized chat script needs to be used.
		/// </summary>
		[SerializeField, ShowIf("UseSerializedChatAsset")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private SerializedChatScriptDX serializedChatAsset;
		/// <summary>
		/// The chat text to use if using inline text.
		/// </summary>
		[SerializeField, ShowIf("UseInlineText"), TextArea(minLines: 5, maxLines: 15)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private string inlineChatText = "";
		#endregion

		#region CONSTRUCTORS
		public AutoChat() {
			
		}
		/// <summary>
		/// A quick way to create an AutoChat behavior.
		/// This should only be used during prototyping.
		/// </summary>
		/// <param name="inlineChatText"></param>
		/// <param name="freePlayerOnClose"></param>
		/// <param name="timeToWait"></param>
		public AutoChat(string inlineChatText, bool freePlayerOnClose, float timeToWait) {
			Debug.Assert(Application.isEditor);
			this.scriptReferenceType = ChatScriptReferenceType.Inline;
			this.inlineChatText = inlineChatText;
			this.freePlayerOnClose = freePlayerOnClose;
			this.timeToWait = timeToWait;
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION
		public override StoryBeatReaction OnStoryBeatLoad() {
			
			return delegate(StoryBeatReactionSequence sequence) {
				GameController.Instance.RunEndOfFrame(() => {
				
					Debug.Log("Running auto chat...");

					// Tell the players to wait.
					// I used to *not* have these checks because I thought the null-conditional would protect against it.
					// But there issues with SetFSMState *actually* being called but failing.
					// I'm literally not sure how that's even possible. But whatever.
					if (DungeonPlayer.Instance != null) {
						DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Wait);	
					}
					if (CrawlerPlayer.Instance != null) {
						CrawlerPlayer.Instance?.SetState(CrawlerPlayerState.Wait);
					}
					
					// Create a placeholder for the final chat script and switch based on the reference type to use.
					IChatScript chatScript;
					switch (this.scriptReferenceType) {
						case ChatScriptReferenceType.Inline:
							chatScript = new PlainChatScript(rawText: this.inlineChatText);
							break;
						case ChatScriptReferenceType.ChatAsset:
							// chatScript = this.simpleChatAsset;
							chatScript = this.scriptAssetType == ChatScriptAssetType.Simple 
								? this.simpleChatAsset
								: this.serializedChatAsset;
							break;
						case ChatScriptReferenceType.TextAsset:
							chatScript = new PlainChatScript(rawText: this.chatTextAsset.text);
							break;
						default:
							throw new System.Exception("This should never be reached!");
					}
				
					// Wait a moment, then go.
					GameController.Instance.WaitThenRun(this.timeToWait, () => {
						ChatControllerDX.GlobalOpen(
							chatScript: chatScript, 
							chatClosedCallback: delegate(string str, int num, bool toggle) {
								
								// Upon closing the chat, free the players if set to do so.
								if (this.freePlayerOnClose == true) {
									DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Free);
									CrawlerPlayer.Instance?.SetState(CrawlerPlayerState.Free);
								}
								
								// Execute the next reaction.
								sequence.ExecuteNextReaction();
								
							});
					
					});
				});
			};
			
			
		}
		#endregion

		#region ODIN HELPERS - GENERAL
		private bool UseTextAsset() {
			return this.scriptReferenceType == ChatScriptReferenceType.TextAsset;
		}
		private bool UseChatAsset() {
			return this.scriptReferenceType == ChatScriptReferenceType.ChatAsset;
		}
		private bool UseSimpleChatAsset() {
			return this.UseChatAsset() 
			       && this.scriptAssetType == ChatScriptAssetType.Simple;
		}
		private bool UseSerializedChatAsset() {
			return this.UseChatAsset() 
			       && this.scriptAssetType == ChatScriptAssetType.Serialized;
		}
		private bool UseInlineText() {
			return this.scriptReferenceType == ChatScriptReferenceType.Inline;
		}
		/// <summary>
		/// The string to use for the foldout groups in the inspector.
		/// </summary>
		protected override string FoldoutGroupTitle {
			get {
				switch (this.scriptReferenceType) {
					case ChatScriptReferenceType.Inline:
						return "Auto Chat (" + "Inline" + ")";
					case ChatScriptReferenceType.ChatAsset:
						if (this.scriptAssetType == ChatScriptAssetType.Simple) {				// Simple chat asset
							return "Auto Chat (" + (this.simpleChatAsset == null
								? "NA" 
								: this.simpleChatAsset.name) + ")";
						} else {																// Serialized chat asset
							return "Auto Chat (" + (this.serializedChatAsset == null 
								? "NA"
								: this.serializedChatAsset.name) + ")";		
						}
					case ChatScriptReferenceType.TextAsset:
						return "Auto Chat (" + (this.chatTextAsset == null ? "NA" : this.chatTextAsset.name) + ")";
					default:
						return "Auto Chat (" + "???" + ")";
				}
			}
		}
		#endregion

		
		
	}

	
}