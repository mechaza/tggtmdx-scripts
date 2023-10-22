using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// This encapsulates the functionality of manipulating the background of the analysis screen.
	/// </summary>
	public class CombatantAnalysisDXBackground : MonoBehaviour {
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to tween this element in.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Background", "Tweening")]
		private float tweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to tween this element out.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private float tweenOutTime = 0.5f;
		#endregion
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The easing to use when tweening this element in.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Background", "Tweening")]
		private Ease easeInType = Ease.Linear;
		/// <summary>
		/// The easing to use when tweening this element out.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private Ease easeOutType = Ease.Linear;
		#endregion
		
		#region FIELDS - TWEENING : GRAPHICS
		/// <summary>
		/// The color value to use when the corner fade is displayed.
		/// </summary>
		[Title("Graphics")]
		[SerializeField, TabGroup("Background", "Tweening")]
		private Color cornerFadeDisplayColor = Color.white;
		/// <summary>
		/// The color value to use when the corner fade is hidden.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private Color cornerFadeHiddenColor = Color.clear;
		/// <summary>
		/// The color value to use when the background image is displayed.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private Color backgroundImageDisplayColor = Color.white;
		/// <summary>
		/// The color value to use when the background image is hidden.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private Color backgroundImageHiddenColor = Color.clear;
		/// <summary>
		/// The color value to use when the scroll image is displayed.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private Color scrollImageDisplayColor = Color.white;
		/// <summary>
		/// The color value to use when the scroll image is hidden.
		/// </summary>
		[SerializeField, TabGroup("Background", "Tweening")]
		private Color scrollImageHiddenColor = Color.clear;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The entire image that gets overlayed when entering the analysis screen.
		/// </summary>
		[SerializeField, TabGroup("Background","Scene References")]
		private Image fullScreenBackgroundImage;
		/// <summary>
		/// The image that gets placed on top of the full screen image for the checker effect.
		/// </summary>
		[SerializeField, TabGroup("Background","Scene References")]
		private Image scrollEffectImage;
		/// <summary>
		/// The image that fades to black in the lower left.
		/// Used to give better contrast to the combatant's level.
		/// </summary>
		[SerializeField, TabGroup("Background","Scene References")]
		private Image cornerFadeImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this element.
		/// </summary>
		public void ResetState() {
			// Kill any tweens operating on these elements.
			this.KillTweens();
			this.fullScreenBackgroundImage.color = Color.clear;
			this.cornerFadeImage.color = Color.clear;
			this.scrollEffectImage.color = Color.clear;
		}
		/// <summary>
		/// Kills any tweens operating on this element.
		/// </summary>
		private void KillTweens() {
			this.cornerFadeImage.DOKill(complete: true);
			this.fullScreenBackgroundImage.DOKill(complete: true);
			this.scrollEffectImage.DOKill(complete: true);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the element into view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Present(CombatantAnalysisParams analysisParams) {
			
			// Totally reset the state.
			this.ResetState();
			
			// Fade in the two images.
			this.cornerFadeImage.DOColor(
					endValue: this.cornerFadeDisplayColor,
					duration: this.tweenInTime)
				.SetEase(ease: this.easeInType);
			this.fullScreenBackgroundImage.DOColor(
					endValue: this.backgroundImageDisplayColor,
					duration: this.tweenInTime)
				.SetEase(ease: this.easeInType);
			this.scrollEffectImage.DOColor(
					endValue: this.scrollImageDisplayColor,
					duration: this.tweenInTime)
				.SetEase(ease: this.easeInType);
			
		}
		/// <summary>
		/// Dismisses the element from view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Dismiss(CombatantAnalysisParams analysisParams) {
			
			// Kill any ongoing tweens.
			this.KillTweens();
			
			// Fade out the two images.
			this.cornerFadeImage.DOColor(
					endValue: this.cornerFadeHiddenColor,
					duration: this.tweenOutTime)
				.SetEase(ease: this.easeOutType);
			this.fullScreenBackgroundImage.DOColor(
					endValue: this.backgroundImageHiddenColor,
					duration: this.tweenOutTime)
				.SetEase(ease: this.easeOutType);
			this.scrollEffectImage.DOColor(
					endValue: this.scrollImageHiddenColor,
					duration: this.tweenInTime)
				.SetEase(ease: this.easeInType);
			
		}
		#endregion

		
		
	}
	
}