using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// The element that will flash the combatant's name on the screen.
	/// </summary>
	public class CombatantAnalysisDXNameBanner : MonoBehaviour {

		#region FIELDS - TWEENING : POSITION
		/// <summary>
		/// The position the name anchor should be in when hiding.
		/// </summary>
		[Title("Positions")]
		[SerializeField, TabGroup("Banner", "Tweening")]
		private Vector2Int nameAnchorHidingPos = new Vector2Int();
		/// <summary>
		/// The position the name anchor should be in when displayed.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Tweening")]
		private Vector2Int nameAnchorDisplayPos = new Vector2Int();
		/// <summary>
		/// The position the name anchor should be when tweening out on close.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Tweening")]
		private Vector2Int nameAnchorDismissPos = new Vector2Int();
		#endregion
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when tweening the name anchor in.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Banner", "Tweening")]
		private float nameAnchorTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the name anchor out.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Tweening")]
		private float nameAnchorTweenOutTime = 0.2f;
		/// <summary>
		/// The amount of time to fade the banner bar in.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Tweening")]
		private float bannerBarFadeInTime = 0.1f;
		/// <summary>
		/// The amount of time to fade the banner bar out.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Tweening")]
		private float bannerBarFadeOutTime = 0.1f;
		#endregion
		
		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the name anchor in.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Banner", "Tweening")]
		private Ease nameAnchorEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the name anchor out.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Tweening")]
		private Ease nameAnchorEaseOutType = Ease.InOutCirc;
		#endregion

		#region FIELDS - TWEENING : COLORS
		/// <summary>
		/// The color the bar should be when displayed.
		/// </summary>
		[Title("Colors")]
		[SerializeField, TabGroup("Banner", "Tweening")]
		private Color bannerBarDisplayColor = Color.white;
		/// <summary>
		/// The color the bar should be when hidden.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Tweening")]
		private Color bannerBarHiddenColor = Color.clear;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : GENERAL
		/// <summary>
		/// Contains all of the objects for this banner.
		/// </summary>
		[Title("General")]
		[SerializeField, TabGroup("Banner", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The GameObject that should be used for navigating left/right on the banner.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Scene References")]
		private CombatantAnalysisDXBannerSelectable bannerNavigationSelectable;
		/// <summary>
		/// The text mesh that actually displays the combatant's name.
		/// </summary>
		[SerializeField, TabGroup("Banner","Scene References")]
		private SuperTextMesh nameLabel;
		/// <summary>
		/// The rect transform that should be tweened to animate the combatant's name.
		/// </summary>
		[SerializeField, TabGroup("Banner","Scene References")]
		private RectTransform nameAnchorRectTransform;
		/// <summary>
		/// The image that serves as the backing for the text.
		/// Fades in/out with the screen.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Scene References")]
		private Image bannerBarImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : ARROWS
		/// <summary>
		/// The arrow to be on the left side of the combatant's name.
		/// </summary>
		[Title("Arrows")]
		[SerializeField, TabGroup("Banner", "Scene References")]
		private CombatantAnalysisDXBannerArrow leftBannerArrow;
		/// <summary>
		/// The arrow to be on the right side of the combatant's name.
		/// </summary>
		[SerializeField, TabGroup("Banner", "Scene References")]
		private CombatantAnalysisDXBannerArrow rightBannerArrow;
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this element.
		/// </summary>
		public void ResetState() {
			// Kill any tweens currently going.
			this.KillTweens();
			// Snap the anchor to its hiding position.
			this.nameAnchorRectTransform.anchoredPosition = this.nameAnchorHidingPos;
			// Also set the label to be blank.
			this.nameLabel.Text = "";
			// Make the banner clear.
			this.bannerBarImage.color = this.bannerBarHiddenColor;
			// Turn the banner arrows off. Be sure to snap it, so the fade does not happen.
			this.leftBannerArrow.SetVisible(status: false, snap: true);
			this.rightBannerArrow.SetVisible(status: false, snap: true);
		}
		/// <summary>
		/// Snaps tweens on this element to a given state.
		/// </summary>
		private void KillTweens() {
			// Kill any tweens currently going.
			this.nameAnchorRectTransform.DOKill(complete: true);
			this.bannerBarImage.DOKill(complete: true);
		}
		#endregion

		#region BUILDING
		public void Rebuild(CombatantAnalysisParams analysisParams) {
			
			this.KillTweens();
			
			// Set the text on the label.
			this.nameLabel.Text = analysisParams.CurrentCombatant.metaData.name;
			
			// I need to reset the position of the name anchor before tweening without also resetting the bar's alpha.
			this.nameAnchorRectTransform.anchoredPosition = this.nameAnchorHidingPos;
			this.nameAnchorRectTransform.DOAnchorPos(
					endValue: this.nameAnchorDisplayPos, 
					duration: this.nameAnchorTweenInTime, 
					snapping: true)
				.SetEase(ease: this.nameAnchorEaseInType);
			
		}
		#endregion
		
		#region ANIMATIONS
		/// <summary>
		/// Animates the element into view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Present(CombatantAnalysisParams analysisParams) {
			
			// Reset the state.
			this.ResetState();
			
			// Fade the bar in as well.
			this.bannerBarImage.DOColor(
					endValue: this.bannerBarDisplayColor, 
					duration: this.bannerBarFadeInTime)
				.SetEase(ease: Ease.Linear);
			
			// Fade the left/right arrows in if there are multiple combatants.
			this.leftBannerArrow.SetVisible(status: analysisParams.HasVisibleBannerArrows);
			this.rightBannerArrow.SetVisible(status: analysisParams.HasVisibleBannerArrows);
			
			// Set the text on the label.
			this.nameLabel.Text = analysisParams.CurrentCombatant.metaData.name;
			
			// Tween the anchor in.
			this.nameAnchorRectTransform.DOAnchorPos(
					endValue: this.nameAnchorDisplayPos, 
					duration: this.nameAnchorTweenInTime, 
					snapping: true)
				.SetEase(ease: this.nameAnchorEaseInType);
			
			
		}
		/// <summary>
		/// Dismisses this element from view.
		/// </summary>
		/// <param name="analysisParams">The parameters used to build this element.</param>
		public void Dismiss(CombatantAnalysisParams analysisParams) {
			
			// Kill any tweens currently active.
			this.KillTweens();
			
			// Set the arrows to be not visible.
			this.leftBannerArrow.SetVisible(status: false);
			this.rightBannerArrow.SetVisible(status: false);
			
			// Tween the anchor out.
			this.nameAnchorRectTransform.DOAnchorPos(
					endValue: this.nameAnchorDismissPos, 
					duration: this.nameAnchorTweenOutTime, 
					snapping: true)
				.SetEase(ease: this.nameAnchorEaseOutType);
			
			// Fade the bar out as well.
			this.bannerBarImage.DOColor(
					endValue: this.bannerBarHiddenColor, 
					duration: this.bannerBarFadeOutTime)
				.SetEase(ease: Ease.Linear);
			
		}
		#endregion

		#region NAVIGATION EVENTS
		/// <summary>
		/// Tells the EventSystem to select the Selectable attached to the name banner.
		/// </summary>
		public void SelectNameBanner() {
			Debug.Log("Selecting the Name Banner.");
			EventSystem.current.SetSelectedGameObject(this.bannerNavigationSelectable.gameObject);
		}
		#endregion
		
	}
	
}