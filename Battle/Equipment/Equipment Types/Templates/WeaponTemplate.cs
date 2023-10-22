using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using Grawly.Battle.Equipment.Badges;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment {
	
	/// <summary>
	/// A template that defines the characteristics of a weapon.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Battle/Equipment/Weapon Template")]
	public class WeaponTemplate : EquipmentItemTemplate, IWeaponIDHaver {

		#region FIELDS - GENERAL
		/// <summary>
		/// The raw ID number for this weapon template.
		/// </summary>
		[SerializeField]
		private int RawIDNumber = 0;
		/// <summary>
		/// A list of character IDs who represent whos actually able to use this weapon.
		/// </summary>
		[OdinSerialize]
		public List<CharacterIDType> AblePlayers { get; private set; } = new List<CharacterIDType>();
		#endregion

		#region INTERFACE IMPLEMENTATION - IWEAPONIDHAVER
		/// <summary>
		/// The ID associated with this weapon.
		/// </summary>
		public WeaponID WeaponID {
			get {
				return new WeaponID() {
					IDNumber = this.RawIDNumber
				};
			}
		}
		#endregion
		
		#region FIELDS - BADGES
		/// <summary>
		/// The default badge grid to use for this weapon.
		/// </summary>
		[OdinSerialize, Title("Badges")]
		public BadgeGridTemplate BadgeGridTemplate { get; private set; }
		#endregion
		
		#region EDITOR HELPERS
		/// <summary>
		/// Sets the raw ID.
		/// ONLY USE THIS IN EDITOR PROGRAMMING.
		/// </summary>
		/// <param name="rawIDNumber"></param>
		public void SetRawID(int rawIDNumber) {
			if (Application.isEditor == false) {
				throw new System.Exception("This should not be called anywhere other than the editor!");
			}
			this.RawIDNumber = rawIDNumber;
		}
		#endregion
		
	}
	
}