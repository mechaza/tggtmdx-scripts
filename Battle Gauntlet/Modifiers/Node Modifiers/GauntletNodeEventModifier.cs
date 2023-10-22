using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Gauntlet.Nodes;
using Grawly.Chat;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace Grawly.Gauntlet.Modifiers.NodeModifiers {

	/// <summary>
	/// A specialized version of the typical modifier that makes it easier for me to implement events. 
	/// </summary>
	public abstract class GauntletNodeEventModifier : GauntletNodeModifier, IOnEnterNode, IOnExitNode, IOnSubmitNode, IOnCompleteNode {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The event this modifier should respond to specifically.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "General")]
		private GauntletNodeEventType eventType;
		/// <summary>
		/// Should this modifier be removed from the gauntlet node upon completion?
		/// If not, it will be read out the next time a marker enters.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "General")]
		private bool removeModifierOnChatComplete = true;
		#endregion

		#region ACTUAL IMPLEMENTATION
		/// <summary>
		/// Gets called on the appropriate event and is what handles the intended functionality.
		/// </summary>
		/// <param name="eventParams">The parameters being sent to this modifier.</param>
		/// <param name="reactionSequence">The reaction sequence itself.</param>
		/// <returns></returns>
		protected abstract GauntletReaction ModifierEvent(GauntletNodeEventParams eventParams);
		#endregion

		#region INTERFACE IMPLEMENTATIONS
		public GauntletReaction OnCompleteNode(GauntletNodeEventParams eventParams) {
			if (this.eventType == GauntletNodeEventType.OnComplete) {
				return this.ModifierEvent(eventParams);
			} else {
				return delegate (GauntletReactionSequence reactionSequence) {
					reactionSequence.ExecuteNextReaction();
				};
			}
		}
		public GauntletReaction OnEnterNode(GauntletNodeEventParams eventParams) {
			if (this.eventType == GauntletNodeEventType.OnEnter) {
				return this.ModifierEvent(eventParams);
			} else {
				return delegate (GauntletReactionSequence reactionSequence) {
					reactionSequence.ExecuteNextReaction();
				};
			}
		}
		public GauntletReaction OnExitNode(GauntletNodeEventParams eventParams) {
			if (this.eventType == GauntletNodeEventType.OnExit) {
				return this.ModifierEvent(eventParams);
			} else {
				return delegate (GauntletReactionSequence reactionSequence) {
					reactionSequence.ExecuteNextReaction();
				};
			}
		}
		public GauntletReaction OnSubmitNode(GauntletNodeEventParams eventParams) {
			if (this.eventType == GauntletNodeEventType.OnSubmit) {
				return this.ModifierEvent(eventParams);
			} else {
				return delegate (GauntletReactionSequence reactionSequence) {
					reactionSequence.ExecuteNextReaction();
				};
			}
		}
		#endregion

	}


}