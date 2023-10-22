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
	/// The different types of items available for purchase in the prototype shop.
	/// </summary>
	public enum PrototypeShopItemType {
		
		None					= 0,
		BattleBehaviorItem		= 1,	// Something that gets placed in the player's inventory.
		EquipmentItem			= 2,	// Somethin the player is able to equip on a party member.
		SpecialItem				= 3,	// An item that can unlock special functionality.
		
	}
}