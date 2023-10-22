using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Grawly.UI;

namespace Grawly.Gauntlet.Modifiers.NodeModifiers {

	/// <summary>
	/// Opens a tutorial when the specific event is triggered.
	/// </summary>
	[System.Serializable]
	public class TutorialOnNodeEvent : GauntletNodeModifier, IOnEnterNode, IOnExitNode, IOnSubmitNode, IOnCompleteNode {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The event this modifier should respond to specifically.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "General")]
		private GauntletNodeEventType eventType;
		/// <summary>
		/// The tutorial to bring up.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "General")]
		private TutorialTemplate tutorialTemplate;
		/// <summary>
		/// Should this modifier be removed from the gauntlet node upon completion?
		/// If not, it will be read out the next time a marker enters.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "General")]
		private bool removeModifierOnChatComplete = true;
		#endregion

		#region EVENTS
		public GauntletReaction OnEnterNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				if (this.eventType == GauntletNodeEventType.OnEnter) {
					this.OpenTutorial(
						eventParams: eventParams, 
						reactionSequence: reactionSequence, 
						tutorialTemplate: this.tutorialTemplate);
				} else {
					reactionSequence.ExecuteNextReaction();
				}
			};
		}
		public GauntletReaction OnExitNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				if (this.eventType == GauntletNodeEventType.OnExit) {
					this.OpenTutorial(
						eventParams: eventParams, 
						reactionSequence: reactionSequence,
						tutorialTemplate: this.tutorialTemplate);
				} else {
					reactionSequence.ExecuteNextReaction();
				}
			};
		}
		public GauntletReaction OnSubmitNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				if (this.eventType == GauntletNodeEventType.OnSubmit) {
					this.OpenTutorial(
						eventParams: eventParams,
						reactionSequence: reactionSequence,
						tutorialTemplate: this.tutorialTemplate);
				} else {
					reactionSequence.ExecuteNextReaction();
				}
			};
		}
		public GauntletReaction OnCompleteNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				if (this.eventType == GauntletNodeEventType.OnComplete) {
					this.OpenTutorial(
						eventParams: eventParams, 
						reactionSequence: reactionSequence,
						tutorialTemplate: this.tutorialTemplate);
				} else {
					reactionSequence.ExecuteNextReaction();
				}
			};
		}
		#endregion

		#region ACTUAL CODE TO RUN
		/// <summary>
		/// Segmenting this into its own method. This is what should open the tutorial.
		/// </summary>
		/// <param name="eventParams"></param>
		/// <param name="reactionSequence"></param>
		/// <param name="tutorialTemplate"></param>
		private void OpenTutorial(GauntletNodeEventParams eventParams, GauntletReactionSequence reactionSequence, TutorialTemplate tutorialTemplate) {
			GauntletController.instance.SetFSMState(GauntletStateType.Wait);
			TutorialScreenController.OpenTutorial(
				tutorialTemplate: tutorialTemplate,
				actionOnClose: delegate {
					// Remove this modifier from the node if it was specified to do so.
					if (this.removeModifierOnChatComplete == true) {
						eventParams.GauntletNode.RemoveModifier(this);
					}
					
					GauntletController.instance.SetFSMState(GauntletStateType.Free);
					reactionSequence.ExecuteNextReaction();
				});
		}
		#endregion

		#region INSPECTOR JUNK
		private string inspectorDescription = "Shows a tutorial when the specific event is triggered.";
		protected override string InspectorDescription {
			get {
				return this.inspectorDescription;
			}
		}
		#endregion

	}


}