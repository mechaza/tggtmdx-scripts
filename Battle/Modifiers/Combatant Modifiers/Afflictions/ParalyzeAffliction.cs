using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// The "Null" affliction that is present at almost all times.
	/// </summary>
	public class ParalyzeAffliction : CombatantModifier, ICombatantAffliction, IOnTurnReady, ITurnBehaviorOverride, IOnAttacked, IOnPreTurn {

		#region INTERFACE IMPLEMENTATION - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Paralyze;
			}
		}
		/*public bool CanMoveOnReady {
			get {
				return true;
			}
		}*/
		/*public Color Color {
			get {
				return Color.blue;
			}
		}*/
		#endregion

		#region FIELDS - STATE
		/// <summary>
		/// The timer that says how many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
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
		
		#region INTERFACE IMPLEMENTATION
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does not work anymore!");
			return delegate (BattleReactionSequence battleReactionSequence) {
				
				// Decrement the timer at the start of the turn.
				this.timer -= 1;
				
				if (timer == 0) {
					GameController.Instance.RunEndOfFrame(delegate {
						BattleNotifier.DisplayNotifier("Affliction reverted!", 3f);
						combatantOwner.CureAffliction(animateCure: true);
					});
					GameController.Instance.WaitThenRun(timeToWait: 3.1f, action: delegate {
						battleReactionSequence.ExecuteNextReaction(); 
					});
					
					
				} else {
					GameController.Instance.RunEndOfFrame(delegate {
						BattleNotifier.DisplayNotifier(combatantOwner.metaData.name + " is paralyzed!", 3f);
					});
					GameController.Instance.WaitThenRun(timeToWait: 3.1f, action: delegate {
						battleReactionSequence.ExecuteNextReaction(); 
					});
				}
				
				/*// First off, create a sequence.
				Sequence seq = DOTween.Sequence();

				// Decrement the timer at the start of the turn.
				this.timer -= 1;
				// If it finally hit zero, remove the affliction.
				if (timer == 0) {
					seq.AppendCallback(new TweenCallback(delegate {
						BattleNotifier.DisplayNotifier("Affliction reverted!", 3f);
						combatantOwner.CureAffliction(animateCure: true);
					}));
				} else {
					seq.AppendCallback(new TweenCallback(delegate {
						BattleNotifier.DisplayNotifier(combatantOwner.metaData.name + " is paralyzed!", 3f);
					}));
				}
				seq.AppendInterval(interval: 3f);

				seq.OnComplete(new TweenCallback(delegate { battleReactionSequence.ExecuteNextReaction(); }));
				seq.Play();*/
			};
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ITURNBEHAVIOROVERRIDE
		/// <summary>
		/// On overriding the turn, do nothing.
		/// </summary>
		public void ExecuteTurn() {
			// Just send the turn skip event.
			Debug.Log("Skipping turn for " + this.combatantOwner.metaData.name);
			BattleController.Instance.FSM.SendEvent("Skip Turn");
		}
		/// <summary>
		/// Just a fifty fifty chance of not working.
		/// </summary>
		public bool TakesPriority {
			get {
				float randomValue = Random.value;
				Debug.Log("Value: " + randomValue + " (" + this.combatantOwner.metaData.name + ")");
				return randomValue > 0.1f;
				// return Random.value > 0.5f;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONATTACKED
		/// <summary>
		/// When someone else attacks this combatant, they should get paralyzed.
		/// </summary>
		/// <param name="damageCalculationSet"></param>
		public BattleReaction OnAttacked(DamageCalculationSet damageCalculationSet) {
			
			return delegate (BattleReactionSequence battleReactionSequence) {

				// If the element type is not physical, just back up.
				if (damageCalculationSet.BattleBehavior.elementType != ElementType.Phys) {
					battleReactionSequence.ExecuteNextReaction();
					
				} else if (damageCalculationSet.PrimarySource.SetAffliction(afflictionType: AfflictionType.Paralyze, animateAffliction: true) == AfflictionSetResultsType.Success) {
					// Make the source of the attack paralyzed if they aren't already affected by an affliction.
					BattleNotifier.DisplayNotifier("Paralyzed!", 3f, BattleNotifier.BattleNotifierMessageType.Harmful);
					GameController.Instance.WaitThenRun(3f, delegate {
						battleReactionSequence.ExecuteNextReaction();
					});
					
					
				} else {
					battleReactionSequence.ExecuteNextReaction();
				}
				
			};
			
			
			/*
			BattleController.Instance.AddReaction(
					reactionSequenceType: BattleReactionSequenceType.BehaviorEvaluated,
					battleReaction: delegate (BattleReactionSequence battleReactionSequence) {
						
						Sequence seq = DOTween.Sequence();
						seq.AppendCallback(new TweenCallback(delegate {

							// Make the source of the attack paralyzed if they aren't already affected by an affliction.
							if (damageCalculationSet.PrimarySource.SetAffliction(afflictionType: AfflictionType.Paralyze, animateAffliction: true) == AfflictionSetResultsType.Success) {
								BattleNotifier.DisplayNotifier("Paralyzed!", 3f, BattleNotifier.BattleNotifierMessageType.Harmful);
								seq.AppendInterval(interval: 3f);
							}
							
						}));
						
						seq.OnComplete(new TweenCallback(delegate {
							battleReactionSequence.ExecuteNextReaction();
						}));

						seq.Play();

					});*/
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

