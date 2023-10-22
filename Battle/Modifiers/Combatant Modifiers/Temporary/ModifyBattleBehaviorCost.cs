using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.Modifiers;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Modifiers.Temporary {

	/// <summary>
	/// Modifies the cost of a battle behavior that matches the given criteria.
	/// </summary>
	[System.Serializable]
	public class ModifyBattleBehaviorCost : CombatantModifier, IInterceptBattleBehaviorCost {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The different types of elements that should be intercepted.
		/// </summary>
		[SerializeField]
		private List<ElementType> elementsToIntercept = new List<ElementType>();
		/// <summary>
		/// How much should the behavior cost be multiplied by, if it matches the specified criteria?
		/// </summary>
		[SerializeField]
		private float amountToMultiply = 1f;
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
			}

			// Only modify the cost if the list contains the specified element.
			if (this.elementsToIntercept.Contains(behavior.elementType) == true) {
				return (int)(this.amountToMultiply * runningCost);
			} else {
				return runningCost;
			}
		}
		#endregion

		#region INSPECTOR STUFF
		private static string inspectorDescription = "Modifies the cost of a battle behavior that matches the given criteria. Yeah.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}
		#endregion

	}


}