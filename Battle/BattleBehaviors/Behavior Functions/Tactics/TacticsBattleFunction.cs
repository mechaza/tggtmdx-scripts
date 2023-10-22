using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Battle.Functions {

	public class TacticsBattleFunction : StandardBattleBehaviorFunction {

		/*/// <summary>
		/// Execute the function and perform the necessary calculations.
		/// </summary>
		/// <param name="source">Who the move is originating from.</param>
		/// <param name="targets">A list of targets that are being affected by the move.</param>
		/// <param name="battleBehavior">The BattleBehavior this function is attached to.</param>
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			BattleController.Instance.StartCoroutine(this.AllOutAttackRoutine(source: source, targets: targets, battleBehavior: battleBehavior));
		}*/

		#region CALCULATIONS
		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			DamageCalculationSet dcs = base.GenerateDamageCalculationSet(source, targets, self);
			return dcs;
		}
		#endregion

		/// <summary>
		/// Just applies the modifiers.
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		private DamageCalculationSet TacticsCostPass(DamageCalculationSet dcs) {
			BattleController.Instance.Players
				.Where(p => dcs.BattleBehavior.participants.Contains(p.participantType))		// Find the players that are participants in this tactics behavior.
				.Where(p => p != dcs.Sources[0])												// Single out the one who initiated it because theyre going to get cost deducted later (BAD DESIGN!)
				.ToList()					
				.ForEach(p => p.DeductBehaviorCost(behavior: dcs.BattleBehavior));				// For everyone else, deduct the cost.
			return dcs;
		}

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Basic way of calculating tactics for right now.";
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