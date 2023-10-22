using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Chat;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Grawly.Dungeon.Interactable {
	
	/// <summary>
	/// A very simple way of inspecting things in a scene.
	/// Approach, inspect, read text, leave.
	/// If you need something more complicated, use something else.
	/// </summary>
	[TypeInfoBox("Lets a DungeonPlayer interact with this object, read text, then leave.")]
	public class SimpleInspectable : MonoBehaviour, IDungeonPlayerInteractionHandler {

		#region FIELDS - TOGGLES : VISIBILITY
		/// <summary>
		/// Should this inspectable's meshrenderer be made invisible?
		/// </summary>
		[SerializeField]
		private bool isInvisible = false;
		#endregion
		
		#region FIELDS - TOGGLES : CHAT TEXT
		/// <summary>
		/// The text to read out upon interaction.
		/// </summary>
		[SerializeField, TextArea(minLines: 5, maxLines: 8)]
		private string chatText = ": > ";
		#endregion

		#region FIELDS - TOGGLES : UNITY EVENTS
		/// <summary>
		/// Should events be invoked upon closing the chat?
		/// </summary>
		[SerializeField]
		private bool useChatCloseEvents = false;
		/// <summary>
		/// Should the player be freed if the chat returns a result of True in its results?
		/// </summary>
		[SerializeField, ShowIf("useChatCloseEvents")]
		private bool freeOnConfirm = false;
		/// <summary>
		/// Should the player be freed if the chat returns a result of False in its results?
		/// </summary>
		[SerializeField, ShowIf("useChatCloseEvents")]
		private bool freeOnDecline = true;
		/// <summary>
		/// An event to be invoked upon a chat exiting with its bool toggle set to "true."
		/// </summary>
		[SerializeField, ShowIf("useChatCloseEvents")]
		private UnityEvent OnChatConfirm = new UnityEvent();
		/// <summary>
		/// An event to be invoked upon a chat exiting with its bool toggle set to "false."
		/// </summary>
		[SerializeField, ShowIf("useChatCloseEvents")]
		private UnityEvent OnChatDecline = new UnityEvent();
		#endregion
		
		#region UNITY CALLS
		private void Start() {
			// Disable the meshrenderer so I can't see it.
			if (this.GetComponent<MeshRenderer>() != null) {
				// this.GetComponent<MeshRenderer>().enabled = false;
				this.GetComponent<MeshRenderer>().enabled = !this.isInvisible;
			}
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IDUNGEONPLAYERINTERACTIONHANDLER
		/// <summary>
		/// Upon interacting, just execute a simple script.
		/// </summary>
		public void OnPlayerInteract() {
			
			// Make the DungeonPlayer wait.
			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait); 
			
			// Create a chat script from the text provided.
			PlainChatScript chatScript = new PlainChatScript(rawText: this.chatText);
			
			// Open the script, then free the player upon completion.
			ChatControllerDX.GlobalOpen(
				chatScript: chatScript,
				chatClosedCallback: delegate(string str, int num, bool toggle) {
					// If using events upon closing the chat, figure out what to do.
					if (this.useChatCloseEvents == true) {
						this.ChatClosed(str, num, toggle);
					} else {
						// If not using events (i.e., just displaying text and nothing else,)
						// free up the player.
						DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
					}
					
				});
			
		}
		#endregion

		#region CHAT CLOSED EVENTS
		/// <summary>
		/// If using events when the chat is closed, this is run.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="num"></param>
		/// <param name="toggle"></param>
		private void ChatClosed(string str, int num, bool toggle) {
			
			// If the toggle is set to true, execute the associated events.
			if (toggle == true) {
				Debug.Log("CLOSING WITH TOGGLE SET TO TRUE");
				this.OnChatConfirm.Invoke();
				if (this.freeOnConfirm == true) {
					DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
				}
				
			} else {
				Debug.Log("CLOSING WITH TOGGLE SET TO FALSE");
				this.OnChatDecline.Invoke();
				if (this.freeOnDecline == true) {
					DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
				}
			}
		}
		#endregion

		#region PROTOTYPE BEHAVIORS
		/// <summary>
		/// I used to call the SceneController directly but referecnes can be broken at runtime.
		/// I'm drunk.
		/// </summary>
		/// <param name="sceneIndex"></param>
		public void PrototypeLoadScene(int sceneIndex) {
			SceneController.instance.BasicLoadSceneWithFade(sceneIndex: sceneIndex);
		}
		#endregion
		
	}
}