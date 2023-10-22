using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// The different menus that can be transitioned to on the menu slider.
	/// </summary>
	public enum BadgeBoardSliderMenuType {
		
		None			= 0,
		WeaponSelect	= 1,
		WeaponAction	= 2,
		BadgeSelect		= 3,
		PlayerSelect		= 4,	// This actually has to come before the weapon select.
		
	}
}