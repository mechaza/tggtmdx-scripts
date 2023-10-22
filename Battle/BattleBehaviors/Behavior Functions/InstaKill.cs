using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// Bypasses a lot of passes and just kills or doesn't kill an enemy.
	/// </summary>
	[System.Serializable]
	public class InstaKill : StandardBattleBehaviorFunction {

		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			
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
			
			return dcs;
		}

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Bypasses a lot of passes and just kills or doesn't kill an enemy. Crits basically amplify accuracy.";
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