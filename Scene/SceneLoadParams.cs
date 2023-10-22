using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using Grawly.CodeBits;
using Sirenix.Serialization;
using Grawly.Calendar;
using Grawly.Dungeon;

namespace Grawly {

	/// <summary>
	/// A set of parameters that help determine how a scene should be loaded and what to do when it is.
	/// </summary>
	[System.Serializable]
	public class SceneLoadParams {

		#region FIELDS - SCENES
		/// <summary>
		/// The name of the scene to load up.
		/// </summary>
		[TabGroup("Scene Load Params", "Scenes"), Tooltip("The name of the scene to load up."), SerializeField]
		private string sceneName = "";
		/// <summary>
		/// The name of the scene to load up.
		/// </summary>
		public string SceneName {
			get {
				return this.sceneName;
			} set {
				// Only set the scene name if one does not exist.
				if (sceneName == "") {
					this.sceneName = value;
				} else {
					Debug.LogError("You're attempting to set a scene name on a set of params that already has one!");
				}
			}
		}
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should the AudioController continue to play music, even during the transition?
		/// </summary>
		[TabGroup("Scene Load Params", "Toggles"), Tooltip("Should the AudioController continue to play music, even during the transition?"), SerializeField]
		private bool dontStopMusic = false;
		/// <summary>
		/// Should the AudioController continue to play music, even during the transition?
		/// </summary>
		public bool DontStopMusic {
			get {
				return this.dontStopMusic;
			} set {
				this.dontStopMusic = value;
			}
		}
		/// <summary>
		/// The position where a player should spawn when a scene is loaded. -1 means the default position of the grandma model.
		/// </summary>
		[TabGroup("Scene Load Params", "Toggles"), Tooltip("The ID of the position where the player should be when they spawn. -1 does not affect anything.")]
		public DungeonSpawnType spawnPositionType = DungeonSpawnType.Default;
		#endregion
		
		#region FIELDS - TIME OF DAY
		/// <summary>
		/// The time of day to show as the start, if transitioning between times.
		/// </summary>
		public TimeOfDayType startTime = TimeOfDayType.Morning;
		/// <summary>
		/// The time of day to show as the end, if transitioning between times.
		/// </summary>
		public TimeOfDayType endTime = TimeOfDayType.Morning;
		#endregion

		#region FIELDS - ON SCENE LOAD
		/// <summary>
		/// The callback to run when the scene loads.
		/// </summary>
		public System.Action OnSceneLoaded = null;
		/// <summary>
		/// A list of Code Bits to run when the scene is finally loaded.
		/// </summary>
		[TabGroup("Scene Load Params", "On Scene Load"), Tooltip("A list of Code Bits to run when the scene is finally loaded."), SerializeField]
		private List<CodeBit> onSceneLoadCodeBits = new List<CodeBit>();
		/// <summary>
		/// A list of Code Bits to run when the scene is finally loaded.
		/// </summary>
		public List<CodeBit> OnSceneLoadCodeBits {
			get {
				return this.onSceneLoadCodeBits;
			}
		}
		#endregion

		#region PROPERTIES - FLAGS
		/// <summary>
		/// Should the screen that shows the transition between times of the day be used?
		/// </summary>
		public bool UseTimeTransition {
			get {
				// Theoretically this should actually be true if the start/end times are different.
				return !(this.startTime == this.endTime);
			}
		}
		#endregion
		
	}


}