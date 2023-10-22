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
	/// The way in which a weapon collection set should be saved out at runtime.
	/// </summary>
	[System.Serializable]
	public class SerializableWeaponCollectionSet {
		
		#region FIELDS - STATE
		/// <summary>
		/// A list of serializable weapons that can be saved out to disk.
		/// </summary>
		[OdinSerialize]
		public List<SerializableWeapon> allSerializableWeapons = new List<SerializableWeapon>();
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// This constructor is usually used when creating a gamesave.
		/// </summary>
		public SerializableWeaponCollectionSet() {
			this.allSerializableWeapons = new List<SerializableWeapon>();
		}
		/// <summary>
		/// Create a new serializable weapon collection set from one that already exists.
		/// </summary>
		/// <param name="weaponCollectionSet"></param>
		public SerializableWeaponCollectionSet(WeaponCollectionSet weaponCollectionSet) {
			this.allSerializableWeapons = weaponCollectionSet.AllWeapons
				.Select(w => new SerializableWeapon(w))
				.ToList();
		}
		#endregion
		
	}
}