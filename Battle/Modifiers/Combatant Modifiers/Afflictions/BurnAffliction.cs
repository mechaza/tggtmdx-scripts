using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// The "Null" affliction that is present at almost all times.
	/// </summary>
	public class BurnAffliction : CombatantModifier, ICombatantAffliction, IOnBehaviorEvaluated, IOnTurnReady {

		#region FIELDS - TIMER
		/// <summary>
		/// The timer that says how many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Burn;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does not work anymore!");
			return delegate (BattleReactionSequence battleReactionSequence) {
				// First off, create a sequence.
				Sequence seq = DOTween.Sequence();

				// Decrement the timer at the start of the turn.
				this.timer -= 1;
				// If it finally hit zero, remove the affliction.
				if (timer == 0) {
					seq.AppendCallback(new TweenCallback(delegate {
						this.combatantOwner.CombatantAnimator.AnimateFocusHighlight(
							combatant: this.combatantOwner,
							time: null);
						BattleNotifier.DisplayNotifier("Affliction reverted!");
						combatantOwner.CureAffliction(animateCure: true);
					}));
					seq.AppendInterval(interval: 3f);
				}

				seq.OnComplete(new TweenCallback(delegate { battleReactionSequence.ExecuteNextReaction(); }));
				seq.Play();
			};
			
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONBEHAVIOREVALUATED
		public BattleReaction OnBehaviorEvaluated() {
			
			return delegate (BattleReactionSequence battleReactionSequence) {

				// Determine how much damage to give out.
				int damageAmount = (int)((int)combatantOwner.MaxHP * 0.2f);

				// Animate the damage amount before actually overriding it.
				this.combatantOwner.CombatantAnimator.SimpleAnimateHarmfulCalculation(
					combatant: this.combatantOwner,
					damageAmount: damageAmount,
					resourceType: BehaviorCostType.HP);

				// Now go ahead and deduct it.
				this.combatantOwner.SimpleEvaluateDamage(
						damageAmount: damageAmount,
						resourceType: BehaviorCostType.HP);

				// Display the battle notifier.
				BattleNotifier.DisplayNotifier(combatantOwner.metaData.name + " took burn damage!", 3f);

				// Wait three seconds, then run the next reaction.
				GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
					battleReactionSequence.ExecuteNextReaction();
				});

				
			};
			
		}
		#endregion

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


