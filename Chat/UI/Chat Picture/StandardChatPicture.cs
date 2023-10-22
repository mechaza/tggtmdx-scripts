using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine.UI;

namespace Grawly.Chat {

	/// <summary>
	/// The standard way of using the ChatPicture.
	/// </summary>
	public class StandardChatPicture : ChatPicture {

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time to take when tweening the chat picture in by default.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Picture", "Toggles")]
		private float chatPictureDefaultTweenInTime = 0.3f;
		/// <summary>
		/// The amount of time to take when tweening the chat picture out by default.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private float chatPictureDefaultTweenOutTime = 0.3f;
		/// <summary>
		/// The amount of time to take when scaling the chat picture in by default.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private float chatPictureDefaultScaleInTime = 0.3f;
		/// <summary>
		/// The amount of time to take when scaling the chat picture out by default.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private float chatPictureDefaultScaleOutTime = 0.3f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The ease type to use when tweening the chat picture in by default.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Picture", "Toggles")]
		private Ease chatPictureDefaultPositionInEaseType = Ease.InQuint;
		/// <summary>
		/// The ease type to use when tweening the chat picture out by default.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private Ease chatPictureDefaultPositionOutEaseType = Ease.OutQuint;
		/// <summary>
		/// The ease type to use when scaling the chat picture in by default.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private Ease chatPictureDefaultScaleInEaseType = Ease.InQuint;
		/// <summary>
		/// The ease type to use when scaling the chat picture out by default.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private Ease chatPictureDefaultScaleOutEaseType = Ease.OutQuint;
		#endregion

		#region FIELDS - TOGGLES : ROTATION
		/// <summary>
		/// The rotation the chat picture should be in when about to start.
		/// </summary>
		[Title("Rotation")]
		[SerializeField, TabGroup("Picture", "Toggles")]
		private float chatPictureStartRotation = 0f;
		/// <summary>
		/// The rotation the chat picture should be in when activated.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private float chatPictureActiveRotation = 0f;
		/// <summary>
		/// The rotation the chat picture should be in when ending.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private float chatPictureEndRotation = 0f;
		#endregion

		#region FIELDS - TOGGLES : POSITIONS
		/// <summary>
		/// The position the chat picture when it should be "on screen".
		/// </summary>
		[Title("Positions")]
		[SerializeField, TabGroup("Picture", "Toggles")]
		private Vector2 chatPictureActivePosition = new Vector2();
		/// <summary>
		/// The position the chat picture should be when its about to tween in.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private Vector2 chatPictureStartPosition = new Vector2();
		/// <summary>
		/// The position the chat picture should tween to when its tweening out.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Toggles")]
		private Vector2 chatPictureEndPosition = new Vector2();
		#endregion

		#region FIELDS - SCENE REFERENCES : PIVOTS
		/// <summary>
		/// The main pivot for the chat picture.
		/// </summary>
		[Title("Pivots")]
		[SerializeField, TabGroup("Picture", "Scene References")]
		private RectTransform chatPictureMainPivotRectTransform;
		/// <summary>
		/// The child pivot for the rect transform.
		/// </summary>
		[SerializeField, TabGroup("Picture", "Scene References")]
		private RectTransform chatPictureChildPivotRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The actual image for the chat picture.
		/// </summary>
		[Title("Images")]
		[SerializeField, TabGroup("Picture", "Scene References")]
		private Image chatPictureImage;
		#endregion

		#region UNITY CALLS
		private void Start() {
			// Totally reset the state on start.
			this.ResetState();
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Presents a picture onto the chat.
		/// </summary>
		/// <param name="chatPictureParams">The parameters containing the picture as well as any other data needed.</param>
		public override void PresentPicture(ChatPictureParams chatPictureParams) {

			// Reset the state to its initial values.
			this.ResetState();

			// Set the sprite on the image.
			// this.chatPictureImage.sprite = chatPictureParams.pictureSprite;
			this.chatPictureImage.overrideSprite = chatPictureParams.pictureSprite;

			// Tween the main pivot in.
			this.chatPictureMainPivotRectTransform.DOAnchorPos(
				endValue: this.chatPictureActivePosition, 
				duration: this.chatPictureDefaultTweenInTime, 
				snapping: true)
				.SetEase(ease: this.chatPictureDefaultPositionInEaseType);

			// Also rotate it.
			this.chatPictureMainPivotRectTransform.DORotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.chatPictureActiveRotation), 
				duration: this.chatPictureDefaultTweenInTime, 
				mode: RotateMode.FastBeyond360)
				.SetEase(ease: this.chatPictureDefaultPositionInEaseType);

			// Also scale it in.
			this.chatPictureMainPivotRectTransform.DOScale(
				endValue: 1f,
				duration: this.chatPictureDefaultScaleInTime)
				.SetEase(ease: this.chatPictureDefaultScaleInEaseType);



		}
		/// <summary>
		/// Dismisses the chat picture.
		/// </summary>
		/// <param name="chatPictureParams">The parameters that were used in constructing this chat picture.</param>
		protected override void DismissPicture(ChatPictureParams chatPictureParams) {

			// Kill any tweens on the main pivot in case the picture gets dismissed before the tween is complete.
			this.chatPictureMainPivotRectTransform.DOKill(complete: true);

			// Tween the main pivot out.
			this.chatPictureMainPivotRectTransform.DOAnchorPos(
				endValue: this.chatPictureEndPosition,
				duration: this.chatPictureDefaultTweenOutTime,
				snapping: true)
				.SetEase(ease: this.chatPictureDefaultPositionOutEaseType);

			// Also rotate it.
			this.chatPictureMainPivotRectTransform.DORotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.chatPictureEndRotation),
				duration: this.chatPictureDefaultTweenOutTime,
				mode: RotateMode.Fast)
				.SetEase(ease: this.chatPictureDefaultPositionOutEaseType);

			// Also scale it out.
			this.chatPictureMainPivotRectTransform.DOScale(
				endValue: 0f,
				duration: this.chatPictureDefaultScaleOutTime)
				.SetEase(ease: this.chatPictureDefaultScaleOutEaseType);

		}
		#endregion

		#region SPECIFIC ANIMATIONS
		/// <summary>
		/// Reverts the ChatPicture to the state where it should be before it gets tweened in.
		/// </summary>
		public override void ResetState() {
			// Kill any tweens on the main pivot.
			this.chatPictureMainPivotRectTransform.DOKill(complete: true);
			// Reset the position, rotation, and scale of the main pivot.
			this.chatPictureMainPivotRectTransform.anchoredPosition = this.chatPictureStartPosition;
			this.chatPictureMainPivotRectTransform.localEulerAngles = new Vector3(x: 0f, y: 0f, z: this.chatPictureStartRotation);
			this.chatPictureMainPivotRectTransform.localScale = Vector3.zero;
		}
		#endregion

		#region SPECIAL EFFECTS
		/// <summary>
		/// Shakes the picture vigorously.
		/// </summary>
		/// <param name="time">The amount of time to spend shaking it like you mean it.</param>
		/// <param name="magnitude">How powerful is your groove?</param>
		public override void Shake(float time = 0.2F, float magnitude = 10) {
			throw new System.NotImplementedException();
		}
		#endregion


	}


}