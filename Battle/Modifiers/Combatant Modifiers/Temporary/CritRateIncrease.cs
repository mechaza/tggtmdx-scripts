using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Boosts the combatant's critical hit rate. Done by modifying the crit rate on outgoing damage calculations.
	/// </summary>
	[System.Serializable]
	public class CritRateIncrease : CombatantModifier, IInterceptIncomingDCS, IOnTurnReady {

		#region FIELDS
		/// <summary>
		/// The amount to multiply the critical hit rate by.
		/// </summary>
		[SerializeField]
		private float multiplier = 1.5f;
		/// <summary>
		/// The number of turns this modifier should be in effect for.
		/// </summary>
		[SerializeField]
		private int timer = 3;
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			dcs.damageCalculations
				.Where(dc => dc.source == self)
				.ToList()
				.ForEach(dc => dc.rawCritRate *= this.multiplier);
			return dcs;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does not work anymore!");
			return delegate (BattleReactionSequence battleReactionSequence) {
				// First off, create a sequence.
				Sequence seq = DOTween.Sequence();

				// Check if the timer will run out if it is allowed to go another turn.
				if (this.timer - 1 == -1) {
					seq.AppendCallback(new TweenCallback(delegate {
						combatantOwner.RemoveModifier(modifier: this);
						BattleNotifier.DisplayNotifier("Critical hit rate reverted!");
					}));
					seq.AppendInterval(interval: 3f);
				} else {
					// If the timer can still go, I guess just... put the function into a callback since I need to do that anyway.
					seq.AppendCallback(new TweenCallback(delegate {
						this.timer -= 1;
					}));
				}

				seq.OnComplete(new TweenCallback(delegate { battleReactionSequence.ExecuteNextReaction(); }));
				seq.Play();
			};
			/*// Check if the timer will run out if it is allowed to go another turn.
			if (this.timer - 1 == -1) {
				seq.AppendCallback(new TweenCallback(delegate {
					self.RemoveModifier(modifier: this);
					BattleNotifier.DisplayNotifier("Critical hit rate reverted!");
				}));
				seq.AppendInterval(interval: 3f);
			} else {
				// If the timer can still go, I guess just... put the function into a callback since I need to do that anyway.
				seq.AppendCallback(new TweenCallback(delegate {
					this.timer -= 1;
				}));
			}
			return seq;*/
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Boosts the combatant's critical hit rate. Done by modifying the crit rate on outgoing damage calculations.";
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