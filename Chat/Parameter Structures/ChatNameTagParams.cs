using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// A way to encapsulate parameters when making calls to the chat box.
	/// </summary>
	public class ChatNameTagParams {

		#region FIELDS
		/// <summary>
		/// The color that the name tag text should change to.
		/// </summary>
		public GrawlyColorTypes? nameTagLabelColorType;
		/// <summary>
		/// The color the name tag backing should change to.
		/// </summary>
		public GrawlyColorTypes? nameTagBackingColorType;
		#endregion

		#region PROPERTIES
		/// <summary>
		/// The tag to prepend the name label with.
		/// </summary>
		public string NameLabelColorTag {
			get {
				if (this.nameTagLabelColorType != null) {
					return "<c=" + GrawlyColors.ToHexFromRGB(c: GrawlyColors.colorDict[this.nameTagLabelColorType.Value]) + ">";
				} else {
					return "";
				}
				
			}
		}
		#endregion

	}


}