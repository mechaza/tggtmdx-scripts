using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Boosts the power of the specified elemental types by the given amount.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Boosts the power of the specified elemental typed attacks by the given amount.")]
	public class PowerBoost : BattleBehaviorModifier, IInterceptIncomingDCS {

		#region FIELDS
		/// <summary>
		/// The element types to boost power by.
		/// </summary>
		[SerializeField]
		private List<ElementType> elementTypes = new List<ElementType>();
		/// <summary>
		/// The amount to multiply the amount by.
		/// </summary>
		[SerializeField]
		private float multiplier = 1f;
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			dcs.damageCalculations
				.Where(dc => dc.source == self)												// Go through each calculation where this is the source of the calculation.
				.Where(dc => this.elementTypes.Contains(dc.behavior.elementType))			// See if this modifier contains the element associated with the behavior.
				.ToList()
				.ForEach(dc => dc.rawDamageAmount = (int)((float)dc.rawDamageAmount * this.multiplier));			// For all results, multiply the amount by the given multiplier.
			return dcs;
		}
		#endregion

		/*#region INSPECTOR STUFF
		private static string inspectorDescription = "Boosts the power of the specified elemental typed attacks by the given amount.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}
		#endregion*/

	}


}