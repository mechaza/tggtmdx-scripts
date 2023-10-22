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
using Grawly.Shop.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;
using Grawly.Battle;

namespace Grawly.Shop.Prototype {
	
	/// <summary>
	/// An item to be available for purchase in the prototype shop.
	/// </summary>
	public class PrototypeShopItem : IMenuable, IProtoShopItem {

		#region FIELDS - STATE
		/// <summary>
		/// The prototype item template assiegned to this object.
		/// </summary>
		private PrototypeShopItemTemplate CurrentItemTemplate { get; }
		#endregion

		#region PROPERTIES - IPROTOSHOPITEM
		/// <summary>
		/// The name for this item.
		/// </summary>
		public string ItemName => this.CurrentItemTemplate.ItemName;
		/// <summary>
		/// The description for this item.
		/// </summary>
		public string ItemDescription => this.CurrentItemTemplate.ItemDescription;
		/// <summary>
		/// The cost for this item, to the player.
		/// </summary>
		public int ItemCost => this.CurrentItemTemplate.ItemCost;
		/// <summary>
		/// The kind of item this template is representing.
		/// </summary>
		public PrototypeShopItemType PrototypeShopItemType => this.CurrentItemTemplate.PrototypeShopItemType;
		/// <summary>
		/// The BattleBehavior to add to the player's inventory, if this template represents a BattleBehaviorItem.
		/// </summary>
		public BattleBehavior BattleBehavior => this.CurrentItemTemplate.BattleBehavior;
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Creates a new PrototypeShopItem from the template passed in.
		/// </summary>
		/// <param name="prototypeItemTemplate"></param>
		public PrototypeShopItem(PrototypeShopItemTemplate prototypeItemTemplate) {
			this.CurrentItemTemplate = prototypeItemTemplate;
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public string PrimaryString => this.ItemName;
		public string QuantityString => this.ItemCost.ToString();
		public string DescriptionString => this.DescriptionString;
		public Sprite Icon => DataController.GetDefaultElementalIcon(ElementType.Assist);
		#endregion

	}
}