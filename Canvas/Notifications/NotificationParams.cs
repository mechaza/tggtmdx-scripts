using System.Collections;
using System.Collections.Generic;
using Grawly.Battle.Intros;
using Grawly.Battle.Outros;
using UnityEngine;

namespace Grawly.UI {
	
	/// <summary>
	/// The data structure that contains parameters for the notification.
	/// </summary>
	public struct NotificationParams {

		#region FIELDS - TOGGLES : FLAGS
		/// <summary>
		/// Is this notification persistent?
		/// </summary>
		public bool isPersistent;
		#endregion
		
		#region FIELDS - TOGGLES : METADATA
		/// <summary>
		/// The text to display for this notification.
		/// </summary>
		public string primaryText;
		/// <summary>
		/// Secondary text to use if needed.
		/// </summary>
		public string secondaryText;
		/// <summary>
		/// The sprite to use for this notification's icon.
		/// </summary>
		public Sprite icon;
		/// <summary>
		/// The color associated with this notification.
		/// </summary>
		public Color color;
		#endregion

		#region PROPERTIES
		/// <summary>
		/// Do these params have secondary text that can be read out?
		/// </summary>
		public bool HasSecondaryText {
			get {
				return this.secondaryText != "";
			}
		}

		#endregion
		
	}

}