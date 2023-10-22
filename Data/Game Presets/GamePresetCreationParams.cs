using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grawly.Calendar;
using Grawly.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly {
	
	/// <summary>
	/// A container class to help with creating Game Presets.
	/// </summary>
	[System.Serializable]
	public class GamePresetCreationParams {
		// bool duplicateBattleBehaviors, bool createScene, bool addToPresetReference

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The name to use for this preset.
		/// </summary>
		public string gamePresetName = "";
		#endregion
		
		#region FIELDS - TOGGLES : COPIES
		/// <summary>
		/// Should BattleBehaviors be duplicated when 
		/// </summary>
		public bool createBattleBehaviors = true;
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// Generates a preset with these parameters.
		/// </summary>
		[Button]
		public void GeneratePreset() {
			Debug.Log("Generating preset: " + this.gamePresetName);
		}
		#endregion
		
	}
}