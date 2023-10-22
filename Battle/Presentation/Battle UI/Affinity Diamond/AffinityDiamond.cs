using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Sirenix.Serialization;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// A general class for helping me manage places where I use the Affinity Diamond.
	/// </summary>
	public abstract class AffinityDiamond : SerializedMonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that has all the visuals for the diamond.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		protected GameObject allVisuals;
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The front image for the diamond. This is where the white is.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), Title("Images"), SerializeField]
		protected Image diamondBackingFrontImage;
		/// <summary>
		/// The Image for the anger bar.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		protected Image angerBarImage;
		/// <summary>
		/// The Image for the happy bar.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		protected Image happyBarImage;
		/// <summary>
		/// The Image for the eager bar.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		protected Image eagerBarImage;
		/// <summary>
		/// The Image for the scared bar.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		protected Image scaredBarImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : RECTTRANSFORMS
		/// <summary>
		/// The RectTransform for the anger bar.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), Title("Rect Transforms"), SerializeField]
		protected RectTransform angerBarRectTransform;
		/// <summary>
		/// The RectTransform for the happy bar.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		protected RectTransform happyBarRectTransform;
		/// <summary>
		/// The RectTransform for the eager bar.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		protected RectTransform eagerBarRectTransform;
		/// <summary>
		/// The RectTransform for the scared bar.
		/// </summary>
		[TabGroup("Affinity", "Scene References"), SerializeField]
		protected RectTransform scaredBarRectTransform;
		#endregion


		#region FIELDS - TOGGLES : POSITIONS
		/// <summary>
		/// The start position for the anger bar.
		/// </summary>
		[TabGroup("Affinity", "Toggles"), Title("Start Positions"), SerializeField]
		protected Vector2 angerBarStartPos;
		/// <summary>
		/// The start position for the happy bar.
		/// </summary>
		[TabGroup("Affinity", "Toggles"), SerializeField]
		protected Vector2 happyBarStartPos;
		/// <summary>
		/// The start position for the eager bar.
		/// </summary>
		[TabGroup("Affinity", "Toggles"), SerializeField]
		protected Vector2 eagerBarStartPos;
		/// <summary>
		/// The start position for the scared bar.
		/// </summary>
		[TabGroup("Affinity", "Toggles"), SerializeField]
		protected Vector2 scaredBarStartPos;
		/// <summary>
		/// The end position for the anger bar.
		/// </summary>
		[TabGroup("Affinity", "Toggles"), Title("End Positions"), SerializeField]
		protected Vector2 angerBarEndPos;
		/// <summary>
		/// The end position for the happy bar.
		/// </summary>
		[TabGroup("Affinity", "Toggles"), SerializeField]
		protected Vector2 happyBarEndPos;
		/// <summary>
		/// The end position for the eager bar.
		/// </summary>
		[TabGroup("Affinity", "Toggles"), SerializeField]
		protected Vector2 eagerBarEndPos;
		/// <summary>
		/// The end position for the scared bar.
		/// </summary>
		[TabGroup("Affinity", "Toggles"), SerializeField]
		protected Vector2 scaredBarEndPos;
		#endregion

		#region HELPERS
		/// <summary>
		/// Gets the interpolated position for the specified mood.
		/// </summary>
		/// <param name="moodType">The mood that the position is being requested for.</param>
		/// <param name="currentMoodValue">The interpolant.</param>
		/// <returns></returns>
		protected Vector2 GetPositionForAffinityMood(CombatantMoodType moodType, float currentMoodValue) {
			switch (moodType) {
				case CombatantMoodType.Angry:
					return Vector2.Lerp(a: this.angerBarStartPos, b: angerBarEndPos, t: currentMoodValue);
				case CombatantMoodType.Happy:
					return Vector2.Lerp(a: this.happyBarStartPos, b: happyBarEndPos, t: currentMoodValue);
				case CombatantMoodType.Eager:
					return Vector2.Lerp(a: this.eagerBarStartPos, b: eagerBarEndPos, t: currentMoodValue);
				case CombatantMoodType.Scared:
					return Vector2.Lerp(a: this.scaredBarStartPos, b: scaredBarEndPos, t: currentMoodValue);
				default:
					Debug.LogError("COULDN'T DETERMINE POSITION FOR MOOD. RETURNING ZERO");
					return Vector2.zero;
			}
		}
		#endregion

		#region VISIBILITY / ANIMATIONS
		/// <summary>
		/// Sets whether the diamond visuals are visible or not.
		/// </summary>
		/// <param name="status"></param>
		public abstract void SetVisibility(bool status);
		/// <summary>
		/// Sets the visuals on this affinity diamond based on the specified combatant.
		/// </summary>
		/// <param name="combatant">The combatant who is being represented.</param>
		public abstract void SnapAffinityVisuals(Combatant combatant);
		/// <summary>
		/// Increments the mood/affinity of the given type for the specified combatant by 1 point.
		/// </summary>
		/// <param name="combatant">The combatant to increment the affinity for.</param>
		/// <param name="moodType">The mood type to animate the increment for.</param>
		/// <param name="oldSeverity">The old severity of the mood.</param>
		/// <param name="newSeverity">The new severity of the mood.</param>
		public abstract void AnimateAffinityIncrement(Combatant combatant, CombatantMoodType moodType, CombatantMoodSeverityType oldSeverity, CombatantMoodSeverityType newSeverity);
		#endregion

	}


}