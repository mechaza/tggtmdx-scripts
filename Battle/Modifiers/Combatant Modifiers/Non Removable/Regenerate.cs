using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Regenerates a percentage of the specified resource every turn.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Regenerates a percentage of the specified resource every turn.")]
	public class Regenerate : BattleBehaviorModifier, IOnTurnReady {

		#region FIELDS
		/// <summary>
		/// The resource to regernerate from every turn ready.
		/// </summary>
		[SerializeField]
		private BehaviorCostType resource;
		[SerializeField]
		private RegenerationType regenerationType;
		/// <summary>
		/// The amount to regenerate every turn ready, as a percentage.
		/// </summary>
		[SerializeField, Range(min: 0f, max: 1f), ShowIf("IsPercentageType")]
		private float percentage = 0.5f;
		/// <summary>
		/// The amount to generate every turn ready, as a static amount.
		/// </summary>
		[SerializeField, HideIf("IsPercentageType")]
		private int amt = 0;
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNREADY
		/// <summary>
		/// At the start of every turn, add a percentage to the specified resource.
		/// </summary>
		/// <param name="seq"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public BattleReaction OnTurnReady() {
			// throw new System.Exception("This does not work any more");
			return delegate (BattleReactionSequence battleReactionSequence) {

				Debug.LogWarning("TODO: make sure I animate resources getting restored!");

				// First off, create a sequence.
				Sequence seq = DOTween.Sequence();

				if (this.IsPercentageType() == true) {
					seq.AppendCallback(new TweenCallback(delegate {

						// Animate the highlight on the combatant.
						this.combatant.CombatantAnimator.AnimateFocusHighlight(combatant: this.combatant, time: 3f);

						if (this.resource == BehaviorCostType.MP) {
							this.combatant.MP += (int)((float)this.combatant.MaxMP * this.percentage);
							BattleNotifier.DisplayNotifier("MP restored!");
						} else if (this.resource == BehaviorCostType.ST) {
							this.combatant.SP += (int)((float)this.combatant.MaxSP * this.percentage);
							BattleNotifier.DisplayNotifier("SP restored!");
						} else {
							this.combatant.HP += (int)((float)this.combatant.MaxHP * this.percentage);
							BattleNotifier.DisplayNotifier("HP restored!");
						}
					}));
				} else {
					seq.AppendCallback(new TweenCallback(delegate {

						// Animate the highlight on the combatant.
						this.combatant.CombatantAnimator.AnimateFocusHighlight(combatant: this.combatant, time: 3f);

						if (this.resource == BehaviorCostType.MP) {
							this.combatant.MP += this.amt;
							BattleNotifier.DisplayNotifier("MP restored!");
						} else if (this.resource == BehaviorCostType.ST) {
							this.combatant.SP += this.amt;
							BattleNotifier.DisplayNotifier("SP restored!");
						} else {
							this.combatant.HP += this.amt;
							BattleNotifier.DisplayNotifier("HP restored!");
						}
					}));
				}
				seq.AppendInterval(interval: 3f);

				seq.OnComplete(new TweenCallback(delegate { battleReactionSequence.ExecuteNextReaction(); }));
				seq.Play();
			};
			/*if (this.IsPercentageType() == true) {
				seq.AppendCallback(new TweenCallback(delegate {
					if (this.resource == BehaviorCostType.MP) {
						self.MP += (int)((float)self.MaxMP * this.percentage);
						BattleNotifier.DisplayNotifier("MP restored!");
					} else if (this.resource == BehaviorCostType.ST) {
						self.SP += (int)((float)self.MaxSP * this.percentage);
						BattleNotifier.DisplayNotifier("SP restored!");
					} else {
						self.HP += (int)((float)self.MaxHP * this.percentage);
						BattleNotifier.DisplayNotifier("HP restored!");
					}
				}));
			} else {
				seq.AppendCallback(new TweenCallback(delegate {
					if (this.resource == BehaviorCostType.MP) {
						self.MP += this.amt;
						BattleNotifier.DisplayNotifier("MP restored!");
					} else if (this.resource == BehaviorCostType.ST) {
						self.SP += this.amt;
						BattleNotifier.DisplayNotifier("SP restored!");
					} else {
						self.HP += this.amt;
						BattleNotifier.DisplayNotifier("HP restored!");
					}
				}));
			}
			seq.AppendInterval(interval: 3f);
			return seq;*/
		}
		private bool IsPercentageType() {
			return (this.regenerationType == RegenerationType.Percentage);
		}
		#endregion


		/*#region INSPECTOR STUFF
		private static string inspectorDescription = "Regenerates a percentage of the specified resource every turn.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}	
		
		#endregion*/

		/// <summary>
		/// Just an enum to help with building out menus.
		/// </summary>
		private enum RegenerationType {
			Percentage = 0,
			Static = 1,
		}

	}

}