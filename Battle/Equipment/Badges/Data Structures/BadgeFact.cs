using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.UI.MenuLists;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// A "fact" about a badge which can be used to build the BadgeInfoDetails in the BadgeInfo.
	/// </summary>
	public class BadgeFact {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The text that this fact should display.
		/// </summary>
		public string FactText { get; set; } = "";
		#endregion

	}
}