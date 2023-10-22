using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using Grawly.Battle.Modifiers;

namespace Grawly.Battle.Functions {
	
	/// <summary>
	/// Downs an enemy.
	/// </summary>
	[System.Serializable]
	public class Knockout : StandardBattleBehaviorFunction {

		#region OVERRIDES
		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			// Initialize the new damage calculation set from the base.
			DamageCalculationSet damageCalculationSet = base.GenerateDamageCalculationSet(source: source, targets: targets, self: self);
			damageCalculationSet = damageCalculationSet.CalculatePass(this.KnockoutPass);
			// Return it.
			return damageCalculationSet;
		}
		protected override string InspectorDescription {
			get {
				return "Downs an enemy while dealing only 1HP.";
			}
		}
		#endregion

		#region SPECIFIC CALLS
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		private DamageCalculationSet KnockoutPass(DamageCalculationSet dcs) {
			
			// Go through each calculation where the final target has active buffs...
			foreach (DamageCalculation dc in dcs.damageCalculations) {
				dc.rawDamageAmount = 1;
				dc.finalResistance = ResistanceType.Wk;
			}
			
			/*// Go through each calculation where the final target has active buffs...
			foreach (DamageCalculation dc in dcs.damageCalculations.Where(dc => dc.FinalTarget.statusModifiers.ActivePowerBuffs.Count > 0)) {
				dc.powerBoosts.AddRange(                                                    // Add a new range of debuffs to the power boosts by...
					dc.FinalTarget.statusModifiers.ActivePowerBuffs.Select(					// ... transforming those active buffs... 
						bt => new KeyValuePair<PowerBoostType, PowerBoostIntentionType>(	// ... Creating a new KVP from them...
							key: bt,														// ... and debuffing.
							value: PowerBoostIntentionType.Debuff)));
			}*/
			
			return dcs;
		}
		#endregion
		
	}
}