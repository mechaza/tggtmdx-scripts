using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Grawly.UI.MenuLists;

namespace Grawly.Shop {
	
	/// <summary>
	/// Helps give access to the bust up sprite shown in the shop menu.
	/// </summary>
	public class ShopKeeperBustUp : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The variable to help keep track of whatever routine is controlling the bust up's animation.
		/// Needed so I can interrupt it if needed.
		/// </summary>
		private Coroutine CurrentAnimationRoutine { get; set; }
		#endregion
		
		#region FIELDS - TWEENING : POSITIONS
		/// <summary>
		/// The position the bust up graphic's main pivot should be in when its in hiding.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		[Title("Positions")]
		private Vector2 bustUpGraphicHidingPos = new Vector2();
		/// <summary>
		/// The position the bust up graphic's main pivot should be in when displayed.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private Vector2 bustUpGraphicDisplayPos = new Vector2();
		/// <summary>
		/// The position the bust up graphic's main pivot should tween to when selecting a top menu item.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private Vector2 bustUpGraphicDismissPos = new Vector2();
		/// <summary>
		/// The position the speech bubble's main pivot should be in when hiding.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private Vector2 speechBubbleHidingPos = new Vector2();
		/// <summary>
		/// The position the speech bubble's main pivot should be in when its displayed.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private Vector2 speechBubbleDisplayPos = new Vector2();
		/// <summary>
		/// The position the speech bubble's main pivot should tween to when selecting a top menu item.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private Vector2 speechBubbleDismissPos = new Vector2();
		#endregion

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take before actually tweening the bust up into view.
		/// This is primarily for when the shop is first entered.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		[Title("Timing")]
		private float bustUpGraphicStartupDelay = 1f;
		/// <summary>
		/// The amount of time to take when tweening the bust up graphic into its display position.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float bustUpGraphicTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the bust up graphic to its dismiss position.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float bustUpGraphicTweenOutTime = 0.2f;
		/// <summary>
		/// The amount of time to take to fade the bust up graphic in.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float bustUpGraphicFadeInTime = 0.1f;
		/// <summary>
		/// The amount of time to take to fade the bust up graphic out.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float bustUpGraphicFadeOutTime = 0.1f;
		/// <summary>
		/// The amount of time to wait before actually showing the speech bubble.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float speechBubbleStartupDelay = 3f;
		/// <summary>
		/// The amount of time to take when tweening the speech bubble in.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float speechBubbleTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the speech bubble out.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float speechBubbleTweenOutTime = 0.2f;
		/// <summary>
		/// The amount of time to wait before fading in the speech bubble's text.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float speechBubbleTextStartupDelay = 3.5f;
		/// <summary>
		/// The amount of time to take to fade the speech bubble in.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float speechBubbleFadeInTime = 0.1f;
		/// <summary>
		/// The amount of time to take to fade the speech bubble out.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float speechBubbleFadeOutTime = 0.1f;
		/// <summary>
		/// The amount of time to take while fading in the speech bubble's text.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private float speechBubbleTextFadeInTime = 0.2f;
		#endregion

		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the bust up graphic to its display position.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		[Title("Easing")]
		private Ease bustUpGraphicEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the bust up graphic to its dismiss position.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private Ease bustUpGraphicEaseOutType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the speech bubble to its display position.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private Ease speechBubbleEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the speech bubble to its dismiss position.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Tweening")]
		private Ease speechBubbleEaseOutType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : GENERAL
		/// <summary>
		/// Contains all of the objects inside the shop keeper bust up.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The label to display the text itself on the speech bubble.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private SuperTextMesh speechBubbleTextLabel;
		#endregion

