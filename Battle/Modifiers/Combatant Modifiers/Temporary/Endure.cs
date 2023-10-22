using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {
	
	/// <summary>
	/// Applied once at the start of a battle if a Combatant has the BattleBehavior Endure.
	/// Lets the combatant survive a fatal move with 1HP. Gets removed when it goes into effect, so it only works once per battle.
	/// </summary>
	[System.Serializable]
	public class Endure : CombatantModifier, IInterceptIncomingDCSLate {

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		public DamageCalculationSet InterceptIncomingDCSLate(DamageCalculationSet dcs, Combatant self) {

			// If this gets set to true, this modifier will be removed from its owner combatant.
			bool activated = false;

			dcs.damageCalculations
				.Where(dc => dc.FinalTarget == self)
				.Where(dc => dc.TargetWillDie)
				.ToList()
				.ForEach(dc => {
					// Set the amount of damage so the target would have one HP remaining.
					dc.rawDamageAmount = dc.FinalTarget.HP - 1;

					Debug.Log(self.metaData.name + " endured the hit!");

					// Mark that this combatant should have its endure modifier removed.
					activated = true;

					BattleNotifier.DisplayNotifier(self.metaData.name + " endured the hit!", 3f);

				});

			if (activated == true) {
				self.RemoveModifier(modifier: this);
			}

			return dcs;
		}
		#endregion

		#region INSPECTOR STUFF
		private static string inspectorDescription = "Lets the combatant survive a fatal move with 1HP. Gets removed when it goes into effect. Calculation ocurrs after all other passes are made.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}
		#endregion

	}


}