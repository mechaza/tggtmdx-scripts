using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// The different states a grid slot can be in during runtime.
	/// </summary>
	public enum BadgeGridSlotAvailabilityType {
		Available	= 0,		// Available to slip a badge into
		Occupied	= 1,		// Another badge is currently using this slot.
		Blocked		= 2,		// This slot is never going to be available.
	}
}