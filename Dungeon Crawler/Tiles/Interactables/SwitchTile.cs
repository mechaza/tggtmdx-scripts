using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// Fires a Unity Event when interacted with.
	/// </summary>
	public class SwitchTile : CrawlerFloorTile, IPlayerInteractHandler, IPlayerApproachHandler, IPlayerLookAwayHandler {

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// Should a prompt be shown when approaching this tile?
		/// </summary>
		[SerializeField]
		private bool showPrompt = true;
		/// <summary>
		/// The string to display when approaching this tile.
		/// </summary>
		[SerializeField, ShowIf("showPrompt")]
		private string promptText = "Interact";
		#endregion

		#region FIELDS - TOGGLES : EVENTS
		/// <summary>
		/// UnityEvent to run when approaching this tile.
		/// </summary>
		[SerializeField]
		private UnityEvent onApproachEvent;
		/// <summary>
		/// UnityEvent to run when interacting with this tile.
		/// </summary>
		[SerializeField]
		private UnityEvent onInteractEvent;
		/// <summary>
		/// UnityEvent to run when looking away from this tile.
		/// </summary>
		[SerializeField]
		private UnityEvent onLookAwayEvent;
		#endregion
		
		#region PROPERTIES - STATE
		/// <summary>
		/// Should the action prompt be shown?
		/// </summary>
		private bool ShowPrompt {
			get {
				// If the action prompt is NOT empty and I explicitly want to show the prompt, return true.
				return this.showPrompt && this.promptText != "";
			}
		}
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// By default, don't worry about doing anything on approach.
		/// </summary>
		public void OnApproach(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			// If the prompt should be shown, do so.
			if (this.ShowPrompt == true) {
				CrawlerActionPrompt.Instance?.Display(promptText: this.promptText);
			}
			
			// Run the onApproach event.
			this.onApproachEvent.Invoke();
			
		}
		/// <summary>
		/// When leaving, dismiss the prompt.
		/// </summary>
		public void OnLookAway() {
			
			// Dismiss the prompt. This should work regardless of it was shown or not.
			CrawlerActionPrompt.Instance?.Dismiss();
			
			// Invoke the look away event.
			this.onLookAwayEvent.Invoke();
			
		}
		/// <summary>
		/// On interaction, do... something.
		/// </summary>
		public void OnInteract(CrawlerProgressionSet crawlerProgressionSet, int floorNumber) {
			CrawlerActionPrompt.Instance?.Dismiss();
			// Invoke the interaction event.
			this.onInteractEvent.Invoke();
		}
		#endregion

		
	}

	
}