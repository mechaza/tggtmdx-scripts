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
	/// A way to identify badge-related classes that should be able to provide their ID.
	/// </summary>
	public interface IWeaponIDHaver {
		/// <summary>
		/// The ID that should be generated from a class that implements this interface.
		/// </summary>
		WeaponID WeaponID { get; }
	}
}