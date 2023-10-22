using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.BattleMenu;
using System.Linq;

namespace Grawly.Battle.Functions {

	public class BatonTouchBattleBehaviorFunction : BattleBehaviorFunction {

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Pass the baton to another player. Only available after exploiting a weak point.";
		/// <summary>
		/// A string that details how this function works.
		/// </summary>
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		#endregion

		#region FUNCTION
		/// <summary>
		/// Execute the function and perform the necessary calculations.
		/// </summary>
		/// <param name="source">Who the move is originating from.</param>
		/// <param name="targets">A list of targets that are being affected by the move.</param>
		/// <param name="battleBehavior">The BattleBehavior this function is attached to.</param>
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			BattleController.Instance.StartCoroutine(this.ExecuteAsCoroutine(source: source, targets: targets, battleBehavior: battleBehavior));
		}
		private IEnumerator ExecuteAsCoroutine(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			// If this is the version of the move that's being used as a tactics move, deduct the cost from the source.
			if (battleBehavior.behaviorType == BehaviorType.Tactics) {
				Debug.Log("Baton Touch DX has been used! Deducting cost.");
				source.DeductBehaviorCost(behavior: battleBehavior);
			}

			// Play the animation for the baton touch by calling the animation controller for it.
			yield return BatonTouchDXAnimationController.instance.PlayBatonTouchAnimation(sourcePlayer: source as Player, targetPlayer: targets.First() as Player);

			BattleController.Instance.BatonTouch(source, targets.First());
			BackToBattleController();
		}
		#endregion

		#region CALCULATION DELEGATES 
		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			throw new System.NotImplementedException();
		}
		#endregion

	}


}