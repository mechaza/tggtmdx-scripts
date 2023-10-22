using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI;
using System;
using Grawly.Battle;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Dungeon {

	public class EnemySpawnTerminal : MonoBehaviour, IPlayerInteractable {


		#region FIELDS - TEMPLATES
		/// <summary>
		/// The spawns that are accessible from this terminal.
		/// </summary>
		[SerializeField, TabGroup("Terminal", "Templates")]
		private List<BattleTemplate> battleTemplates = new List<BattleTemplate>();
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The menu list to build up the battle templates with.
		/// </summary>
		[SerializeField, TabGroup("Terminal", "Scene References")]
		private EnemySpawnTerminalMenuList menuList;
		#endregion

		#region PLAYER INTERACTIABLE IMPLEMENTATION
		public string GetInteractableName() {
			return "Enemy Spawn Terminal";
		}

		public void PlayerEnter() {
			DungeonPlayer.Instance.nodeLabel.ShowLabel(this);
		}

		public void PlayerExit() {
			DungeonPlayer.Instance.nodeLabel.HideLabel();
		}

		public void PlayerInteract() {
			DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Wait);
			// Re-enable the menu list.
			this.menuList.gameObject.SetActive(true);
			// Pass a reference to this terminal to the menu list. It's not very safe but. whatever. This is for debugging.
			this.menuList.terminal = this;
			// Prep the menu list for usage.
			this.menuList.PrepareMenuList(allMenuables: this.battleTemplates.Cast<IMenuable>().ToList(), startIndex: 0);
			// Select the first object.
			this.menuList.SelectFirstMenuListItem();
		}
		#endregion

		#region EVENTS
		/// <summary>
		/// Gets called from the menu list, which in turn was called from the menu list item.
		/// Anyway, this is for starting up a battle.
		/// </summary>
		/// <param name="battleTemplate"></param>
		public void PickedBattleTemplate(BattleTemplate battleTemplate) {
			this.menuList.gameObject.SetActive(false);
			BattleController.Instance.StartBattle(battleTemplate: battleTemplate);
		}
		/// <summary>
		/// Gets called from the menu list when I need to close out the terminal completely.
		/// </summary>
		public void CloseTerminal() {
			
			// Turn off the menu list.
			this.menuList.gameObject.SetActive(false);
			
			// Deselect the list item because technically its still active. It wont receive the OnSubmit event but an issue happens with highlighting the first button sometimes if i dont do this.
			EventSystem.current.SetSelectedGameObject(null);
			
			// Turn the player back on after a moment.
			// Doing this because I'd try to close the menu but it would immediately interact again. 
			GameController.Instance.WaitThenRun(timeToWait: 0.5f, action: () => {
				DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
			});
			
		}
		#endregion

	}

}