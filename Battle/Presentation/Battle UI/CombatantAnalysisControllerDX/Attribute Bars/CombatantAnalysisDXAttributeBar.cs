using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// The bar that displays a combatant' attribute of a specific type on the analysis screen.
	/// </summary>
	public class CombatantAnalysisDXAttributeBar : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The attribute that should be associated with this attribute bar.
		/// </summary>
		[SerializeField, TabGroup("Attribute Bar", "Toggles")]
		private AttributeType attributeType = AttributeType.ST;
		#endregion
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to tween the bar's fill in.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Background", "Tweening")]
		private float fillInTime = 0.5f;
		/// <summary>
		/// The amount of time to tween the bar's fill out.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private float fillOutTime = 0.5f;
		#endregion
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The easing to use when tweening the bar's fill in.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Background", "Tweening")]
		private Ease fillEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the bar's fill out.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private Ease fillEaseOutType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - TWEENING : OTHER
		/// <summary>
		/// The lowest value the fill should go to on this bar.
		/// </summary>
		[Title("Other")]
		[SerializeField, TabGroup("Background", "Tweening")]
		private float minimumFillAmount = 0.1f;
		/// <summary>
		/// The highest value the fill should go to on this bar.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private float maximumFillAmount = 1f;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The image that should be filled/colored depending on the combatant's stat of the given type.
		/// </summary>
		[SerializeField, TabGroup("Attribute Bar", "Scene References")]
		private Image attributeBarFrontImage;
		/// <summary>
		/// The text mesh that displays the type of attribute being presented.
		/// </summary>
		[SerializeField, TabGroup("Attribute Bar", "Scene References")]
		private SuperTextMesh attributeTypeLabel;
		/// <summary>
		/// The text mesh that displays the value of the attribute being presented.
		/// </summary>
		[SerializeField, TabGroup("Attribute Bar", "Scene References")]
		private SuperTextMesh attributeValueLabel;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this element.
		/// </summary>
		public void ResetState() {
			
			// Kill any ongoing tweens.
			this.KillTweens();
			
			// Reset the fill value to the minimum value.
			this.attributeBarFrontImage.fillAmount = this.minimumFillAmount;
			
			// Also reset the text on the value label.
			this.attributeValueLabel.Text = "";
			
		}
		/// <summary>
		/// Kill any tweens currently operating on this attribute bar.
		/// </summary>
		private void KillTweens() {
			this.attributeBarFrontImage.DOKill(complete: true);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the element into view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Present(CombatantAnalysisParams analysisParams) {
			
			// Set the color of the fill based on what attribute is in use.
			this.attributeBarFrontImage.color = GrawlyColors.GetColorFromAttributeType(attributeType: this.attributeType);
			
			// Get the value of the attribute associated with this bar, specifically.
			int attributeValue = analysisParams.CurrentCombatant.GetDynamicAttribute(attributeType: this.attributeType);
			
			// Determine the fill value from 0 to 1.
			float rawFillValue = Mathf.InverseLerp(a: 0, b: 99, value: attributeValue);
			// Scale that value between the allowed floor/ceilings.
			float scaledFillValue = Mathf.Lerp(a: this.minimumFillAmount, b: this.maximumFillAmount, t: rawFillValue);
			// Also get the scaled time to tween in the fill.
			float scaledTweenTime = Mathf.Lerp(a: 0f, b: this.fillInTime, t: rawFillValue);
			
			// Tween the bar's fill amount.
			this.attributeBarFrontImage.DOFillAmount(
				endValue: scaledFillValue,
				duration: scaledTweenTime)
				.SetEase(ease: this.fillEaseInType);
			
			// Also change the text.
			this.attributeValueLabel.Text = attributeValue.ToString();
			
		}
		/// <summary>
		/// Dismisses this element from view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for dismissal.</param>
		public void Dismiss(CombatantAnalysisParams analysisParams) {
			
			// Just in case, kill any ongoing tweens.
			this.KillTweens();
			
			// Figure out what the current fill value is.
			float currentFillValue = this.attributeBarFrontImage.fillAmount;
			// Get the scaled tween time to make sure it "feels" good to empty out.
			float scaledTweenTime = Mathf.Lerp(a: 0f, b: this.fillOutTime, t: currentFillValue);
			
			// Tween the bar's fill amount.
			this.attributeBarFrontImage.DOFillAmount(
					endValue: this.minimumFillAmount,
					duration: scaledTweenTime)
				.SetEase(ease: this.fillEaseOutType);
			
		}
		#endregion

		
	}
}