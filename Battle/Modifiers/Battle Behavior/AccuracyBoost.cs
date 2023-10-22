using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Boosts the accuracy of DamageCalculations that line up with a set of rules.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Multiplies the accuracy of DamageCalculations that line up with a set of rules. Stacks with other accuracy boosts.")]
	public class AccuracyBoost : BattleBehaviorModifier, IInterceptIncomingDCS {

		#region FIELDS - RULES
		/// <summary>
		/// The kind of multiplier to use. Mostly for help with the inspector.
		/// </summary>
		[SerializeField]
		private AccuracyMultiplierType multiplierType = AccuracyMultiplierType.Affliction;
		/// <summary>
		/// The elemental type to multipy, if that is the mode set.
		/// </summary>
		[SerializeField, ShowIf("IsElementMultiplier")]
		private ElementType elementType = ElementType.None;
		/// <summary>
		/// The type of affliction toahgagsdfiojlkfnzasd,jkh YOU know what imd ogin
		/// </summary>
		[SerializeField, HideIf("IsElementMultiplier")]
		private List<AfflictionType> afflictionTypes = new List<AfflictionType>();
		/// <summary>
		/// The amount to multiply the accuracy by.
		/// </summary>
		[SerializeField, Range(min: 0f, max: 2f)]
		private float multiplierValue = 1f;
		#endregion


		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		/// <summary>
		/// Intercepts an incoming DamageCalculationSet and... Well, actually, for now, it's going to Reflect. Let's try that.
		/// </summary>
		/// <param name="dcs"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			// Figure out how I wanna do this.
			if (this.multiplierType == AccuracyMultiplierType.Element) {
				dcs.damageCalculations
				.Where(dc => dc.source == self)												// Inside the DCS, find calculations where this combatant is the target.
				.Where(dc => dc.behavior.elementType == this.elementType)					// If the behavior in that calculation has an element matching the type in this script,
				.ToList()
				.ForEach(dc => dc.rawAccuracy *= this.multiplierValue);						// Multiply the raw accuracy. This will collapse into an accuracy "type" later on.
			} else {
				dcs.damageCalculations
					.Where(dc => dc.source == self)                                                          // Inside the DCS, find calculations where this combatant is the target.
					.Where(dc => dc.behavior.afflictionTypes.Intersect(this.afflictionTypes).Count() > 0)   // Find damage calculations where the intersection is more than zero.
					.ToList()
					.ForEach(dc => dc.rawAccuracy *= this.multiplierValue);                     // Multiply the raw accuracy. This will collapse into an accuracy "type" later on.
			}
			return dcs;
		}
		#endregion

		/*#region INSPECTOR STUFF
		private static string inspectorDescription = "Multiplies the accuracy of DamageCalculations that line up with a set of rules. Stacks with other accuracy boosts.";
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
		public bool IsElementMultiplier() {
			return (this.multiplierType == AccuracyMultiplierType.Element);
		}
		#endregion

		/// <summary>
		/// A simple way I can get a dropdown going in the inspector.
		/// </summary>
		private enum AccuracyMultiplierType {
			Element = 0,
			Affliction = 1,
		}

	}


}