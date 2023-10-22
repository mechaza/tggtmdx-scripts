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
	/// The new DungeonController. Keeps track of certain kinds of state.
	/// </summary>
	public class DungeonControllerDX : MonoBehaviour {

		public static DungeonControllerDX Instance { get; private set; }

		/*#region FIELDS - STATE
		/// <summary>
		/// The amount of time since a battle was last encountered.
		/// Helps to make sure I don't spawn too many battles at once.
		/// </summary>
		public float TimeSinceLastBattle { get; set; } = 0f;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Update() {
			
			if (DungeonPlayer.Instance.CurrentState == DungeonPlayerStateType.Free) {
				// If the dungeon player is free, increment the time since the last battle.
				this.TimeSinceLastBattle += Time.deltaTime;
			} else if (BattleController.Instance.IsBattling == true) {
				// If battling
				this.TimeSinceLastBattle = 0f;
			}
		}
		#endregion

		#region STATE MANAGEMENT
		/// <summary>
		/// Completely and totally resets the state of the DungeonController.
		/// </summary>
		public void ResetState() {
			this.TimeSinceLastBattle = 0f;
		}
		#endregion*/
		
	}
}