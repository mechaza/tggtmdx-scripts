using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// This is how the diamond should appear when negotiating.
	/// </summary>
	public class AffinityDiamondDX : AffinityDiamond {

		public static AffinityDiamondDX instance;

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The SuperTextMesh showing the Angry label.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), Title("Labels"), SerializeField]
		private SuperTextMesh angryTextLabel;
		/// <summary>
		/// The SuperTextMesh showing the Happy label.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		private SuperTextMesh happyTextLabel;
		/// <summary>
		/// The SuperTextMesh showing the Eager label.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		private SuperTextMesh eagerTextLabel;
		/// <summary>
		/// The SuperTextMesh showing the Scared label.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		private SuperTextMesh scaredTextLabel;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		#endregion

		#region BASE - VISIBILITY / ANIMATIONS
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

		#region UNIQUE - VISIBILITY / ANIMATIONS
		/// <summary>
		/// Presents the affinity diamond.
		/// </summary>
		/// <param name="combatant">The combatant who is currently being represented by the diamond.</param>
		public void PresentDiamond(Combatant combatant) {
			throw new System.NotImplementedException("");
		}
		/// <summary>
		/// Pulses the emoji on the diamond to be of the specified mood/severity.
		/// </summary>
		/// <param name="moodType">The mood to pulse.</param>
		/// <param name="moodSeverity">The severity at which to pulse the emoji.</param>
		private void PulseEmoji(CombatantMoodType moodType, CombatantMoodSeverityType moodSeverity) {
			throw new System.NotImplementedException("not done yet");
		}
		#endregion

	}

}