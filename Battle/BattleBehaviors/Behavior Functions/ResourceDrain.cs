using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// Drains a resource from an enemy.
	/// </summary>
	[System.Serializable]
	public class ResourceDrain : StandardBattleBehaviorFunction {

		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			Debug.LogWarning("Resource Drain needs to be animated properly. See if it's worthwhile to change how animations work.");
			// Initialize the new damage calculation set from the base.
			DamageCalculationSet damageCalculationSet = base.GenerateDamageCalculationSet(source: source, targets: targets, self: self);
			damageCalculationSet = damageCalculationSet.CalculatePass(this.ResourceDrainPass);
			// Return it.
			return damageCalculationSet;
		}

		/// <summary>
		/// Go through the damage calculations and just add the HP. 
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		private DamageCalculationSet ResourceDrainPass(DamageCalculationSet dcs) {
			// Just go through each one and add the amount to the source's HP.
			switch (dcs.BattleBehavior.targetResource) {
				case BehaviorCostType.HP:
					dcs.damageCalculations.ForEach(dc => dc.source.HP += dc.rawDamageAmount);
					break;
				case BehaviorCostType.MP:
					dcs.damageCalculations.ForEach(dc => dc.source.MP += dc.rawDamageAmount);
					break;
				default:
					Debug.LogError("Could not determine which resource to drain! Is the TargetResource in the BattleBehavior set properly?");
					break;
			}
			return dcs;
		}

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Drains a resource from an enemy. Resource that is drained is specified in the targetResource field of the BattleBehavior.";
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