using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;
using System.Linq;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// Attack does double damage, but costs triple.
	/// Guard command is disabled.
	/// </summary>
	public class HornyAffliction : CombatantModifier, ICombatantAffliction, IInterceptIncomingDCS, IOnTurnReady, IInterceptBattleBehaviorCost, ITopLevelSelectionRestrictor  {

		#region FIELDS - TIMER
		/// <summary>
		/// The timer that says how many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Horny;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMODIFYBATTLEBEHAVIORCOST
		/// <summary>
		/// Modifies the cost of a battle behavior.
		/// </summary>
		/// <param name="runningCost">The *current* value of the behavior cost. This is important when I need to chain modifiers.</param>
		/// <param name="self">The combatant who owns this behavior.</param>
		/// <param name="behavior">The behavior being probed for its modified cost.</param>
		/// <returns>The new cost for the behavior.</returns>
		public int InterceptBattleBehaviorCost(int runningCost, Combatant self, BattleBehavior behavior) {

			// For this version of the modifier, just return the original cost if it's an item.
			if (behavior.behaviorType == BehaviorType.Item) {
				return runningCost;
			} else {
				return runningCost * 3;
			}

		}
		#endregion

		#region INTERFACE IMPLEMENTATION
		/// <summary>
		/// Doubles attack.
		/// </summary>
		/// <param name="dcs"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {

			dcs.damageCalculations
				.Where(dc => dc.source == self)                                     // Go through all the calculations where this combatant is a source
				.Where(dc => dcs.BattleBehavior.behaviorType == BehaviorType.Special)
				// .Where(dc => this.elementTypes.Contains(dc.behavior.elementType))   // Check if the element being used is one that can be amplified.
				.ToList()
				.ForEach(dc => dc.rawDamageAmount *= 2);

			/*// If the matches count is greater than zero, that means this amplifier has been used and should be removed upon completion.
			if (matches.Count > 0) {
				// Go through each calculation.
				matches.ForEach(dc => dc.rawDamageAmount = (int)((float)dc.rawDamageAmount * this.multiplier));
				// Remove the reference from the combatant.
				self.RemoveModifier(this);
			}*/

			return dcs;
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

					this.combatantOwner.CombatantAnimator.AnimateFocusHighlight(
						combatant: this.combatantOwner,
						time: null);
					BattleNotifier.DisplayNotifier("Affliction reverted!");
					combatantOwner.CureAffliction(animateCure: true);

					GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
						battleReactionSequence.ExecuteNextReaction();
					});

				} else {
					battleReactionSequence.ExecuteNextReaction();
				}

			};

		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ITOPLEVELSELECTIONRESTRICTOR
		/// <summary>
		/// Returns the list of top level selections that are allowed.
		/// </summary>
		/// <returns></returns>
		public List<BattleMenuDXTopLevelSelectionType> RestrictTopLevelSelections() {
			return new List<BattleMenuDXTopLevelSelectionType>() {
				BattleMenuDXTopLevelSelectionType.Analysis,
				BattleMenuDXTopLevelSelectionType.Attack,
				BattleMenuDXTopLevelSelectionType.BatonTouch,
				BattleMenuDXTopLevelSelectionType.Escape,
				// BattleMenuDXTopLevelSelectionType.Guard,
				BattleMenuDXTopLevelSelectionType.Item,
				BattleMenuDXTopLevelSelectionType.Masque,
				BattleMenuDXTopLevelSelectionType.Skill,
				BattleMenuDXTopLevelSelectionType.Escape,
			};
		}
		#endregion
		
		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Attack does double damage, but costs triple. Guard is disabled.";
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