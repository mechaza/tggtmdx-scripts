using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.UI.Legacy;
using System.Linq;
using DG.Tweening;
using Grawly.Battle.WorldEnemies;
using Grawly.Battle.BattleMenu;
using Grawly.Battle.Modifiers;

namespace Grawly.Battle.BehaviorAnimation {

	/// <summary>
	/// The standard way in which offensive BattleBehaviors should be animated. This shows things like the damage being dealt, etc.
	/// </summary>
	[System.Serializable]
	public class OffensiveSingleTarget : BattleBehaviorAnimation {

		#region ROUTINES
		/// <summary>
		/// Runs the animation as normal. Remember that this takes place before I have actually evaluated the damage calculations themselves (e.x., enemies being hit still have their original HP etc)
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet associated with the behavior being evaluated right now.</param>
		/// <returns></returns>
		public override IEnumerator ExecuteAnimation(DamageCalculationSet damageCalculationSet) {

			// This only is relevant for when a target intercepts an animation. I do not use it in many instances.
			bool canContinueAnimation = true;
			// This works under the assumption that there will only be one target who does this.
			if (damageCalculationSet.TargetsInterceptAttackAnimation == true) {
				yield return damageCalculationSet
					.Targets.First()
					.GetModifiers<IInterceptOpponentAttackAnimation>().First()
					.OnInterceptOpponentAttackAnimation(
					dcs: damageCalculationSet);
			}

			// Now that That is Done, continue if you are allowed to continue the animation.
			if (damageCalculationSet.PrimarySource is Player && canContinueAnimation == true) {
				yield return this.PlayerSourceAnimationRoutine(damageCalculationSet: damageCalculationSet);
			} else if (damageCalculationSet.PrimarySource is Enemy && canContinueAnimation == true) {
				yield return this.EnemySourceAnimationRoutine(damageCalculationSet: damageCalculationSet);
			} else {
				Debug.LogError("Couldn't determine what routine to use! canContinueAnimation is set to " + canContinueAnimation + ", if that helps.");
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
				damageCalculation.FinalTarget.CombatantAnimator.AnimateHarmfulCalculation(damageCalculation: damageCalculation);

				// If the calculation was reflected, tell the original target to play its Reflection animation.
				if (damageCalculation.WasReflected == true) {
					damageCalculation.OriginalTarget.CombatantAnimator.AnimateReflection(damageCalculation: damageCalculation);
				}

				// Wait a moment.
				yield return new WaitForSeconds(.2f);

			}

			yield return new WaitForSeconds(1.5f);

			/*//
			//
			// I basically just copy/pasted this from the EffectsController.
			// I'm going to have to fix it in a bit but I just want to make sure it actually works.
			//
			//

			BattleBehavior behavior = damageCalculationSet.BattleBehavior;

			yield return new WaitForSeconds(.1f);

			// If the player made a critical hit, play the animation that accompanies that.
			foreach (DamageCalculation damageCalculation in damageCalculationSet.damageCalculations) {
				if (damageCalculation.accuracyType == AccuracyType.Critical || (damageCalculation.resistance == ResistanceType.Wk && damageCalculation.accuracyType != AccuracyType.Miss)) {
					if (Random.value > .8f) {
						Debug.Log("NOTE: I used to show the player's cut in sprite here.");
						// CutInAnimation.PrepareCutInAnimation(((Player)damageCalculation.source).cutIn);
						AudioController.Instance.PlaySFX(SFXType.PlayerExploit);
						EffectsController.PlayCriticalHitAnimation((Player)damageCalculation.source);
						yield return new WaitForSeconds(1.5f);
						break;
					}
				}
			}

			// Animate the player's status.
			damageCalculationSet.PrimarySource.CombatantAnimator.AnimateBehaviorUse(damageCalculationSet: damageCalculationSet);

			// Wait for that animation to complete.
			yield return new WaitForSeconds(1f);

			// Rush the camera to the enemy. It will change depending on how many enemies are being target.
			if (damageCalculationSet.Targets.Count(t => t is Enemy) > 1) {
				Debug.Log("More than one enemy. Using special Head On getter.");
				BattleCameraController.Instance.ActivateVirtualCamera(BattleCameraController.BattleCameraType.HeadOnCamera);
				// BattleCameraController.Instance.ActivateVirtualCamera(BattleCameraController.Instance.headOnBattleCamera, "Head On Camera");
			} else if (damageCalculationSet.Targets[0] is Enemy) {
				// If only one enemy, use their own Virtaul Camera. Every enemy has one! Wow!
				// may want to refactor this too? hmm
				BattleCameraController.Instance.ActivateVirtualCamera(((Enemy)damageCalculationSet.damageCalculations[0].target).WorldEnemyDX.ZoomInCamera, "Enemy Zoom In Camera");
			}

			// Wait for the camera to zoom in.
			yield return new WaitForSeconds(0.2f);


			// Go through each of the calculations made and create the effects from them.
			foreach (DamageCalculation damageCalculation in damageCalculationSet.damageCalculations) {

				// Determine who the target should be based on whether or not this was a Reflection.
				Combatant finalTarget = (damageCalculation.resistance == ResistanceType.Ref) ? damageCalculation.source : damageCalculation.target;

				// If this move will kill the enemy, use the animation that will kill the enemy instead.
				Debug.LogWarning("You may want to refactor this to be inside one of the targeted methods! It wont work if an enemy reflects an attack anyway!");

				if (finalTarget.HP - damageCalculation.amountToDeduct <= 0) {
					this.PlayDeathBattleFX(target: finalTarget, behavior: behavior);
				} else {
					// Play the prefab associated with this behavior.
					this.PlayBattleFX(target: finalTarget, behavior: behavior);
				}

				// Animate the target.
				this.AnimateTarget(target: finalTarget, damageCalculation: damageCalculation, damageCalculationSet: damageCalculationSet);

				// Wait a moment.
				yield return new WaitForSeconds(.2f);

			}

			yield return new WaitForSeconds(1.5f);*/
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
			if (damageCalculation.resistance == ResistanceType.Ref) {
				// Check if the move was reflected.
				this.AnimateTargetPlayerReflection(target: damageCalculation.target as Player, damageCalculation: damageCalculation, damageCalculationSet: damageCalculationSet);
				// damageCalculation = damageCalculation.Reflect();
				// this.AnimateTargetEnemy(target: damageCalculation.target as Enemy, damageCalculation: damageCalculation, damageCalculationSet: damageCalculationSet);
				return;
			}
			Debug.Log("NOTE: I used to call receive attack here");
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

			//GameObject enemyCursor = (GameObject)GameObject.Instantiate(LegacyBattleMenuController.Instance.enemyCursorPrefab);
			//enemyCursor.transform.SetParent(LegacyBattleMenuController.Instance.enemyCursorPositionAnchor.transform);
			//enemyCursor.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
			//enemyCursor.transform.localEulerAngles = Vector3.zero;
			//enemyCursor.transform.position = target.WorldEnemyDX.Behavior.GetCursorPosition(worldEnemyDX: target.WorldEnemyDX);
			//LegacyEnemyCursor ec = enemyCursor.GetComponent<LegacyEnemyCursor>();

			//ec.StartCoroutine(ec.PlayCursorFillAnimation(enemy: target, damageTuple: damageCalculation));
		
			// Build the cursor from the damage calculation.
			EnemyCursorDXController.Instance.BuildEnemyCursorFromDamageCalculation(worldEnemyDX: target.WorldEnemyDX, damageCalculation: damageCalculation);

			if (damageCalculation.amountToDeduct > 0) {
				// Shake the enemy if the amount of damage was higer than zero (i.e, they were attacked.)
				// This is VERY risky though I'm explicitly disabling the brain for a quick second when starting to shake.
				// The shake effect doesn't work very well otherwise. (given how the virtual camera is a child of the thing.. being shaken)
				BattleCameraController.Instance.MainCamera.GetComponent<Cinemachine.CinemachineBrain>().enabled = false;
				target.WorldEnemyDX.transform.DOShakePosition(duration: 0.2f, strength: 0.2f, vibrato: 50, randomness: 90, snapping: false, fadeOut: false).OnComplete(delegate {
					BattleCameraController.Instance.MainCamera.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
				});
			}
		}
		#endregion

