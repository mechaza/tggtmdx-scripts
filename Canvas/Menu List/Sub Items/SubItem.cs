using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.UI.SubItem {

	/// <summary>
	/// A sub item that can be part of a menu list item.
	/// Mostly going to work with the assumption it will take inputs in left/right fashion.
	/// </summary>
	public abstract class SubItem : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The type of sub item this is.
		/// Handy for when I need to scan a list of different items in a sub item container.
		/// </summary>
		public abstract SubItemType SubItemType { get; }
		#endregion

		#region BUILDING
		/// <summary>
		/// Builds the sub item.
		/// </summary>
		protected internal abstract void Build(SubItemParams subItemParams);
		#endregion

		#region VISUALS
		/// <summary>
		/// Puts this sub item in the highlighted state, when the regular item is highlighted.
		/// </summary>
		/// <param name="subItemParams">The params used to build this sub item.</param>
		protected internal abstract void Highlight(SubItemParams subItemParams);
		/// <summary>
		/// Puts this sub item in the dehighlighted state, when the regular item is dehighlighted.
		/// </summary>
		/// <param name="subItemParams">The params used to build this sub item.</param>
		protected internal abstract void Dehighlight(SubItemParams subItemParams);
		#endregion

	}

}