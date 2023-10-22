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

namespace Grawly.Shop.UI {
	
	/// <summary>
	/// An item that is available for purchase in the shop menu's inventory.
	/// </summary>
	public class ShopMenuInventoryListItem : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// The shop item currently assigned to this MenuItem.
		/// </summary>
		public ShopItem CurrentShopItem { get; private set; }
		#endregion
		
		#region PROPERTIES - STATE
		/// <summary>
		/// Is this item usable?
		/// </summary>
		protected override bool IsUsable { get; }
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the objects as children.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private GameObject allObjects;
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
		/// <summary>
		/// The image showing the item icon's front.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private Image itemIconFrontImage;
		/// <summary>
		/// The image showing tie item icon's dropshadow.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private Image itemIconDropshadowImage;
		/// <summary>
		/// The primary label for this inventory item.
		/// Usually used for the item's name.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private SuperTextMesh itemPrimaryLabel;
		/// <summary>
		/// The label for the inventory item's quantity/price.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private SuperTextMesh itemQuantityLabel;
		#endregion
		
		
		#region BUILDING
		/// <summary>
		/// Builds this inventory item.
		/// </summary>
		/// <param name="item"></param>
		public override void BuildMenuItem(IMenuable item) {
			
			// Save the ShopItem this MenuItem is representing.
			this.CurrentShopItem = (ShopItem) item;
			
			throw new NotImplementedException();
			
		}
		#endregion

		#region HIGHLIGHTING
		protected internal override void Highlight(IMenuable item) {
			throw new NotImplementedException();
		}
		protected internal override void Dehighlight(IMenuable item) {
			throw new NotImplementedException();
		}
		#endregion

		#region EVENT SYSTEM
		public override void OnSelect(BaseEventData eventData) {
			throw new NotImplementedException();
		}
		public override void OnDeselect(BaseEventData eventData) {
			throw new NotImplementedException();
		}
		public override void OnSubmit(BaseEventData eventData) {
			throw new NotImplementedException();
		}
		public override void OnCancel(BaseEventData eventData) {
			throw new NotImplementedException();
		}
		#endregion
		
		
	}
}