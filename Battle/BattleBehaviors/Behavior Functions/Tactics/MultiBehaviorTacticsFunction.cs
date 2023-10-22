using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Functions {

	public class MultiBehaviorTacticsFunction : StandardBattleBehaviorFunction {

		#region FIELDS - BEHAVIORS
		/// <summary>
		/// A list of behaviors that should have their effects all calculated or whatejver.
		/// </summary>
		[SerializeField]
		private List<BattleBehavior> behaviors = new List<BattleBehavior>();
		#endregion

		#region EXECUTION
		/// Execute the function and perform the necessary calculations.
		/// </summary>
		/// <param name="source">Who the move is originating from.</param>
		/// <param name="targets">A list of targets that are being affected by the move.</param>
		/// <param name="battleBehavior">The BattleBehavior this function is attached to.</param>
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {

			BattleController.Instance.StartCoroutine(AsCoroutine(source, targets, battleBehavior));
		}
		protected IEnumerator AsCoroutine(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			yield return new WaitForEndOfFrame();
			// Generate a new DCS.
			DamageCalculationSet dcs = this.GenerateDamageCalculationSet(source: source, targets: targets, self: battleBehavior);
			// Go through each calculation and evaluate it. (Don't worry about deducing costs; I dont need to for tactics stuff.
			dcs.damageCalculations.ForEach(dc => dc.target.EvaluateDamageCalculation(dc));
			// Enable one more by default.
			BattleController.Instance.EnableOneMore();
			// Back to the battle controller.
			BattleBehaviorFunction.BackToBattleController();
		}
		#endregion

		#region CALCULATIONS
		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			DamageCalculationSet dcs = new DamageCalculationSet(source: source, targets: targets, battleBehavior: self);
			dcs = dcs.CalculatePass(this.GenerateSetsOverBehaviorsPass);
			dcs = dcs.CalculatePass(this.TacticsCostPass);
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
				.Where(p => dcs.BattleBehavior.participants.Contains(p.participantType))        // Find the players that are participants in this tactics behavior.
				.ToList()
				.ForEach(p => p.DeductBehaviorCost(behavior: dcs.BattleBehavior));              // For everyone else, deduct the cost.
			return dcs;
		}
		/// <summary>
		/// Generates calculations based on the different behaviors listed here.
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		private DamageCalculationSet GenerateSetsOverBehaviorsPass(DamageCalculationSet dcs) {
			this.behaviors												
				.ForEach(b => {																// Go through each of the behaviors defined in this class.
					DamageCalculationSet newDCS = base.GenerateDamageCalculationSet(		// Create a new calculation set based on the formula in the base class.
						source: dcs.PrimarySource, 
						targets: dcs.Targets,
						self: b);
					dcs.damageCalculations.AddRange(newDCS.damageCalculations);				// Add the resulting calculations to the new DCS.
				});	
			return dcs;
		}

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "A tactics move that uses multiple behaviors to create an aggregate effect.";
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