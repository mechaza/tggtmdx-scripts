using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// The different kinds of slots that are avaialble to be filled in a Badge Grid.
	/// </summary>
	public enum BadgeGridSlotType {
		None		= 0,	// Effectively, unusable.
		Standard	= 1,	// A normal slot.
		Boost		= 2,	// A slot that can apply a buff to anything put into it.
		Nerf		= 3,	// A slot that nerfs the effect of whats slotted in.
		Boundary	= 4,	// The boundary of the grid. Similar to None, but I feel like I'm gonna need this type.
	}
	
}