using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI;
using Grawly.Battle;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.EventSystems;

namespace Grawly.Dungeon {

	public class AutoBattleTerminal : MonoBehaviour, IPlayerInteractable {

		public static AutoBattleTerminal instance;

		#region FIELDS - BATTLES
		/// <summary>
		/// The battle templates to pick from when auto battling.
		/// </summary>
		[SerializeField]
		private List<BattleTemplate> battleTemplates = new List<BattleTemplate>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		#endregion

		#region PLAYER INTERACTIABLE IMPLEMENTATION
		public string GetInteractableName() {
			return "Auto Battle Terminal";
		}

		public void PlayerEnter() {
			DungeonPlayer.Instance.nodeLabel.ShowLabel(this);
		}

		public void PlayerExit() {
			DungeonPlayer.Instance.nodeLabel.HideLabel();
		}

		public void PlayerInteract() {
			DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Wait);
			this.StartNextBattle();
		}
		#endregion

		#region FIELDS - AUTOBATTLING
		/// <summary>
		/// Starts a battle totally at random.
		/// </summary>
		public void StartNextBattle() {
			this.StartNextBattle(index: Random.Range(minInclusive: 0, maxExclusive: this.battleTemplates.Count));	
		}
		/// <summary>
		/// Starts a battle at the given index.
		/// </summary>
		/// <param name="index">The index of the next</param>
		private void StartNextBattle(int index) {
			this.StartNextBattle(battleTemplate: this.battleTemplates[index]);
		}
		/// <summary>
		/// Starts the battle with the given template.
		/// </summary>
		/// <param name="battleTemplate"></param>
		public void StartNextBattle(BattleTemplate battleTemplate) {
			// Reset the battle controller state
			// BattleController.Instance.ResetBattleControllerState(battleTemplate: battleTemplate);
			// Prepare the battle.
			throw new System.NotImplementedException("i removed the dungeon controller");
			// DungeonController.Instance.PrepareBattle(battleTemplate: battleTemplate);
		}
		#endregion

	}


}