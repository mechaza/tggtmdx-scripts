using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// Displays or hides the prompt as needed.
	/// </summary>
	public class CrawlerActionPrompt : MonoBehaviour {
  
		public static CrawlerActionPrompt Instance { get; private set; }

		#region PROPERTIES - STATE
		/// <summary>
		/// Is the prompt currently displayed?
		/// </summary>
		public bool IsDisplayed {
			get {
				// If the label text is not blank, it's displayed.
				return this.promptLabel1.Text != "";
			} 
		}
		/// <summary>
		/// The prompt currently being shown.
		/// Includes color tags.
		/// </summary>
		public string CurrentPrompt {
			get {
				return this.promptLabel1.Text;
			}
		}
		#endregion
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to take when tweening in.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private float tweenTime = 0.2f;
		/// <summary>
		/// The amount of time to take to fade in the prompt.
		/// </summary>
		[SerializeField]
		private float fadeTime = 0.2f;
		/// <summary>
		/// The easing to use when easing in.
		/// </summary>
		[SerializeField]
		private Ease easeInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when easing out.
		/// </summary>
		[SerializeField]
		private Ease easeOutType = Ease.InOutCirc;
		/// <summary>
		/// The position to set the prompt when hiding.
		/// </summary>
		[SerializeField]
		private Vector2 hidingPos;
		/// <summary>
		/// The position to set the prompt when displayed.
		/// </summary>
		[SerializeField]
		private Vector2 displayPos;
		/// <summary>
		/// The position to set the prompt when finishing out.
		/// </summary>
		[SerializeField]
		private Vector2 finishPos;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The main pivot to tween around.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private RectTransform mainPivotRectTransform;
		/// <summary>
		/// The canvas group containing the elements. This can be used for fading and junk.
		/// </summary>
		[SerializeField]
		private CanvasGroup promptCanvasGroup;
		/// <summary>
		/// The text mesh to use for showing the prompt.
		/// </summary>
		[SerializeField]
		private SuperTextMesh promptLabel1;
		/// <summary>
		/// The text mesh to use for showing the prompt.
		/// </summary>
		[SerializeField]
		private SuperTextMesh promptLabel2;
		/// <summary>
		/// The image showing the "front" of the prompt.
		/// Good if I need to change colors.
		/// </summary>
		[SerializeField]
		private Image promptFrontImage;
		/// <summary>
		/// The image for hodling icons.
		/// </summary>
		[SerializeField]
		private Image promptIcon;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Kills the tweens on the prompt and "snaps" it to the correct spot.
		/// </summary>
		private void SnapCurrentTweens() {
			this.mainPivotRectTransform.DOKill(complete: true);
			this.promptCanvasGroup.DOKill(complete: true);
			this.promptFrontImage.DOKill(complete: true);
			this.promptIcon.DOKill(complete: true);
		}
		/// <summary>
		/// Completely and totally resets the state of the action prompt.
		/// </summary>
		private void ResetState() {
			
			// Set the state to Not Displayed.
			// this.IsDisplayed = false;
			
			// Snap the current tweens.
			this.SnapCurrentTweens();
			// Set the alpha on the canvas group.
			this.promptCanvasGroup.alpha = 0f;
			// Snap it to the correct position.
			this.mainPivotRectTransform.anchoredPosition = this.hidingPos;
			// Reset the text on the prompt labels.
			this.promptLabel1.Text = "";
			this.promptLabel2.Text = "";
			// Hide the icon and turn it off.
			this.promptIcon.overrideSprite = null;
			this.promptIcon.gameObject.SetActive(false);
		}
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Displays the action prompt with the specified text.
		/// </summary>
		/// <param name="promptText">The text to display on the prompt.</param>
		/// <param name="promptType">The kind of text to display.</param>
		public void Display(string promptText, ActionPromptType promptType = ActionPromptType.None) {
			
			// Reset the state completely but set the state to Displayed after.
			this.ResetState();
			
			// Tween the prompt in.
			this.mainPivotRectTransform.DOAnchorPos(
				endValue: this.displayPos,
				duration: this.tweenTime,
				snapping: true)
				.SetEase(this.easeInType);
			
			// Fade the canvas group in.
			this.promptCanvasGroup.DOFade(
				endValue: 1f, 
				duration: this.fadeTime)
				.SetEase(Ease.Linear);
			
			// Compute the prefix to use on the label and assign it with the prompt text.
			string labelPrefix = GrawlyColors.STMPrefix(c: this.GetLabelColorFromPromptType(promptType));
			this.promptLabel1.Text = labelPrefix + promptText;
			
			// Also change the color of the image.
			this.promptFrontImage.color = GetImageColorFromPromptType(promptType);
			
		}
		/// <summary>
		/// Dismisses the action prompt.
		/// </summary>
		public void Dismiss() {
			
			// Snap the tweens.
			this.SnapCurrentTweens();
			
			// Tween the prompt out.
			this.mainPivotRectTransform.DOAnchorPos(
					endValue: this.finishPos,
					duration: this.tweenTime,
					snapping: true)
				.SetEase(this.easeOutType);
			
			// Fade the canvas group out.
			this.promptCanvasGroup.DOFade(
					endValue: 0f, 
					duration: this.fadeTime)
				.SetEase(Ease.Linear);
			
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Determines the kind of color to use on the prompt image when passed the specified prompt type.
		/// </summary>
		/// <param name="promptType">The prompt type.</param>
		/// <returns>The color associated with the given prompt type.</returns>
		private Color GetImageColorFromPromptType(ActionPromptType promptType) {
			switch (promptType) {
				case ActionPromptType.None:
					return GrawlyColors.Red;
				default:
					throw new System.NotImplementedException("Prompt has no associated color.");
			}
		}
		/// <summary>
		/// Determines the kind of color to use on the prompt label when passed the specified prompt type.
		/// </summary>
		/// <param name="promptType">The prompt type.</param>
		/// <returns>The color associated with the given prompt type.</returns>
		private Color GetLabelColorFromPromptType(ActionPromptType promptType) {
			switch (promptType) {
				case ActionPromptType.None:
					return GrawlyColors.White;
				default:
					throw new System.NotImplementedException("Prompt has no associated color.");
			}
		}
		#endregion
		
		
	}

	
}