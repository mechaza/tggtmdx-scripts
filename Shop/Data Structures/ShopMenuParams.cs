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


namespace Grawly.Shop {
	
	/// <summary>
	/// A data structure to help with encapsulating the data that needs to be passed around when building the shop menu.
	/// </summary>
	public class ShopMenuParams : IStoreCatalog {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The store template that contains the data needed for building a shop.
		/// </summary>
		private ShopStoreTemplate ShopStoreTemplate { get; }
		/// <summary>
		/// The theme template that should be used for whatever shop is built with these params.
		/// </summary>
		public ShopThemeTemplate ShopThemeTemplate { get; }
		/// <summary>
		/// The GameVariables currently in use. I'm fucking lazy.
		/// </summary>
		public GameVariables CurrentVariables { get; }
		#endregion

		#region INTERFACE PROPERTIES - ISTORECATALOG
		/// <summary>
		/// The items that should be available in a store.
		/// </summary>
		public List<ShopItem> ShopItems {
			get {
				return this.ShopStoreTemplate.ShopItems;
			}
		}
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Creates the paaramters that should be used in displaying a shop.
		/// </summary>
		/// <param name="storeTemplate">The template containing logical aspects about the shop.</param>
		/// <param name="themeTemplate">The template containing how the shop should be stylized.</param>
		/// <param name="gameVariables">The GameVariables currently in use.</param>
		public ShopMenuParams(ShopStoreTemplate storeTemplate, ShopThemeTemplate themeTemplate, GameVariables gameVariables) {
			this.ShopStoreTemplate = storeTemplate;
			this.ShopThemeTemplate = themeTemplate;
			this.CurrentVariables = gameVariables;
		}
		#endregion
		
	}
}