using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// A way of identifying a badge.
	/// Used in connecting a SerializableBadge to its associated BadgeTemplate
	/// when constructing it from disk.
	/// </summary>
	[System.Serializable]
	public class BadgeID {
		
		#region FIELDS
		/// <summary>
		/// The ID number associated with this BadgeID.
		/// </summary>
		[OdinSerialize]
		public int IDNumber { get; set; }
		#endregion
		
		#region EQUALITY OVERRIDES
		public bool Equals(BadgeID other) {
			return IDNumber == other.IDNumber;
		}
		public override bool Equals(object obj) {
			return obj is BadgeID other && Equals(other);
		}
		public override int GetHashCode() {
			return IDNumber;
		}
		#endregion
		
	}
}