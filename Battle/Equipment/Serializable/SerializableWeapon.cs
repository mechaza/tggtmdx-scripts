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
	/// The way in which a weapon should be saved out in memory.
	/// </summary>
	[System.Serializable]
	public class SerializableWeapon : SerializableEquipmentItem {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The ID that identifies this weapon.
		/// </summary>
		[OdinSerialize]
		public WeaponID WeaponID { get; set; }
		/// <summary>
		/// A list of badge placements that can be used to create the BadgeGrid for the weapon when loading.
		/// </summary>
		[OdinSerialize]
		public List<SerializableBadgePlacement> SerializableBadgePlacements { get; set; } = new List<SerializableBadgePlacement>();
		#endregion
		
		#region CONSTRUCTORS
		public SerializableWeapon(Weapon weapon) {
			// Save the weapon ID.
			this.WeaponID = weapon.WeaponID;
			// Transform the badge placements in the weapon's badge grid into ones that can be serialized.
			this.SerializableBadgePlacements = weapon.BadgeGrid.CurrentBadgePlacements
				.Select(bp => new SerializableBadgePlacement(bp))
				.ToList();
		}
		#endregion
		
	}
	
}