using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// Cannot move.
	/// </summary>
	public class FearAffliction : CombatantModifier, ICombatantAffliction, IOnTurnReady, ITurnBehaviorOverride {

		#region FIELDS - TIMER
		/// <summary>
		/// The timer that says how many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
		#endregion

		#region FIELDS - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Fear;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does not work anymore!");
			return delegate (BattleReactionSequence battleReactionSequence) {

				// Highlight this combatant depending on what type they are.
				this.combatantOwner.CombatantAnimator.AnimateFocusHighlight(combatant: this.combatantOwner);

				// Change the bust up image if the combatant is a player.
				if (this.combatantOwner is Player) {
					BattleMenuControllerDX.instance.SetPlayerBustUpSprite(player: this.combatantOwner as Player);
				}

				// Decrement the timer at the start of the turn.
				this.timer -= 1;
				// If it finally hit zero, remove the affliction.
				if (timer == 0) {
					BattleNotifier.DisplayNotifier("Affliction reverted!", 3f);
					combatantOwner.CureAffliction(animateCure: true);
				} else {
					BattleNotifier.DisplayNotifier(combatantOwner.metaData.name + " is scared!", 3f);
				}

				GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
					battleReactionSequence.ExecuteNextReaction();
				});

			};

		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ITURNBEHAVIOROVERRIDE
		/// <summary>
		/// This affliction should always take priority.
		/// </summary>
		public bool TakesPriority {
			get {
				return true;
			}
		}
		/// <summary>
		/// The combatant should restore health on their turn.
		/// </summary>
		public void ExecuteTurn() {
			BattleController.Instance.FSM.SendEvent("Skip Turn");
		}
		#endregion

		#region INSPECTOR BULLSHIT
		private string inspectorString = "Cannot move.";
		protected override string InspectorDescription {
			get {
				return this.inspectorString;
			}
		}
		#endregion

	}


}