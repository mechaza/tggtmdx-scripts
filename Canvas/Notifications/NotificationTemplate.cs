using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.UI {
	
	/// <summary>
	/// A kind of debug way for me to make notifications.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Debug/Notification")]
	public class NotificationTemplate : SerializedScriptableObject {
		
		#region FIELDS - TOGGLES : FLAGS
		/// <summary>
		/// Is this notification persistent?
		/// </summary>
		public bool IsPersistent = true;
		#endregion
		
		#region FIELDS - TOGGLES : METADATA
		/// <summary>
		/// The text to display for this notification.
		/// </summary>
		public string PrimaryText = "";
		/// <summary>
		/// Additional text to use if needed.
		/// </summary>
		public string SecondaryText = "";
		/// <summary>
		/// The sprite to use for this notification's icon.
		/// </summary>
		public Sprite Icon;
		/// <summary>
		/// The color type for this notification.
		/// </summary>
		public GrawlyColorTypes colorType = GrawlyColorTypes.White;
		#endregion

		#region PROPERTIES
		/// <summary>
		/// The color associated with this notification.
		/// </summary>
		public Color Color {
			get {
				return GrawlyColors.colorDict[this.colorType];
			}
		}
		#endregion

		#region GENERATORS
		/// <summary>
		/// Creates a notification params from this template.
		/// </summary>
		/// <returns></returns>
		public NotificationParams ToParams() {
			return new NotificationParams() {
				primaryText =  this.PrimaryText,
				secondaryText = this.SecondaryText,
				color =  this.Color,
				icon = this.Icon,
				isPersistent = this.IsPersistent
			};
		}
		#endregion
		
	}

	
}