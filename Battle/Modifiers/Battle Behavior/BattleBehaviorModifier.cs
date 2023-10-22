using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Defines certain passive functionalities this behavior will have.
	/// </summary>
	public abstract class BattleBehaviorModifier {

		#region FIELDS - STATE
		/// <summary>
		/// The BattleBehavior this modifier is attached to.
		/// </summary>
		protected BattleBehavior behavior;
		/// <summary>
		/// The combatant who owns the behavior that this modifier is attached to.
		/// </summary>
		protected Combatant combatant;
		#endregion

		#region CONSTRUCTORS
		public BattleBehaviorModifier() {

		}
		public BattleBehaviorModifier(BattleBehavior behavior, Combatant combatant) {
			this.behavior = behavior;
			this.combatant = combatant;
		}
		/// <summary>
		/// Assigns the behavior stored in this modifier and returns itself.
		/// </summary>
		/// <param name="behavior">The behavior to assign to this modifier.</param>
		/// <param name="combatant">The combatant who owns the behavior.</param>
		/// <returns>The modifier that was just called.</returns>
		public BattleBehaviorModifier AssignBattleBehaviorAndCombatant(BattleBehavior behavior, Combatant combatant) {
			this.behavior = behavior;
			this.combatant = combatant;
			return this;
		}
		#endregion

	}


}