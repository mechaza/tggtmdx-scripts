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
	/// The different kinds of actions that can be performed on a weapon action item.
	/// </summary>
	public enum WeaponActionItemType {
		
		None		= 0,
		Edit		= 1,		// Edit the grid attached to this weapon.
		Clear		= 2,		// Remove all badges from this weapon's grid.
		Check		= 3,		// Check the badges attached to this weapon's grid.
		Equip		= 4,		// Equips the selected weapon onto the player.
		
	}
}