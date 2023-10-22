using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Sirenix.OdinInspector;
using System.Linq;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Grawly.Battle.Functions {

	/// <summary>
	/// The actual implementation of how a BattleBehavior should operate.
	/// </summary>
	public abstract class BattleBehaviorFunction {

		#region FUNCTION
		/// <summary>
		/// Execute the function and perform the necessary calculations.
		/// </summary>
		/// <param name="source">Who the move is originating from.</param>
		/// <param name="targets">A list of targets that are being affected by the move.</param>
		/// <param name="battleBehavior">The BattleBehavior this function is attached to.</param>
		public abstract void Execute(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior);
		#endregion

		#region BEHAVIOR EVALUATION UTILITIES
		/// <summary>
		/// Checks if the target is weak to the move it is being attacked with. If it is, tell the battle controller to enter OneMore.
		/// Legacy.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that may have downed the target.</param>
		/// <param name="self"></param>
		protected static void CheckForOneMore(DamageCalculation damageCalculation, BattleBehavior self) {
			// If the attack missed or the target is already down, just return.
			if (damageCalculation.accuracyType == AccuracyType.Miss || damageCalculation.FinalTarget.IsDown == true) {
				return;
				// Probe the DamageCalculation for if the hit was critical or exploited a weakness.
			} else if (damageCalculation.TargetWillBeDowned == true) {
				// Set the target down
				damageCalculation.FinalTarget.SetDownedStatus(isDown: true);
				BattleController.Instance.EnableOneMore();
			}
		}
		/// <summary>
		/// Call this when the behavior is done evaluating.
		/// </summary>
		protected static void BackToBattleController() {
			BattleController.Instance.BehaviorEvaluatedEvent();
		}
		#endregion

		#region DAMAGE TUPLE CALCULATIONS
		/// <summary>
		/// General function to generate the DamageCalculationSet based on the function type (or, for that matter, anything else if I choose to change it.)
		/// </summary>
		/// <param name="source">The combatant who initiated the move.</param>
		/// <param name="targets">The combatants that are being targeted.</param>
		/// <param name="self">The behavior that owns this function.</param>
		/// <returns>The DamageCalculationSet which contains the different calculations that need to be animated and evaluated.</returns>
		protected abstract DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self);
		#endregion

		#region INSPECTOR JUNK
#if UNITY_EDITOR
		/// <summary>
		/// This is what I need to use for making sure info boxes appear in the inspector without actually having to assign a field to accompany it.
		/// </summary>
		[PropertyOrder(int.MinValue), OnInspectorGUI]
		private void DrawIntroInfoBox() {
			SirenixEditorGUI.InfoMessageBox(this.InspectorDescription);
		}
#endif
		/// <summary>
		/// The string that gets used in the info box that describes this BattleBehaviorFunction.
		/// </summary>
		protected abstract string InspectorDescription { get; }
		#endregion


	}


}
