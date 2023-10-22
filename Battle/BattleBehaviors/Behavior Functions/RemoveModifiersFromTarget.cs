using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.Modifiers;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// Removes a given type of modifier from a target.
	/// </summary>
	[System.Serializable]
	public class RemoveModifiersFromTarget : StandardBattleBehaviorFunction {

		#region FIELDS - MODIFIERS
		/// <summary>
		/// The combatant modifiers that will be cloned and applied to the combatant.
		/// </summary>
		[OdinSerialize, TypeFilter("AllowableTypesToRemove"), InfoBox(message: "THIS ONLY WORKS FOR SHIELDS RN", InfoMessageType.Warning)]
		private List<System.Type> modifierTypesToRemove = new List<System.Type>();
		// private List<CombatantModifier> combatantModifiers = new List<CombatantModifier>();
		#endregion

		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			// Initialize the new damage calculation set from the base.
			DamageCalculationSet damageCalculationSet = base.GenerateDamageCalculationSet(source: source, targets: targets, self: self);
			damageCalculationSet = damageCalculationSet.CalculatePass(this.RemoveModifiersPass);
			// Return it.
			return damageCalculationSet;
		}

		/// <summary>
		/// Just applies the modifiers.
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		private DamageCalculationSet RemoveModifiersPass(DamageCalculationSet dcs) {
			try {

				dcs.Targets.ForEach(c => c.RemoveModifier(modifierTypeToRemove: typeof(RepellantShield)));

				/*this.modifierTypesToRemove.ForEach(t => {
					
					dcs.Targets.ForEach(c => c.RemoveModifier(modifierTypeToRemove: t));
				});*/

			} catch (System.Exception e) {
				Debug.LogError("Couldn't run RemoveModifiersFromTarget for " + dcs.BattleBehavior + "! Returning DCS as is.");
			}
			return dcs;
		}

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Removes the given types of modifiers from a target. ONLY REMOVES SHIELDS RN SORRY YALL";
		/// <summary>
		/// A string that details how this function works.
		/// </summary>
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		private List<System.Type> AllowableTypesToRemove() {
			return typeof(CombatantModifier)
				.Assembly
				.GetTypes()
				// .Where(t => t.GetType() == typeof(CombatantModifier))
				.Where(t => t.IsSubclassOf(typeof(CombatantModifier)))
				.ToList();
		}
		#endregion

	}



}