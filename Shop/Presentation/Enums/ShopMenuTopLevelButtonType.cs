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
	/// The different kinds of top level buttons are visible in the shop menu's top level selections.
	/// </summary>
	public enum ShopMenuTopLevelButtonType {
		None		= 0,
		Weapons		= 1,
		Armor		= 2,
		Accessories	= 3,
		Items		= 4,
		Talk		= 5,
		Leave		= 6,
	}
}