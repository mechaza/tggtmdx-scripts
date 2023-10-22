using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Grawly.Chat;
using DG.Tweening;
using Sirenix.OdinInspector;
using Grawly.Story;
using Grawly.Toggles;
using Grawly.Toggles.Audio;

namespace Grawly.Dungeon {
	
	/// <summary>
	/// Anything that can be used as a "spawn point" should have this.
	/// I just wanna use this to be safe really.
	/// </summary>
	public interface IDungeonSpawnPoint {

		#region PROPERTIES
		/// <summary>
		/// The position associated with whatever this interface is attached to.
		/// </summary>
		Vector3 SpawnPointPosition { get; }
		/// <summary>
		/// The rotation associated with whatever this interface is attached to.
		/// </summary>
		Quaternion SpawnPointRotation { get; }
		#endregion
		
	}
	
}