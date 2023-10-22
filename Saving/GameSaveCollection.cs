using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.Toggles;
using System.Linq;

namespace Grawly {

	/// <summary>
	/// Stores the game saves that are written out, plus additional config information that should be stored across all files.
	/// </summary>
	[System.Serializable]
	public class GameSaveCollection {

		#region FIELDS - SAVES
		/// <summary>
		/// The save files associated with this save collection.
		/// </summary>
		[SerializeField]
		private List<GameSave> gameSaves = new List<GameSave>(capacity: 16);
		/// <summary>
		/// The save slot that was last used. Good for just if the player wants to hit "continue" and not pick anything in particular.
		/// </summary>
		[SerializeField]
		private int lastSaveSlot = 0;
		#endregion

		#region FIELDS - GAMETOGGLES
		/// <summary>
		/// The GameToggleSet that should be used for general configuration.
		/// I.e., not for anything specific to any save.
		/// </summary>
		[OdinSerialize]
		public GameToggleSetDX gameToggleSet;
		#endregion

		#region PROPERTIES - SAVES
		/// <summary>
		/// The save slot that was last used. Good for just if the player wants to hit "continue" and not pick anything in particular.
		/// </summary>
		public int LastSaveSlot {
			get {
				return this.lastSaveSlot;
			}
		}
		/// <summary>
		/// The last game save to be used.
		/// </summary>
		public GameSave LastSave {
			get {
				return this.GetLastSave();
			}
		}
		/// <summary>
		/// Does this save collection have any data?
		/// Helpful for enabling the continue button on the initialization scene or not.
		/// </summary>
		public bool HasAnySaveData {
			get {
				// If there are ANY saves that are not a new game, return true.
				return this.gameSaves.Count(gs => gs.newGame == false) > 0;
			}
		}
		#endregion

		#region CONSTRUCTOR
		/// <summary>
		/// A blank constructor that just adds default elements.
		/// </summary>
		public GameSaveCollection() {
			// :(
			for (int i = 0; i < this.gameSaves.Capacity; i++) {
				this.gameSaves.Add(new GameSave());
			}
			// Generate a new set as well.
			this.gameToggleSet = DataController.GetDefaultGameTogglesTemplate().GenerateSet();

		}
		#endregion

		#region SAVING
		/// <summary>
		/// Saves the user configuration to the save collection.
		/// </summary>
		/// <param name="gameToggleSet">The toggles that say how the game should be configured.</param>
		public void SaveConfig(GameToggleSetDX gameToggleSet) {
			this.gameToggleSet = gameToggleSet;
		}
		/// <summary>
		/// Saves the specified GameVariables in the slot provided.
		/// </summary>
		/// <param name="gameVariables">The variables to save.</param>
		/// <param name="slot">The slot to save it in.</param>
		public void Save(GameVariables gameVariables, int slot) {
			// Save the variables in the specified slot.
			this.gameSaves[slot] = new GameSave(vars: gameVariables);
			// Remember the slot it was saved in. This is needed for if the file is ever re-opened and someone wants to pick "continue."
			this.lastSaveSlot = slot;
		}
		#endregion

		#region LOADING
		/// <summary>
		/// Returns all of the game saves.
		/// </summary>
		/// <returns></returns>
		public List<GameSave> GetAllSaves() {
			return this.gameSaves;
		}
		/// <summary>
		/// Returns the last save that was used.
		/// This is needed for if the file is ever re-opened and someone wants to pick "continue."
		/// </summary>
		/// <returns></returns>
		public GameSave GetLastSave() {
			try {
				return this.gameSaves[this.lastSaveSlot];
			} catch (System.Exception e) {
				Debug.LogError("Couldn't get last save slot! Reason: " + e.Message + "\nReturning null.");
				return null;
			}
		}
		#endregion

		#region SAVE MANIPULATION
		/// <summary>
		/// Increments the number of times the GameSave in the specified slot has been loaded.
		/// </summary>
		/// <param name="slot">The slot of the GameSave to increment the load count for.</param>
		/// <returns>The GameSave that wound up being incremented.</returns>
		public GameSave IncrementGameSaveLoadCount(int slot) {
			GameSave gameSave = this.gameSaves[slot];
			gameSave.loadCount += 1;
			this.gameSaves[slot] = gameSave;
			return gameSave;
		}
		#endregion
		
		#region CHECKS
		/// <summary>
		/// Checks if there is actual save data at the given slot.
		/// </summary>
		/// <param name="slot"></param>
		/// <returns></returns>
		public bool HasSaveData(int slot) {
			// If the save at the given slot is a new game, say as much.
			return this.GetAllSaves()[slot].newGame ? false : true;
		}
		#endregion
		
	}
}