		#region REFLECTIONS
		/// <summary>
		/// Animates the player that was previously being targeted by an enemy so that they show a Reflection.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="damageCalculation"></param>
		/// <param name="damageCalculationSet"></param>
		private void AnimateTargetPlayerReflection(Player target, DamageCalculation damageCalculation, DamageCalculationSet damageCalculationSet) {
			Debug.LogError("Make sure to add reflections back.");
		}
		/// <summary>
		/// Animates the enemy that was previously being targeted by a player so that they show a Reflection.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="damageCalculation"></param>
		/// <param name="damageCalculationSet"></param>
		private void AnimateTargetEnemyReflection(Enemy target, DamageCalculation damageCalculation, DamageCalculationSet damageCalculationSet) {
			Debug.LogError("Make sure to add reflections back.");
		}
		#endregion*/

		/*#region GENERAL ANIMATIONS
		/// <summary>
		/// Instansiates a BattleFX for an attack.
		/// </summary>
		private void PlayBattleFX(Combatant target, BattleBehavior behavior) {

			// Grab the resource from the behavior's definition.
			// GameObject obj = behavior.battleVFX;
			GameObject obj = this.visualEffect;

			// Make sure it is not null. It would be null if it was never prepared to begin with.
			// If it wasn't found, just instantiate the Critical Hit BFX.
			if (obj != null) {
				obj = GameObject.Instantiate<GameObject>(obj);
			} else {
				Debug.LogWarning("BattleFX was not found! Using default battle fx.");
				obj = GameObject.Instantiate<GameObject>(DataController.Instance.GetBFX(BFXType.DefaultAttack));
			}

			// Play the sound effect
			// AudioClip sfx = behavior.battleSFX;
			AudioClip sfx = this.audioEffect;

			if (sfx != null) {
				AudioController.Instance.PlaySFX(this.audioEffect);
			} else {
				Debug.LogWarning("SFX was not found! Using default SFX.");
				AudioController.Instance.PlaySFX(SFXType.DefaultAttack);
			}



			// Adjust its position.
			if (target.GetType().Name == "Enemy") {
				// Get the world enemy's position, and then move back a little bit so that the effect
				// can be placed in the correct spot.
				WorldEnemyDX we = ((Enemy)target).WorldEnemyDX;
				obj.transform.position = we.Behavior.GetDefaultParticleEffectSpawnPosition(worldEnemyDX: we) + (we.transform.forward * -1.5f);
			} else {
				// Debug.LogWarning("HAVENT FIXED THIS FOR A PLAYER YET");
				// If the target is a Player, parent the object to one of the PlayerStatusEffects (which uses an anchor)
				Debug.Log("NOTE: I used to put a battle FX on the player status here.");
				// ((Player)target).playerStatus.PlaceBattleFX(obj);
			}
		}
		/// <summary>
		/// Spawns a Death prefab when its seen that this will kill a target.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="behavior"></param>
		private void PlayDeathBattleFX(Combatant target, BattleBehavior behavior) {
			// If the target is a player, used the P-version of the BattleFX
			string suffix = target.GetType().Name == "Player" ? "P" : "";

			GameObject obj = GameObject.Instantiate(DataController.Instance.GetBFX(BFXType.EnemyDeath));

			// Play the sound effect
			Debug.LogWarning("Change this to play the death sound");
			AudioController.Instance.PlaySFX(SFXType.DefaultAttack);
			// Adjust its position.
			if (target.GetType().Name == "Enemy") {
				// Get the world enemy's position, and then move back a little bit so that the effect
				// can be placed in the correct spot.
				WorldEnemyDX we = ((Enemy)target).WorldEnemyDX;
				obj.transform.position = we.Behavior.GetDefaultParticleEffectSpawnPosition(worldEnemyDX: we) + (we.transform.forward * -1.5f);
				// we.gameObject.SetActive(false);
				Debug.Log("NOTE: I used to call 'set visuals active' here");
				// we.SetVisualsActive(false);
			} else {
				// Debug.LogWarning("HAVENT FIXED THIS FOR A PLAYER YET");
				// If the target is a Player, parent the object to one of the PlayerStatusEffects (which uses an anchor)
				Debug.Log("I used to put a battle FX on the player status here.");
				// ((Player)target).playerStatus.PlaceBattleFX(obj);
			}
		}
		#endregion
*/

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "The standard way in which offensive BattleBehaviors should be animated. This shows things like the damage being dealt, etc.";
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