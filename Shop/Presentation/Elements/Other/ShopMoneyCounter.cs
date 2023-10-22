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
	/// Displays the current running total of the player's money.
	/// </summary>
	public class ShopMoneyCounter : MonoBehaviour {
		
		public static ShopMoneyCounter Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// A reference to whatever routine is currently animating on the counter.
		/// </summary>
		private Coroutine CurrentAnimationRoutine { get; set; }
		#endregion

		#region FIELDS - TWEENING : POSITIONS
		/// <summary>
		/// The position the counter should be in when in hiding.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Tweening")]
		[Title("Positions")]
		private Vector2 counterHidingPos = new Vector2();
		/// <summary>
		/// The position the counter should be in when displayed.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Tweening")]
		private Vector2 counterDisplayPos = new Vector2();
		#endregion

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to wait before tweening the counter in.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Tweening")]
		[Title("Timing")]
		private float counterTweenDelayTime = 1f;
		/// <summary>
		/// The amount of time to take when tweening the counter in.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Tweening")]
		private float counterTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the counter out.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Tweening")]
		private float counterTweenOutTime = 0.5f;
		/// <summary>
		/// The amount of time to wait before fading in the counter label.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Tweening")]
		private float counterLabelFadeDelayTime = 0.2f;
		/// <summary>
		/// The amount of time to take to fade the counter label in.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Tweening")]
		private float counterLabelFadeInTime = 0.5f;
		#endregion

		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the counter in.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Tweening")]
		[Title("Easing")]
		private Ease counterEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the counter out.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Tweening")]
		private Ease counterEaseOutType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the objects as children.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The main pivot for manipulating position for the counter.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Scene References")]
		private RectTransform counterMainPivot;
		/// <summary>
		/// The image serving as the front for the counter.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Scene References")]
		private Image counterBackingFrontImage;
		/// <summary>
		/// The image serving as the dropshadow for the counter.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Scene References")]
		private Image counterBackingDropshadowImage;
		/// <summary>
		/// The image to help fade in the text on the counter.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Scene References")]
		private Image counterBackingMaskImage;
		/// <summary>
		/// The STM representing the current funds available.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Scene References")]
		private SuperTextMesh currentFundsTextLabel;
		/// <summary>
		/// The STM representing potential subtraction of funds.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Scene References")]
		private SuperTextMesh subtractedFundsTextLabel;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Kill all tweens currently operating on the counter.
		/// </summary>
		private void KillAllTweens() {
			
			// Kill anything that DOTween may be referencing.
			this.counterMainPivot.DOKill(complete: true);
			this.counterBackingFrontImage.DOKill(complete: true);
			this.counterBackingDropshadowImage.DOKill(complete: true);
			this.counterBackingMaskImage.DOKill(complete: true);
			
			// Also kill any coroutines operating on the counter, if it is not null.
			if (this.CurrentAnimationRoutine != null) {
				this.StopCoroutine(this.CurrentAnimationRoutine);
			}
			
		}
		/// <summary>
		/// Completely and totally resets the state of the counter.
		/// </summary>
		public void ResetState() {
			
			// Kill all tweens. Bottom text.
			this.KillAllTweens();
			
			// Snap the main pivot back to its hiding spot.
			this.counterMainPivot.anchoredPosition = this.counterHidingPos;

			// Remove the text on the total/subtracted labels.
			this.currentFundsTextLabel.Text = "";
			this.subtractedFundsTextLabel.Text = "";
			
			// Turn all of the objects off.
			this.allObjects.SetActive(false);
			
		}
		/// <summary>
		/// Prepares the counter with the template/theme provided.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Prepare(ShopMenuParams shopMenuParams) {
			
			// Assign the colors of the images.
			this.counterBackingFrontImage.color = shopMenuParams.ShopThemeTemplate.MoneyCounterFrontColor;
			this.counterBackingMaskImage.color = shopMenuParams.ShopThemeTemplate.MoneyCounterFrontColor;
			this.counterBackingDropshadowImage.color = shopMenuParams.ShopThemeTemplate.MoneyCounterDropshadowColor;
			
			// Assign the materials for the labels.
			this.currentFundsTextLabel.textMaterial = shopMenuParams.ShopThemeTemplate.MoneyCounterTotalFundsLabelMaterial;
			this.subtractedFundsTextLabel.textMaterial = shopMenuParams.ShopThemeTemplate.MoneyCounterSubtractedFundsLabelMaterial;

		}
		#endregion

		#region ANIMATIONS - PRESENTATION
		/// <summary>
		/// Presents the money counter (wow) (whoa) (cool)
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
		/// Presents the money counter (wow) (whoa) (cool) (omg)
		/// </summary>
		/// <param name="shopMenuParams"></param>
		/// <returns></returns>
		private IEnumerator PresentRoutine(ShopMenuParams shopMenuParams) {
			
			// Wait for the delay amount.
			yield return new WaitForSeconds(this.counterTweenDelayTime);
			
			// Tween to the display position.
			this.counterMainPivot.DOAnchorPos(
				endValue: counterDisplayPos,
				duration: this.counterTweenInTime,
				snapping: true)
				.SetEase(ease: this.counterEaseInType);

			// Wait a little longer before fading the counter in.
			yield return new WaitForSeconds(this.counterLabelFadeDelayTime);
			
			// Assign the text to the total money label and then actually fade the mask out.
			this.currentFundsTextLabel.Text = shopMenuParams.ShopThemeTemplate.MoneyCounterTotalFundsLabelPrefix
			                                  + "$"
			                                  + shopMenuParams.CurrentVariables.Money.ToString();
			
			this.counterBackingMaskImage.DOFade(
				endValue: 0f, 
				duration: this.counterLabelFadeInTime)
				.SetEase(ease: Ease.Linear);

		}
		#endregion
		
	}
}