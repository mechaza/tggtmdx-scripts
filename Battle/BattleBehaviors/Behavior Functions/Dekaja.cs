using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// Basically just debuffs an enemy from any boosts they might have.
	/// </summary>
	[SerializeField]
	public class Dekaja : StandardBattleBehaviorFunction {

		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			Debug.LogWarning("Dekaja needs to be animated properly. See if it's worthwhile to change how animations work.");
			// Initialize the new damage calculation set from the base.
			DamageCalculationSet damageCalculationSet = base.GenerateDamageCalculationSet(source: source, targets: targets, self: self);
			damageCalculationSet = damageCalculationSet.CalculatePass(this.DekajaPass);
			// Return it.
			return damageCalculationSet;
		}

		/// <summary>
		/// Just applies the modifiers.
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		private DamageCalculationSet DekajaPass(DamageCalculationSet dcs) {

			// Go through each calculation where the final target has active buffs...
			foreach (DamageCalculation dc in dcs.damageCalculations.Where(dc => dc.FinalTarget.statusModifiers.ActivePowerBuffs.Count > 0)) {
				dc.powerBoosts.AddRange(                                                    // Add a new range of debuffs to the power boosts by...
					dc.FinalTarget.statusModifiers.ActivePowerBuffs.Select(					// ... transforming those active buffs... 
						bt => new KeyValuePair<PowerBoostType, PowerBoostIntentionType>(	// ... Creating a new KVP from them...
							key: bt,														// ... and debuffing.
							value: PowerBoostIntentionType.Debuff)));
			}
			
			return dcs;
		}

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Removes status boosts from an enemy. This does not necessarily give them negative buffs; it just removes positive ones.";
		/// <summary>
		/// A string that details how this function works.
		/// </summary>
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		#endregion


	}


}