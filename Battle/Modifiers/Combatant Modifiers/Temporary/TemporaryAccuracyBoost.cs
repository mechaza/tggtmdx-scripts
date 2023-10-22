using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Similar to Accuracy Boost on BattleBehaviorModifier, but runs on a timer.
	/// </summary>
	[System.Serializable]
	public class TemporaryAccuracyBoost : CombatantModifier, IInterceptIncomingDCS, IOnTurnReady {

		#region FIELDS - RULES
		/// <summary>
		/// The kind of multiplier to use. Mostly for help with the inspector.
		/// </summary>
		[SerializeField]
		private AccuracyMultiplierType multiplierType = AccuracyMultiplierType.Affliction;
		/// <summary>
		/// The elemental type to multipy, if that is the mode set.
		/// </summary>
		[SerializeField, ShowIf("IsElementMultiplier")]
		private List<ElementType> elementTypes = new List<ElementType>();
		/// <summary>
		/// The type of affliction toahgagsdfiojlkfnzasd,jkh YOU know what imd ogin
		/// </summary>
		[SerializeField, HideIf("IsElementMultiplier")]
		private List<AfflictionType> afflictionTypes = new List<AfflictionType>();
		/// <summary>
		/// The amount to multiply the accuracy by.
		/// </summary>
		[SerializeField, Range(min: 0f, max: 2f)]
		private float multiplierValue = 1f;
		/// <summary>
		/// The number of turns this modifier is in effect.
		/// </summary>
		[SerializeField]
		private int timer = 3;
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		/// <summary>
		/// Intercepts an incoming DamageCalculationSet and... Well, actually, for now, it's going to Reflect. Let's try that.
		/// </summary>
		/// <param name="dcs"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			// Figure out how I wanna do this.
			if (this.multiplierType == AccuracyMultiplierType.Element) {
				dcs.damageCalculations
				.Where(dc => dc.source == self)                                             // Inside the DCS, find calculations where this combatant is the target.
				.Where(dc => this.elementTypes.Contains(dc.behavior.elementType))           // If the behavior in that calculation has an element matching the type in this script,
				.ToList()
				.ForEach(dc => dc.rawAccuracy *= this.multiplierValue);                     // Multiply the raw accuracy. This will collapse into an accuracy "type" later on.
			} else {
				dcs.damageCalculations
				.Where(dc => dc.source == self)															 // Inside the DCS, find calculations where this combatant is the target.
				.Where(dc => dc.behavior.afflictionTypes.Intersect(this.afflictionTypes).Count() > 0)	// Find damage calculations where the intersection is more than zero.
				.ToList()
				.ForEach(dc => dc.rawAccuracy *= this.multiplierValue);                     // Multiply the raw accuracy. This will collapse into an accuracy "type" later on.
			}
			return dcs;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does nto work anymore!");
			return delegate (BattleReactionSequence battleReactionSequence) {
				// First off, create a sequence.
				Sequence seq = DOTween.Sequence();

				// Check if the timer will run out if it is allowed to go another turn.
				if (this.timer - 1 == -1) {
					seq.AppendCallback(new TweenCallback(delegate {
						combatantOwner.RemoveModifier(modifier: this);
						BattleNotifier.DisplayNotifier("Accuracy modifier reverted!");
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
					BattleNotifier.DisplayNotifier("Accuracy modifier reverted!");
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

		#region INSPECTOR STUFF
		private static string inspectorDescription = "Multiplies the accuracy of DamageCalculations that line up with a set of rules. Gets removed after a certain amouint of time.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}
		#endregion

		#region ODIN FUNCTIONS
		/// <summary>
		/// Checks whether or not this is an elemental multiplier.
		/// </summary>
		/// <returns></returns>
		public bool IsElementMultiplier() {
			return (this.multiplierType == AccuracyMultiplierType.Element);
		}
		#endregion

		/// <summary>
		/// A simple way I can get a dropdown going in the inspector.
		/// </summary>
		private enum AccuracyMultiplierType {
			Element = 0,
			Affliction = 1,
		}

	}


}