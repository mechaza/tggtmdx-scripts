using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Amplifies attacks of the given type by a set multiplier. Is removed from combatant when it goes into effect.
	/// </summary>
	[System.Serializable]
	public class AttackAmplifier : CombatantModifier, IInterceptIncomingDCS {

		#region FIELDS
		/// <summary>
		/// The list of element types to amplify.
		/// </summary>
		[SerializeField]
		private List<ElementType> elementTypes = new List<ElementType>();
		/// <summary>
		/// The amount to multiply by.
		/// </summary>
		[SerializeField]
		private float multiplier = 1f;
		/// <summary>
		/// Should this modifier be removed from the combatant when they attack?
		/// </summary>
		[SerializeField]
		private bool removeOnAttack = true;
		#endregion

		#region CONSTRUCTORS
		public AttackAmplifier() {

		}
		/// <summary>
		/// This particular constructor is meant for when I'm adding this modifier at runtime.
		/// Not meant for use inside of the inspector.
		/// </summary>
		/// <param name="multiplier"></param>
		/// <param name="elementTypes"></param>
		public AttackAmplifier(float multiplier, List<ElementType> elementTypes, bool removeOnAttack) {
			this.elementTypes = elementTypes;
			this.multiplier = multiplier;
			this.removeOnAttack = removeOnAttack;
		}
		/// <summary>
		/// This particular constructor is meant for when I'm adding this modifier at runtime.
		/// Not meant for use inside of the inspector.
		/// </summary>
		/// <param name="multiplier"></param>
		/// <param name="elementTypes"></param>
		public AttackAmplifier(float multiplier, System.Array elementTypes, bool removeOnAttack) {
			this.elementTypes = new List<ElementType>(elementTypes.Cast<ElementType>());
			this.multiplier = multiplier;
			this.removeOnAttack = removeOnAttack;
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {

			List<DamageCalculation> matches = dcs.damageCalculations
				.Where(dc => dc.source == self)										// Go through all the calculations where this combatant is a source
				.Where(dc => this.elementTypes.Contains(dc.behavior.elementType))	// Check if the element being used is one that can be amplified.
				.ToList();

			// If the matches count is greater than zero, that means this amplifier has been used and should be removed upon completion.
			if (matches.Count > 0) {
				// Go through each calculation.
				matches.ForEach(dc => dc.rawDamageAmount = (int)((float)dc.rawDamageAmount * this.multiplier));
				// Remove the reference from the combatant.
				self.RemoveModifier(this);
			}

			return dcs;
		}
		#endregion


		#region INSPECTOR BULLSHIT
		private static string descriptionText = "Amplifies attacks of the given type by a set multiplier. Is removed from combatant when it goes into effect.";
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		#endregion
	}


}