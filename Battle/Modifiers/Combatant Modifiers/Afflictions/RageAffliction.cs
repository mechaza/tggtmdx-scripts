using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;
using System.Linq;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// The "Null" affliction that is present at almost all times.
	/// </summary>
	public class RageAffliction : CombatantModifier, ICombatantAffliction, IOnTurnReady, ITurnBehaviorOverride, IInterceptIncomingDCS {

		

		#region INTERFACE IMPLEMENTATION - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Rage;
			}
		}
		public bool CanMoveOnReady {
			get {
				Debug.LogWarning("reminder that rage doesnt have a timer");
				return false;
			}
		}
		public Color Color {
			get {
				return Color.red;
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
				
				// Decrement the timer at the start of the turn.
				this.timer -= 1;

				// If it finally hit zero, remove the affliction.
				if (timer == 0) {
					BattleNotifier.DisplayNotifier("Affliction reverted!", 3f);
					combatantOwner.CureAffliction(animateCure: true);
					GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
						battleReactionSequence.ExecuteNextReaction();
					});
				} else {
					// Otherwise, just keep going.
					BattleNotifier.DisplayNotifier(combatantOwner.metaData.name + " is enraged!", 3f);
					GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
						battleReactionSequence.ExecuteNextReaction();
					});

					
				}
			};
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ITURNBEHAVIOROVERRIDE
		/// <summary>
		/// Rage should always take priority.
		/// </summary>
		public bool TakesPriority {
			get {
				return true;
			}
		}
		/// <summary>
		/// Just pick a random enemy and use attack.
		/// </summary>
		public void ExecuteTurn() {
			BattleController.Instance.PrepareBehaviorEvaluation(
				source: this.combatantOwner,
				targets: new List<Combatant>() { this.combatantOwner.Opponents[Random.Range(minInclusive: 0, maxExclusive: this.combatantOwner.Opponents.Count - 1)]},
				behavior: this.combatantOwner.AllBehaviors[BehaviorType.Attack].First());
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		/// <summary>
		/// Incoming attacks take more damage. Outgoing attacks give more damage.
		/// </summary>
		/// <param name="dcs"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			dcs.damageCalculations.Where(dc => dc.target == this.combatantOwner).ToList().ForEach(dc => dc.rawDamageAmount *= 2);
			dcs.damageCalculations.Where(dc => dc.source == this.combatantOwner).ToList().ForEach(dc => dc.rawDamageAmount *= 2);
			return dcs;
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
