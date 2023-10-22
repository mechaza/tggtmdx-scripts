using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Gauntlet {

	/// <summary>
	/// Basically, a way to mark the status of a gauntlet node.
	/// Also helps me with saving their status and decorating them.
	/// </summary>
	public enum GauntletNodeStatusType {
		New = 0,			// Node should be "new" by default, until the player scrolls over it, if they meet the conditions.
		Normal = 1,			// Normal is when the node is open and can be done.
		Completed = 2,		// Self explanatory.
		Locked = 3,			// Node can also be this by default if there are conditions which prevent them from engaging with it.
	}


}