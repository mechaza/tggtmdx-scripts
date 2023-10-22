using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;

namespace Grawly.UI.SubItem {

	/// <summary>
	/// A very basic class that basically acts as a wrapper so I can pass things to sub items to build them.
	/// </summary>
	public class SubItemParams {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The type of sub item that should be used.
		/// </summary>
		public SubItemType SubItemType { get; private set; } = SubItemType.None;
		/// <summary>
		/// The currently selected option.
		/// </summary>
		public string CurrentOption { get; private set; }
		#endregion

		#region CONSTRUCTORS
		public SubItemParams(SubItemType subItemType, string currentOption) {
			this.SubItemType = subItemType;
			this.CurrentOption = currentOption;
		}
		#endregion

		#region STATIC FIELDS
		/// <summary>
		/// Empty parameters to use when sub items are unavailable.
		/// </summary>
		public static SubItemParams EmptyParams { get; private set; } = new SubItemParams(SubItemType.None, currentOption: "");
		#endregion

	}


}