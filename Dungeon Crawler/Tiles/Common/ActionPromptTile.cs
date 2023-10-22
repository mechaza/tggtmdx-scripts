using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.Legacy;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// I noticed a lot of tiles have action prompts so why don't I just make this a tile of its own.
	/// </summary>
	[DisallowMultipleComponent]
	public class ActionPromptTile : CrawlerFloorTile, IPlayerInteractHandler, IPlayerApproachHandler, IPlayerLookAwayHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The text to display when standing in front of the doorway.
		/// </summary>
		[SerializeField, TabGroup("Doorway","Toggles")]
		private string promptText = "";
		#endregion
		
		#region INTERFACE IMPLEMENTATION
		public void OnApproach(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			// Upon approach, display the prompt text.
			CrawlerActionPrompt.Instance?.Display(promptText: this.promptText);
		}
		public void OnLookAway() {
			// Dismiss the prompt. This should work regardless of it was shown or not.
			CrawlerActionPrompt.Instance?.Dismiss();
		}
		public void OnInteract(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			// Dismiss the prompt.
			CrawlerActionPrompt.Instance?.Dismiss();
		}
		#endregion
		
	}
}