using System.Collections;
using System.Collections.Generic;
using Grawly.Shop.Behaviors;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Shop {
	
	/// <summary>
	/// Contains the information needed for displaying an item in the shop as well as what it actually contains.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Shop/Shop Item Template")]
	public class ShopItemTemplate : SerializedScriptableObject, IShopItem {

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The title that this item should use in stores.
		/// </summary>
		[OdinSerialize, Title("General")]
		public string ItemName { get; private set; } = "";
		/// <summary>
		/// The description for this item.
		/// </summary>
		[OdinSerialize]
		public string ItemDescription { get; } = "";
		#endregion

		#region FIELDS - TOGGLES : COST
		/// <summary>
		/// Does this item have a price?
		/// </summary>
		[OdinSerialize, Title("Cost")]
		public bool HasPrice { get; private set; } = true;
		/// <summary>
		/// The price for this item, if it has one.
		/// </summary>
		[OdinSerialize, ShowIf("HasPrice")]
		public int ItemPrice { get; private set; } = 100;
		#endregion
		
		#region FIELDS - BEHAVIORS
		/// <summary>
		/// The behavior that drives how this item should behave in a shop.
		/// </summary>
		[OdinSerialize, Title("Behaviors")]
		public ShopItemBehavior ItemBehavior { get; private set; }
		#endregion
		
	}
}