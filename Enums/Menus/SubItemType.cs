using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.UI.SubItem {

	/// <summary>
	/// Defines the different kinds of possible ways there can be a sub item installed in a menu list item.
	/// </summary>
	public enum SubItemType {
		None = 0,
		SimpleToggle = 1,
		ValueRange = 2,
		EnumRange = 3,
	}

}