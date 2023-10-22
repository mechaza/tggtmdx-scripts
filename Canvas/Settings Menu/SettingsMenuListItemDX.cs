using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI.SubItem;
using Sirenix.OdinInspector;
using Grawly.Toggles;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Grawly.UI {

	/// <summary>
	/// What gets built on the menu list inside the settings menu.
	/// </summary>
	public class SettingsMenuListItemDX : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// Can the current combatant use this behavior?
		/// </summary>
		protected override bool IsUsable {
			get {
				// FEEL FREE TO OVERRIDE THIS IF YOU WANT TO DISABLE CERTAIN SETTINGS THAT SHOULD BE OBFUSCATED.
				return true;
			}
		}
		/// <summary>
		/// The GameToggle this item is currently representing.
		/// </summary>
		private GameToggleDX gameToggle;
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
			this.gameToggle = (GameToggleDX)item;
			
			// Build the sub item container. If the toggle has sub item params, use those. Otherwise, use the empty params.
			this.subItemContainer.Build(subItemParams: this.gameToggle is GTSubItem
					? (this.gameToggle as GTSubItem).CurrentSubItemParams 
					: SubItemParams.EmptyParams);

			// Dehighlight it. Note that this will also dehighlight the sub item a second time. I'm okay with that.
			this.Dehighlight(item: this.gameToggle);

		}
		protected internal override void Dehighlight(IMenuable item) {

			// Also set their material.
			this.settingPrimaryLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Default");
			this.settingDescriptionLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Default");
			
			// Assign the primary and description labels.
			this.settingPrimaryLabel.Text = "<c=black>" + item.PrimaryString;
			this.settingDescriptionLabel.Text = "<c=black>" + item.DescriptionString;
			
			// Set the color on the backing image.
			this.backingImage.color = Color.clear;
			this.backingDropshadowImage.color = Color.clear;

			// Generate sub item params from the item and pass them to the sub item container.
			// If the toggle has sub item params, use those. Otherwise, use the empty params.
			this.subItemContainer.Dehighlight(subItemParams: this.gameToggle is GTSubItem 
				? (this.gameToggle as GTSubItem).CurrentSubItemParams
				: SubItemParams.EmptyParams);

		}
		protected internal override void Highlight(IMenuable item) {

			// Also set their material.
			this.settingPrimaryLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			this.settingDescriptionLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			// Assign the primary and description labels.
			this.settingPrimaryLabel.Text = "<c=white>" + item.PrimaryString;
			this.settingDescriptionLabel.Text = "<c=white>" + item.DescriptionString;

			// Set the color on the backing image.
			this.backingImage.color = GrawlyColors.GetColorFromToggleCategory(categoryType: this.gameToggle.CategoryType);
			this.backingDropshadowImage.color = Color.black;

			// Generate sub item params from the item and pass them to the sub item container.
			this.subItemContainer.Highlight(subItemParams: this.gameToggle is GTSubItem 
				? (this.gameToggle as GTSubItem).CurrentSubItemParams 
				: SubItemParams.EmptyParams);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - UNITY EVENTS
		public override void OnSubmit(BaseEventData eventData) {
			// If the toggle is a submit handler, well, submit.
			(this.gameToggle as GTSubmitHandler)?.OnSubmit(eventData);
		}
		public override void OnCancel(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(type: SFXType.Close);
			this.Dehighlight(item: this.gameToggle);
			// Tell the settings menu controller to go back.
			SettingsMenuControllerDX.instance.CancelledMenuListItem(categoryType: this.gameToggle.CategoryType);
			// PauseMenuController.Instance?.FSM.SendEvent("Back");
			// throw new System.NotImplementedException("OnCancel called. What are you going to do about it?");
		}
		public override void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(type: SFXType.Hover);
			this.Dehighlight(item: this.gameToggle);
		}
		public override void OnSelect(BaseEventData eventData) {
			this.Highlight(this.gameToggle);
		}
		#endregion

		#region OVERRIDDEN EVENTS
		/// <summary>
		/// Upon moving left or right, the sub item should be notified.
		/// </summary>
		/// <param name="moveDir"></param>
		protected override void OnHorizontalMove(HorizontalMoveDirType moveDir) {

			// throw new System.NotImplementedException("haha");
			AudioController.instance?.PlaySFX(type: SFXType.Hover);
			(this.gameToggle as GTHorizontalMoveHandler)?.OnHorizontalMenuMove(moveDir: moveDir);

			// Highlight the option. This will rebuild the sub item anyway, and it's safe to assume this is the item being highlighted.
			this.Highlight(item: this.gameToggle);
			// throw new System.NotImplementedException("Alert the sub item container about the horizontal event.");
		}
		#endregion

	}


}