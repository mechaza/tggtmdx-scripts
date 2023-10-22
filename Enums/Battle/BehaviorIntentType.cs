using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Grawly.Battle {

	/// <summary>
	/// A very basic way of collapsing the intention of a move. 
	/// </summary>
	[Flags]
	public enum IntentType {
		None = 0,
		Malicious = 1,				// E.x., attacking an enemy.
		Assistive = 2,				// E.x, healing an ally.
		SelfPreservation = 4,		// E.x., "I'm taking care of myself.
	}


}