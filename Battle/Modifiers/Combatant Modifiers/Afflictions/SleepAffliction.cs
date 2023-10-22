using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// The "Null" affliction that is present at almost all times.
	/// </summary>
	public class SleepAffliction : CombatantModifier, ICombatantAffliction, IOnAttacked, IOnTurnReady, ITurnBehaviorOverride, IOnPreTurn {

		#region FIELDS - TIMER
		/// <summary>
		/// The timer that says how many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Sleep;
			}
		}
		#endregion

		#region IONATTACKED EXTRA GARBAGE 
		private bool readyToTrigger = false;
		public bool ReadyToTrigger {
			get {
				return this.readyToTrigger;
			}
		}
		public BattleReaction OnPreTurn() {
			// This is needed so that paralyzation doesn't activate the same turn it was sent.
			return delegate(BattleReactionSequence battleReactionSequence) {
				this.readyToTrigger = true;
				battleReactionSequence.ExecuteNextReaction();
			};
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IONATTACKED
		/// <summary>
		/// Might wake up the combatant if they were attacked.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet involved with this compuation.</param>
		public BattleReaction OnAttacked(DamageCalculationSet damageCalculationSet) {

			return delegate (BattleReactionSequence battleReactionSequence) {
				throw new System.NotImplementedException("add this");
				battleReactionSequence.ExecuteNextReaction();
			};
			
			BattleController.Instance.AddReaction(
					reactionSequenceType: BattleReactionSequenceType.BehaviorEvaluated,
					battleReaction: delegate (BattleReactionSequence battleReactionSequence) {

						Sequence seq = DOTween.Sequence();
						seq.AppendCallback(new TweenCallback(delegate {
							// Only wake up the combatant if the amount is greater than zero (i.e., they were ..hit with something)
							BattleNotifier.DisplayNotifier(combatantOwner.metaData.name + " woke up!", 3f);
							combatantOwner.CureAffliction(animateCure: true);
							
						}));
						seq.AppendInterval(interval: 3f);
						seq.OnComplete(new TweenCallback(delegate {
							battleReactionSequence.ExecuteNextReaction();
						}));

						seq.Play();

					});
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does not work anymore!");
			return delegate (BattleReactionSequence battleReactionSequence) {

				// Decrement the timer at the start of the turn.
				this.timer -= 1;
				// If it finally hit zero, remove the affliction.
				if (timer == 0) {
					BattleNotifier.DisplayNotifier("Affliction reverted!", 3f);
					combatantOwner.CureAffliction(animateCure: true);
				} else {
					BattleNotifier.DisplayNotifier(combatantOwner.metaData.name + " is asleep!", 3f);
				}

				GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
					battleReactionSequence.ExecuteNextReaction();
				});

			};
			
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ITURNBEHAVIOROVERRIDE
		/// <summary>
		/// This affliction should always take priority.
		/// </summary>
		public bool TakesPriority {
			get {
				return true;
			}
		}
		/// <summary>
		/// The combatant should restore health on their turn.
		/// </summary>
		public void ExecuteTurn() {
			throw new System.NotImplementedException();
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Cannot move. Restores HP/MP. Wakes up when hit.";
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
