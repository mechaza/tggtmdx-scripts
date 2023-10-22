using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Applies a wall to the given combatant for a set number of turns.
	/// </summary>
	[System.Serializable]
	public class ElementWall : CombatantModifier, IInterceptIncomingDCS, IOnTurnReady {

		#region FIELDS
		/// <summary>
		/// A list of elemental types to set up walls for.
		/// </summary>
		[SerializeField]
		private List<ElementType> elementTypes = new List<ElementType>();
		/// <summary>
		/// The number of turns this wall should be set up for. When it hits zero, it should be removed from the combatant.
		/// </summary>
		[SerializeField]
		private int timer = 3;
		/// <summary>
		/// The amount to multiply incoming attacks by.
		/// </summary>
		[SerializeField]
		private float multiplier = 1f;
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		/// <summary>
		/// Upon intercept, check to see if the element of the specified type is contained within the damage calculations where this combatant is also a target. If so, modify it.
		/// </summary>
		/// <param name="dcs"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			Debug.Log("Attempting to intercept...");
			dcs.damageCalculations
				.Where(dc => dc.target == self)
				.Where(dc => this.elementTypes.Contains(dc.behavior.elementType))
				.ToList()
				.ForEach(dc => {
					Debug.Log(self.metaData.name + " resists attack due to elemental wall!");
					dc.rawDamageAmount = (int)(this.multiplier * (float)dc.rawDamageAmount);
				});
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
						BattleNotifier.DisplayNotifier("Elemental wall has disappeared!");
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
					BattleNotifier.DisplayNotifier("Elemental wall has disappeared!");
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
		private static string descriptionText = "Sets up a wall for a set number of turns that weakens attacks of a given elemental type.";
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