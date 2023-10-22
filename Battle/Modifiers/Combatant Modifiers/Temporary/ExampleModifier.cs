using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// A test modifier to make sure i get shit working.
	/// </summary>
	public class ExampleModifier : CombatantModifier, IOnTurnStart, IOnTurnEnd, IPowerBooster {

		#region INTERFACE IMPLEMENTATION - IONTURNSTART
		public BattleReaction OnTurnStart() {
			// throw new System.Exception("Thid doesn't work anymore!");
			return delegate (BattleReactionSequence battleReactionSequence) {
				// First off, create a sequence.
				Sequence seq = DOTween.Sequence();

				seq.AppendCallback(new TweenCallback(delegate {
					BattleNotifier.DisplayNotifier("OOOOOO HERES ONE...", 3f);
				}));
				seq.AppendInterval(3f);
				seq.AppendCallback(new TweenCallback(delegate {
					BattleNotifier.DisplayNotifier("ANTOHER.", 3f);
				}));
				seq.AppendInterval(3f);

				seq.OnComplete(new TweenCallback(delegate { battleReactionSequence.ExecuteNextReaction(); }));
				seq.Play();
			};
			/*	seq.AppendCallback(new TweenCallback(delegate {
					BattleNotifier.DisplayNotifier("OOOOOO HERES ONE...", 3f);
				}));
				seq.AppendInterval(3f);
				seq.AppendCallback(new TweenCallback(delegate {
					BattleNotifier.DisplayNotifier("ANTOHER.", 3f);
				}));
				seq.AppendInterval(3f);
				return seq;*/
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNEND
		public BattleReaction OnTurnEnd() {
			throw new System.Exception("Thid doesn't work anymore!");
			/*seq.AppendCallback(new TweenCallback(delegate {
				BattleNotifier.DisplayNotifier("the turn has ended but at what fuckgin cost...", 3f);
			}));
			seq.AppendInterval(3f);
			seq.AppendCallback(new TweenCallback(delegate {
				BattleNotifier.DisplayNotifier("who are you. what are u doign he.", 3f);
			}));
			seq.AppendInterval(3f);
			seq.AppendCallback(new TweenCallback(delegate {
				BattleNotifier.DisplayNotifier(self.metaData.name + " loves ass", 3f);
			}));
			seq.AppendInterval(3f);
			return seq;*/
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IPOWERBOOSTER
		public float GetPowerBoost(PowerBoostType boostType) {
			switch (boostType) {
				case PowerBoostType.Attack:
					return 3f;
				case PowerBoostType.Defense:
					return -5f;
				case PowerBoostType.Accuracy:
					return 0f;
				default:
					throw new System.Exception("hey what the fuck");
			}
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