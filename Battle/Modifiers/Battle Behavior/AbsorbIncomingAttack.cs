using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Absorbs an incoming attack of the given elemental type.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Absorbs an attack of the specified elemental type/affliction type.")]
	public class AbsorbIncomingAttack : BattleBehaviorModifier, IInterceptIncomingDCS {

		#region FIELDS
		/// <summary>
		/// The element type to nullify when this combatant is being attacked.
		/// </summary>
		[SerializeField]
		private ElementType elementType;
	
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		/// <summary>
		/// Intercepts an incoming DamageCalculationSet and... Well, actually, for now, it's going to Reflect. Let's try that.
		/// </summary>
		/// <param name="dcs"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			dcs.damageCalculations
				.Where(dc => dc.FinalTarget == self)                                                          // Inside the DCS, find calculations where this combatant is the target.
				.Where(dc => dc.behavior.elementType == this.elementType)                                // If the behavior in that calculation has an element matching the type in this script,
				.ToList()
				.ForEach(dc => dc.Absorb());                               // Absorb it.
			return dcs;
		}
		#endregion

	}


}