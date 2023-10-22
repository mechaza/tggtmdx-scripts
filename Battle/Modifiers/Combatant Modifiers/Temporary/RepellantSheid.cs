using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Reflects a physical/elemental move only once.
	/// </summary>
	[System.Serializable]
	public class RepellantShield : CombatantModifier, IInterceptIncomingDCS {

		#region FIELDS
		/// <summary>
		/// Does this shield block elemental attacks?
		/// </summary>
		[SerializeField]
		public bool BlocksElemental { get; private set; }
		/// <summary>
		/// Does this shield block physical attacks?
		/// </summary>
		public bool BlocksPhysical {
			get {
				// lol
				return !this.BlocksElemental;
			}
		}
		#endregion

		#region FIELDS - CONSTANTS
		/// <summary>
		/// The different kinds of elements that can be reflected.
		/// </summary>
		private List<ElementType> elementalTypes = new List<ElementType>() {
			ElementType.Fire,
			ElementType.Ice,
			ElementType.Wind,
			ElementType.Elec,
			ElementType.Psi,
			ElementType.Nuke,
			ElementType.Light,
			ElementType.Curse,
		};
		#endregion

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		/// <summary>
		/// Upon intercept, check to see if the element of the specified type is contained within the damage calculations where this combatant is also a target. If so, modify it.
		/// </summary>
		/// <param name="dcs"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self) {
			Debug.LogWarning("I don't think this works well with moves that hit more than once. It will just reflect them all.");

			if (this.BlocksElemental == true) {
				dcs.damageCalculations
				.Where(dc => dc.FinalTarget == self)
				.Where(dc => this.elementalTypes.Contains(dc.behavior.elementType))
				.ToList()
				.ForEach(dc => {
					dc.Reflect();
					self.RemoveModifier(this);
				});
			} else {
				dcs.damageCalculations
				.Where(dc => dc.FinalTarget == self)
				.Where(dc => dc.behavior.elementType == ElementType.Phys)
				.ToList()
				.ForEach(dc => {
					dc.Reflect();
					self.RemoveModifier(this);
				});
			}

			
			return dcs;
		}
		#endregion


		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Reflects a physical/elemental move only once. If Block Elemental is false, it will block a physical attack.";
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