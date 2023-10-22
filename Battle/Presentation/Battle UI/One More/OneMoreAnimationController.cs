using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// This is effectively what controls the "one more" animation on screen.
	/// </summary>
	public class OneMoreAnimationController : MonoBehaviour {

		public static OneMoreAnimationController instance;

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time it should take for the one more graphic to tween in.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private float graphicTweenInTime;
		/// <summary>
		/// The amount of time for the graphic to wait in the middle of the screen.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private float graphicPauseTime;
		/// <summary>
		/// The amount of time to take for the graphic to fade in.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private float graphicFadeTime;
		/// <summary>
		/// The amount of time for the anime lines to be on screen before fading them out.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private float animeLinesWaitTime;
		/// <summary>
		/// The amount of time to take for the anime lines to fade out.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private float animeLinesFadeTime;
		/// <summary>
		/// The ease type to use for the graphic tweening in.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private Ease graphicTweenInEaseType;
		/// <summary>
		/// The ease type to use for the graphic tweening out.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private Ease graphicTweenOutEaseType;
		/// <summary>
		/// The initial color that the anime speed lines should be.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private Color initialAnimeSpeedLinesColor;
		/// <summary>
		/// The initial position of the one more graphic.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private Vector2 oneMoreGraphicInitialPosition;
		/// <summary>
		/// The midway position of the one more graphic.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private Vector2 oneMoreGraphicMidPosition;
		/// <summary>
		/// The final position of the one more graphic.
		/// </summary>
		[SerializeField, TabGroup("One More", "Toggles")]
		private Vector2 oneMoreGraphicFinalPosition;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the other visual elements for the one more animation.
		/// </summary>
		[SerializeField, TabGroup("One More", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The image that shows the One More graphic.
		/// </summary>
		[SerializeField, TabGroup("One More", "Scene References")]
		private Image oneMoreImage;
		/// <summary>
		/// The image that shows the anime speed lines.
		/// </summary>
		[SerializeField, TabGroup("One More", "Scene References")]
		private Image animeSpeedLinesImage;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Just resets the animation to its initial state.
		/// </summary>
		private void ResetAnimationState() {
			this.oneMoreImage.GetComponent<RectTransform>().anchoredPosition = this.oneMoreGraphicInitialPosition;
			this.oneMoreImage.color = Color.clear;
			this.animeSpeedLinesImage.color = this.initialAnimeSpeedLinesColor;
		}
		/// <summary>
		/// Plays the one more animation.
		/// </summary>
		[ShowInInspector, TabGroup("One More", "Debug"), HideInEditorMode]
		public void PlayOneMoreAnimation() {

			// Rollback the animation state.
			this.ResetAnimationState();

			// Turn on the visuals object.
			this.allObjects.SetActive(true);

			// Create a new sequence and append some Tweens.
			Sequence seq = DOTween.Sequence();

			// Append a callback that tweens/fades the graphic in..
			seq.AppendCallback(new TweenCallback(delegate {

				this.oneMoreImage.GetComponent<RectTransform>().DOAnchorPos(
					endValue: this.oneMoreGraphicMidPosition,
					duration: this.graphicTweenInTime,
					snapping: true)
					.SetEase(ease: this.graphicTweenInEaseType);

				this.oneMoreImage.DOColor(endValue: Color.white, duration: this.graphicFadeTime);

			}));

			
			// Wait for an amount of time.
			seq.AppendInterval(interval: this.graphicPauseTime + this.graphicTweenInTime);

			seq.AppendCallback(new TweenCallback(delegate {

				this.oneMoreImage.GetComponent<RectTransform>().DOAnchorPos(
				endValue: this.oneMoreGraphicFinalPosition,
				duration: this.graphicTweenInTime,
				snapping: true)
				.SetEase(ease: this.graphicTweenOutEaseType);

				this.animeSpeedLinesImage.DOColor(endValue: Color.clear, duration: this.animeLinesFadeTime);

			}));

			seq.AppendInterval(interval: graphicTweenInTime);

			// When the tween is complete, turn them off.
			seq.OnComplete(new TweenCallback(delegate {
				this.allObjects.SetActive(false);
			}));

			// Play the sequence.
			seq.Play();
		}
		#endregion

	}


}