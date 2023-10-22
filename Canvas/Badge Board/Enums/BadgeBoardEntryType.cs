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
	/// The different ways that the badge board can be entered.
	/// Helpful when determining if all players should be selectable or if one in particular should be.
	/// </summary>
	public enum BadgeBoardEntryType {
		None			= 0,
		AllPlayers		= 1,	// All of the players should be available for weapon editing.
		SinglePlayer	= 2,	// Only one player is available for weapon editing. Usually called in battle.
	}
}