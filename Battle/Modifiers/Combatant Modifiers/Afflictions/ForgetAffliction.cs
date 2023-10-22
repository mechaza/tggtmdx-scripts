using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// Cannot use skills.
	/// </summary>
	public class ForgetAffliction : CombatantModifier, ICombatantAffliction, IBehaviorRestrictor {

		#region FIELDS - TIMER
		/// <summary>
		/// The timer that says how many turns are left in this affliction.
		/// </summary>
		private int timer = 4;
		#endregion

		#region FIELDS - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.Forget;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IBEHAVIORRESTRICTOR
		/// <summary>
		/// Don't allow any behaviors through.
		/// </summary>
		/// <param name="specialBehaviors"></param>
		/// <returns></returns>
		public List<BattleBehavior> RestrictUsableBehaviors(List<BattleBehavior> specialBehaviors) {
			// empty list lol
			return new List<BattleBehavior>();
		}
		#endregion

		#region INSPECTOR BULLSHIT
		private string inspectorString = "Cannot use skills.";
		protected override string InspectorDescription {
			get {
				return this.inspectorString;
			}
		}
		#endregion

	}


}