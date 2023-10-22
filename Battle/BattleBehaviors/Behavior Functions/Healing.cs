using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using Grawly.Battle.Modifiers;

namespace Grawly.Battle.Functions.Standard {

	/// <summary>
	/// Heals an ally.
	/// </summary>
	[System.Serializable]
	public class Healing : StandardBattleBehaviorFunction {

		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			Debug.LogWarning("HEALING MOVES CURRENTLY DO NOT TAKE INTO ACCOUNT INTERCEPTIONS. THIS IS NOT GOOD BUT ALSO I NEED TO GET THIS OUT OF THE WAY.");
			
			// Initialize the new damage calculation set.
			DamageCalculationSet damageCalculationSet = new DamageCalculationSet(source: source, targets: targets, battleBehavior: self);
			damageCalculationSet = base.StandardCalculationPass(damageCalculationSet);
			damageCalculationSet = damageCalculationSet.CalculatePass(this.CollapseCalculationPass);
			// Return it.
			return damageCalculationSet;
		}

		/// <summary>
		/// This should be run after interception and is used to "collapse" the amount that will be used in the final calculation.
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		protected override DamageCalculationSet CollapseCalculationPass(DamageCalculationSet dcs) {

			// The idea here is that some interceptions may be able to change the "accuracy" or final amount of the calculation.
			// When this pass is reached, a final calculation will be performed.

			dcs.damageCalculations.ForEach(dc => {
				dc.accuracyType = AccuracyType.Normal;
			});

			return dcs;
		}


		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Heals an ally. Sort of specialized in comparison to the standard calculation in that I don't particularly enjoy having accuracy play a big role.";
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