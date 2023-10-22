using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Nullifies an incoming attack of the given elemental type.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Nullifies an attack of the specified elemental type/affliction type.")]
	public class NullifyIncomingAttack : BattleBehaviorModifier, IInterceptIncomingDCS {

		#region FIELDS
		/// <summary>
		/// The kind of nullifier to use. Mostly for help with the inspector.
		/// </summary>
		[SerializeField]
		private NullifierType nullifierType = NullifierType.Affliction;
		/// <summary>
		/// The element type to nullify when this combatant is being attacked.
		/// </summary>
		[SerializeField, ShowIf("IsElementNullifier")]
		private ElementType elementType;
		/// <summary>
		/// The kind of affliction to nullify.
		/// </summary>
		[SerializeField, HideIf("IsElementNullifier")]
		private AfflictionType afflictionType;
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		/// <summary>
		/// Intercepts an incoming DamageCalculationSet and... Well, actually, for now, it's going to Reflect. Let's try that.
		/// </summary>
		/// <param name="dcs"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			if (this.nullifierType == NullifierType.Element) {
				dcs.damageCalculations
				.Where(dc => dc.target == self)															 // Inside the DCS, find calculations where this combatant is the target.
				.Where(dc => dc.behavior.elementType == this.elementType)								 // If the behavior in that calculation has an element matching the type in this script,
				.Where(dc => dc.target.CheckResistanceBreak(elementType: this.elementType) == false)	 // Important to note that this cannot go on if the target has a resistance break applied to them.
				.ToList()
				.ForEach(dc => dc.Nullify());                               // Reflect it.
			} else {
				dcs.damageCalculations
				.Where(dc => dc.target == self)												// Inside the DCS, find calculations where this combatant is the target.
				.Where(dc => dc.behavior.afflictionTypes.Contains(this.afflictionType))		// If the behavior in that calculation has an element matching the type in this script,
				.ToList()
				.ForEach(dc => dc.NullifyAffliction());										// Null it.
			}
			return dcs;
		}
		#endregion

		/*#region INSPECTOR STUFF
		private static string inspectorDescription = "Nullifies an attack of the specified elemental type/affliction type.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}
		#endregion*/

		#region ODIN FUNCTIONS
		/// <summary>
		/// Checks whether or not this is an elemental multiplier.
		/// </summary>
		/// <returns></returns>
		public bool IsElementNullifier() {
			return (this.nullifierType == NullifierType.Element);
		}
		#endregion

		/// <summary>
		/// A simple way I can get a dropdown going in the inspector.
		/// </summary>
		private enum NullifierType {
			Element = 0,
			Affliction = 1,
		}

	}


}