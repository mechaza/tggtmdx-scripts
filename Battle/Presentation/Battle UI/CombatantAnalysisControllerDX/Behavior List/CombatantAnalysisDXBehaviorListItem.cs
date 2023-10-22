using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// Represents a behavior in the menu list within the combatant analysis screen.
	/// </summary>
	public class CombatantAnalysisDXBehaviorListItem : MenuItem {

		#region FIELDS - STATE : GENERAL
		/// <summary>
		/// The BattleBehavior that is currently being used on this list item.
		/// </summary>
		public BattleBehavior CurrentBattleBehavior { get; set; }
		#endregion

		#region FIELDS - STATE : TWEENS
		/// <summary>
		/// The tween that is currently changing the color on the highlight bar's color,
		/// if the overwrite animation is playing.
		/// </summary>
		private Tween CurrentOverwriteAnimationTween { get; set; }
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The STM used for this move's name.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private SuperTextMesh behaviorNameLabel;
		/// <summary>
		/// The cost for this behavior.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private SuperTextMesh behaviorCostLabel;
		/// <summary>
		/// The image used to represent the behavior's icon.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image behaviorElementalIconImage;
		/// <summary>
		/// The highlight for the icon itself. Yes I have two highlights is there a fucking problem?
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image behaviorElementalIconHighlightImage;
		/// <summary>
		/// The image that is used as a backing for the behavior's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image behaviorElementalIconBackingFrontImage;
		/// <summary>
		/// The image that is used as a backing's dropshadow for the behavior's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image behaviorElementalIconBackingDropshadowFrontImage;
		/// <summary>
		/// The GameObject that serves as a sort of Highlight.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private GameObject behaviorHighlightBarGameObject;
		/// <summary>
		/// The image that should be used when animating a behavior being written to the item.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image overwriteAnimationBarImage;
		#endregion
		
		#region PROPERTIES - MENUITEM : STATE
		/// <summary>
		/// Is this item usable?
		/// </summary>
		protected override bool IsUsable {
			get {
				return true;
			}
		}
		#endregion

		#region SPECIAL ANIMATIONS
		/// <summary>
		/// Plays an animation that shows this item being overwritten.
		/// Note that this should only really be used when not interacting with the list.
		/// </summary>
		/// <param name="fadeTime">The amount of time to fade it by.</param>
		public void PlayOverwriteAnimation(float fadeTime = 1f) {
			// Just use the current behavior.
			this.PlayOverwriteAnimation(
				newBehavior: this.CurrentBattleBehavior, 
				overwriteSavedMenuable: false, 
				fadeTime: fadeTime);
		}
		/// <summary>
		/// Plays an animation that shows this item being overwritten.
		/// Note that this should only really be used when not interacting with the list.
		/// </summary>
		/// <param name="newBehavior">The behavior to flash.</param>
		/// <param name="overwriteSavedMenuable">Should the saved menuable also be overwritten?.</param>
		/// <param name="fadeTime">The amount of time to fade it by.</param>
		public void PlayOverwriteAnimation(BattleBehavior newBehavior, bool overwriteSavedMenuable = false, float fadeTime = 1f) {

			if (overwriteSavedMenuable == true) {
				Debug.Log("Overwriting menuable.");
				this.CurrentBattleBehavior = newBehavior;
			}
			
			// Dehighlight the visuals.
			this.Dehighlight(newBehavior);
			
			// Kill + complete the current overwrite animation tween, if one exists.
			this.CurrentOverwriteAnimationTween?.Kill(complete: true);
			
			// Now reset the image's visuals and play the tween.
			this.overwriteAnimationBarImage.color = GrawlyColors.Red;
			this.CurrentOverwriteAnimationTween = this.overwriteAnimationBarImage.DOColor(
				endValue: Color.clear, 
				duration: fadeTime)
				.SetEase(Ease.Linear);

		}
		#endregion
		
		#region OVERRIDES - MENUITEM : BUILDING
		/// <summary>
		/// Builds the list item with the provided menuable.
		/// </summary>
		/// <param name="item">The BattleBehavior to build with.</param>
		public override void BuildMenuItem(IMenuable item) {
			// First and foremost, save a reference to the item.
			this.CurrentBattleBehavior = (BattleBehavior)item;

			// Call Dehighlight. This effectively builds the strings.
			this.Dehighlight(item: item);

			// Set the sprite on the elemental icon and its highlight
			this.behaviorElementalIconImage.overrideSprite = item.Icon;
			this.behaviorElementalIconHighlightImage.overrideSprite = item.Icon;
		}
		/// <summary>
		/// Sets the visuals on this item to be highlighted.
		/// </summary>
		/// <param name="item">The BattleBehavior to build with.</param>
		protected internal override void Highlight(IMenuable item) {
			// Rebuild the labels with the appropriate strings.
			this.behaviorNameLabel.Text = (this.IsUsable == true ? "<c=white>" : "<c=gray>") + item.PrimaryString;
			this.behaviorCostLabel.Text = (this.IsUsable == true ? "<c=white>" : "<c=gray>") + (item as BattleBehavior).QuantityString;

			// Also update the elemental icon images
			this.behaviorElementalIconHighlightImage.gameObject.SetActive(true);
			this.behaviorElementalIconBackingFrontImage.color = (((BattleBehavior)item).behaviorType == BehaviorType.Special) ? Grawly.GrawlyColors.colorDict[GrawlyColorTypes.Blue] : Grawly.GrawlyColors.colorDict[GrawlyColorTypes.Yellow];
			this.behaviorElementalIconBackingDropshadowFrontImage.color = Color.white;

			// Turn on the highlight bar.
			this.behaviorHighlightBarGameObject.SetActive(true);
			// Also assign the text on the info box.
			// TODO: Do I need to set the info box text too?
			// BattleMenuDXBehaviorMenuList.instance.SetInfoBoxText(text: item.DescriptionString);
		}
		/// <summary>
		/// Sets the visuals on this item to be dehighlighted.
		/// </summary>
		/// <param name="item">The BattleBehavior to build with.</param>
		protected internal override void Dehighlight(IMenuable item) {
			// Rebuild the labels with the appropriate strings.
			this.behaviorNameLabel.Text = (this.IsUsable == true ? "<c=black>" : "<c=gray>") + item.PrimaryString;
			this.behaviorCostLabel.Text = (this.IsUsable == true ? "<c=black>" : "<c=gray>") +  (item as BattleBehavior).QuantityString;

			// Also update the elemental icon images
			this.behaviorElementalIconHighlightImage.gameObject.SetActive(false);
			this.behaviorElementalIconBackingFrontImage.color = Color.black;
			this.behaviorElementalIconBackingDropshadowFrontImage.color = Color.clear;

			// Turn off the highlight bar.
			this.behaviorHighlightBarGameObject.SetActive(false);
		}
		#endregion
		
		#region OVERRIDES - MENUITEM : EVENTS
		public override void OnSelect(BaseEventData eventData) {
			this.Highlight(item: this.CurrentBattleBehavior);
			// Also invoke the appropriate event in the callback set, if one is available.
			CombatantAnalysisControllerDX.Instance.CurrentCallbackSet?.OnBehaviorItemSelect.Invoke((
				combatant: CombatantAnalysisControllerDX.Instance.CurrentCombatant, 
				menuList: CombatantAnalysisControllerDX.Instance.BehaviorMenuList,
				listItem: this));
			// TODO: Maybe change it so I don't need both of these calls?
		}
		public override void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(item: this.CurrentBattleBehavior);
			AudioController.instance?.PlaySFX(SFXType.Hover);
			// Also invoke the appropriate event in the callback set, if one is available.
			CombatantAnalysisControllerDX.Instance.CurrentCallbackSet?.OnBehaviorItemDeselect.Invoke((
				combatant: CombatantAnalysisControllerDX.Instance.CurrentCombatant, 
				menuList: CombatantAnalysisControllerDX.Instance.BehaviorMenuList,
				listItem: this));
			// TODO: Maybe change it so I don't need both of these calls?
		}
		public override void OnSubmit(BaseEventData eventData) {
			CombatantAnalysisControllerDX.Instance.TriggerEvent(eventName: "Behavior Submit");
			// Also invoke the appropriate event in the callback set, if one is available.
			CombatantAnalysisControllerDX.Instance.CurrentCallbackSet?.OnBehaviorItemSubmit.Invoke((
				combatant: CombatantAnalysisControllerDX.Instance.CurrentCombatant, 
				menuList: CombatantAnalysisControllerDX.Instance.BehaviorMenuList,
				listItem: this));
			// TODO: Maybe change it so I don't need both of these calls?
		}
		public override void OnCancel(BaseEventData eventData) {
			
			// Play the close sound effect.
			AudioController.instance?.PlaySFX(SFXType.Close);
			
			
			// Trigger an event on the analysis controller telling it Cancel was hit on the list item.
			CombatantAnalysisControllerDX.Instance.TriggerEvent(eventName: "Behavior Cancel");
			
			// Also invoke the appropriate event in the callback set, if one is available.
			CombatantAnalysisControllerDX.Instance.CurrentCallbackSet?.OnBehaviorItemCancel.Invoke((
				combatant: CombatantAnalysisControllerDX.Instance.CurrentCombatant, 
				menuList: CombatantAnalysisControllerDX.Instance.BehaviorMenuList,
				listItem: this));
			// TODO: Maybe change it so I don't need both of these calls?
			
		}
		protected override void OnHorizontalMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMove(moveDir);
			
			// Tell the analysis controller that there was a horizontal event on the behavior item.
			CombatantAnalysisHelperDX.Instance.CurrentHorizontalMovement = moveDir;
			CombatantAnalysisControllerDX.Instance.TriggerEvent(eventName: "Behavior Horizontal Movement");
			
			// Also invoke the appropriate event in the callback set, if one is available.
			CombatantAnalysisControllerDX.Instance.CurrentCallbackSet?.OnBehaviorItemHorizontalMove.Invoke((
				combatant: CombatantAnalysisControllerDX.Instance.CurrentCombatant, 
				menuList: CombatantAnalysisControllerDX.Instance.BehaviorMenuList,
				listItem: this,
				horizontalDir: moveDir));
			// TODO: Maybe change it so I don't need both of these calls?
			
		}
		#endregion
		
		
		
	}
	
}