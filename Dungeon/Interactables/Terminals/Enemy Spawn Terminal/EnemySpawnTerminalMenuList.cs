using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Grawly.Dungeon;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;

namespace Grawly.UI {

	/// <summary>
	/// A list to pick out a battle template from.
	/// </summary>
	public class EnemySpawnTerminalMenuList : MenuList {

		public static EnemySpawnTerminalMenuList instance;

		#region FIELDS - STATE
		/// <summary>
		/// A reference to the terminal being called from. Needed so I can tell it to close out.
		/// </summary>
		public EnemySpawnTerminal terminal;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region EVENTS
		/// <summary>
		/// Gets called from the ESTMLI when a template has been picked and will subsequently run the battle.
		/// </summary>
		/// <param name="battleTemplate"></param>
		public void PickedBattleTemplate(BattleTemplate battleTemplate) {
			this.terminal.PickedBattleTemplate(battleTemplate: battleTemplate);
		}
		/// <summary>
		/// Gets called from the menu list item when the back button is hit.
		/// </summary>
		public void Close() {
			this.terminal.CloseTerminal();
		}
		#endregion

	}


}