using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

namespace Grawly.Battle.Navigator {

	/// <summary>
	/// Controls the visuals of the Battle Navigator as it exists in the scene.
	/// </summary>
	[RequireComponent(typeof(BattleNavigator))]
	public class BattleNavigatorCanvasController : MonoBehaviour {

		// public static BattleNavigatorCanvasController Instance;

		#region FIELDS - TOGGLES : POSITIONS
		/// <summary>
		/// The hiding position of the diamond graphic.
		/// </summary>
		[Title("Positions")]
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private Vector2 diamondGraphicHidingPos;
		/// <summary>
		/// The display position of the diamond graphic.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private Vector2 diamondGraphicDisplayPos;
		/// <summary>
		/// The hiding position of the navigator graphic.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private Vector2 navigatorPortraitHidingPos;
		/// <summary>
		/// The display position of the navigator graphic.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private Vector2 navigatorPortraitDisplayPos;
		#endregion

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time it takes for the text box visuals to tween in.
		/// </summary>
		[SerializeField, Title("Timing"), TabGroup("Navigator", "Toggles")]
		private float textBoxVisualsTweenInTime = 0.1f;
		/// <summary>
		/// The amount of time it takes for the text box visuals to tween out.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private float textBoxVisualsTweenOutTime = 0.1f;
		/// <summary>
		/// The amount of time it takes for the diamond graphic to tween in.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private float diamondGraphicTweenInTime = 0.1f;
		/// <summary>
		/// The amount of time it takes for the diamond graphic to tween out.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private float diamondGraphicTweenOutTime = 0.1f;
		/// <summary>
		/// The amount of time it takes for the navigator graphic to tween in.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private float navigatorPortraitTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time it takes for the navigator graphic to tween out.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private float navigatorPortraitTweenOutTime = 0.2f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The easing to use when tweening the text box visuals in.
		/// </summary>
		[SerializeField, Title("Easing"), TabGroup("Navigator", "Toggles")]
		private Ease textBoxVisualsEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the text box visuals out.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private Ease textBoxVisualsEaseOutType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the diamond graphic in.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private Ease diamondGraphicEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the diamond graphic out.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private Ease diamondGraphicEaseOutType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the navigator portrait in.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private Ease navigatorPortraitEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the navigator portrait out.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Toggles")]
		private Ease navigatorPortraitEaseOutType = Ease.InOutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES : RECT TRANSFORMS
		/// <summary>
		/// The RectTransform that contains all the visuals for the navigator.
		/// </summary>
		[SerializeField, Title("RectTransforms"), TabGroup("Navigator", "Scene References")]
		private RectTransform allVisualsObject;
		/// <summary>
		/// The RectTransform used for the diamond graphic.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Scene References")]
		private RectTransform diamondGraphicRectTransform;
		/// <summary>
		/// Contains the visuals for the text box rect transform.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Scene References")]
		private RectTransform textBoxGraphicRectTransform;
		/// <summary>
		/// The RectTransform that acts as the parent for the navigator images.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Scene References")]
		private RectTransform navigatorPortraitRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : OTHER
		/// <summary>
		/// The front image of the navigator's bust up.
		/// </summary>
		[SerializeField, Title("Images"), TabGroup("Navigator", "Scene References")]
		private Image navigatorBustUpFrontImage;
		/// <summary>
		/// The backing image of the navigator's bust up.
		/// </summary>
		[SerializeField, TabGroup("Navigator", "Scene References")]
		private Image navigatorBustUpBackImage;
		/// <summary>
		/// The Text element where text can be displayed.
		/// </summary>
		[SerializeField, Title("Text"), TabGroup("Navigator", "Scene References")]
		private Text navigatorText;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			
		}
		private void Start() {
			// Reset all the visuals upon start.
			this.ResetVisuals();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Totally resets the visuals of the navigator canvas to their hiding positions.
		/// </summary>
		private void ResetVisuals() {
			// Kill any tweens on all the relevant scene references, if there are any.
			this.KillAllTweens(complete: true);
			// Snap transforms to their hiding positions.
			this.ResetPositions();
			// Also make sure nothing is written to the text and change its color to clear.
			this.WriteNavigatorText(text: "");
			this.navigatorText.color = Color.clear;
		}
		/// <summary>
		/// Kills all tweens on any relevant objects.
		/// </summary>
		/// <param name="complete">Should the tweens be snapped to their "complete" state?</param>
		private void KillAllTweens(bool complete = true) {
			this.diamondGraphicRectTransform.DOKill(complete: complete);
			this.navigatorPortraitRectTransform.DOKill(complete: complete);
			this.textBoxGraphicRectTransform.DOKill(complete: complete);
			this.navigatorText.DOKill(complete: complete);
		}
		/// <summary>
		/// Resets the positions of all relevant objects to their hiding positions.
		/// </summary>
		private void ResetPositions() {
			// this.textBoxGraphicRectTransform.anchoredPosition = this.textBoxVisualsHidingPos;
			this.textBoxGraphicRectTransform.transform.localScale = new Vector3(x: 1f, y: 0f, z: 1f);
			this.navigatorPortraitRectTransform.anchoredPosition = this.navigatorPortraitHidingPos;
			this.diamondGraphicRectTransform.anchoredPosition = this.diamondGraphicHidingPos;
		}
		#endregion

		#region PRESENTATION - GENERAL
		/// <summary>
		/// Presents the navigator with the specified parameters.
		/// </summary>
		/// <param name="navigatorParams">The parameters to use when presenting this navigator.</param>
		public void PresentNavigator(BattleNavigatorParams navigatorParams) {
			this.PresentNavigator(text: navigatorParams.dialogueText);
		}
		/// <summary>
		/// Presents the navigator with the specified text and portrait image.
		/// </summary>
		/// <param name="text">The text to write out to the navigator text box.</param>
		/// <param name="navigatorSprite">The image to use for the navigator's portrait image.</param>
		public void PresentNavigator(string text, Sprite navigatorSprite) {
			// Override the navigator's sprites, if that is required.
			this.navigatorBustUpFrontImage.overrideSprite = navigatorSprite;
			this.navigatorBustUpBackImage.overrideSprite = navigatorSprite;
			this.PresentNavigator(text: text);
		}
		/// <summary>
		/// Presents the navigator with the specified text.
		/// </summary>
		/// <param name="text">The text to write out to the navigator text box.</param>
		[Button, HideInEditorMode]
		public void PresentNavigator(string text) {
			// Kill any and all tweens if they're still going.
			this.KillAllTweens(complete: true);
			// Animate everything to slide in.
			this.AnimateTextBox(show: true);
			this.AnimateDiamondGraphic(show: true);
			this.AnimateNavigatorPortrait(show: true);
			// Also write the text out.
			this.WriteNavigatorText(text: text);

		}
		/// <summary>
		/// Dismisses the navigator. This tweens everything out, basically.
		/// </summary>
		[Button, HideInEditorMode]
		public void DismissNavigator() {
			// Kill any tweens if they are active.
			this.KillAllTweens(complete: true);
			// Animate everything to slide out.
			this.AnimateTextBox(show: false);
			this.AnimateDiamondGraphic(show: false);
			this.AnimateNavigatorPortrait(show: false);
		}
		#endregion

		#region PRESENTATION - TEXT
		/// <summary>
		/// Writes out the specified string to the navigator text box.
		/// </summary>
		/// <param name="text">The text that should be written out to the navigator text box.</param>
		private void WriteNavigatorText(string text) {
			// Just set the text.
			this.navigatorText.text = text;
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the text box to tween in or out.
		/// </summary>
		/// <param name="show">Whether or not to display the text box.</param>
		private void AnimateTextBox(bool show) {
			// Determine the values to use based on the boolean passed in.
			float scaleValue = (show == true) ? 1f : 0f;
			Color textColor = (show == true) ? Color.white : Color.clear;
			float duration = (show == true) ? this.textBoxVisualsTweenInTime : textBoxVisualsTweenOutTime;
			Ease ease = (show == true) ? this.textBoxVisualsEaseInType : textBoxVisualsEaseOutType;
			// Perform the tweens.
			this.textBoxGraphicRectTransform.DOScaleY(endValue: scaleValue, duration: duration).SetEase(ease: ease);
			this.navigatorText.DOColor(endValue: textColor, duration: duration).SetEase(ease: Ease.Linear);
		}
		/// <summary>
		/// Animates the diamond graphic to tween in or out.
		/// </summary>
		/// <param name="show">Whether or not to display the diamond graphic.</param>
		private void AnimateDiamondGraphic(bool show) {
			// Figure out what values to use for the tween.
			Vector2 endPos = (show == true) ? this.diamondGraphicDisplayPos : this.diamondGraphicHidingPos;
			float duration = (show == true) ? this.diamondGraphicTweenInTime : this.diamondGraphicTweenOutTime;
			Ease ease = (show == true) ? this.diamondGraphicEaseInType : this.diamondGraphicEaseOutType;
			// Perform the tween.
			this.diamondGraphicRectTransform.DOAnchorPos(endValue: endPos, duration: duration, snapping: true).SetEase(ease: ease);
		}
		/// <summary>
		/// Animates the navigator portrait to tween in or out.
		/// </summary>
		/// <param name="show">Whether or not to display the navigator graphic.</param>
		private void AnimateNavigatorPortrait(bool show) {
			// Figure out what values to use for the tween.
			Vector2 endPos = (show == true) ? this.navigatorPortraitDisplayPos : this.navigatorPortraitHidingPos;
			float duration = (show == true) ? this.navigatorPortraitTweenInTime : this.navigatorPortraitTweenOutTime;
			Ease ease = (show == true) ? this.navigatorPortraitEaseInType : this.navigatorPortraitEaseOutType;
			// Perform the tween.
			this.navigatorPortraitRectTransform.DOAnchorPos(endValue: endPos, duration: duration, snapping: true).SetEase(ease: ease);
		}
		#endregion

	}


}