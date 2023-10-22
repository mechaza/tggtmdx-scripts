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
using Grawly.Menus;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Shop.Prototype {
	
	/// <summary>
	/// A template that can be used to create an item in the prototype store.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Shop/Prototype/Prototype Item Template")]
	public class PrototypeShopItemTemplate : SerializedScriptableObject, IProtoShopItem {

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The name for this item.
		/// </summary>
		[OdinSerialize, TabGroup("Item", "Toggles"), Title("General")]
		public string ItemName { get; private set; } = "";
		/// <summary>
		/// The description for this item.
		/// </summary>
		[OdinSerialize, TabGroup("Item", "Toggles")]
		public string ItemDescription { get; private set; } = "";
		/// <summary>
		/// The cost for this item, to the player.
		/// </summary>
		[OdinSerialize, TabGroup("Item", "Toggles")]
		public int ItemCost { get; private set; } = 100;
		#endregion

		#region FIELDS - TOGGLES : ITEM
		/// <summary>
		/// The kind of item this template is representing.
		/// </summary>
		[OdinSerialize, TabGroup("Item", "Toggles"), Title("Item")]
		public PrototypeShopItemType PrototypeShopItemType { get; set; } = PrototypeShopItemType.None;
		/// <summary>
		/// The BattleBehavior to add to the player's inventory, if this template represents a BattleBehaviorItem.
		/// </summary>
		[OdinSerialize, TabGroup("Item", "Toggles"), ShowIf("IsBattleBehaviorItem")]
		public BattleBehavior BattleBehavior { get; set; }
		#endregion
		
		#region GENERATION
		/// <summary>
		/// Uses the data inside this template to generate an item at runtime.
		/// </summary>
		/// <returns></returns>
		public PrototypeShopItem GeneratePrototypeItem() {
			// Generate a new shop item using this specific template.
			return new PrototypeShopItem(prototypeItemTemplate: this);
		}
		#endregion

		#region ODIN HELPERS
		private bool IsBattleBehaviorItem() {
			return this.PrototypeShopItemType == PrototypeShopItemType.BattleBehaviorItem;
		}
		private bool IsEquipmentItem() {
			return this.PrototypeShopItemType == PrototypeShopItemType.EquipmentItem;
		}
		private bool IsSpecialItem() {
			return this.PrototypeShopItemType == PrototypeShopItemType.SpecialItem;
		}
		#endregion
		
	}
}