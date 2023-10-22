using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Chat;
using Grawly.DungeonCrawler;
using Sirenix.OdinInspector;

namespace Grawly.Dungeon {

	public class DungeonNPC : MonoBehaviour, IPlayerInteractable {

		#region FIELDS - NPC VARIABLES
		/// <summary>
		/// The name of this NPC.
		/// </summary>
		[TabGroup("Metadata", "Metadata")]
		public string npcName;
		#endregion

		#region PLAYER INTERACTABLE IMPLEMENTATION
		public void PlayerEnter() {
			// DungeonPlayer.Instance.nodeLabel.ShowLabel(this);
			CrawlerActionPrompt.Instance?.Display(promptText: this.npcName);
		}
		public void PlayerExit() {
			// DungeonPlayer.Instance.nodeLabel.HideLabel();
			CrawlerActionPrompt.Instance?.Dismiss();
		}
		public void PlayerInteract() {

			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);
			CrawlerActionPrompt.Instance?.Dismiss();
			this.GetComponent<PlayMakerFSM>()?.SendEvent("INTERACT");
			Unity.VisualScripting.CustomEvent.Trigger(target: this.gameObject, name: "INTERACT");

		}
		public string GetInteractableName() {
			return npcName;
		}
		#endregion


	}


}