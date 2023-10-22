using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;
using System.Linq;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// One of 3 actions each turn: give money to enemy, use recovery item, throw away item.
	/// </summary>
	public class ConfuseAffliction : CombatantModifier, IOnTurnReady, ITurnBehaviorOverride, ICombatantAffliction {

		#region FIELDS - STATE
		/// <summary>
		/// How many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Confuse;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		/// <summary>
		/// On turn ready, I should tell the notifier that the combatant is confused.
		/// </summary>
		/// <returns></returns>
		public BattleReaction OnTurnReady() {
			return delegate (BattleReactionSequence battleReactionSequence) {
				
				// Decrement the timer at the start of the turn.
				this.timer -= 1;

				// Highlight the combatant but make it stop just before executing the next reaction.
				this.combatantOwner.CombatantAnimator.AnimateFocusHighlight(combatant: this.combatantOwner, time: 2.95f);

				// If it finally hit zero, remove the affliction.
				if (timer == 0) {
					BattleNotifier.DisplayNotifier("Affliction reverted!");
					combatantOwner.CureAffliction(animateCure: true);
				} else {
					BattleNotifier.DisplayNotifier(this.combatantOwner.metaData.name + " is confused!");
				}

				GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
					battleReactionSequence.ExecuteNextReaction();
				});

			};
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ITURNBEHAVIOR
		/// <summary>
		/// For confuse, this should always return true because the combatant will always pick out the same moves.
		/// </summary>
		public bool TakesPriority {
			get {
				return true;
			}
		}
		/// <summary>
		/// Just randomly picks out a behavior and runs it.
		/// </summary>
		public void ExecuteTurn() {
			Debug.LogWarning("Confuse affliction still needs to be implemented properly.");
			// I still don't exactly know what I wanna do here.
			if (this.combatantOwner is Player) {
				// If the player has any healing items, use a random one.
				if (GameController.Instance.Variables.Items.Keys.Count(i => i.elementType == ElementType.Healing) > 0) {
					BattleController.Instance.PrepareBehaviorEvaluation(
						source: this.combatantOwner,
						targets: new List<Combatant>() { BattleController.Instance.AllAliveCombatants[Random.Range(minInclusive: 0, maxExclusive: BattleController.Instance.AllAliveCombatants.Count - 1)] },
						behavior: GameController.Instance.Variables.Items.Keys.Where(i => i.elementType == ElementType.Healing).First());
				} else {
					// If I have no healing items, just skip.
					GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
						BattleController.Instance.FSM.SendEvent("Skip Turn");
					});
				}

			} else if (this.combatantOwner is Enemy) {
				// Debug.Log("Idk what else to do here so I'm just setting it up to throw money at you");
				// int moneyThrownAway = Random.Range(min: 1000, max: 5000);
				int moneyThrownAway = Random.Range(minInclusive: 1000, maxExclusive: 5000);
				GameController.Instance.Variables.Money += moneyThrownAway;
				BattleNotifier.DisplayNotifier(text: this.combatantOwner.metaData.name + " threw away " + moneyThrownAway + " dollars!", time: 3f);
				GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
					BattleController.Instance.FSM.SendEvent("Skip Turn");
				});
				// throw new System.NotImplementedException("do some research on what the confuse affliction is supposed to do.");
			}

		}
		#endregion

		#region INSPECTOR BULLSHIT
		private string inspectorString = "One of 3 actions each turn: give money to enemy, use recovery item, throw away item.";
		protected override string InspectorDescription {
			get {
				return this.inspectorString;
			}
		}
		#endregion

	}


}