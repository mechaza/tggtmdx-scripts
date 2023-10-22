using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Chat;
using Grawly.UI.Legacy;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// Transitions scenes when the player lands on it.
	/// </summary>
	public class SceneTransitionTile : CrawlerFloorTile, IPlayerLandHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The scene to transition to upon stepping on this tile.
		/// </summary>
		[SerializeField]
		private string targetSceneName = "";
		#endregion
		
		#region INTERFACE IMPLEMENTATION
		/// <summary>
		/// Gets called when the tile is stepped on directly.
		/// </summary>
		public void OnLand(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Wait);
			AudioController.instance.StopMusic(track: 0, fade: 1.5f);
			SceneController.instance.BasicLoadSceneWithFade(
				sceneName: this.targetSceneName,
				fadeTime: 2f);
		}
		#endregion
	}
}