using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// Damage decreased.
	/// </summary>
	public class HungerAffliction : CombatantModifier, ICombatantAffliction, IInterceptIncomingDCS, IOnTurnReady {

		#region FIELDS - TIMER
		/// <summary>
		/// The timer that says how many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
		#endregion

		#region FIELDS - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Hunger;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does not work anymore!");
			return delegate (BattleReactionSequence battleReactionSequence) {

				// Decrement the timer at the start of the turn.
				this.timer -= 1;
				// If it finally hit zero, remove the affliction.
				if (timer == 0) {
					BattleNotifier.DisplayNotifier("Affliction reverted!", 3f);
					combatantOwner.CureAffliction(animateCure: true);
				} else {
					BattleNotifier.DisplayNotifier(combatantOwner.metaData.name + " is asleep!", 3f);
				}

				GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
					battleReactionSequence.ExecuteNextReaction();
				});

			};

		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			throw new System.NotImplementedException();
		}
		#endregion

		#region INSPECTOR BULLSHIT
		private string inspectorString = "Damage decreased.";
		protected override string InspectorDescription {
			get {
				return this.inspectorString;
			}
		}
		#endregion

	}


}