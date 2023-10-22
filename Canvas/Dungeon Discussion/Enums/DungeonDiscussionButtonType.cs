using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Grawly.Gauntlet;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using Grawly.UI.Legacy;
using Grawly.Chat;
using UnityEngine.Events;

namespace Grawly.UI {
	
	/// <summary>
	/// The different kinds of button types to pick from on the dungeon discussion screen.
	/// </summary>
	public enum DungeonDiscussionButtonType {
		None			= 0,
		EnterDungeon	= 1,
		Talk			= 2,
		Leave			= 3,
	}
}