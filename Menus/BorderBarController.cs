using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Grawly.Menus {

	/// <summary>
	/// I got sick of having to re-do this code all time for the fucking borders so!!! whatever.
	/// </summary>
	public class BorderBarController : MonoBehaviour {

		#region FIELDS - TOGGLES : POSITIONING
		/// <summary>
		/// The position for the top bar when its hiding.
		/// </summary>
		[TabGroup("Bars", "Toggles"), Title("Positioning"), SerializeField]
		private Vector2 topBarInitialPosition;
		/// <summary>
		/// The position for the top bar when its hiding.
		/// </summary>
		[TabGroup("Bars", "Toggles"), SerializeField]
		private Vector2 bottomBarInitialPosition;
		/// <summary>
		/// The position for the top bar when its being shown.
		/// </summary>
		[TabGroup("Bars", "Toggles"), SerializeField]
		private Vector2 topBarFinalPosition;
		/// <summary>
		/// The position for the bottom bar when its being shown.
		/// </summary>
		[TabGroup("Bars", "Toggles"), SerializeField]
		private Vector2 bottomBarFinalPosition;
		#endregion

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time for the bars to tween in.
		/// </summary>
		[TabGroup("Bars", "Toggles"), Title("Timing"), SerializeField]
		private float tweenInTime = 0.5f;
		/// <summary>
		/// The amount of time for the bars to tween out.
		/// </summary>
		[TabGroup("Bars", "Toggles"), SerializeField]
		private float tweenOutTime = 0.5f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The easing when tweening the bars in.
		/// </summary>
		[TabGroup("Bars", "Toggles"), Title("Easing"), SerializeField]
		private Ease tweenInEaseType = Ease.InOutQuint;
		/// <summary>
		/// The easing when tweening the bars out.
		/// </summary>
		[TabGroup("Bars", "Toggles"), SerializeField]
		private Ease tweenOutEaseType = Ease.InOutQuint;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The image for the top bar.
		/// </summary>
		[TabGroup("Bars", "Scene References"), Title("Images"), SerializeField]
		private Image topBar;
		/// <summary>
		/// The image for the bottom bar.
		/// </summary>
		[TabGroup("Bars", "Scene References"), SerializeField]
		private Image bottomBar;
		#endregion

		#region UNITY CALLS
		private void Start() {
			this.ResetBars();
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Resets the bars to their inital positions.
		/// </summary>
		private void ResetBars() {
			this.topBar.GetComponent<RectTransform>().anchoredPosition = this.topBarInitialPosition;
			this.bottomBar.GetComponent<RectTransform>().anchoredPosition = this.bottomBarInitialPosition;
		}
		/// <summary>
		/// Tweens the bars to be visible or not.
		/// </summary>
		/// <param name="visible">Should the bars be visible or not?</param>
		public void SetVisible(bool visible) {
			if (visible) {
				this.TweenIn();
			} else {
				this.TweenOut();
			}
		}
		/// <summary>
		/// Tweens the bars in.
		/// </summary>
		[Button]
		private void TweenIn() {
			this.topBar.GetComponent<RectTransform>().DOKill(complete: true);
			this.bottomBar.GetComponent<RectTransform>().DOKill(complete: true);

			// Snap the bars to the initial positions.
			this.topBar.GetComponent<RectTransform>().anchoredPosition = this.topBarInitialPosition;
			this.bottomBar.GetComponent<RectTransform>().anchoredPosition = this.bottomBarInitialPosition;

			// Tween them in.
			this.topBar.GetComponent<RectTransform>().DOAnchorPos(
				endValue: this.topBarFinalPosition, 
				duration: this.tweenInTime, 
				snapping: true)
				.SetEase(ease: this.tweenInEaseType);

			this.bottomBar.GetComponent<RectTransform>().DOAnchorPos(
				endValue: this.bottomBarFinalPosition,
				duration: this.tweenInTime,
				snapping: true)
				.SetEase(ease: this.tweenInEaseType);

		}
		/// <summary>
		/// Tweens the bars out.
		/// </summary>
		[Button]
		private void TweenOut() {
			this.topBar.GetComponent<RectTransform>().DOKill(complete: true);
			this.bottomBar.GetComponent<RectTransform>().DOKill(complete: true);

			// Snap the bars to the final positions.
			this.topBar.GetComponent<RectTransform>().anchoredPosition = this.topBarFinalPosition;
			this.bottomBar.GetComponent<RectTransform>().anchoredPosition = this.bottomBarFinalPosition;

			// Tween them out.
			this.topBar.GetComponent<RectTransform>().DOAnchorPos(
				endValue: this.topBarInitialPosition,
				duration: this.tweenOutTime,
				snapping: true)
				.SetEase(ease: this.tweenOutEaseType);

			this.bottomBar.GetComponent<RectTransform>().DOAnchorPos(
				endValue: this.bottomBarInitialPosition,
				duration: this.tweenOutTime,
				snapping: true)
				.SetEase(ease: this.tweenOutEaseType);

		}
		#endregion

	}


}