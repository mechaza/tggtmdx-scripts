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
using Grawly.Battle;

namespace Grawly.Shop.Prototype {
	
	/// <summary>
	/// Defines the data required for a shop item in the prototype shop.
	/// </summary>
	public interface IProtoShopItem {
		
		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The name for this item.
		/// </summary>
		public string ItemName { get; } 
		/// <summary>
		/// The description for this item.
		/// </summary>
		public string ItemDescription { get; } 
		/// <summary>
		/// The cost for this item, to the player.
		/// </summary>
		public int ItemCost { get; } 
		#endregion

		#region FIELDS - TOGGLES : ITEM
		/// <summary>
		/// The kind of item this template is representing.
		/// </summary>
		public PrototypeShopItemType PrototypeShopItemType { get; } 
		/// <summary>
		/// The BattleBehavior to add to the player's inventory, if this template represents a BattleBehaviorItem.
		/// </summary>
		public BattleBehavior BattleBehavior { get; }
		#endregion
		
	}
}