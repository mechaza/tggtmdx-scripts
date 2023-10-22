using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Gauntlet.Nodes;
using Grawly.Chat;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace Grawly.Gauntlet.Modifiers.NodeModifiers {

	/// <summary>
	/// When a player enters a node that has this modifier, it brings up a chat.
	/// </summary>
	[System.Serializable]
	public class ChatOnNodeEvent : GauntletNodeModifier, IOnEnterNode, IOnExitNode, IOnSubmitNode, IOnCompleteNode {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The event this modifier should respond to specifically.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "General")]
		private GauntletNodeEventType eventType;
		/// <summary>
		/// The script to use for the chat.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "General")]
		private TextAsset chatScript;
		/// <summary>
		/// Should this modifier be removed from the gauntlet node upon completion?
		/// If not, it will be read out the next time a marker enters.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "General")]
		private bool removeModifierOnChatComplete = true;
		#endregion

		#region INTERFACE IMPLEMENTATION - IONENTERNODE
		/// <summary>
		/// When the marker enters this node, it should bring up a chat.
		/// </summary>
		/// <param name="gauntletNode">The gauntlet node that was just entered.</param>
		/// <param name="gauntletMarker">The marker that just entered the node.</param>
		/// <returns>Executable code that brings up a chat.</returns>
		public GauntletReaction OnEnterNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {

				if (this.eventType == GauntletNodeEventType.OnEnter) {
					GauntletController.instance.SetFSMState(GauntletStateType.Wait);

					ChatControllerDX.GlobalOpen(
						textAsset: this.chatScript,
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							if (this.removeModifierOnChatComplete == true) {
								eventParams.GauntletNode.RemoveModifier(this);
							}
							GauntletController.instance.SetFSMState(GauntletStateType.Free);
							reactionSequence.ExecuteNextReaction();
						});
				} else {
					reactionSequence.ExecuteNextReaction();
				}
			};
		}
		#endregion
		public GauntletReaction OnExitNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {

				if (this.eventType == GauntletNodeEventType.OnExit) {
					GauntletController.instance.SetFSMState(GauntletStateType.Wait);
					ChatControllerDX.GlobalOpen(
						textAsset: this.chatScript,
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							if (this.removeModifierOnChatComplete == true) {
								eventParams.GauntletNode.RemoveModifier(this);
							}
							GauntletController.instance.SetFSMState(GauntletStateType.Free);
							reactionSequence.ExecuteNextReaction();
						});
				} else {
					reactionSequence.ExecuteNextReaction();
				}
			};
		}
		public GauntletReaction OnSubmitNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {

				if (this.eventType == GauntletNodeEventType.OnSubmit) {
					GauntletController.instance.SetFSMState(GauntletStateType.Wait);

					ChatControllerDX.GlobalOpen(
						textAsset: this.chatScript,
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							if (this.removeModifierOnChatComplete == true) {
								eventParams.GauntletNode.RemoveModifier(this);
							}
							GauntletController.instance.SetFSMState(GauntletStateType.Free);
							reactionSequence.ExecuteNextReaction();
						});
				} else {
					reactionSequence.ExecuteNextReaction();
				}
			};
		}
		public GauntletReaction OnCompleteNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {

				if (this.eventType == GauntletNodeEventType.OnComplete) {
					GauntletController.instance.SetFSMState(GauntletStateType.Wait);

					ChatControllerDX.GlobalOpen(
						textAsset: this.chatScript,
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							if (this.removeModifierOnChatComplete == true) {
								eventParams.GauntletNode.RemoveModifier(this);
							}
							GauntletController.instance.SetFSMState(GauntletStateType.Free);
							reactionSequence.ExecuteNextReaction();
						});
				} else {
					reactionSequence.ExecuteNextReaction();
				}
			};
		}

		#region INSPECTOR JUNK
		private string inspectorDescription = "Just opens up a chat when the marker enters this node.";
		protected override string InspectorDescription {
			get {
				return this.inspectorDescription;
			}
		}
		#endregion

	}


}