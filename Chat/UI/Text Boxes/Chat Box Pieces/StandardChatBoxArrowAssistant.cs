using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

namespace Grawly.Chat {

	/// <summary>
	/// This is just a quick class so I can easily abstract out how I want the arrows on the chat box to move around.
	/// </summary>
	public class StandardChatBoxArrowAssistant : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to take when tweening the dropshadows around.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Assistant", "Toggles")]
		private float tweenTime = 0.2f;
		/// <summary>
		/// The easing to use when moving the dropshadows around.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Assistant", "Toggles")]
		private Ease tweenEaseType = Ease.OutCirc;
		#endregion

		#region FIELDS - POSITIONS
		/// <summary>
		/// The initial position for the left front arrow.
		/// </summary>
		[Title("Active Positions")]
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 leftArrowFrontActivePosition = new Vector2();
		/// <summary>
		/// The initial position for the left dropshadow arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 leftArrowDropshadowActivePosition = new Vector2();
		/// <summary>
		/// The initial position for the right front arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 rightArrowFrontActivePosition = new Vector2();
		/// <summary>
		/// The initial position for the right dropshadow arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 rightArrowDropshadowActivePosition = new Vector2();
		/// <summary>
		/// The initial position for the right front arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 topRightArrowFrontActivePosition = new Vector2();
		/// <summary>
		/// The initial position for the top right dropshadow arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 topRightArrowDropshadowActivePosition = new Vector2();
		/// <summary>
		/// The hiding position for the left front arrow.
		/// </summary>
		[Title("Hiding Positions")]
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 leftArrowFrontHidingPosition = new Vector2();
		/// <summary>
		/// The hiding position for the left dropshadow arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 leftArrowDropshadowHidingPosition = new Vector2();
		/// <summary>
		/// The hiding position for the right front arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 rightArrowFrontHidingPosition = new Vector2();
		/// <summary>
		/// The hiding position for the right dropshadow arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 rightArrowDropshadowHidingPosition = new Vector2();
		/// <summary>
		/// The hiding position for the top right front arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 topRightArrowFrontHidingPosition = new Vector2();
		/// <summary>
		/// The hiding position for the top right dropshadow arrow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Positions")]
		private Vector2 topRightArrowDropshadowHidingPosition = new Vector2();
		#endregion

		#region FIELDS - SCENE REFERENCES : RECTTRANSFORMS
		/// <summary>
		/// The RectTransform that has all of the other arrows.
		/// </summary>
		[Title("Groups")]
		[SerializeField, TabGroup("Assistant", "Scene References")]
		private RectTransform dropshadowRectTransform;
		/// <summary>
		/// The RectTransform for the left arrow front.
		/// </summary>
		[Title("Arrows")]
		[SerializeField, TabGroup("Assistant", "Scene References")]
		private RectTransform leftArrowFrontRectTransform;
		/// <summary>
		/// The RectTransform for the left arrow dropshadow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Scene References")]
		private RectTransform leftArrowDropshadowRectTransform;
		/// <summary>
		/// The RectTransform for the right arrow front.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Scene References")]
		private RectTransform rightArrowFrontRectTransform;
		/// <summary>
		/// The RectTransform for the right arrow dropshadow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Scene References")]
		private RectTransform rightArrowDropshadowRectTransform;
		/// <summary>
		/// The RectTransform for the top right arrow front.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Scene References")]
		private RectTransform topRightArrowFrontRectTransform;
		/// <summary>
		/// The RectTransform for the top right arrow dropshadow.
		/// </summary>
		[SerializeField, TabGroup("Assistant", "Scene References")]
		private RectTransform topRightArrowDropshadowRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The list of images that make up the front graphics. 
		/// I primarily just want this so I can fade the colors for the option picker but you never know.
		/// </summary>
		[Title("Images")]
		[SerializeField, TabGroup("Assistant", "Scene References")]
		private List<Image> frontBoxImages = new List<Image>();
		#endregion

		#region UNITY CALLS
		private void Start() {
			// Totally reset the arrow position upon start.
			this.SetArrowPosition(ChatSpeakerPositionType.None, snapping: true);
		}
		#endregion

		#region PUBLIC CALLS
		/// <summary>
		/// Sets the arrows to the position where it shows the specified position to be speaking.
		/// </summary>
		/// <param name="currentSpeakerPosition">The position of the person who is currently speaking.</param>
		/// <param name="snapping">Should this be tweened, or should it be snapped instantly?</param>
		[Button]
		public void SetArrowPosition(ChatSpeakerPositionType currentSpeakerPosition, bool snapping = false) {

			// Debug.Log("Setting arrow position to " + currentSpeakerPosition.ToString());

			switch (currentSpeakerPosition) {
				case ChatSpeakerPositionType.None:
					this.TweenLeftArrow(visible: false, snapping: snapping);
					this.TweenRightArrow(visible: false, snapping: snapping);
					this.TweenTopRightArrow(visible: false, snapping: snapping);
					break;
				case ChatSpeakerPositionType.Left:
					this.TweenLeftArrow(visible: true, snapping: snapping);
					this.TweenRightArrow(visible: false, snapping: snapping);
					this.TweenTopRightArrow(visible: false, snapping: snapping);
					break;
				case ChatSpeakerPositionType.Right:
					this.TweenLeftArrow(visible: false, snapping: snapping);
					this.TweenRightArrow(visible: true, snapping: snapping);
					this.TweenTopRightArrow(visible: false, snapping: snapping);
					break;
				case ChatSpeakerPositionType.TopRight:
					this.TweenLeftArrow(visible: false, snapping: snapping);
					this.TweenRightArrow(visible: false, snapping: snapping);
					this.TweenTopRightArrow(visible: true, snapping: snapping);
					break;
				default:
					Debug.LogWarning("Not able to tween the arrow position with the given enum type.");
					break;
			}
		}
		#endregion

