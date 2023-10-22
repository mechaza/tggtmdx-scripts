using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Temporarily overrides a resistance definition of a combatant.
	/// </summary>
	[System.Serializable]
	public class ResistanceChange : CombatantModifier, IOnTurnReady, IInterceptIncomingDCS {

		#region FIELDS
		/// <summary>
		/// The element that should be associated with this resistance change.
		/// </summary>
		[SerializeField]
		private ElementType elementType;
		/// <summary>
		/// Whether or not this change should "wall" or "break" a resistance.
		/// </summary>
		[SerializeField]
		private ResistanceShieldType shieldType;
		/// <summary>
		/// The number of turns this modifier should be in effect for.
		/// </summary>
		[SerializeField]
		private int timer = 3;
		#endregion

		#region FIELDS - COMPUTED
		/// <summary>
		/// In order to make resistance changes work, I need to first make sure that walls/breaks work as they are supposed to.
		/// </summary>
		private List<ResistanceType> ResistancesToCover {
			get {
				if (this.shieldType == ResistanceShieldType.Break) {
					return new List<ResistanceType>() {
						ResistanceType.Str,
						ResistanceType.Nul,
						ResistanceType.Ref,
						ResistanceType.Abs,
					};
				} else {
					return new List<ResistanceType>() {
						ResistanceType.Wk,
					};
				}
			}
		}
		#endregion

		#region CHECK FOR BREAK
		/// <summary>
		/// Checks whether or not the specified element is the same type as the one of this modifier, and that this modifier also breaks that element.
		/// This is used in Combatant to see if it is going to override a NullifyIncomingAttack.
		/// </summary>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public bool BreaksElement(ElementType elementType) {
			// Check if this resistance change breaks the specified element AND that the type is the same.
			if (this.shieldType == ResistanceShieldType.Break && this.elementType == elementType) {
				return true;
			} else {
				return false;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			dcs.damageCalculations
				.Where(dc => dc.target == self)											// Find DamageCalculations where this combatant is being targetd.
				.Where(dc => dc.behavior.elementType == this.elementType)				// Find DamageCalculations where the element of the behavior is equal to the one being negated.
				.Where(dc => this.ResistancesToCover.Contains(dc.finalResistance))			// If the calculation says that this resistance needs to go into effect given the innate resistance of the combatant
				.ToList()
				.ForEach(dc => dc.finalResistance = ResistanceType.Nm);						// Override that in the damage calculation.
			return dcs;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does nto work anymore");
			return delegate (BattleReactionSequence battleReactionSequence) {
				// First off, create a sequence.
				Sequence seq = DOTween.Sequence();

				if (this.timer - 1 == -1) {
					seq.AppendCallback(new TweenCallback(delegate {
						combatantOwner.RemoveModifier(modifier: this);
						BattleNotifier.DisplayNotifier("Resistance reverted!");
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
			// Check if the timer will run out if it is allowed to go another turn.
			/*if (this.timer - 1 == -1) {
				seq.AppendCallback(new TweenCallback(delegate {
					self.RemoveModifier(modifier: this);
					BattleNotifier.DisplayNotifier("Resistance reverted!");
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
		private static string descriptionText = "Temporarily overrides a resistance definition of a combatant. Walls cover innate weaknesses, while breaks suppress innate resistances.";
		/// <summary>
		/// A string that details how this function works.
		/// </summary>
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		#endregion

		/// <summary>
		/// A simple enum I'm using in order to make a dropdown for whether or not this resistance change should "wall" or "break" a combatant.
		/// </summary>
		private enum ResistanceShieldType {
			Wall = 0,
			Break = 1,
		}

	}


}