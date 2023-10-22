using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.UI.MenuLists;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Shop {
	
	/// <summary>
	/// The primary information for something being listed in a store at runtime.
	/// </summary>
	public class ShopItem : IMenuable, IShopItem {

		#region FIELDS - TEMPLATES
		/// <summary>
		/// The template that was used to create this shop item.
		/// </summary>
		private ShopItemTemplate Template { get; }
		#endregion

		#region PROPERTIES - ISHOPITEM
		/// <summary>
		/// The name that identifies this shop item.
		/// </summary>
		public string ItemName => this.Template.ItemName;
		/// <summary>
		/// The description detailing this item.
		/// </summary>
		public string ItemDescription => this.Template.ItemDescription;
		/// <summary>
		/// Whether or not this item has a price.
		/// </summary>
		public bool HasPrice => this.Template.HasPrice;
		/// <summary>
		/// The prices associated with this item, if it has one.
		/// </summary>
		public int ItemPrice => this.Template.ItemPrice;
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Creates a ShopItem from the provided template.
		/// </summary>
		/// <param name="itemTemplate">The template to use in creating this shop item.</param>
		public ShopItem(ShopItemTemplate itemTemplate) {
			this.Template = itemTemplate;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public string PrimaryString => this.Template.ItemName;
		public string QuantityString => throw new NotImplementedException("ADD THIS");
		public string DescriptionString => this.Template.ItemDescription;
		public Sprite Icon => throw new NotImplementedException("ADD THIS");
		#endregion
		
		
	}
	
}