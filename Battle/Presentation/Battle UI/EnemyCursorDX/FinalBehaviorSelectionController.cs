using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// This is probably overkill but this controls the little move you see when picking a combatant.
	/// </summary>
	public class FinalBehaviorSelectionController : MonoBehaviour {

		public static FinalBehaviorSelectionController instance;

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to take when tweening the object in.
		/// </summary>
		[SerializeField, TabGroup("Final Behavior", "Toggles")]
		private float scaleTweenTime;
		/// <summary>
		/// The ease type to use when scaling the object in.
		/// </summary>
		[SerializeField, TabGroup("Final Behavior", "Toggles")]
		private Ease scaleEaseType;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the visuals for the final selection item.
		/// </summary>
		[SerializeField, TabGroup("Final Behavior", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The SuperTextMesh that has the behavior's name on it.
		/// </summary>
		[SerializeField, TabGroup("Final Behavior", "Scene References")]
		private SuperTextMesh finalMoveNameLabel;
		/// <summary>
		/// The SuperTextMesh that has the behavior's cost on it.
		/// </summary>
		[SerializeField, TabGroup("Final Behavior", "Scene References")]
		private SuperTextMesh finalMoveCostLabel;
		/// <summary>
		/// The image that has the behavior's icon.
		/// </summary>
		[SerializeField, TabGroup("Final Behavior", "Scene References")]
		private Image finalBehaviorIconImage;
		#endregion

		#region FIELDS - SCENE REFERENCES FROM THE THINGS ABOVE
		/// <summary>
		/// The rect transform that has all the visuals inside it.
		/// </summary>
		private RectTransform allObjectsRectTransform;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				this.allObjectsRectTransform = this.allObjects.GetComponent<RectTransform>();
			}
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Shows the final behavior with the information from the passed in behavior.
		/// </summary>
		/// <param name="finalBehaviorSelection">The behavior that the player is considering using.</param>
		public void ShowFinalBehavior(BattleBehavior finalBehaviorSelection) {
			// Turn on the visuals but make the scale zero.
			this.allObjects.SetActive(true);
			this.allObjectsRectTransform.localScale = Vector3.zero;
			// Prep the labels/icon on the visuals.
			this.finalMoveNameLabel.Text = finalBehaviorSelection.behaviorName;
			this.finalMoveCostLabel.Text = finalBehaviorSelection.QuantityString;
			// this.finalBehaviorIconImage.sprite = finalBehaviorSelection.Icon;
			this.finalBehaviorIconImage.overrideSprite = finalBehaviorSelection.Icon;
			// Scale the object visuals in.
			this.allObjectsRectTransform.DOScale(endValue: 1f, duration: this.scaleTweenTime).SetEase(ease: this.scaleEaseType);
		}
		/// <summary>
		/// Just hide the behavior's visuals.
		/// </summary>
		public void HideFinalBehavior() {
			this.allObjects.SetActive(false);
		}
		#endregion

	}


}