using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// This is how the affinity diamond should appear when on an enemy cursor.
	/// </summary>
	public class AffinityDiamondCursorVisuals : AffinityDiamond {

		#region VISIBILITY / ANIMATIONS
		/// <summary>
		/// Sets whether the diamond visuals are visible or not.
		/// </summary>
		/// <param name="status"></param>
		public override void SetVisibility(bool status) {
			throw new System.NotImplementedException("Not done yet");
		}
		/// <summary>
		/// Sets the visuals on this affinity diamond based on the specified combatant.
		/// </summary>
		/// <param name="combatant">The combatant who is being represented.</param>
		public override void SnapAffinityVisuals(Combatant combatant) {
			throw new System.NotImplementedException("Not done yet");
		}
		/// <summary>
		/// Increments the mood/affinity of the given type for the specified combatant by 1 point.
		/// </summary>
		/// <param name="combatant">The combatant to increment the affinity for.</param>
		/// <param name="moodType">The mood type to animate the increment for.</param>
		/// <param name="oldSeverity">The old severity of the mood.</param>
		/// <param name="newSeverity">The new severity of the mood.</param>
		public override void AnimateAffinityIncrement(Combatant combatant, CombatantMoodType moodType, CombatantMoodSeverityType oldSeverity, CombatantMoodSeverityType newSeverity) {
			throw new System.NotImplementedException("not done yet");
		}
		#endregion


	}

}