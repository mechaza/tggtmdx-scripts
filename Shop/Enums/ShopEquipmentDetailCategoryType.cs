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
using Grawly.UI.MenuLists;

namespace Grawly.Shop {
	
	/// <summary>
	/// Identifies the different kinds of equipment categories that
	/// will show up on the equipment summary in the shop screen.
	/// </summary>
	public enum ShopEquipmentDetailCategoryType {
		None		= 0,
		Weapon		= 1,
		Armor		= 2,
		Accessory	= 3,
	}
}