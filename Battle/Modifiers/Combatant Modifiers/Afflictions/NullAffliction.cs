using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle.Modifiers.Afflictions {

	/// <summary>
	/// The "Null" affliction that is present at almost all times.
	/// </summary>
	public class NullAffliction : CombatantModifier, ICombatantAffliction {

		#region INTERFACE IMPLEMENTATION - ICOMBATANTAFFLICTION
		public AfflictionType Type {
			get {
				return AfflictionType.None;
			}
		}
		public bool CanMoveOnReady {
			get {
				return true;
			}
		}
		public Color Color {
			get {
				return Color.white;
			}
		}
		#endregion


		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "No description yet.";
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

