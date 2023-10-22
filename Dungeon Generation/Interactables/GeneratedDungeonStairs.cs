using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Dungeon.Generation;

namespace Grawly.Dungeon.Interactable {
	public class GeneratedDungeonStairs : MonoBehaviour, IPlayerInteractable {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The number of floors to climb when using these stairs.
		/// </summary>
		[SerializeField]
		private int floorCount = 1;
		#endregion

		#region PLAYER INTERACTABLE IMPLEMENTATION
		public void PlayerEnter() {
			DungeonPlayer.Instance.nodeLabel.ShowLabel(this);
		}
		public void PlayerExit() {
			DungeonPlayer.Instance.nodeLabel.HideLabel();
		}
		public void PlayerInteract() {
			DungeonPlayer.Instance.nodeLabel.HideLabel();
			ProceduralDungeonController.instance.ProceedThroughStairs(floorIncrement: this.floorCount);
			// throw new System.Exception("Deprecated this.");
			// DungeonGenerator.Instance.AdvanceFloor();
		}
		public string GetInteractableName() {
			return "Proceed";
		}
		#endregion
	}

}