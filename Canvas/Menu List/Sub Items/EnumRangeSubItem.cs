using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.UI.SubItem {

	/// <summary>
	/// A subitem that allows display of a range of values with specific definitions. 
	/// </summary>
	public class EnumRangeSubItem : SubItem {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The kind of sub item this is.
		/// </summary>
		public override SubItemType SubItemType {
			get {
				return SubItemType.EnumRange;
			}
		}
		#endregion

		#region BUILDING
		protected internal override void Build(SubItemParams subItemParams) {
			throw new System.NotImplementedException();
		}
		#endregion

		#region VISUALS
		protected internal override void Highlight(SubItemParams subItemParams) {
			throw new System.NotImplementedException();
		}
		protected internal override void Dehighlight(SubItemParams subItemParams) {
			throw new System.NotImplementedException();
		}
		#endregion

	}


}