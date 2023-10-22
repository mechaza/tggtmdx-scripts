using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Chat;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Grawly.Dungeon.Interactable  {
	
	/// <summary>
	/// The different ways in which a transition can occur when using a dungeon door.
	/// </summary>
	public enum DungeonDoorTransitionType {
		None			= 0,
		Fade			= 1,
		Tween			= 2,
	}
}