using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// Calculates healing in the same way as the move should normally.
	/// </summary>
	[System.Serializable]
	public class Healing : PauseBehaviorFunction {

		#region FIELDS - INHERITED
		/// <summary>
		/// Is this function asynchonous? I.e., healing probably isn't, but the menu to bring up learning a skill card is.
		/// </summary>
		public override bool IsAsynchronous {
			get {
				return false;
			}
		}
		#endregion

		#region FUNCTION
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior self) {
			Debug.Log("Executing pause menu's healing move.");
			targets.ForEach(t => t.HP -= DynamicCalculation(source: source, target: t, self: self));
			// Deduct the cost of the move as long as its not an item.
			if (self.behaviorType != BehaviorType.Item) {
				source.DeductBehaviorCost(behavior: self);
			}
			
		}
		#endregion

		#region THING I STOLE FROM THE STANDARDBATTLEBEHAVIORFUNCTION
		protected int DynamicCalculation(Combatant source, Combatant target, BattleBehavior self) {

			// https://www.gamefaqs.com/boards/945498-shin-megami-tensei-persona-4/48852804
			// Depending on the type of attack (magic or not), a different resource will need to be used.
			/*float attackStat = 1f;
			if (self.costType == BehaviorCostType.MP) { attackStat = source.MA; } else { attackStat = source.ST; }*/

			float attackStat = (self.costType == BehaviorCostType.MP) ? source.DynamicMA : source.DynamicST;
			
			// If the move ignores EN, the defense factor is 1. Otherwise, its the target's EN.
			float defenseFactor = self.ignoreEN ? 1f : target.DynamicEN;

			// int dmg = (int)((5f * Mathf.Sqrt(((float)source.ST / defenseFactor) * self.basePower)) * Random.Range(0.95f, 1.05f));
			// (int)(Mathf.Pow(f: self.basePower * DebugController.GetDebugField("behaviorPowerMultiplier"), p: DebugController.GetDebugField("behaviorPowerExponent")))
			int dmg = (int)((
				5f 
				* Mathf.Sqrt(((float)source.DynamicST / defenseFactor) 
				* (int)(Mathf.Pow(
					f: self.basePower * GameController.Instance.DifficultyToggles.BattleBehaviorPowerMultiplier, 
					p: GameController.Instance.DifficultyToggles.BattleBehaviorPowerExponent)))) 
				* Random.Range(0.95f, 1.05f));

			/*f: self.basePower * DebugController.GetDebugField("behaviorPowerMultiplier"), 
					p: DebugController.GetDebugField("behaviorPowerExponent")))))*/

			// Make sure to grab the boost multipliers.
			// dmg = (int)((float)dmg * source.statusModifiers.GetPowerBoost(PowerBoostType.Attack) / target.statusModifiers.GetPowerBoost(PowerBoostType.Defense));
			dmg = (int)((float)dmg * source.GetPowerBoost(PowerBoostType.Attack) / target.GetPowerBoost(PowerBoostType.Defense));

			// If this was a healing move, negate the amount.
			if (self.elementType == ElementType.Healing) { dmg = -dmg; }

			return dmg;
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Calculates healing in the same way as the move should normally.";
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