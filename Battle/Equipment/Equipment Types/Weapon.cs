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
	/// A weapon that is used by a combatant in battle.
	/// </summary>
	public class Weapon : EquipmentItem<WeaponTemplate, SerializableWeapon>, IWeaponIDHaver, IBadgeGridHaver, IMenuable {

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The players who are allowed to use this weapon.
		/// </summary>
		public List<CharacterIDType> AblePlayers => this.ItemTemplate.AblePlayers;
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IWEAPONIDHAVER
		/// <summary>
		/// The ID associated with this weapon.
		/// </summary>
		public WeaponID WeaponID => this.ItemTemplate.WeaponID;
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IBADGEGRIDHAVER
		/// <summary>
		/// The BadgeGrid that this weapon should be using.
		/// </summary>
		public BadgeGrid BadgeGrid { get; set; }
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Creates a weapon using the template passed in.
		/// </summary>
		/// <param name="weaponTemplate">The weapon template to build this weapon with.</param>
		/// <param name="badgeCollectionSet">The badge collection set that should be used to associate badges with.</param>
		public Weapon(WeaponTemplate weaponTemplate, BadgeCollectionSet badgeCollectionSet) : base(weaponTemplate) {
			// Create the badge grid from the grid template contained within the weapon template.
			this.BadgeGrid = new BadgeGrid(
				badgeGridTemplate: weaponTemplate.BadgeGridTemplate, 
				badgeCollectionSet: badgeCollectionSet);
		}
		/// <summary>
		/// Creates a weapon using the data that was previously saved to disk.
		/// </summary>
		/// <param name="serializableWeapon">The data of a weapon that was previously saved out to memory.</param>
		/// <param name="badgeCollectionSet">The badge collection set that should be used to associate badges with.</param>
		public Weapon(SerializableWeapon serializableWeapon, BadgeCollectionSet badgeCollectionSet) : base(DataController.GetWeaponTemplate(serializableWeapon.WeaponID)) {
			// Create a new BadgeGrid with the info provided.
			// The difference between this one and the constructor above is that I'm providing the BadgePlacements
			// instead of relying on the grid to make it on its own.
			this.BadgeGrid = new BadgeGrid(
				badgeGridTemplate: ((WeaponTemplate) this.ItemTemplate).BadgeGridTemplate,
				badgePlacements: serializableWeapon.SerializableBadgePlacements.Select(sbp => new BadgePlacement(sbp, badgeCollectionSet)).ToList());
		}
		#endregion

		#region MODIFIERS
		/// <summary>
		/// Gets all of the modifiers associated with this weapon.
		/// This includes the badge grid and any other associated behaviors I may want to include.
		/// </summary>
		/// <typeparam name="T">The type of modifiers to get.</typeparam>
		/// <returns></returns>
		public List<T> GetModifiers<T>() {
			// For right now, just grab the grid's modifiers.
			return this.BadgeGrid.GetModifiers<T>();
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public string PrimaryString => this.ItemTemplate.ItemName;
		public string QuantityString => throw new System.Exception("This is never used!");
		public string DescriptionString => this.ItemTemplate.ItemDescription;
		public Sprite Icon => DataController.GetDefaultElementalIcon(elementType: ElementType.Assist);
		#endregion

	}
	
	
}