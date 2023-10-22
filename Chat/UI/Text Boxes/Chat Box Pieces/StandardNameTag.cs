using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

namespace Grawly.Chat {

	/// <summary>
	/// The standard way in which the name tag should be represented.
	/// </summary>
	public class StandardNameTag : ChatBoxNameTag {

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time to scale the name tag in.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private float nameTagScaleInTime = 0.2f;
		/// <summary>
		/// The amount of time to scale the name tag out.
		/// </summary>
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private float nameTagScaleOutTime = 0.2f;
		/// <summary>
		/// The amount of time to rotate the name tag in.
		/// </summary>
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private float nameTagRotateInTime = 0.2f;
		/// <summary>
		/// The amount of time to rotate the name tag out.
		/// </summary>
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private float nameTagRotateOutTime = 0.2f;
		/// <summary>
		/// The amount of time to take when changing the tag color.
		/// </summary>
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private float nameTagColorTweenTime = 0.2f;
		#endregion

		#region FIELDS - TOGGLES : ROTATION
		/// <summary>
		/// The rotation for when the name tag is active.
		/// </summary>
		[Title("Rotation")]
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private float nameTagActiveRotation = 0f;
		/// <summary>
		/// The rotation for when the name tag is hidden.
		/// </summary>
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private float nameTagHiddenRotation;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The type of easing to use when scaling the name tag in.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private Ease nameTagScaleInEaseType = Ease.OutCirc;
		/// <summary>
		/// The type of easing to use when scaling the name tag out.
		/// </summary>
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private Ease nameTagScaleOutEaseType = Ease.OutCirc;
		/// <summary>
		/// The type of easing to use when rotating the name tag out.
		/// </summary>
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private Ease nameTagRotateInEaseType = Ease.OutCirc;
		/// <summary>
		/// The type of easing to use when rotating the name tag in.
		/// </summary>
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private Ease nameTagRotateOutEaseType = Ease.OutCirc;
		#endregion

		#region FIELDS - POSITIONS
		/// <summary>
		/// As the chat box moves around, the name tag will need to follow
		/// because it is not actually a child of it.
		/// This value is the offset from the base of the ChatBox.
		/// </summary>
		[Title("Offset")]
		[SerializeField, TabGroup("Name Tag", "Toggles")]
		private Vector2 nameTagPositionOffset = new Vector2();
		#endregion

		#region FIELDS - SCENE REFERENCES : RECTTRANSFORMS
		/// <summary>
		/// The main pivot rect transform for the name tag.
		/// </summary>
		[Title("RectTransforms")]
		[SerializeField, TabGroup("Name Tag", "Scene References")]
		private RectTransform nameTagMainPivotRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image for the front of the name tag.
		/// </summary>
		[Title("Images")]
		[SerializeField, TabGroup("Name Tag", "Scene References")]
		private Image nameTagFrontImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : TEXT
		/// <summary>
		/// The SuperTextMesh for the name tag.
		/// </summary>
		[Title("Text")]
		[SerializeField, TabGroup("Name Tag", "Scene References")]
		private SuperTextMesh nameTagLabel;
		#endregion

		#region UNITY CALLS
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Resets the state of the nametag.
		/// </summary>
		public override void ResetState() {
			this.nameTagLabel.Text = "";
			this.nameTagMainPivotRectTransform.localScale = Vector3.zero;
		}
		/// <summary>
		/// Displays the name tag.
		/// </summary>
		/// <param name="text">The text to show on the name tag.</param>
		/// <param name="nameTagParams">Any additional parameters to show on the tag.</param>
		[Button]
		public override void DisplayTag(string text, ChatNameTagParams nameTagParams) {

			// Scale the name tag in.
			this.nameTagMainPivotRectTransform.DOScale(
				endValue: 1f,
				duration: this.nameTagScaleInTime)
				.SetEase(ease: this.nameTagScaleInEaseType);

			// Rotate the name tag in.
			this.nameTagMainPivotRectTransform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.nameTagActiveRotation), 
				duration: this.nameTagRotateInTime,
				mode: RotateMode.FastBeyond360)
				.SetEase(ease: this.nameTagRotateInEaseType);

			// Also set the name on the tag.
			// this.nameTagLabel.Text = text;
			this.nameTagLabel.Text = nameTagParams.NameLabelColorTag + text;

			// Change the color on the tag as well.
			this.nameTagFrontImage.DOKill(complete: true);
			this.nameTagFrontImage.DOColor(
				endValue: GrawlyColors.colorDict[nameTagParams.nameTagBackingColorType ?? GrawlyColorTypes.Purple], 
				duration: this.nameTagColorTweenTime);

		}
		/// <summary>
		/// Dismisses the name tag.
		/// </summary>
		/// <param name="nameTagParams">Any additional parameters for the tag.</param>
		[Button]
		public override void DismissTag(ChatNameTagParams nameTagParams) {

			// Scale the name tag out.
			this.nameTagMainPivotRectTransform.DOScale(
				endValue: 0f,
				duration: this.nameTagScaleOutTime)
				.SetEase(ease: this.nameTagScaleOutEaseType);

			// Rotate the name tag out.
			this.nameTagMainPivotRectTransform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.nameTagHiddenRotation),
				duration: this.nameTagRotateOutTime,
				mode: RotateMode.FastBeyond360)
				.SetEase(ease: this.nameTagRotateOutEaseType);
		}
		#endregion

	}


}