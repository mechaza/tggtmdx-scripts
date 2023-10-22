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
using Grawly.Battle;
using Grawly.Shop.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Shop {
	
	/// <summary>
	/// The menu list item that should be part of the menu list in the shop where a player can pick a... player.
	/// </summary>
	public class ShopPlayerMenuListItem : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// The current ShopPlayer this list item represents.
		/// </summary>
		public ShopPlayer CurrentShopPlayer { get; private set; }
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects contained inside of this list item.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The label displaying the player's name.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private SuperTextMesh playerNameLabel;
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
				throw new NotImplementedException("Maybe make a way to infer whether theres a condition that renders a player not selectable?");
			}
		}
		#endregion
		
		#region OVERRIDES - MENUITEM : BUILDING
		/// <summary>
		/// Builds the MenuItem with the provided IMenuable.
		/// </summary>
		/// <param name="item"></param>
		public override void BuildMenuItem(IMenuable item) {
			
			// Save the shop player.
			this.CurrentShopPlayer = (ShopPlayer) item;
			
			// Dehighlight the list item.
			this.Dehighlight(item: item);
			
		}
		/// <summary>
		/// Sets the visuals on this item to be highlighted.
		/// </summary>
		/// <param name="item"></param>
		protected internal override void Highlight(IMenuable item) {
			Debug.LogWarning("FIX THIS TO NOT USE PROTOTYPE VALUES");
			this.playerNameLabel.Text = ShopMenuController.Instance.CurrentShopParams.ShopThemeTemplate.PrototypeLabelPrefix + item.PrimaryString;
			this.highlightFrontImage.color = ShopMenuController.Instance.CurrentShopParams.ShopThemeTemplate.PrototypeColor1;
			this.highlightDropshadowImage.color = ShopMenuController.Instance.CurrentShopParams.ShopThemeTemplate.PrototypeColor2;
		}
		/// <summary>
		/// Sets the visuals on this item to be dehighlighted.
		/// </summary>
		/// <param name="item"></param>
		protected internal override void Dehighlight(IMenuable item) {
			
			Debug.LogWarning("FIX THIS TO NOT USE PROTOTYPE VALUES");
			this.playerNameLabel.Text = ShopMenuController.Instance.CurrentShopParams.ShopThemeTemplate.PrototypeLabelPrefix + item.PrimaryString;
			this.highlightFrontImage.color = Color.clear;
			this.highlightDropshadowImage.color = Color.clear;
			
		}
		#endregion
		
		#region OVERRIDES - MENUITEM : EVENTS
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