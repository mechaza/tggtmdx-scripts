using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI.MenuLists;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment {
	
	/// <summary>
	/// Used to identify a specific weapon.
	/// </summary>
	[System.Serializable]
	public class WeaponID {
		
		#region FIELDS
		/// <summary>
		/// The ID number associated with this WeaponID.
		/// </summary>
		[OdinSerialize]
		public int IDNumber { get; set; }
		#endregion
		
		#region EQUALITY OVERRIDES
		public bool Equals(WeaponID other) {
			return IDNumber == other.IDNumber;
		}
		public override bool Equals(object obj) {
			return obj is WeaponID other && Equals(other);
		}
		public override int GetHashCode() {
			return IDNumber;
		}
		#endregion
		
	}
}