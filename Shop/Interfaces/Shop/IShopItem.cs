using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.UI.MenuLists;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Shop {
	
	/// <summary>
	/// Defines the characteristics of an item that will be displayed in a shop.
	/// </summary>
	public interface IShopItem {

		#region PROPERTIES - GENERAL
		/// <summary>
		/// The name that identifies this shop item.
		/// </summary>
		public string ItemName { get; }
		/// <summary>
		/// The description that details this shop item.
		/// </summary>
		public string ItemDescription { get; }
		#endregion

		#region PROPERTIES - COST
		/// <summary>
		/// Does this item have a price associated with it?
		/// </summary>
		public bool HasPrice { get; }
		/// <summary>
		/// The price associated with this item, if it has one.
		/// </summary>
		public int ItemPrice { get; }
		#endregion
		
	}
}