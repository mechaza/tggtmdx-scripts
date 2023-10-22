using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Grawly.Battle;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Grawly.UI {

	/// <summary>
	/// Represents an item from the inventory as part of an InventoryMenuList.
	/// </summary>
	public sealed class InventoryMenuListItem : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// The index of this menu list item as a child of its parent in the hierarchy. Primarily for keeping track of which was last highlighted. I hate this.
		/// </summary>
		[SerializeField, TabGroup("Menu Item", "Toggles")]
		private int index = -1;
		/// <summary>
		/// The index of this menu list item as a child of its parent in the hierarchy. Primarily for keeping track of which was last highlighted. I hate this.
		/// </summary>
		public int Index {
			get {
				return this.index;
			}
		}
		/// <summary>
		/// The behavior that is to be used for this menu list item.
		/// </summary>
		public IMenuable item { get; private set; }
		/// <summary>
		/// Is this item available for use? I.e., if not, will play a buzzer sound or soemthing.
		/// </summary>
		protected override bool IsUsable {
			get {

				// If this is an inventory item, check if its pause functions are higher than zero. I need to do this calculation here
				// because the item list is the only list that has a scrollbar. I don't need to override the others.
				if (this.item is InventoryItem) {
					// Debug.Log("PAUSE MENU: Item is inventory item. Only can use if pause function count is higher than zero.");
					return ((InventoryItem)this.item).behavior.PauseFunctions.Count > 0;
				} else if (this.item is BattleBehavior) {

					// If this is a battle behavior, its good to assume that this is a skill being used.
					// Return whether or not the source player is capable of using it.
					// That and if it has more than 0 pause functions.
					return PauseMenuController.instance
						.SourcePlayer.HasResourcesForBehavior(behavior: (BattleBehavior)this.item)
						&& ((BattleBehavior)this.item).PauseFunctions.Count > 0;

				} else {
					// Otherwise, return  what was passed in earlier when building this item.
					return true;
				}
				
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label used to represent this item in the menu.
		/// </summary>
		[SerializeField, TabGroup("Menu Item", "Scene References")]
		private SuperTextMesh nameLabel;
		/// <summary>
		/// The label used to represent the number of items available.
		/// </summary>
		[SerializeField, TabGroup("Menu Item", "Scene References")]
		private SuperTextMesh countLabel;
		/// <summary>
		/// The image used to represent the icon.
		/// </summary>
		[SerializeField, TabGroup("Menu Item", "Scene References")]
		private Image iconImage;
		/// <summary>
		/// The object that appears behind this list item when it's highlighted.
		/// </summary>
		[SerializeField, TabGroup("Menu Item", "Scene References")]
		private GameObject highlightObject;
		#endregion

		#region BUILDING
		public override void BuildMenuItem(IMenuable item) {
			// Convert the item to a battle behavior and save it.
			this.item = item;
			// Re-enable the icon image if it was previously truend off.
			this.iconImage.gameObject.SetActive(true);
			// Build the button.
			this.Dehighlight(item: this.item);
		}
		#endregion

		#region GRAPHICS
		/// <summary>
		/// Gets called from PlayMaker.
		/// It is implicitly assumed that this script has a reference to the item it needs to build at this point.
		/// </summary>
		public void Highlight() {
			this.Highlight(item: this.item);
		}
		/// <summary>
		/// Gets called from PlayMaker.
		/// It is implicitly assumed that this script has a reference to the item it needs to build at this point.
		/// </summary>
		public void Dehighlight() {
			this.Dehighlight(item: this.item);
		}
		protected internal override void Dehighlight(IMenuable item) {

		

			// this.nameLabel.Text = "<c=black>" + ((InventoryItem)item).behavior.behaviorName;
			// this.countLabel.Text = "<c=black>x" + ((InventoryItem)item).Count;
			// this.nameLabel.Text = "<c=black>" + item.PrimaryString;
			// this.countLabel.Text = "<c=black>" + item.QuantityString;

			// Determine what color should be used as a prefix if this can be used.
			string colorPrefix = this.IsUsable == true ? "<c=black>" : "<c=gray>";

			this.nameLabel.Text = colorPrefix + item.PrimaryString;
			this.countLabel.Text = colorPrefix + item.QuantityString;

			this.nameLabel.textMaterial = DataController.GetDefaultSTMMaterial(materialName: "UIDefault");
			this.countLabel.textMaterial = DataController.GetDefaultSTMMaterial(materialName: "UIDefault");
			this.nameLabel.Rebuild();
			this.countLabel.Rebuild();
			// this.iconImage.sprite = item.Icon;
			this.iconImage.overrideSprite = item.Icon;
			// this.iconImage.sprite = ((InventoryItem)item).behavior.GetMenuSprite();
			this.highlightObject.SetActive(false);
		}
		protected internal override void Highlight(IMenuable item) {
			// this.nameLabel.Text = "<c=white>" + ((InventoryItem)item).behavior.behaviorName;
			// this.countLabel.Text = "<c=white>x" + ((InventoryItem)item).Count;
			// this.nameLabel.Text = "<c=white>" + item.PrimaryString;
			// this.countLabel.Text = "<c=white>" + item.QuantityString;

			

			// Determine what color should be used as a prefix if this can be used.
			string colorPrefix = this.IsUsable == true ? "<c=white>" : "<c=white>";

			this.nameLabel.Text = colorPrefix + item.PrimaryString;
			this.countLabel.Text = colorPrefix + item.QuantityString;

			this.nameLabel.textMaterial = DataController.GetDefaultSTMMaterial(materialName: "UI Dropshadow 2");
			this.countLabel.textMaterial = DataController.GetDefaultSTMMaterial(materialName: "UI Dropshadow 2");
			this.nameLabel.Rebuild();
			this.countLabel.Rebuild();
			// this.iconImage.sprite = item.Icon;
			this.iconImage.overrideSprite = item.Icon;
			// this.iconImage.sprite = ((InventoryItem)item).behavior.GetMenuSprite();
			this.highlightObject.SetActive(true);
		}
		#endregion

		#region UI EVENT TRIGGERS
		public override void OnSelect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Hover);
			// Debug.Log("PAUSE MENU: Overriding FSM's currentMenuItem variable with GameObject representing " + this.inventoryItem.behavior.behaviorName);
			// Pass a reference to this item to the FSM. It will need it if it gets used.
			// PauseMenuController.Instance.FSM.FsmVariables.GetVariable("currentMenuItem").SafeAssign(val: this.gameObject);
			// PauseMenuController.Instance.CurrentSelectedInventoryMenuListItemGameObject = this.gameObject;
			PauseMenuController.instance.SetCurrentItemIndex(index: this.Index);
			// Build the info box to describe the item.
			PauseMenuController.instance.BuildInfoBox(description: this.item.DescriptionString);
			// Highlight this item in the list.
			this.Highlight(item: this.item);
		}
		public override void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(item: this.item);
		}
		public override void OnSubmit(BaseEventData eventData) {
			// Only submit if this item is actually usable.
			if (this.IsUsable == true) {
				AudioController.instance?.PlaySFX(SFXType.Select);
				PauseMenuController.instance.FSM.SendEvent("Picked Item");
			} else {
				// If it's not valid, play the error sound.
				AudioController.instance?.PlaySFX(type: SFXType.Invalid, scale: 1f);
			}
			
		}
		public override void OnCancel(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Close);
			PauseMenuController.instance.FSM.SendEvent("Back");
		}
		#endregion

	}


}