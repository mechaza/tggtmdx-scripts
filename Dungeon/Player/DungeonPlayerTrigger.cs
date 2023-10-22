using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Dungeon {

	/// <summary>
	/// The collider that is in front of the player that is used to interact with things in front of them.
	/// </summary>
	public class DungeonPlayerTrigger : MonoBehaviour {

		public static DungeonPlayerTrigger instance { get; private set; }

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The collider attached to this trigger.
		/// </summary>
		private BoxCollider BoxCollider { get; set; }
		#endregion
		
		#region FIELDS - STATE
		/// <summary>
		/// The object inside of the player's trigger area.
		/// </summary>
		public IPlayerInteractable CurrentInteractable { get; private set; }
		/// <summary>
		/// The GameObject for the current interactable.
		/// Handy for checking if its active in the hierarchy because I shouldn't be able to interact with it if it isn't. 
		/// (This becomes relevant when it deactivates while still within the bounds of the trigger collider)
		/// </summary>
		private GameObject currentInteractableGameObject;
		/// <summary>
		/// Should the hitbox logically be inside of an interactable?
		/// This is important because I'm sorta checking every frame if it is and I don't want to call HideLabel every frame if something happens.
		/// </summary>
		private bool shouldBeInsideInteractable = false;
		#endregion

		#region FIELDS - DEBUG VARIABLES
		[Header("Debug Variables")]
		/// <summary>
		/// The material to show what the bounds of this material are.
		/// </summary>
		[SerializeField]
		private Material triggerMaterial;
		/// <summary>
		/// The color for when the trigger is being activated by the player.
		/// </summary>
		[SerializeField]
		private Color triggerActiveColor;
		/// <summary>
		/// The color for when the trigger is not activated by the player but is ready.
		/// </summary>
		[SerializeField]
		private Color triggerReadyColor;
		#endregion

		private void Awake() {
			if (instance == null) {
				instance = this;
			}

			this.BoxCollider = this.GetComponent<BoxCollider>();
		}
		private void FixedUpdate() {
			// Sometimes when I destory or deactivate an interactable the node label doesn't hide immediately.
			// If I theoretically SHOULD be inside of an object but something happened to it (destroyed or deactivated,) do something about it.
			if (this.shouldBeInsideInteractable == true && (this.currentInteractableGameObject == null || this.currentInteractableGameObject.activeInHierarchy == false)) {
				this.shouldBeInsideInteractable = false;
				this.currentInteractableGameObject = null;
				this.CurrentInteractable = null;
				DungeonPlayer.Instance?.nodeLabel.HideLabel();
			}
		}
		private void OnTriggerEnter(Collider other) {
			 
			if (other.GetComponent<IPlayerInteractable>() != null) {
				this.CurrentInteractable = other.GetComponent<IPlayerInteractable>();
				this.CurrentInteractable.PlayerEnter();
				this.currentInteractableGameObject = other.gameObject;
				this.shouldBeInsideInteractable = true;
			}
			
			foreach (IDungeonPlayerApproachHandler approachHandler in other.GetComponents<IDungeonPlayerApproachHandler>()) {
				approachHandler.OnDungeonPlayerApproach();
			}
			
		}
		private void OnTriggerExit(Collider other) {
			// The collider might have made contact with multiple interactables. Make sure you're Exiting the correct one.
			if (other.GetComponent<IPlayerInteractable>() != null && other.GetComponent<IPlayerInteractable>() == this.CurrentInteractable) {
				this.CurrentInteractable.PlayerExit();
				this.CurrentInteractable = null;
				this.currentInteractableGameObject = null;
				this.shouldBeInsideInteractable = false;
			}
			
			foreach (IDungeonPlayerApproachHandler approachHandler in other.GetComponents<IDungeonPlayerApproachHandler>()) {
				approachHandler.OnDungeonPlayerLeave();
			}
			
		}

		/// <summary>
		/// Calls PlayerInteract on all IPlayerInteractables the DungeonPlayerTrigger is currently keeping track of.
		/// </summary>
		public void CallPlayerInteractable() {
			
			//
			//
			// This line in particular is legacy and should be removed in the future.
			this.CurrentInteractable?.PlayerInteract();
			// The line above is legacy and should be removed in the future.
			//
			//
			
			// Figure out how to assemble the overlap box.
			Vector3 targetCenter = this.transform.position + this.BoxCollider.center;
			Vector3 targetHalfExtents = this.BoxCollider.bounds.extents;
			Quaternion targetOrientation = Quaternion.identity;
			
			// Get a list of anything that was interactable.
			var interactionHandlers = Physics.OverlapBox(
					center: targetCenter,
					halfExtents: targetHalfExtents, 
					orientation: targetOrientation)
				.SelectMany(c => c.gameObject.GetComponents<IDungeonPlayerInteractionHandler>())
				.ToList();

			// While I AM making a list, assert that the count should be one or less for the time being.
			// The only exception is the DungeonObjectPrompt component, which has a valid reason for being present too.
			Debug.Assert(interactionHandlers
				.Where(i => (i is DungeonObjectPrompt) == false)
				.ToList()
				.Count <= 1);
			
			// Iterate through the list and call the OnPlayerInteract function.
			foreach (IDungeonPlayerInteractionHandler handler in interactionHandlers) {
				handler.OnPlayerInteract();
			}
			
		}

	}
}