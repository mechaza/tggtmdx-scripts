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
using Grawly.Menus;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Shop.Prototype {
	/// <summary>
	/// An item that is listed in the prototype shop.
	/// </summary>
	public class PrototypeStoreMenuItem : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// The shop item assigned to this menu item.
		/// </summary>
		public PrototypeShopItem CurrentShopItem { get; private set; }
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label displaying the item's name.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private SuperTextMesh itemNameLabel;
		/// <summary>
		/// The label displaying the item's cost.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private SuperTextMesh itemCostLabel;
		/// <summary>
		/// The image for the highlight's front.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private Image highlightFrontImage;
		/// <summary>
		/// The image for the highlight's dropshadow.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private Image highlightDropshadowImage;
		#endregion

		#region PROPERTIES - MENUITEM : STATE
		/// <summary>
		/// Is this item usable? I imagine.
		/// </summary>
		protected override bool IsUsable {
			get {
				switch (this.CurrentShopItem.PrototypeShopItemType) {
					case PrototypeShopItemType.BattleBehaviorItem:
						// Figure out if the player can affort this item.
						int availableFunds = GameController.Instance.Variables.Money;
						int itemCost = this.CurrentShopItem.ItemCost;
						bool playerCanAfford = (availableFunds - itemCost) >= 0;
						return playerCanAfford;
					default:
						throw new NotImplementedException("I have not defined functionality for this scenario!");
				}
			}
		}
		#endregion

		#region OVERRIDES - MENUITEM : BUILDING
		/// <summary>
		/// Builds the MenuItem with the provided IMenuable.
		/// </summary>
		/// <param name="item"></param>
		public override void BuildMenuItem(IMenuable item) {
			
			// Save the item that was just passed in.
			this.CurrentShopItem = (PrototypeShopItem) item;
			
			// Build it by dehighlighting it.
			this.Dehighlight(this.CurrentShopItem);
			
		}
		/// <summary>
		/// Sets the visuals on this item to be highlighted.
		/// </summary>
		/// <param name="item"></param>
		protected internal override void Highlight(IMenuable item) {
			this.itemNameLabel.Text =  "<c=black>" + item.PrimaryString;
			this.itemCostLabel.Text = "<c=black>" + item.QuantityString;
			this.highlightFrontImage.color = Color.white;
			this.highlightDropshadowImage.color = Color.black;
		}
		/// <summary>
		/// Sets the visuals on this item to be dehighlighted.
		/// </summary>
		/// <param name="item"></param>
		protected internal override void Dehighlight(IMenuable item) {
			this.itemNameLabel.Text = "<c=white>" + item.PrimaryString;
			this.itemCostLabel.Text = "<c=white>" + item.QuantityString;
			this.highlightFrontImage.color = Color.clear;
			this.highlightDropshadowImage.color = Color.clear;
		}
		#endregion

		#region OVERRIDES - MENUITEM : EVENTS
		public override void OnSelect(BaseEventData eventData) {
			this.Highlight(item: this.CurrentShopItem);
		}
		public override void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Hover);
			this.Dehighlight(item: this.CurrentShopItem);
		}
		public override void OnSubmit(BaseEventData eventData) {
			
			// If this item is usable (read: is purchasable), branch on what needs to be done upon submission.
			if (this.IsUsable == true) {
				AudioController.instance?.PlaySFX(SFXType.Select);
				switch (this.CurrentShopItem.PrototypeShopItemType) {
					case PrototypeShopItemType.BattleBehaviorItem:
						GameController.Instance.Variables.AddItemToInventory(item: this.CurrentShopItem.BattleBehavior, quantity: 1);
						GameController.Instance.Variables.Money -= this.CurrentShopItem.ItemCost;
						PrototypeShopMenuController.Instance.MoneyCounter.UpdateCounter(gameVariables: GameController.Instance.Variables);
						break;
					default:
						throw new System.NotImplementedException("I HAVENT ACCOUNTED FOR OTHER SITUATONS");
				}
			} else {
				AudioController.instance?.PlaySFX(SFXType.Invalid);
				throw new NotImplementedException();
			}
			
		}
		public override void OnCancel(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Close);
			PrototypeShopMenuController.Instance.Close();
		}
		#endregion
	}
}