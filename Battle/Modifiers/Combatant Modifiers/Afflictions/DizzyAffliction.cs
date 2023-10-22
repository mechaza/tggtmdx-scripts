using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// Accuracy decreased.
	/// </summary>
	public class DizzyAffliction : CombatantModifier, ICombatantAffliction, IInterceptIncomingDCS, IOnTurnReady {

		#region FIELDS - STATE
		/// <summary>
		/// How many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
		#endregion

		#region FIELDS - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Dizzy;
			}
		}
		#endregion

		#region INTERFACEIMPLEMENTATION - IINTERCEPTINCOMINGDCS
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			// Outgoing attacks should have lower accuracy. Incoming attacks should be buffed.
			dcs.damageCalculations.Where(dc => dc.source == this.combatantOwner).ToList().ForEach(dc => dc.rawAccuracy *= 0.5f);
			dcs.damageCalculations.Where(dc => dc.target == this.combatantOwner).ToList().ForEach(dc => dc.rawAccuracy *= 1.5f);
			return dcs;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		public BattleReaction OnTurnReady() {
			return delegate (BattleReactionSequence battleReactionSequence) {
				// Decrement the timer at the start of the turn.
				this.timer -= 1;

				// If it finally hit zero, remove the affliction.
				if (timer == 0) {
					BattleNotifier.DisplayNotifier("Affliction reverted!");
					combatantOwner.CureAffliction(animateCure: true);
					GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
						battleReactionSequence.ExecuteNextReaction();
					});
				} else {
					battleReactionSequence.ExecuteNextReaction();
				}
			};
		}
		#endregion

		#region INSPECTOR BULLSHIT
		private string inspectorString = "Accuracy decreased.";
		protected override string InspectorDescription {
			get {
				return this.inspectorString;
			}
		}

		#endregion

	}


}