using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Modifiers.Important {

	/// <summary>
	/// Ensures the target cannot be downed, receives less damage, and will not be afflicted by an ailment.
	/// </summary>
	[System.Serializable]
	public class DefendModifier : CombatantModifier, IOnTurnReady, IInterceptIncomingDCSLate {

		#region INTERFACE IMPLEMENTATIONS
		/// <summary>
		/// Upon turn start, if this function is being called like, at all, make sure to remove it.
		/// </summary>
		/// <returns></returns>
		public BattleReaction OnTurnReady() {
			return delegate (BattleReactionSequence battleReactionSequence) {
				this.combatantOwner.RemoveModifier(this);
				battleReactionSequence.ExecuteNextReaction();
			};
		}
		/// <summary>
		/// Intercept any incoming attacks and make sure that this combatant is not downed or afflicted with an ailment, and also decrease the attack amount.
		/// </summary>
		/// <param name="dcs"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public DamageCalculationSet InterceptIncomingDCSLate(DamageCalculationSet dcs, Combatant self) {

			// Start off by reducing damage.
			dcs.damageCalculations
				.Where(dc => dc.FinalTarget == self)
				.Where(dc => dc.TargetTookHPDamage)
				.ToList()
				.ForEach(dc => dc.rawDamageAmount = (int)(dc.rawDamageAmount * 0.5f));

			// Next, nullify afflictions.
			dcs.damageCalculations
				.Where(dc => dc.target == self)
				.ToList()
				.ForEach(dc => dc.NullifyAffliction());

			// Finally, ensure the target won't be downed.
			dcs.damageCalculations
				.Where(dc => dc.target == self)
				.ToList()
				.ForEach(dc => dc.DoNotAllowTargetToBeDowned());

			// Remove this modifier from the combatant after all that.
			self.RemoveModifier(this);

			return dcs;

		}
		#endregion

		#region INSPECTOR BULLSHIT
		private string inspectorString = "IF YOU ARE SEEING THIS MESSAGE: DO NOT USE THIS MODIFIER HERE. Gets added when the Defend option is picked.";
		protected override string InspectorDescription {
			get {
				return this.inspectorString;
			}
		}

		
		#endregion

	}


}