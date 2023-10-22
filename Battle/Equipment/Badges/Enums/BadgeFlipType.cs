using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// The different ways a badge can be flipped.
	/// </summary>
	public enum BadgeFlipType {
		Normal			= 0,		// No transformations applied.
		HorizontalFlip	= 1,		// Flipped horizontally.
		VerticalFlip	= 2,		// Flipped veritcally.
		BothFlip		= 3,		// Flipped both horizontally and vertically.
	}
}