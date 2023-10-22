using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.Modifiers;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// Applies a number of modifiers to the targets. They are cloned as to not affect the BattleBehavior.
	/// </summary>
	[System.Serializable]
	public class AddModifiersToTargets : StandardBattleBehaviorFunction {

		#region FIELDS - MODIFIERS
		/// <summary>
		/// The combatant modifiers that will be cloned and applied to the combatant.
		/// </summary>
		[OdinSerialize]
		private List<CombatantModifier> combatantModifiers = new List<CombatantModifier>();
		#endregion

		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			Debug.LogWarning("Resource Drain needs to be animated properly. See if it's worthwhile to change how animations work.");
			// Initialize the new damage calculation set from the base.
			DamageCalculationSet damageCalculationSet = base.GenerateDamageCalculationSet(source: source, targets: targets, self: self);
			damageCalculationSet = damageCalculationSet.CalculatePass(this.ApplyModifiersPass);
			// Return it.
			return damageCalculationSet;
		}

		/// <summary>
		/// Just applies the modifiers.
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		private DamageCalculationSet ApplyModifiersPass(DamageCalculationSet dcs) {
			try {
				dcs.Targets								 // Go through each target
				.ForEach(c => this.combatantModifiers						// Go through each modifier,              
					.ForEach(m => c.AddModifier(modifier: m.Clone())));     // and add it as a clone.
			} catch (System.Exception e) {
				Debug.LogError("Couldn't run ApplyModifiersPass for " + dcs.BattleBehavior + "! Returning DCS as is.");
			}
			return dcs;
		}

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Applies a number of modifiers to the targets. They are cloned as to not affect the BattleBehavior.";
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