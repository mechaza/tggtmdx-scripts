using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Grawly.DungeonCrawler.Generation;
using Grawly.UI.Legacy;
using Sirenix.Serialization;
using System.Linq;
using Grawly.Chat;

namespace Grawly.DungeonCrawler.Events {
	
	/// <summary>
	/// An event that should be invoked inside of a cralwer dungeon.
	/// Most of the time, this will be associated with an event tile.
	/// </summary>
	[CreateAssetMenu(fileName = "New Crawler Event", menuName = "Grawly/Crawler/Crawler Event")]
	public class CrawlerEventTemplate : SerializedScriptableObject {

		//
		//
		//	Please remember that this class is subject to change if I want to use things other than chats.
		//
		//
		
		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// Should the player be freed when this event is finished executing?
		/// </summary>
		[SerializeField]
		private bool freePlayerOnComplete = true;
		/// <summary>
		/// The type of event this is.
		/// </summary>
		[SerializeField]
		private CrawlerEventType eventType = CrawlerEventType.Chat;
		#endregion
		
		#region FIELDS - TOGGLES : CHAT
		/// <summary>
		/// The kind of script that should be read when this event is invoked.
		/// </summary>
		[SerializeField, ShowIf("IsChatEvent")]
		private ChatScriptReferenceType scriptType = ChatScriptReferenceType.Inline;
		/// <summary>
		/// The chat text to use if using inline text.
		/// </summary>
		[SerializeField, ShowIf("IsInlineChat"), TextArea(minLines: 5, maxLines: 15)]
		private string inlineChatText = "";
		/// <summary>
		/// The chat script to use if using a plain text file.
		/// </summary>
		[SerializeField, ShowIf("IsTextAssetChat")]
		private TextAsset chatTextAsset;
		/// <summary>
		/// The chat script asset to use if set to reference an actual IChatScript.
		/// </summary>
		[SerializeField, ShowIf("IsSerializedAssetChat")]
		private SimpleChatScriptDX chatScriptAsset;
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Executes this event.
		/// </summary>
		/// <param name="parentTile">The tile that invoked this template's events.</param>
		public void ExecuteEvent(EventTile parentTile) {
			
			// Depending on the toggle, the player may or may not be freed upon the events completion.
			
			if (this.freePlayerOnComplete == true) {
				this.ExecuteEvent(
					parentTile: parentTile,
					onEventComplete: () => {
						CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Free);
					});
				
			} else {
				this.ExecuteEvent(
					parentTile: parentTile,
					onEventComplete: () => {
						
					});
			}
		}
		/// <summary>
		/// Executes this event.
		/// </summary>
		/// <param name="parentTile">The tile that invoked this template's events.</param>
		/// <param name="onEventComplete">The callback to run when this event is finished running.</param>
		private void ExecuteEvent(EventTile parentTile, System.Action onEventComplete) {
			
			// Make the player wait.
			CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Wait);
			
			// Execute a chat depending on what kind of script was used.
			switch (this.scriptType) {
				case ChatScriptReferenceType.Inline:
					ChatControllerDX.GlobalOpen(
						scriptLine: this.inlineChatText, 
						simpleClosedCallback: () => {
							onEventComplete.Invoke();
							// Upon closing the confirmation, set this object as inactive.
							GameController.Instance.RunEndOfFrame(action: () => {
								parentTile.gameObject.SetActive(false);
							});
						});
					break;
				case ChatScriptReferenceType.TextAsset:
					ChatControllerDX.GlobalOpen(
						textAsset: this.chatTextAsset,
						chatClosedCallback: ((str, num, toggle) => {
							onEventComplete.Invoke();
							// Upon closing the confirmation, set this object as inactive.
							GameController.Instance.RunEndOfFrame(action: () => {
								parentTile.gameObject.SetActive(false);
							});
						}));
					break;
				case ChatScriptReferenceType.ChatAsset:
					ChatControllerDX.GlobalOpen(
						chatScript: this.chatScriptAsset, 
						chatClosedCallback: ((str, num, toggle) => {
							onEventComplete.Invoke();
							// Upon closing the confirmation, set this object as inactive.
							GameController.Instance.RunEndOfFrame(action: () => {
								parentTile.gameObject.SetActive(false);
							});
						}));
					break;
				default:
					throw new System.Exception("This should never be reached!");
			}
			
			
		}
		#endregion
		
		#region ODIN HELPERS : EVENT TYPE
		private bool IsChatEvent() {
			return this.eventType == CrawlerEventType.Chat;
		}
		#endregion

		#region ODIN HELPERS : CHAT SPECIFIC
		private bool IsInlineChat() {
			return this.IsChatEvent() == true
			       && this.scriptType == ChatScriptReferenceType.Inline;
		}
		private bool IsTextAssetChat() {
			return this.IsChatEvent() == true
			       && this.scriptType == ChatScriptReferenceType.TextAsset;
		}
		private bool IsSerializedAssetChat() {
			return this.IsChatEvent() == true
			       && this.scriptType == ChatScriptReferenceType.ChatAsset;
		}
		#endregion
		
	}
}