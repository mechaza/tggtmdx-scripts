using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Several of the menu windows interacted with in this screen need to be rotated through.
	/// This helps control the animations/state for presenting them.
	/// </summary>
	public class BadgeBoardMenuSlider : MonoBehaviour {

		#region FIELDS - TWEENING : POSITIONS
		/// <summary>
		/// The position the slider's main pivot should be in when at the start.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField, Title("Positions")]
		private Vector2Int sliderStartPos = new Vector2Int();
		/// <summary>
		/// The position the slider's main pivot should be in when presenting the player select.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField]
		private Vector2Int sliderPlayerSelectPos = new Vector2Int();
		/// <summary>
		/// The position the slider's main pivot should be in when presenting the weapon select.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField]
		private Vector2Int sliderWeaponSelectPos = new Vector2Int();
		/// <summary>
		/// The position the slider's main pivot should be in when presenting the weapon select.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField]
		private Vector2Int sliderWeaponActionPos = new Vector2Int();
		/// <summary>
		/// The position the slider's main pivot should be in when presenting the weapon select.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField]
		private Vector2Int sliderBadgeSelectPos = new Vector2Int();
		#endregion

		#region FIELDS - TWEENING : COLORS
		/// <summary>
		/// The color the backing image should be at the start.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField, Title("Colors")]
		private Color backingImageHidingColor = Color.clear;
		/// <summary>
		/// The color the backing image should be when displayed.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField]
		private Color backingImageDisplayColor = Color.white;
		#endregion
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time it should take when tweening to another menu.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField, Title("Timing")]
		private float menuSwitchSlideTime = 0.2f;
		/// <summary>
		/// The amount of time to take when fading the backing image in.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField]
		private float backingImageFadeInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading the backing image out.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField]
		private float backingImageFadeOutTime = 0.5f;
		#endregion

		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the menu options around.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField, Title("Easing")]
		private Ease menuSwitchSlideEaseType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when scaling the backing image in.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField]
		private Ease backingImageScaleInEaseType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when scaling the backing image in.
		/// </summary>
		[TabGroup("Slider", "Tweening"), SerializeField]
		private Ease backingImageScaleOutEaseType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the objects used in this script as children.
		/// </summary>
		[TabGroup("Slider", "Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The RectTransform that should be moved around left and right to show what menu is currently being used.
		/// </summary>
		[TabGroup("Slider", "Scene References"), SerializeField]
		private RectTransform mainPivotRectTransform;
		/// <summary>
		/// The image for the backing of the slider.
		/// </summary>
		[TabGroup("Slider", "Scene References"), SerializeField]
		private Image sliderBackingImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Kills all tweens and completes them.
		/// </summary>
		private void KillAllTweens() {
			// Kill any ongoing tweens on the main stuff.
			this.mainPivotRectTransform.DOKill(complete: true);
			this.sliderBackingImage.DOKill(complete: true);
		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			
			// Kill all tweens.
			this.KillAllTweens();
			
			// Snap the main pivot to its initial position.
			this.SlideMenu(
				sliderMenuType: BadgeBoardSliderMenuType.PlayerSelect,
				snapToPosition: true);
			
			// Snap the backing image to its hiding color.
			this.sliderBackingImage.color = this.backingImageHidingColor;

		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the element using the data contained in the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters containing the information on how this element should be.</param>
		public void Present(BadgeBoardParams boardParams) {
			
			// Kill all tweens.
			this.KillAllTweens();
			
			// Fade the backing image in.
			this.sliderBackingImage.DOColor(
				endValue: this.backingImageDisplayColor, 
				duration: this.backingImageFadeInTime)
				.SetEase(Ease.Linear);
			
		}
		/// <summary>
		/// Dismisses this element from the screen.
		/// </summary>
		/// <param name="boardParams">The board params that were used to create this object.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			
			// Kill all tweens.
			this.KillAllTweens();
			
			// Fade the backing image out.
			this.sliderBackingImage.DOColor(
					endValue: this.backingImageHidingColor, 
					duration: this.backingImageFadeOutTime)
				.SetEase(Ease.Linear);
		}
		#endregion

		#region TWEENING
		/// <summary>
		/// Slides the menus to focus on the one with the specified ID.
		/// </summary>
		/// <param name="sliderMenuType">The menu type to slide over to.</param>
		/// <param name="snapToPosition">Should the animation be skipped and just automatically snapped to?</param>
		public void SlideMenu(BadgeBoardSliderMenuType sliderMenuType, bool snapToPosition = false) {
			
			// Start by killing all tweens.
			this.KillAllTweens();
			
			// Determine what position to move to.
			Vector2Int targetPos = new Vector2Int();
			switch (sliderMenuType) {
				case BadgeBoardSliderMenuType.WeaponSelect:
					targetPos = this.sliderWeaponSelectPos;
					break;
				case BadgeBoardSliderMenuType.WeaponAction:
					targetPos = this.sliderWeaponActionPos;
					break;
				case BadgeBoardSliderMenuType.BadgeSelect:
					targetPos = this.sliderBadgeSelectPos;
					break;
				case BadgeBoardSliderMenuType.PlayerSelect:
					targetPos = this.sliderPlayerSelectPos;
					break;
				default:
					throw new System.Exception("This should never be reached!");
			}
			
			// If snapping, just manually assign the anchored position.
			if (snapToPosition == true) {
				this.mainPivotRectTransform.anchoredPosition = targetPos;
			} else {
				// If not snapping, start up a tween.
				this.mainPivotRectTransform.DOAnchorPos(
					endValue: targetPos, 
					duration: this.menuSwitchSlideTime,
					snapping: true)
					.SetEase(ease: this.menuSwitchSlideEaseType);
			}
			
		}
		#endregion
		
	}
}