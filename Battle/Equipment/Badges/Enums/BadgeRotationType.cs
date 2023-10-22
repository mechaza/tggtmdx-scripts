using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// The different rotations that can be applied to a badge.
	/// </summary>
	public enum BadgeRotationType {
		Twelve		= 0,	// Upright, basically.
		Three		= 1,	// Rotated clockwise by 90 degrees.
		Six			= 2,	// Rotated clockwise by 180 degrees.
		Nine		= 3,	// Rotated clockwise by 270 degrees.
	}
}