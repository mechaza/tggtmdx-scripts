using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.UI.Legacy;
using System.Linq;
using DG.Tweening;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.BehaviorAnimation {

	/// <summary>
	/// This is what typically gets used for most healing items.
	/// It is very similar to the offensive behavior, but the timing is a bit different.
	/// </summary>
	[System.Serializable]
	public class RestorationSingleTarget : BattleBehaviorAnimation {

		#region ROUTINES
		/// <summary>
		/// Runs the animation as normal. Remember that this takes place before I have actually evaluated the damage calculations themselves (e.x., enemies being hit still have their original HP etc)
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet associated with the behavior being evaluated right now.</param>
		/// <returns></returns>
		public override IEnumerator ExecuteAnimation(DamageCalculationSet damageCalculationSet) {
			if (damageCalculationSet.PrimarySource is Player) {
				yield return this.PlayerSourceAnimationRoutine(damageCalculationSet: damageCalculationSet);
			} else if (damageCalculationSet.PrimarySource is Enemy) {
				yield return this.EnemySourceAnimationRoutine(damageCalculationSet: damageCalculationSet);
			} else {
				Debug.LogError("Couldn't determine what routine to use!");
			}
		}
		/// <summary>
		/// Gets run if the source of the move was a player.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet associated with the behavior being evaluated right now.</param>
		/// <returns></returns>
		protected override IEnumerator PlayerSourceAnimationRoutine(DamageCalculationSet damageCalculationSet) {
			// Grab the behavior.
			BattleBehavior behavior = damageCalculationSet.BattleBehavior;

			// Animate the player's status.
			damageCalculationSet.PrimarySource.CombatantAnimator.AnimateBehaviorUse(damageCalculationSet: damageCalculationSet);

			// Wait for that animation to complete.
			yield return new WaitForSeconds(1f);


			if (damageCalculationSet.damageCalculations.First().FinalTarget is Enemy) {
				// Activate the virtual camera on the enemy.
				BattleCameraController.Instance.ActivateVirtualCamera(((Enemy)damageCalculationSet.damageCalculations[0].target).WorldEnemyDX.ZoomInCamera);
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
					damageCalculation.FinalTarget.CombatantAnimator.AnimateStatusBoost(damageCalculation);
					// damageCalculation.FinalTarget.CombatantAnimator.AnimatePulseColor(damageCalculation: damageCalculation);
				}

				// Pass the calculation over to the final target to animate it.
				if (damageCalculation.TargetHPWasHealed == true) {
					damageCalculation.FinalTarget.CombatantAnimator.AnimateRestorationCalculation(damageCalculation: damageCalculation);
				}

				// If the calculation was reflected, tell the original target to play its Reflection animation.
				if (damageCalculation.WasReflected == true) {
					damageCalculation.OriginalTarget.CombatantAnimator.AnimateReflection(damageCalculation: damageCalculation);
				}

				// Wait a moment.
				yield return new WaitForSeconds(.2f);

			}

			yield return new WaitForSeconds(1.5f);
		}
		/// <summary>
		/// Gets run if the source of the move was an enemy.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet associated with the behavior being evaluated right now.</param>
		/// <returns></returns>
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
					damageCalculation.FinalTarget.CombatantAnimator.AnimatePulseColor(damageCalculation: damageCalculation);
				}

				// Pass the calculation over to the final target to animate it.
				if (damageCalculation.TargetHPWasHealed == true) {
					damageCalculation.FinalTarget.CombatantAnimator.AnimateRestorationCalculation(damageCalculation: damageCalculation);
				}

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
		#endregion

		/*#region TARGETED ANIMATIONS
		/// <summary>
		/// Animates the effects of a damage calculation being evaluated on a Player.
		/// </summary>
		/// <param name="target">The player receiving the effects of this damage calculation.</param>
		/// <param name="damageCalculation">The damage calculation that is being animated.</param>
		/// <param name="damageCalculationSet">The entire DCS, in case that might be needed for one reason or another.</param>
		/// <returns></returns>
		protected override void AnimateTargetPlayer(Player target, DamageCalculation damageCalculation, DamageCalculationSet damageCalculationSet) {
			throw new System.Exception("FIX THIS PLEASE THANKS");
			// target.playerStatus.ReceiveAttack(damageCalculation);
		}
		/// <summary>
		/// Animates the effects of a damage calculation being evaluated on an Enemy.
		/// </summary>
		/// <param name="target">The enemy receiving the effects of this damage calculation.</param>
		/// <param name="damageCalculation">The damage calculation that is being animated.</param>
		/// <param name="damageCalculationSet">The entire DCS, in case that might be needed for one reason or another.</param>
		/// <returns></returns>
		protected override void AnimateTargetEnemy(Enemy target, DamageCalculation damageCalculation, DamageCalculationSet damageCalculationSet) {
			throw new System.Exception("FIX THIS PLEASE THANKS");

		}
		#endregion
*/
		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = " This is what typically gets used for most healing items. It is very similar to the offensive behavior, but the timing is a bit different.";
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