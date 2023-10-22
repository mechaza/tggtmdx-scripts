using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Menus;
using UnityEngine;
using Grawly.UI.SubItem;
using Sirenix.OdinInspector;
using Grawly.Toggles;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Grawly.UI {
	public class BattleTestMenuListItem : MenuItem {
		
		#region FIELDS - STATE
		/// <summary>
		/// Ths should always be yes anyway.
		/// </summary>
		protected override bool IsUsable {
			get {
				return true;
			}
		}
		/// <summary>
		/// The BattleTemplate to use for this selection.
		/// </summary>
		private BattleTemplate battleTemplate;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The STM that shows the name of the particular setting being... set.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private SuperTextMesh settingPrimaryLabel;
		/// <summary>
		/// The STM that describes the setting's functionality.
		/// </summary>
		[SerializeField]
		private SuperTextMesh settingDescriptionLabel;
		/// <summary>
		/// The image for the screenshot display.
		/// </summary>
		[SerializeField]
		private Image screenshotPreviewImage;
		/// <summary>
		/// The image being used for the backing. Perhaps just as a debug for now.
		/// </summary>
		[SerializeField]
		private Image backingImage;
		/// <summary>
		/// The image that makes the highlight look like a dropshadow.
		/// </summary>
		[SerializeField]
		private Image backingDropshadowImage;
		/// <summary>
		/// The class which this item uses to interface with the sub items.
		/// </summary>
		[SerializeField]
		private SubItemContainer subItemContainer;
		#endregion

		#region BUILDING
		public override void BuildMenuItem(IMenuable item) {

			// Save the menuable being built.
			this.battleTemplate = (BattleTemplate)item;

			// Set the screenshot image.
			this.screenshotPreviewImage.overrideSprite = this.battleTemplate.previewScreenshot;
			
			// NOTE THAT THIS IS RIPPED FROM THE SETTINGS. THIS ENTIRE LINE IS JUST GONNA MAKE AN EMPTY ITEM PARAMS
			this.subItemContainer.Build(subItemParams: this.battleTemplate is GTSubItem ? (this.battleTemplate as GTSubItem).CurrentSubItemParams : SubItemParams.EmptyParams);

			// Dehighlight it. Note that this will also dehighlight the sub item a second time. I'm okay with that.
			this.Dehighlight(item: this.battleTemplate);


		}
		protected internal override void Dehighlight(IMenuable item) {

			// Also set their material.
			this.settingPrimaryLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Default");
			this.settingDescriptionLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Default");
			// Assign the primary and description labels.
			this.settingPrimaryLabel.Text = "<c=black>" + item.PrimaryString;
			
			// this.settingDescriptionLabel.Text = "<c=black>" + item.DescriptionString;
			this.settingDescriptionLabel.Text = "<c=black>" + this.battleTemplate.BattleDescription;
			
			// Set the color on the backing image.
			this.backingImage.color = Color.clear;
			this.backingDropshadowImage.color = Color.clear;

			// Fade the screenshot.
			this.screenshotPreviewImage.color = Color.gray;
			
			this.subItemContainer.Dehighlight(subItemParams: this.battleTemplate is GTSubItem ? (this.battleTemplate as GTSubItem).CurrentSubItemParams : SubItemParams.EmptyParams);

		}
		protected internal override void Highlight(IMenuable item) {

			// Also set their material.
			this.settingPrimaryLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			this.settingDescriptionLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			// Assign the primary and description labels.
			this.settingPrimaryLabel.Text = "<c=white>" + item.PrimaryString;
			
			// this.settingDescriptionLabel.Text = "<c=white>" + item.DescriptionString;
			this.settingDescriptionLabel.Text = "<c=white>" + this.battleTemplate.BattleDescription;

			// Set the color on the backing image.
			this.backingImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
			this.backingDropshadowImage.color = Color.black;

			// Brighten the screenshot.
			this.screenshotPreviewImage.color = Color.white;
			
			this.subItemContainer.Highlight(subItemParams: this.battleTemplate is GTSubItem ? (this.battleTemplate as GTSubItem).CurrentSubItemParams : SubItemParams.EmptyParams);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - UNITY EVENTS
		public override void OnSubmit(BaseEventData eventData) {
			
			GameObject selectedObject = eventData.selectedObject;
			
			OptionPicker.instance.Display(
				prompt: "Start this battle?", 
				option1: "Yes", 
				option2: "No", 
				callback1: delegate {
					
					// Send this to the test controller so it can be re-selected on completion.
					BattleTestController.Instance.LastSelectedBattleTemplateItem = this.gameObject;
					
					// Start the battle.
					BattleController.Instance.StartBattle(this.battleTemplate);
					
				},
				callback2: delegate {
					EventSystem.current.SetSelectedGameObject(selectedObject);
				}, 
				reselectOnDone: false);
			
		}
		public override void OnCancel(BaseEventData eventData) {
			GameObject selectedObject = eventData.selectedObject;
			
			OptionPicker.instance.Display(
				prompt: "Return to title?", 
				option1: "Yes", 
				option2: "No", 
				callback1: delegate {
					SceneController.instance.BasicLoadSceneWithFade(sceneIndex: 2);
				},
				callback2: delegate {
					EventSystem.current.SetSelectedGameObject(selectedObject);
				}, 
				reselectOnDone: false);
			
		}
		public override void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(type: SFXType.Hover);
			this.Dehighlight(item: this.battleTemplate);
		}
		public override void OnSelect(BaseEventData eventData) {
			this.Highlight(this.battleTemplate);
		}
		#endregion

		#region OVERRIDDEN EVENTS
		/// <summary>
		/// Upon moving left or right, the sub item should be notified.
		/// </summary>
		/// <param name="moveDir"></param>
		protected override void OnHorizontalMove(HorizontalMoveDirType moveDir) {

			// throw new System.NotImplementedException("adsf");
			
		
			// AudioController.Instance?.PlaySFX(type: SFXType.Hover);
			// (this.gameToggle as GTHorizontalMoveHandler)?.OnHorizontalMenuMove(moveDir: moveDir);
			// this.Highlight(item: this.gameToggle);
		}
		#endregion
		
	}

}