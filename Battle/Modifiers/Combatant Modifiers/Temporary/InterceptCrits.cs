using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Intercepts any incoming critical hits and makes them normal.
	/// </summary>
	[System.Serializable]
	public class InterceptCrits : CombatantModifier, IInterceptIncomingDCSLate {

		#region INTERFACE IMPLEMENTATION - IINTERCEPTINCOMINGDCS
		public DamageCalculationSet InterceptIncomingDCSLate(DamageCalculationSet dcs, Combatant self) {
			dcs.damageCalculations
				.Where(dc => dc.accuracyType == AccuracyType.Critical)
				.ToList()
				.ForEach(dc => dc.accuracyType = AccuracyType.Normal);
			return dcs;
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Intercepts any incoming critical hits and makes them normal. Helpful for scripted battles, to an extent.";
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