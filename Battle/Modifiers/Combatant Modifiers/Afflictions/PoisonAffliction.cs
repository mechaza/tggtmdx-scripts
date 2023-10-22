using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// The "Null" affliction that is present at almost all times.
	/// </summary>
	public class PoisonAffliction : CombatantModifier, ICombatantAffliction, IOnTurnReady, IOnBehaviorEvaluated {

		#region INTERFACE IMPLEMENTATION - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Poison;
			}
		}
		public bool CanMoveOnReady {
			get {
				throw new System.NotImplementedException();
			}
		}
		public Color Color {
			get {
				throw new System.NotImplementedException();
			}
		}
		#endregion

		#region FIELDS - TIMER
		/// <summary>
		/// The timer that says how many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does not work anymore!");

			return delegate (BattleReactionSequence battleReactionSequence) {
				// First off, create a sequence.
				// Sequence seq = DOTween.Sequence();

				// Decrement the timer at the start of the turn.
				this.timer -= 1;
				// If it finally hit zero, remove the affliction.
				if (timer == 0) {
					// Revert the affliction and give a few seconds to notify the player.
					BattleNotifier.DisplayNotifier("Affliction reverted!", 3f);
					combatantOwner.CureAffliction(animateCure: true);
					// After three seconds, continue.
					GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
						battleReactionSequence.ExecuteNextReaction();
					});
					
				} else {
					// If the poison affliction is still set, just continue.
					battleReactionSequence.ExecuteNextReaction();
				}
				
			};

		}
		#endregion

		/// <summary>
		/// The combatant should take damage if they are still poisoned.
		/// </summary>
		/// <returns></returns>
		public BattleReaction OnBehaviorEvaluated() {
			return delegate (BattleReactionSequence battleReactionSequence) {
				// Build a simple calculation. The damage is 1/5th of the combatant's full HP.
				DamageCalculation simpleDamageCalculation = DamageCalculation.BuildSimpleCalculation(target: this.combatantOwner, amt: (int)(this.combatantOwner.MaxHP / 5f));
				// Animate the damage calculation.
				this.combatantOwner.CombatantAnimator.AnimateHarmfulCalculation(damageCalculation: simpleDamageCalculation);
				// Evaluate it.
				this.combatantOwner.EvaluateDamageCalculation(damageCalculation: simpleDamageCalculation);

				// Wait a few seconds and then Continue.
				GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
					battleReactionSequence.ExecuteNextReaction();
				});
			};
		}

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "No description yet.";
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


