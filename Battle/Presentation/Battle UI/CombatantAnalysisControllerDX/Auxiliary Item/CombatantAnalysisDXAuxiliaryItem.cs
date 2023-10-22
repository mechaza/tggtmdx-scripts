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
	/// A special kind of Battle Behavior item that appears above the rest of the items in the analysis.
	/// This is for things like upcoming moves that are unlocked on level up.
	/// </summary>
	public class CombatantAnalysisDXAuxiliaryItem : MonoBehaviour {

		#region FIELDS - STATE : TWEENS
		/// <summary>
		/// The tween that is currently changing the color on the highlight bar's color,
		/// if the overwrite animation is playing.
		/// </summary>
		private Tween CurrentOverwriteAnimationTween { get; set; }
		#endregion
		
		#region FIELDS - TWEENING : STRINGS
		/// <summary>
		/// The prefix to use for the behavior's labels when the the auxiliary item is highlighted.
		/// </summary>
		[Title("String Prefixes (Highlighted)")]
		[SerializeField, TabGroup("Auxiliary Item", "Tweening")]
		private string behaviorLabelHighlightPrefix = "";
		/// <summary>
		/// The prefix to use for the target level's labels when the the auxiliary item is highlighted.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Tweening")]
		private string targetLabelHighlightPrefix = "";
		/// <summary>
		/// The prefix to use for the behavior's labels when the the auxiliary item is dehighlighted.
		/// </summary>
		[Title("String Prefixes (Dehighlighted)")]
		[SerializeField, TabGroup("Auxiliary Item", "Tweening")]
		private string behaviorLabelDehighlightPrefix = "";
		/// <summary>
		/// The prefix to use for the target level's labels when the the auxiliary item is dehighlighted.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Tweening")]
		private string targetLabelDehighlightPrefix = "";
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the visuals as children.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private GameObject allVisuals;
		/// <summary>
		/// The STM used for this move's name.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private SuperTextMesh behaviorNameLabel;
		/// <summary>
		/// The cost for this behavior.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private SuperTextMesh behaviorCostLabel;
		/// <summary>
		/// The image used to represent the behavior's icon.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private Image behaviorElementalIconImage;
		/// <summary>
		/// The highlight for the icon itself. Yes I have two highlights is there a fucking problem?
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private Image behaviorElementalIconHighlightImage;
		/// <summary>
		/// The image that is used as a backing for the behavior's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private Image behaviorElementalIconBackingFrontImage;
		/// <summary>
		/// The image that is used as a backing's dropshadow for the behavior's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private Image behaviorElementalIconBackingDropshadowFrontImage;
		/// <summary>
		/// The GameObject that serves as a sort of Highlight.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private GameObject behaviorHighlightBarGameObject;
		/// <summary>
		/// The GameObject that contains the text mesh saying 'next'
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private GameObject nextLabel;
		/// <summary>
		/// The text mesh signaling the target level to unlock this behavior.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private SuperTextMesh targetLevelLabel;
		/// <summary>
		/// The image that should be used when animating a behavior being written to the item.
		/// </summary>
		[SerializeField, TabGroup("Auxiliary Item", "Scene References")]
		private Image overwriteAnimationBarImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this element.
		/// </summary>
		public void ResetState() {
			// Turn off all the visuals.
			this.allVisuals.SetActive(false);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the element into view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Present(CombatantAnalysisParams analysisParams) {
			this.Rebuild(analysisParams: analysisParams);
		}
		/// <summary>
		/// Dismisses the element from view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Dismiss(CombatantAnalysisParams analysisParams) {
			this.allVisuals.SetActive(false);
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Rebuild the element to reflect any changes in the parameters passed in.
		/// </summary>
		/// <param name="analysisParams">The parameters for this analysis screen. May contain changes to be shown.</param>
		public void Rebuild(CombatantAnalysisParams analysisParams) {
			
			// If this DOES have an auxiliary item, 
			if (analysisParams.HasAuxiliaryItem == true) {
				// Turn the object back on, save the params, and dehighlight.
				this.allVisuals.SetActive(true);
				this.Dehighlight(analysisParams: analysisParams);
			} else {
				// Otherwise, just keep it off.
				this.allVisuals.SetActive(false);
			}
			
		}
		/// <summary>
		/// Rebuild the auxiliary item with the analysis params currently stored in the controller.
		/// </summary>
		public void Rebuild() {
			this.Rebuild(analysisParams: CombatantAnalysisControllerDX.Instance.CurrentAnalysisParams);
		}
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Builds the auxiliary item in such a way where it looks dehighlighted.
		/// </summary>
		/// <param name="analysisParams">The analysis parameters used to build this item.</param>
		private void Dehighlight(CombatantAnalysisParams analysisParams) {
			
			// Grab the auxiliary params from the analysis params, then unpack the behavior from it.
			AuxiliaryItemParams auxiliaryItemParams = analysisParams.CurrentAuxiliaryItemParams;
			
			// Use these to build the item itself.
			this.behaviorNameLabel.Text = this.behaviorLabelDehighlightPrefix + auxiliaryItemParams.ItemName;
			this.behaviorCostLabel.Text = this.behaviorLabelDehighlightPrefix + auxiliaryItemParams.ItemQuantity;
			this.targetLevelLabel.Text = this.targetLabelDehighlightPrefix + auxiliaryItemParams.ItemTargetLevel;
			
			// Set the sprites.
			this.behaviorElementalIconImage.overrideSprite = auxiliaryItemParams.ItemIconSprite;
			this.behaviorElementalIconHighlightImage.overrideSprite = auxiliaryItemParams.ItemIconSprite;
			
			// Also set their colors.
			this.behaviorElementalIconImage.color = Color.white;
			this.behaviorElementalIconHighlightImage.gameObject.SetActive(true);
			this.behaviorElementalIconBackingFrontImage.color = Color.white;
			this.behaviorElementalIconBackingDropshadowFrontImage.color = Color.black;

			this.behaviorHighlightBarGameObject.SetActive(false);

		}
		/// <summary>
		/// Builds the auxiliary item in such a way where it looks highlighted.
		/// </summary>
		/// <param name="analysisParams">The analysis parameters used to build this item.</param>
		private void Highlight(CombatantAnalysisParams analysisParams) {
			
			// Grab the auxiliary params from the analysis params.
			AuxiliaryItemParams auxiliaryItemParams = analysisParams.CurrentAuxiliaryItemParams;
			
			// Use these to build the item itself.
			this.behaviorNameLabel.Text = this.behaviorLabelHighlightPrefix + auxiliaryItemParams.ItemName;
			this.behaviorCostLabel.Text = this.behaviorLabelHighlightPrefix + auxiliaryItemParams.ItemQuantity;
			this.targetLevelLabel.Text = this.targetLabelHighlightPrefix + auxiliaryItemParams.ItemTargetLevel;
			
			// Set the sprites.
			this.behaviorElementalIconImage.overrideSprite = auxiliaryItemParams.ItemIconSprite;
			this.behaviorElementalIconHighlightImage.overrideSprite = auxiliaryItemParams.ItemIconSprite;
			
			// Also set their colors.
			this.behaviorElementalIconImage.color = Color.white;
			this.behaviorElementalIconHighlightImage.gameObject.SetActive(false);
			this.behaviorElementalIconBackingFrontImage.color = Color.black;
			this.behaviorElementalIconBackingDropshadowFrontImage.color = Color.white;
			
			/*// Also set their colors.
			this.behaviorElementalIconImage.color = this.behaviorElementalIconFrontHighlightColor;
			this.behaviorElementalIconHighlightImage.color = this.behaviorElementalIconDropshadowHighlightColor;
			this.behaviorElementalIconBackingFrontImage.color = this.iconBackingFrontImageHighlightColor;
			this.behaviorElementalIconBackingDropshadowFrontImage.color = this.iconBackingDropshadowImageHighlightColor;*/
			
			this.behaviorHighlightBarGameObject.SetActive(true);
			
		}
		#endregion

		#region SPECIAL
		/// <summary>
		/// Plays an animation that shows this item b
		/// Note that this should only really be used when not interacting with the list.
		/// </summary>
		/// <param name="fadeTime">The amount of time to fade it by.</param>
		public void PlayOverwriteAnimation(float fadeTime = 1f) {
			
			// Kill the overwrite animation if it's currently going.
			this.CurrentOverwriteAnimationTween?.Kill(complete: true);
			
			// Dehighlight with the current params.
			this.Dehighlight(analysisParams: CombatantAnalysisControllerDX.Instance.CurrentAnalysisParams);
			
			// Now reset the image's visuals and play the tween.
			this.overwriteAnimationBarImage.color = GrawlyColors.Red;
			this.CurrentOverwriteAnimationTween = this.overwriteAnimationBarImage.DOColor(
					endValue: Color.clear, 
					duration: fadeTime)
				.SetEase(Ease.Linear);
			
		}
		#endregion
		
	}
}