		#region TWEENING CALLS
		/// <summary>
		/// Tweens the left arrow to the specified visibility.
		/// </summary>
		/// <param name="visible">Whether the left arrow should be visible or not.</param>
		/// <param name="snapping">Should this be tweened, or should it be snapped instantly?</param>
		private void TweenLeftArrow(bool visible, bool snapping = false) {

			// Kill any tweens on the transforms if there are any.
			this.leftArrowFrontRectTransform.DOKill(complete: true);
			this.leftArrowDropshadowRectTransform.DOKill(complete: true);

			// If this needs to happen instantly, just set the values and move out.
			if (snapping == true) {
				this.leftArrowFrontRectTransform.anchoredPosition = visible == true ? this.leftArrowFrontActivePosition : this.leftArrowFrontHidingPosition;
				this.leftArrowDropshadowRectTransform.anchoredPosition = visible == true ? this.leftArrowDropshadowActivePosition : this.leftArrowDropshadowHidingPosition;
				return;
			}

			// Tween the front left arrow.
			this.leftArrowFrontRectTransform.DOAnchorPos(
				endValue: visible == true ? this.leftArrowFrontActivePosition : this.leftArrowFrontHidingPosition, 
				duration: this.tweenTime, 
				snapping: true)
				.SetEase(ease: this.tweenEaseType);

			// Tween the front dropshadow arrow.
			this.leftArrowDropshadowRectTransform.DOAnchorPos(
				endValue: visible == true ? this.leftArrowDropshadowActivePosition : this.leftArrowDropshadowHidingPosition,
				duration: this.tweenTime,
				snapping: true)
				.SetEase(ease: this.tweenEaseType);
		}
		/// <summary>
		/// Tweens the right arrow to the specified visibility.
		/// </summary>
		/// <param name="visible">Whether the right arrow should be visible or not.</param>
		/// <param name="snapping">Should this be tweened, or should it be snapped instantly?</param>
		private void TweenRightArrow(bool visible, bool snapping = false) {

			// Kill any tweens on the transforms if there are any.
			this.rightArrowFrontRectTransform.DOKill(complete: true);
			this.rightArrowDropshadowRectTransform.DOKill(complete: true);

			// If this needs to happen instantly, just set the values and move out.
			if (snapping == true) {
				this.rightArrowFrontRectTransform.anchoredPosition = visible == true ? this.rightArrowFrontActivePosition : this.rightArrowFrontHidingPosition;
				this.rightArrowDropshadowRectTransform.anchoredPosition = visible == true ? this.rightArrowDropshadowActivePosition : this.rightArrowDropshadowHidingPosition;
				return;
			}

			// Tween the front left arrow.
			this.rightArrowFrontRectTransform.DOAnchorPos(
				endValue: visible == true ? this.rightArrowFrontActivePosition : this.rightArrowFrontHidingPosition,
				duration: this.tweenTime,
				snapping: true)
				.SetEase(ease: this.tweenEaseType);

			// Tween the front dropshadow arrow.
			this.rightArrowDropshadowRectTransform.DOAnchorPos(
				endValue: visible == true ? this.rightArrowDropshadowActivePosition : this.rightArrowDropshadowHidingPosition,
				duration: this.tweenTime,
				snapping: true)
				.SetEase(ease: this.tweenEaseType);
		}
		/// <summary>
		/// Tweens the top right arrow to the specified visibility.
		/// </summary>
		/// <param name="visible">Whether the top right arrow should be visible or not.</param>
		/// <param name="snapping">Should this be tweened, or should it be snapped instantly?</param>
		private void TweenTopRightArrow(bool visible, bool snapping = false) {

			// Kill any tweens on the transforms if there are any.
			this.topRightArrowFrontRectTransform.DOKill(complete: true);
			this.topRightArrowDropshadowRectTransform.DOKill(complete: true);

			// If this needs to happen instantly, just set the values and move out.
			if (snapping == true) {
				this.topRightArrowFrontRectTransform.anchoredPosition = visible == true ? this.topRightArrowFrontActivePosition : this.topRightArrowFrontHidingPosition;
				this.topRightArrowDropshadowRectTransform.anchoredPosition = visible == true ? this.topRightArrowDropshadowActivePosition : this.topRightArrowDropshadowHidingPosition;
				return;
			}

			// Tween the front left arrow.
			this.topRightArrowFrontRectTransform.DOAnchorPos(
				endValue: visible == true ? this.topRightArrowFrontActivePosition : this.topRightArrowFrontHidingPosition,
				duration: this.tweenTime,
				snapping: true)
				.SetEase(ease: this.tweenEaseType);

			// Tween the front dropshadow arrow.
			this.topRightArrowDropshadowRectTransform.DOAnchorPos(
				endValue: visible == true ? this.topRightArrowDropshadowActivePosition : this.topRightArrowDropshadowHidingPosition,
				duration: this.tweenTime,
				snapping: true)
				.SetEase(ease: this.tweenEaseType);
		}
		#endregion

		#region COLOR CALLS
		/// <summary>
		/// Sets the color on the box images. This is handy for things like the option picker which may want to fade it out.
		/// </summary>
		/// <param name="color">The color to set it to.</param>
		/// <param name="time">The amount of time to take when tweening to the given color.</param>
		public void SetBoxColor(Color color, float time) {
			this.frontBoxImages.ForEach(i => {
				i.DOKill(complete: true); // ensures the color doesn't fuck up if I call this function before its done tweening the color from a previous call
				i.DOColor(endValue: color, duration: time);
			});
		}
		#endregion

	}


}