		#region FIELDS - SCENE REFERENCES : PIVOTS
		/// <summary>
		/// The main pivot for the actual images.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		[Title("Pivots")]
		private RectTransform bustUpGraphicMainPivot;
		/// <summary>
		/// The pivot for the noise effect on the bust up images.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private RectTransform bustUpGraphicNoisePivot;
		/// <summary>
		/// The main pivot for the speech bubble.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private RectTransform speechBubbleMainPivot;
		/// <summary>
		/// The pivot for the noise effect on the speech bubble.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private RectTransform speechBubbleNoisePivot;
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The front image for the bust up.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		[Title("Images")]
		private Image bustUpGraphicFrontImage;
		/// <summary>
		/// The dropshadow image for the bust up.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image bustUpGraphicDropshadowImage;
		/// <summary>
		/// The front image for the speech bubble.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image speechBubbleFrontImage;
		/// <summary>
		/// The dropshadow image for the speech bubble.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image speechBubbleDropshadowImage;
		/// <summary>
		/// The image to use to help with masking the supertextmesh so it looks like it fades in seamlessly.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image speechBubbleMaskingImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally kills all tweens on whatever is currently in motion.
		/// </summary>
		private void KillAllTweens() {
			
			// Halt the current animation coroutine if one exists.
			if (this.CurrentAnimationRoutine != null) {
				this.StopCoroutine(routine: this.CurrentAnimationRoutine);
			}
			
			// Kill tweens on the pivots.
			this.bustUpGraphicMainPivot.DOKill(complete: true);
			this.bustUpGraphicNoisePivot.DOKill(complete: true);
			this.speechBubbleMainPivot.DOKill(complete: true);
			this.speechBubbleNoisePivot.DOKill(complete: true);
			
			// Kill tweens on the images.
			this.bustUpGraphicFrontImage.DOKill(complete: true);
			this.bustUpGraphicDropshadowImage.DOKill(complete: true);
			this.speechBubbleFrontImage.DOKill(complete: true);
			this.speechBubbleDropshadowImage.DOKill(complete: true);
			this.speechBubbleMaskingImage.DOKill(complete: true);
			
		}
		/// <summary>
		/// Completely and totally resets the state of this shop keeper.
		/// </summary>
		public void ResetState() {
		
			// Kill all tweens.
			this.KillAllTweens();
			
			// Snap the main pivots to their hiding positions.
			this.bustUpGraphicMainPivot.anchoredPosition = this.bustUpGraphicHidingPos;
			this.speechBubbleMainPivot.anchoredPosition = this.speechBubbleHidingPos;
			
			// Set the colors on the images to be transparent.
			this.bustUpGraphicFrontImage.color = Color.clear;
			this.bustUpGraphicDropshadowImage.color = Color.clear;
			this.speechBubbleFrontImage.color = Color.clear;
			this.speechBubbleDropshadowImage.color = Color.clear;
			this.speechBubbleMaskingImage.color = Color.clear;
			
			// Clear out the text, if there is any.
			this.speechBubbleTextLabel.Text = "";

			// Turn all of the objects off.
			this.allObjects.SetActive(false);
			
		}
		/// <summary>
		/// Prepares the bust up for use with the templates provided.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Prepare(ShopMenuParams shopMenuParams) {
			// Set the sprites for the bust up.
			this.bustUpGraphicFrontImage.overrideSprite = shopMenuParams.ShopThemeTemplate.ShopKeeperBustUpSprite;
			this.bustUpGraphicDropshadowImage.overrideSprite = shopMenuParams.ShopThemeTemplate.ShopKeeperBustUpSprite;
		}
		#endregion

		#region ANIMATIONS : PRESENTATION
		/// <summary>
		/// Presents the shop keeper bust up from hiding.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Present(ShopMenuParams shopMenuParams) {

			// Turn all of the objects on.
			this.allObjects.SetActive(true);
			
			// Cascade down one more time using the routine version of this function and also save it.
			this.CurrentAnimationRoutine = this.StartCoroutine(
				this.PresentRoutine(shopMenuParams));

		}
		/// <summary>
		/// The actual routine that takes care of presenting the shop keeper bust up from hiding.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		/// <returns></returns>
		private IEnumerator PresentRoutine(ShopMenuParams shopMenuParams) {

			// Wait for the specified delay amount before actually starting.
			yield return new WaitForSeconds(this.bustUpGraphicStartupDelay);

			// Tween the bust up in and fade the graphics in.
			this.bustUpGraphicMainPivot.DOAnchorPos(
				endValue: this.bustUpGraphicDisplayPos, 
				duration: this.bustUpGraphicTweenInTime, 
				snapping: true)
				.SetEase(this.bustUpGraphicEaseInType);
			this.bustUpGraphicFrontImage.DOColor(
				endValue: Color.white, 
				duration: this.bustUpGraphicFadeInTime)
				.SetEase(Ease.Linear);
			this.bustUpGraphicDropshadowImage.DOColor(
					endValue: Color.black, 
					duration: this.bustUpGraphicFadeInTime)
				.SetEase(Ease.Linear);

			// Wait a little longer for the speech bubble to show up.
			yield return new WaitForSeconds(this.speechBubbleTextStartupDelay);
			
			// Start tweening it in.
			this.speechBubbleMainPivot.DOAnchorPos(
				endValue: this.speechBubbleDisplayPos, 
				duration: this.speechBubbleTweenInTime, 
				snapping: true)
				.SetEase(ease: this.speechBubbleEaseInType);
			this.speechBubbleFrontImage.DOColor(
				endValue: Color.white, 
				duration: this.speechBubbleFadeInTime)
				.SetEase(Ease.Linear);
			this.speechBubbleDropshadowImage.DOColor(
					endValue: Color.black, 
					duration: this.speechBubbleFadeInTime)
				.SetEase(Ease.Linear);
			
			// Wait a *little* longer, then begin to fade the text in.
			yield return new WaitForSeconds(this.speechBubbleTextStartupDelay);
			
			// Snap the color of the masking image.
			this.speechBubbleMaskingImage.color = Color.white;
			
			// Assign the text now. I'm drunk.
			throw new NotImplementedException("Display the shop keeper text!");
			/*this.speechBubbleTextLabel.Text = shopMenuParams.ShopThemeTemplate.BustUpSpeechBubbleTextPrefix 
			                                  + shopMenuParams.ShopStoreTemplate.PlaceholderShopKeeperText;*/
			
			// Tween the masking image clear.
			this.speechBubbleMaskingImage.DOFade(
				endValue: 0f,
				duration: this.speechBubbleTextFadeInTime)
				.SetEase(Ease.Linear);
			

		}
		#endregion
		
		
	}
}