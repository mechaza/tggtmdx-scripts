using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// Encapsulates the GameObject for the arrows that appear on the name banner.
	/// These signal if left/right can be pressed to shift combatant focus.
	/// </summary>
	public class CombatantAnalysisDXBannerArrow : MonoBehaviour {

		#region FIELDS - TWEENING : POSITION
		/// <summary>
		/// The position the arrow should be in its initial state.
		/// </summary>
		[Title("Positions")]
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private Vector2Int arrowInitialPos = new Vector2Int();
		/// <summary>
		/// The position the arrow should be in its target state.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private Vector2Int arrowTargetPos = new Vector2Int();
		#endregion
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to complete one loop of the arrow's animation.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private float arrowTweenTime = 0.4f;
		/// <summary>
		/// The amount of time to take when fading in the front image.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private float arrowFrontImageFadeInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading in the dropshadow image.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private float arrowDropshadowImageFadeInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading out the front image.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private float arrowFrontImageFadeOutTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading out the dropshadow image.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private float arrowDropshadowImageFadeOutTime = 0.5f;
		#endregion

		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use for the arrow's animation.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private Ease arrowEaseType = Ease.InOutCirc;
		#endregion

		#region FIELDS - TWEENING : COLORS
		/// <summary>
		/// The color to use for the front image of the arrow when displayed.
		/// </summary>
		[Title("Color")]
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private Color arrowFrontImageColor = Color.white;
		/// <summary>
		/// The color to use for the dropshadow image of the arrow when displayed.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Tweening")]
		private Color arrowDropshadowImageColor = Color.black;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the visuals for this banner arrow.
		/// My intention is that this should be a child of the main pivot.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Scene References")]
		private GameObject allVisuals;
		/// <summary>
		/// The image used for the arrow's front.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Scene References")]
		private Image arrowFrontImage;
		/// <summary>
		/// The image used for the arrow's dropshadow.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Scene References")]
		private Image arrowDropshadowImage;
		/// <summary>
		/// The RectTransform used to manipulate the arrow.
		/// </summary>
		[SerializeField, TabGroup("Arrow", "Scene References")]
		private RectTransform arrowMainPivotRectTransform;
		#endregion

		#region UNITY CALLS
		private void Start() {
			
		}
		#endregion

		#region STATE MANIPULATION
		/// <summary>
		/// Set whether or not the arrow should be visible.
		/// </summary>
		/// <param name="status">Should the arrow be visible or not?</param>
		/// <param name="snap">Should the visibility be "snapped"? Fade will not occur.</param>
		public void SetVisible(bool status, bool snap = false) {

			// Kill any tweens on the images themselves.
			this.arrowFrontImage.DOKill(complete: true);
			this.arrowDropshadowImage.DOKill(complete: true);
			
			if (status == true) {
				// If the visibility is on, reset the animation.
				this.RestartAnimation();
			} else {
				// If visibility is off, kill any potential tweens on the main pivot.
				this.arrowMainPivotRectTransform.DOKill(complete: true);
			}
			
			// Determine the different parameters that need to be used.
			Color frontColor = status ? this.arrowFrontImageColor : Color.clear;
			Color dropshadowColor = status ? this.arrowDropshadowImageColor : Color.clear;
			float frontFadeTime = status ? this.arrowFrontImageFadeInTime : this.arrowFrontImageFadeOutTime;
			float dropshadowFadeTime = status ? this.arrowDropshadowImageFadeInTime : this.arrowDropshadowImageFadeOutTime;

			// If snapping the visibility without fading...
			if (snap == true) {
				// ...just set the color.
				this.arrowFrontImage.color = frontColor;
				this.arrowDropshadowImage.color = dropshadowColor;
			} else {
				// If snap is false, that means the visuals should fade.
				this.arrowFrontImage.DOColor(endValue: frontColor, duration: frontFadeTime).SetEase(Ease.Linear);
				this.arrowDropshadowImage.DOColor(endValue: dropshadowColor, duration: dropshadowFadeTime).SetEase(Ease.Linear);
			}
			
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Restarts the animation.
		/// I need to make sure the left/right arrows do not get out of sync.
		/// </summary>
		private void RestartAnimation() {
			
			// Kill any animations that may be running and reset the position.
			this.arrowMainPivotRectTransform.DOKill(complete: true);
			this.arrowMainPivotRectTransform.anchoredPosition = this.arrowInitialPos;
			
			// Set up the tween that repeats the pointing motion.
			this.arrowMainPivotRectTransform.DOAnchorPos(
					endValue: this.arrowTargetPos,
					duration: this.arrowTweenTime,
					snapping: true)
				.SetEase(ease: this.arrowEaseType)
				.SetLoops(loops: -1)
				.OnComplete(delegate {
					// Upon completion of each loop, reset it back to its initial position.
					this.arrowMainPivotRectTransform.anchoredPosition = this.arrowInitialPos;
				});
		}
		#endregion
		
	}
}

/*#region STATE MANIPULATION
		/// <summary>
		/// Set whether or not the arrow should be visible.
		/// </summary>
		/// <param name="status">Should the arrow be visible or not?</param>
		/// <param name="snap">Should the visibility be "snapped"? Fade will not occur.</param>
		public void SetVisible(bool status, bool snap = false) {

			

			if (status == false && snap == true) {
				// If turning off and snapping, kill the tweens and turn off the visuals.
				this.arrowFrontImage.DOKill(complete: true);
				this.arrowDropshadowImage.DOKill(complete: true);
				this.arrowMainPivotRectTransform.DOKill(complete: true);
				this.allVisuals.SetActive(false);
				return;
			}
			

			
			// Determine the different parameters that need to be used.
			Color frontColor = status ? this.arrowFrontImageColor : Color.clear;
			Color dropshadowColor = status ? this.arrowDropshadowImageColor : Color.clear;
			float frontFadeTime = status ? this.arrowFrontImageFadeInTime : this.arrowFrontImageFadeOutTime;
			float dropshadowFadeTime = status ? this.arrowDropshadowImageFadeInTime : this.arrowDropshadowImageFadeOutTime;

			// If snapping the visibility without fading...
			if (snap == true) {
				// ...just set the color.
				this.arrowFrontImage.color = frontColor;
				this.arrowDropshadowImage.color = dropshadowColor;
			} else {
				// If snap is false, that means the visuals should fade.
				this.arrowFrontImage.DOColor(endValue: frontColor, duration: frontFadeTime).SetEase(Ease.Linear);
				this.arrowDropshadowImage.DOColor(endValue: dropshadowColor, duration: dropshadowFadeTime).SetEase(Ease.Linear);
			}
						
			
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Restarts the animation.
		/// I need to make sure the left/right arrows do not get out of sync.
		/// </summary>
		private void RestartAnimation() {
			
			// Kill any animations that may be running and reset the position.
			this.arrowMainPivotRectTransform.DOKill(complete: true);
			this.arrowMainPivotRectTransform.anchoredPosition = this.arrowInitialPos;
			
			// Set up the tween that repeats the pointing motion.
			this.arrowMainPivotRectTransform.DOAnchorPos(
					endValue: this.arrowTargetPos,
					duration: this.arrowTweenTime,
					snapping: true)
				.SetEase(ease: this.arrowEaseType)
				.SetLoops(loops: -1)
				.OnComplete(delegate {
					// Upon completion of each loop, reset it back to its initial position.
					this.arrowMainPivotRectTransform.anchoredPosition = this.arrowInitialPos;
				});
		}
		#endregion*/