using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Grawly.Menus.Input;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Battle;

namespace Grawly.Menus.NameEntry {

	/// <summary>
	/// This is the bust up that appears on the screen when it's their turn for a name.
	/// </summary>
	public class NameEntryBustUp : MonoBehaviour {

		#region FIELDS - TOGGLES : POSITIONING
		/// <summary>
		/// The position the bust up should be in when they are introduced to the player.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), Title("Positioning"), SerializeField]
		private Vector2 bustUpMainPivotIntroductionPosition;
		/// <summary>
		/// The position the bust up should be in wh
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private Vector2 bustUpMainPivotNameEntryPosition;
		/// <summary>
		/// The position the bust up should be when it's off screen.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private Vector2 bustUpMainPivotHidingPosition;
		/// <summary>
		/// The position the dropshadow should be in when its being shown.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private Vector2 bustUpDropshadowVisiblePosition;
		/// <summary>
		/// The position the dropshadow should be in when its behind the bust up.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private Vector2 bustUpDropshadowHidingPosition;
		#endregion
	
		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time for the bust up to tween into the introduction position.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), Title("Timing"), SerializeField]
		private float bustUpTweenIntoIntroductionPositionTime = 1f;
		/// <summary>
		/// The amount of time for the bust up to tween into the name entry position.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private float bustUpTweenIntoNameEntryPositionTime = 1f;
		/// <summary>
		/// The amount of time for the bust up to tween into the hiding position.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private float bustUpTweenIntoHidingPositionTime = 1f;
		/// <summary>
		/// The amount of time to take when tweening the dropshadow visible.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private float bustUpDropshadowTweenVisibleTime = 1f;
		/// <summary>
		/// The amount of time to take when tweening the dropshadow visible.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private float bustUpDropshadowTweenHidingTime = 1f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The easing to use when tweening the bust up into the introduction position.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), Title("Easing"), SerializeField]
		private Ease bustUpTweenIntoIntroductionPositionEaseType = Ease.InOutQuart;
		/// <summary>
		/// The easing to use when tweening the bust up into the name entry position.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"),SerializeField]
		private Ease bustUpTweenIntoNameEntryPositionEaseType = Ease.InOutQuart;
		/// <summary>
		/// The easing to use when tweening the bust up into the name entry position.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private Ease bustUpTweenIntoHidingPositionEaseType = Ease.InOutQuart;
		/// <summary>
		/// The easing to use when tweening the dropshadow to be visible.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private Ease bustUpTweenDropshadowVisibleEaseType = Ease.InOutQuart;
		/// <summary>
		/// The easing to use when tweening the dropshadow to be visible.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), SerializeField]
		private Ease bustUpTweenDropshadowHidingEaseType = Ease.InOutQuart;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The rect transform of the main pivot of the bust up.
		/// </summary>
		[TabGroup("Bust Up", "Scene References"), Title("Rect Transforms"), SerializeField]
		private RectTransform bustUpMainPivotRectTransform;
		/// <summary>
		/// The image that actually shows the girl receiving her name.
		/// </summary>
		[TabGroup("Bust Up", "Scene References"), Title("Images"), SerializeField]
		private Image bustUpFrontImage;
		/// <summary>
		/// The dropshadow for the image.
		/// </summary>
		[TabGroup("Bust Up", "Scene References"), SerializeField]
		private Image bustUpDropshadowImage;
		#endregion

		#region UNITY CALLS
		private void Start() {
			this.ResetBustUp();
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Snaps the BustUp to its hiding state.
		/// </summary>
		private void ResetBustUp() {
			this.bustUpFrontImage.color = Color.black;
			this.bustUpMainPivotRectTransform.anchoredPosition = this.bustUpMainPivotHidingPosition;
			this.bustUpDropshadowImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}
		/// <summary>
		/// Positions the bust up to the position associated with the given state type.
		/// </summary>
		/// <param name="stateType">The state type associated with the desired position.</param>
		[Button]
		public void ChangeState(NameEntryBustUpStateType stateType) {

			// Kill any tweens on the bust up's RectTransform and any color tweens.
			this.bustUpFrontImage.DOKill(complete: true);
			this.bustUpMainPivotRectTransform.DOKill(complete: true);
			this.bustUpDropshadowImage.GetComponent<RectTransform>().DOKill(complete: true);

			switch (stateType) {
				case NameEntryBustUpStateType.Hiding:

					this.bustUpMainPivotRectTransform.DOAnchorPos(
						endValue: this.bustUpMainPivotHidingPosition,
						duration: this.bustUpTweenIntoHidingPositionTime, 
						snapping: true)
						.SetEase(ease: this.bustUpTweenIntoHidingPositionEaseType);

					this.bustUpDropshadowImage.GetComponent<RectTransform>().DOAnchorPos(
						endValue: this.bustUpDropshadowHidingPosition,
						duration: this.bustUpDropshadowTweenHidingTime,
						snapping: true)
						.SetEase(ease: this.bustUpTweenDropshadowHidingEaseType);

					break;
				case NameEntryBustUpStateType.Introduction:

					// Snap the color to black and tween it to white over a few seconds.
					this.bustUpFrontImage.color = Color.clear;
					this.bustUpFrontImage.DOColor(endValue: Color.white, duration: 5f);
					
					// Also clear the dropshadow.
					this.bustUpDropshadowImage.color = Color.clear;

					// Snap the main pivot to the intro position.
					this.bustUpMainPivotRectTransform.anchoredPosition = this.bustUpMainPivotIntroductionPosition;

					break;
				case NameEntryBustUpStateType.NameEntry:

					// Snap the dropshadow to the correct position and also set its color to visible.
					this.bustUpDropshadowImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
					this.bustUpDropshadowImage.color = Color.black;

					this.bustUpMainPivotRectTransform.DOAnchorPos(
						endValue: this.bustUpMainPivotNameEntryPosition,
						duration: this.bustUpTweenIntoNameEntryPositionTime,
						snapping: true)
						.SetEase(ease: this.bustUpTweenIntoNameEntryPositionEaseType);

					this.bustUpDropshadowImage.GetComponent<RectTransform>().DOAnchorPos(
						endValue: this.bustUpDropshadowVisiblePosition,
						duration: this.bustUpDropshadowTweenVisibleTime,
						snapping: true)
						.SetEase(ease: this.bustUpTweenDropshadowVisibleEaseType);

					break;
				default:
					throw new System.Exception("This should never be reached!");
			}
		}
		#endregion

		#region VISUALS
		/// <summary>
		/// Changes the bust up image to the specified sprite.
		/// </summary>
		/// <param name="sprite">The sprite to use for the bust up.</param>
		public void ChangeBustUpSprite(Sprite sprite) {
			/*	this.bustUpFrontImage.sprite = sprite;
				this.bustUpDropshadowImage.sprite = sprite;*/
			this.bustUpFrontImage.overrideSprite = sprite;
			this.bustUpDropshadowImage.overrideSprite = sprite;
		}
		/// <summary>
		/// Changes the bust up image to the sprite associated with the given player template.
		/// </summary>
		/// <param name="playerTemplate">The player template that contains the sprite for the bust up.</param>
		public void ChangeBustUpSprite(PlayerTemplate playerTemplate) {
			// Just call the version that takes the bust up sprite.
			this.ChangeBustUpSprite(sprite: playerTemplate.bustUp);
		}
		#endregion

		#region ENUM DEFINITIONS
		/// <summary>
		/// The different states that this bust up can exist within.
		/// </summary>
		public enum NameEntryBustUpStateType {
			Hiding = 0,
			Introduction = 1,
			NameEntry = 2,
		}
		#endregion

	}


}