using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// Manages the objects in the combatant analysis screen that display the combatant's body.
	/// </summary>
	public class CombatantAnalysisDXBodyGraphic : MonoBehaviour {

		#region FIELDS - TWEENING : POSITION
		/// <summary>
		/// The position the main anchor should be in when hiding.
		/// </summary>
		[Title("Positions")]
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private Vector2Int mainPivotHidingPos = new Vector2Int();
		/// <summary>
		/// The position the main anchor should be in when displayed.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private Vector2Int mainPivotDisplayPos = new Vector2Int();
		/// <summary>
		/// The position the dropshadow image should be when in hiding.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private Vector2Int dropshadowImageHidingPos = new Vector2Int();
		/// <summary>
		/// The position the dropshadow image should be when in displayed.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private Vector2Int dropshadowImageDisplayPos = new Vector2Int();
		#endregion
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when tweening the main anchor in.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private float mainPivotTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the main anchor out.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private float mainPivotTweenOutTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the dropshadow image in.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private float dropshadowImageTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the dropshadow image out.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private float dropshadowImageTweenOutTime = 0.2f;
		/// <summary>
		/// The amount of time to take when fading the body sprite image in.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private float bodySpriteImageFadeInTime = 0.1f;
		/// <summary>
		/// The amount of time to take when fading the body sprite image out.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private float bodySpriteImageFadeOutTime = 0.2f;
		/// <summary>
		/// The amount of time to take when fading the dropshadow image in.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private float dropshadowImageFadeInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when fading the dropshadow image out.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private float dropshadowImageFadeOutTime = 0.1f;
		#endregion
		
		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the main anchor in.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private Ease mainPivotEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the main anchor out.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private Ease mainPivotEaseOutType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use on the dropshadow image when tweening it in.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private Ease dropshadowImageEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use on the dropshadow image when tweening it out.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Tweening")]
		private Ease dropshadowImageEaseOutType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the other references in its children.
		/// </summary>
		[SerializeField, TabGroup("Graphic","Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The pivot that controls the primary movement of the graphic.
		/// </summary>
		[SerializeField, TabGroup("Graphic","Scene References")]
		private RectTransform graphicMainPivot;
		/// <summary>
		/// The front image showing the combatant's body.
		/// </summary>
		[SerializeField, TabGroup("Graphic","Scene References")]
		private Image combatantBodyFrontImage;
		/// <summary>
		/// The dropshadow image showing the combatant's body.
		/// </summary>
		[SerializeField, TabGroup("Graphic","Scene References")]
		private Image combatantBodyDropshadowImage;
		/// <summary>
		/// The RectTransform that is attached to the dropshadow.
		/// I hate this.
		/// </summary>
		[SerializeField, TabGroup("Graphic", "Scene References")]
		private RectTransform combatantBodyDropshadowRectTransform;
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this element.
		/// </summary>
		public void ResetState() {
			// Kill the tweens.
			this.KillTweens();
			// Reset positions.
			this.graphicMainPivot.anchoredPosition = this.mainPivotHidingPos;
			this.combatantBodyDropshadowRectTransform.anchoredPosition = this.dropshadowImageHidingPos;
			this.combatantBodyFrontImage.color = Color.clear;
			this.combatantBodyDropshadowImage.color = Color.clear;
		}
		/// <summary>
		/// Kill any tweens currently running on this graphic.
		/// </summary>
		private void KillTweens() {
			this.graphicMainPivot.DOKill(complete: true);
			this.combatantBodyDropshadowRectTransform.DOKill(complete: true);
			this.combatantBodyFrontImage.DOKill(complete: true);
			this.combatantBodyDropshadowImage.DOKill(complete: true);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the element into view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Present(CombatantAnalysisParams analysisParams) {
			
			// Reset the graphic to its initial state.
			this.ResetState();
			
			Debug.LogWarning("This needs to be fixed because I really don't like doing this kind of check! " 
			                 + "Everyone has a bust up, try doing that?");
			// TODO: MAKE THIS NOT MESSY
			
			if (analysisParams.CurrentCombatant is Enemy) {
				Sprite bodySprite = (analysisParams.CurrentCombatant as Enemy).EnemySprite;
				this.combatantBodyFrontImage.overrideSprite = bodySprite;
				this.combatantBodyDropshadowImage.overrideSprite = bodySprite;
			} else if (analysisParams.CurrentCombatant is Persona) {
				Sprite bodySprite = (analysisParams.CurrentCombatant as Persona).bustUp;
				this.combatantBodyFrontImage.overrideSprite = bodySprite;
				this.combatantBodyDropshadowImage.overrideSprite = bodySprite;
			} else {
				throw new System.Exception("Combatant is neither enemy nor persona! Please fix this!");
			}
			
			// Tween that shit in.
			this.graphicMainPivot.DOAnchorPos(
					endValue: this.mainPivotDisplayPos, 
					duration: this.mainPivotTweenInTime, 
					snapping: true)
				.SetEase(ease: this.mainPivotEaseInType);
			this.combatantBodyDropshadowRectTransform.DOAnchorPos(
					endValue: this.dropshadowImageDisplayPos, 
					duration: this.dropshadowImageTweenInTime, 
					snapping: true)
				.SetEase(ease: this.dropshadowImageEaseInType);
			this.combatantBodyFrontImage.DOColor(
					endValue: Color.white, 
					duration: this.bodySpriteImageFadeInTime)
				.SetEase(ease: Ease.Linear);
			this.combatantBodyDropshadowImage.DOColor(
				endValue: Color.black, 
				duration: this.dropshadowImageFadeInTime)
				.SetEase(ease: Ease.Linear);

		}
		/// <summary>
		/// Dismisses this element from view.
		/// </summary>
		/// <param name="analysisParams">The parameters used to build this element.</param>
		public void Dismiss(CombatantAnalysisParams analysisParams) {
			
			// Kill any tweens on the graphic.
			this.KillTweens();
			
			// Tween that shit OUT.
			this.graphicMainPivot.DOAnchorPos(
					endValue: this.mainPivotHidingPos, 
					duration: this.mainPivotTweenOutTime, 
					snapping: true)
				.SetEase(ease: this.mainPivotEaseOutType);
			this.combatantBodyDropshadowRectTransform.DOAnchorPos(
					endValue: this.dropshadowImageHidingPos, 
					duration: this.dropshadowImageTweenOutTime, 
					snapping: true)
				.SetEase(ease: this.dropshadowImageEaseOutType);
			this.combatantBodyFrontImage.DOColor(
					endValue: Color.clear, 
					duration: this.bodySpriteImageFadeOutTime)
				.SetEase(ease: Ease.Linear);
			this.combatantBodyDropshadowImage.DOColor(
					endValue: Color.clear, 
					duration: this.dropshadowImageFadeOutTime)
				.SetEase(ease: Ease.Linear);
		}
		#endregion
		
	}
}