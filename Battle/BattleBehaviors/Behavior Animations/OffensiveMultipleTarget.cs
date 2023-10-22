using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Grawly.Battle.BattleMenu;
using Grawly.UI;
namespace Grawly.Battle.BehaviorAnimation {

	/// <summary>
	/// This is what should get used for behaviors that have offensive properties but target multiple combatants.
	/// </summary>
	[System.Serializable]
	public class OffensiveMultipleTarget : BattleBehaviorAnimation {

		#region BATTLEBEHAVIORANIMATION IMPLEMENATION
		public override IEnumerator ExecuteAnimation(DamageCalculationSet damageCalculationSet) {
			if (damageCalculationSet.PrimarySource is Player) {
				yield return this.PlayerSourceAnimationRoutine(damageCalculationSet: damageCalculationSet);
			} else if (damageCalculationSet.PrimarySource is Enemy) {
				yield return this.EnemySourceAnimationRoutine(damageCalculationSet: damageCalculationSet);
			} else {
				Debug.LogError("Couldn't determine what routine to use!");
			}
		}
		protected override IEnumerator EnemySourceAnimationRoutine(DamageCalculationSet damageCalculationSet) {
			// Grab the behavior.
			BattleBehavior behavior = damageCalculationSet.BattleBehavior;

			// Zoom in on the enemy.
			BattleCameraController.Instance.ActivateVirtualCamera(((Enemy)damageCalculationSet.Sources[0]).WorldEnemyDX.ZoomInCamera);

			// Tell the enemy to highlight.
			damageCalculationSet.PrimarySource.CombatantAnimator.AnimateFocusHighlight(combatant: damageCalculationSet.PrimarySource);

			// Animate the enemy's attack.
			damageCalculationSet.PrimarySource.CombatantAnimator.AnimateBehaviorUse(damageCalculationSet: damageCalculationSet);

			// Wait one second.
			yield return new WaitForSeconds(1.0f);

			// Go through each calculation.
			foreach (DamageCalculation damageCalculation in damageCalculationSet.damageCalculations) {

				// If the target was hit and the move has a message to display, do so.
				if (damageCalculation.TargetWasHit == true && damageCalculation.behavior.showNotifierMessageOnContact == true) {
					BattleNotifier.DisplayNotifier(
						text: damageCalculation.behavior.notifierMessageToShowOnContact,
						time: 2f,
						messageType: damageCalculation.behavior.notifierTypeToShowOnContact);
				}

				if (damageCalculation.TargetWasAfflicted == true && damageCalculation.behavior.showNotifierMessageOnAffliction == true) {
					BattleNotifier.DisplayNotifier(
						text: damageCalculation.behavior.notifierMessageToShowOnAffliction,
						time: 2f,
						messageType: damageCalculation.behavior.notifierTypeToShowOnAffliction);
				}

				// If the target was boosted in some way, pulse them.
				if (damageCalculation.TargetWasBuffedOrDebuffed == true) {
					damageCalculation.FinalTarget.CombatantAnimator.AnimateStatusBoost(damageCalculation);
					// damageCalculation.FinalTarget.CombatantAnimator.AnimatePulseColor(damageCalculation: damageCalculation);
				}

				// Pass the calculation over to the final target to animate it.
				damageCalculation.FinalTarget.CombatantAnimator.AnimateHarmfulCalculation(damageCalculation: damageCalculation);

				// If the calculation was reflected, tell the original target to play its Reflection animation.
				if (damageCalculation.WasReflected == true) {
					damageCalculation.OriginalTarget.CombatantAnimator.AnimateReflection(damageCalculation: damageCalculation);
				}

				yield return new WaitForSeconds(0.5f);
			}

			yield return new WaitForSeconds(1.5f);

			// Tell the enemy to dehighlight.
			damageCalculationSet.PrimarySource.CombatantAnimator.AnimateFocusDehighlight(combatant: damageCalculationSet.PrimarySource);

		}
		protected override IEnumerator PlayerSourceAnimationRoutine(DamageCalculationSet damageCalculationSet) {
			// Grab the behavior.
			BattleBehavior behavior = damageCalculationSet.BattleBehavior;

			// Animate the player's status.
			damageCalculationSet.PrimarySource.CombatantAnimator.AnimateBehaviorUse(damageCalculationSet: damageCalculationSet);

			// Wait for that animation to complete.
			yield return new WaitForSeconds(1f);

			// If any of the targets are enemies, activate the head on camera.
			if (damageCalculationSet.Targets.Count(c => c is Enemy) > 0) {
				BattleCameraController.Instance.ActivateVirtualCamera(BattleCameraController.BattleCameraType.HeadOnCamera);
			}

			// Wait for the camera to zoom in.
			yield return new WaitForSeconds(0.2f);

			// Go through each of the calculations made and create the effects from them.
			foreach (DamageCalculation damageCalculation in damageCalculationSet.damageCalculations) {

				// If the target was hit and the move has a message to display, do so.
				if (damageCalculation.TargetWasHit == true && damageCalculation.behavior.showNotifierMessageOnContact == true) {
					BattleNotifier.DisplayNotifier(
						text: damageCalculation.behavior.notifierMessageToShowOnContact,
						time: 2f,
						messageType: damageCalculation.behavior.notifierTypeToShowOnContact);
				}

				if (damageCalculation.TargetWasAfflicted == true && damageCalculation.behavior.showNotifierMessageOnAffliction == true) {
					BattleNotifier.DisplayNotifier(
						text: damageCalculation.behavior.notifierMessageToShowOnAffliction,
						time: 2f,
						messageType: damageCalculation.behavior.notifierTypeToShowOnAffliction);
				}

				// If the target was boosted in some way, pulse them.
				if (damageCalculation.TargetWasBuffedOrDebuffed == true) {
					damageCalculation.FinalTarget.CombatantAnimator.AnimatePulseColor(damageCalculation: damageCalculation);
				}

				// Pass the calculation over to the final target to animate it.
				damageCalculation.FinalTarget.CombatantAnimator.AnimateHarmfulCalculation(damageCalculation: damageCalculation);

				// If the calculation was reflected, tell the original target to play its Reflection animation.
				if (damageCalculation.WasReflected == true) {
					damageCalculation.OriginalTarget.CombatantAnimator.AnimateReflection(damageCalculation: damageCalculation);
				}

				// Wait a moment.
				yield return new WaitForSeconds(.2f);

			}

			yield return new WaitForSeconds(1.5f);
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "This is what should get used for behaviors that have offensive properties but target multiple combatants.";
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