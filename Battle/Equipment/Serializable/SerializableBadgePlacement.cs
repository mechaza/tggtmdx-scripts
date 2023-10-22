using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// A way to save a BadgePlacement out to disk.
	/// </summary>
	[System.Serializable]
	public class SerializableBadgePlacement {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The ID that identifies the badge being saved out.
		/// </summary>
		[OdinSerialize]
		public BadgeID BadgeID { get; set; }
		/// <summary>
		/// The BadgeTransform that contains the orientation for the badge.
		/// </summary>
		[OdinSerialize]
		public BadgeTransform BadgeTransform { get; set; }
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Constructs a SerializableBadgePlacement from the BadgePlacement provided.
		/// </summary>
		/// <param name="badgePlacement"></param>
		public SerializableBadgePlacement(BadgePlacement badgePlacement) {
			this.BadgeID = badgePlacement.Badge.BadgeID;
			this.BadgeTransform = badgePlacement.BadgeTransform;
		}
		#endregion
		
	}
}