using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI.MenuLists;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Equipment {
	
	/// <summary>
	/// Contains all of the weapons currently available to the party.
	/// </summary>
	public class WeaponCollectionSet {
		
		#region PROPERTIES - STATE
		/// <summary>
		/// Does this set actually have weapons in it?
		/// </summary>
		public bool HasWeapons => this.AllWeapons.Count > 0;
		#endregion
		
		#region FIELDS - WEAPONS
		/// <summary>
		/// All of the weapons that belong inside this set.
		/// </summary>
		public List<Weapon> AllWeapons { get; private set; } = new List<Weapon>();
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Creates a blank collection set.
		/// </summary>
		public WeaponCollectionSet() {
			this.AllWeapons = new List<Weapon>();
		}
		/// <summary>
		/// Creates a collection set from the template specified.
		/// </summary>
		/// <param name="weaponCollectionSetTemplate">The WeaponCollectionSetTemplate used to make this set.</param>
		/// <param name="badgeCollectionSet">The badges that need to be appended to the weapons in the set.</param>
		public WeaponCollectionSet(WeaponCollectionSetTemplate weaponCollectionSetTemplate, BadgeCollectionSet badgeCollectionSet) {
			
			// Transform the weapon templates in the set template into usable weapons.
			this.AllWeapons = weaponCollectionSetTemplate.WeaponTemplates
				.Select(wt => new Weapon(weaponTemplate: wt, badgeCollectionSet: badgeCollectionSet))
				.ToList();
			
		}
		/// <summary>
		/// Creates a collection set from one that was previously saved out to disk.
		/// </summary>
		/// <param name="serializableWeaponCollectionSet">The serialized weapon data to construct this set from.</param>
		/// <param name="badgeCollectionSet">The badges that need to be appended to the weapons in the set.</param>
		public WeaponCollectionSet(SerializableWeaponCollectionSet serializableWeaponCollectionSet, BadgeCollectionSet badgeCollectionSet) {
			
			this.AllWeapons = serializableWeaponCollectionSet.allSerializableWeapons		// Go through the saved weapons...
				.Select(sw => new Weapon(sw, badgeCollectionSet))							// ...transform them into real weapons...
				.ToList();
			
		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets the weapon associated with the provided ID.
		/// </summary>
		/// <param name="weaponID">The ID identifying the requested weapon.</param>
		/// <returns>The weapon whose ID matches the one passed as a parameter.</returns>
		public Weapon GetWeapon(WeaponID weaponID) {
			return this.AllWeapons.First(b => b.WeaponID.Equals(weaponID));
		}
		/// <summary>
		/// Gets all of the available weapons that the specified player is allowed to use.
		/// </summary>
		/// <param name="playerIDType"></param>
		/// <returns></returns>
		public List<Weapon> GetWeaponsForPlayer(CharacterIDType playerIDType) {
			// Go through all the weapons where the specified player is able to use it.
			return this.AllWeapons
				.Where(w => w.AblePlayers.Contains(playerIDType))
				.ToList();
		}
		#endregion
		
	}
}