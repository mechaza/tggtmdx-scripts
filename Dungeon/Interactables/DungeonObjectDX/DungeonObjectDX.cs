using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using Grawly.UI;

namespace Grawly.Dungeon {
	
	/// <summary>
	/// A specialized class that can take care of common features required from a DungeonObject.
	/// </summary>
	public class DungeonObjectDX : MonoBehaviour, IDungeonPlayerApproachHandler, IDungeonPlayerInteractionHandler {

		#region FIELDS - TOGGLES : APPROACH
		/// <summary>
		/// Should the info box be displayed upon approaching this DungeonObject?
		/// </summary>
		[SerializeField, TabGroup("General", "Interaction")]
		private bool displayInfoBoxOnApproach = false;
		/// <summary>
		/// The string to display on the Info Box if set to display one on approach.
		/// </summary>
		[SerializeField, TabGroup("General", "Interaction"), ShowIf("displayInfoBoxOnApproach")]
		private string infoBoxText = "";
		#endregion
		
		#region FIELDS - TOGGLES : BOLT
		/// <summary>
		/// Should a flow machine be used when this DungeonObject is interacted with?
		/// </summary>
		[SerializeField, TabGroup("General", "Interaction")]
		private bool useBoltFlowMachine = false;
		/// <summary>
		/// The name of the event to trigger if using a Bolt FSM.
		/// </summary>
		[SerializeField, TabGroup("General", "Interaction"), ShowIf("useBoltFlowMachine")]
		private string boltEventTriggerName = "";
		#endregion

		#region UNITY CALLS
		
		#endregion
		
		#region INTERFACE - IDUNGEONPLAYERAPPROACHHANDLER
		/// <summary>
		/// Called when the DungeonPlayer approaches this DungeonObject.
		/// </summary>
		public void OnDungeonPlayerApproach() {
			
			if (this.displayInfoBoxOnApproach == true) {
				InfoBarController.Instance?.Display(bodyText: this.infoBoxText);
			}
			
		}
		/// <summary>
		/// Called when the DungeonPlayer leaves this DungeonObject.
		/// </summary>
		public void OnDungeonPlayerLeave() {
			if (this.displayInfoBoxOnApproach == true) {
				InfoBarController.Instance?.Dismiss();
			}

		}
		#endregion

		#region INTERFACE - IDUNGEONPLAYERINTERACTIONHANDLER
		/// <summary>
		/// Gets called when the player does try to interact with this object.
		/// </summary>
		public void OnPlayerInteract() {
			if (this.useBoltFlowMachine == true) {
				Unity.VisualScripting.CustomEvent.Trigger(target: this.gameObject, name: this.boltEventTriggerName);
			}
		}
		#endregion
		
	}
	
}