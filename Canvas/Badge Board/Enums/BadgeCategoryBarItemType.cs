using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// The different types of items that will be in the category bar.
	/// Honestly even though there's only two this makes me feel safer.
	/// </summary>
	public enum BadgeCategoryBarItemType {
		All				= 0,	// All badges will be displayed.
		SingleElement	= 1,	// Only those of a single element will be displayed.
	}
	
}