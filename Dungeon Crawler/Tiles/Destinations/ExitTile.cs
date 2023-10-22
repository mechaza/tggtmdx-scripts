using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Calendar;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler  {
	
	/// <summary>
	/// The Exit Tile, where you can leave the dungeon.
	/// </summary>
	public class ExitTile : CrawlerFloorTile, IPlayerInteractHandler {

		#region INTERFACEIMPLEMENTATION - IPLAYERINTERACTHANDLER
		public void OnInteract(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Wait);
			SceneController.instance.BasicLoadSceneWithFade(locationType: LocationType.DungeonLobby);
		}
		#endregion
		
	}
}