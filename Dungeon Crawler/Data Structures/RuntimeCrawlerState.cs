using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// A simple data structure to help keep track of a crawler dungeon's state without explicitly saving it out to disk.
	/// </summary>
	public class RuntimeCrawlerState {

		#region FIELDS - STATE : FLOORS
		/// <summary>
		/// The number of the current floor.
		/// </summary>
		public int CurrentFloorNumber { get; set; } = 0;
		#endregion

		#region FIELDS - STATE : MOVEMENT AND ENCOUNTERS
		/// <summary>
		/// The number of steps taken for this danger level.
		/// </summary>
		public int CurrentDangerLevelStepCount { get; set; } = 0;
		/// <summary>
		/// The current number of steps that has to be reached for the danger level to roll over.
		/// </summary>
		public int CurrentDangerLevelStepTrigger { get; set; } = 0;
		/// <summary>
		/// The current danger level.
		/// </summary>
		public DangerLevelType CurrentDangerLevel { get; set; } = DangerLevelType.None;
		#endregion
		
	}
}