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
	/// Contains a bunch of weapon templates that can be made into a weapon collection set at runtime.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Weapons/Weapon Collection Set Template")]
	public class WeaponCollectionSetTemplate : SerializedScriptableObject {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The weapon templates that should be used for creating the set spawned from this template.
		/// </summary>
		[OdinSerialize]
		public List<WeaponTemplate> WeaponTemplates { get; private set; } = new List<WeaponTemplate>();
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// Go through each of the templates and set their IDs appropriately.
		/// </summary>
		[Button]
		private void RefreshWeaponIDs() {
			for (int i = 0; i < this.WeaponTemplates.Count; i++) {
				this.WeaponTemplates[i].SetRawID(rawIDNumber: i);
			}
		}
		#endregion
		
	}
}