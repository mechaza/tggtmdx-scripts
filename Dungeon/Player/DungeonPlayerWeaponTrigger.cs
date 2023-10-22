using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Invector.vCharacterController;
using System.Linq;
using DG.Tweening;


namespace Grawly.Dungeon {
	
	/// <summary>
	/// The hitbox that can send events on if it hit an enemy or not.
	/// </summary>
	public class DungeonPlayerWeaponTrigger : MonoBehaviour {

		public static DungeonPlayerWeaponTrigger Instance { get; private set; }
		
		#region FIELDS - STATE
		/// <summary>
		/// Can an attack event be invoked as of right now?
		/// Set to false after the first attack so subsequent frames do not get caught.
		/// </summary>
		private bool CanInvokeAttackEvent { get; set; } = true;
		/// <summary>
		/// Upon sucessfully completing an attack, should the player be forced to wait?
		/// </summary>
		public bool DungeonPlayerShouldBeLocked { get; set; } = false;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The collider attached to this trigger.
		/// </summary>
		private BoxCollider BoxCollider { get; set; }
		#endregion

		#region UNITY CALLS
		private void OnEnable() {
			// Reset the flag for whether or not attack events can be invoked.
			this.CanInvokeAttackEvent = true;
		}
		private void OnDisable() {
			// Override the flag that signals if the player should be locked.
			this.DungeonPlayerShouldBeLocked = false;
		}
		private void Awake() {
			Instance = this;
			this.BoxCollider = this.GetComponent<BoxCollider>();
		}
		private void Update() {

			// If able to invoke an attack event, see if you can.
			if (this.CanInvokeAttackEvent == true) {
				// For as long as this weapon trigger is active, grab any overlapping attack handlers.
				List<IDungeonPlayerAttackHandler> attackedComponents = this.GetOverlappingAttackHandlers();
				// If there's at least one component that was attacked...
				if (attackedComponents.Count > 0) {
					// Set the flag so that repeated events are not captured.
					this.CanInvokeAttackEvent = false;
					// Also set the flag that signals whether or not the player should be locked.
					this.DungeonPlayerShouldBeLocked = attackedComponents.First().LockDungeonPlayer;
					// Only invoke the first one.
					attackedComponents.First().OnDungeonPlayerAttack(dungeonPlayer: DungeonPlayer.Instance);
				}
			}
			
		}
		#endregion

		#region STATE CHECKS
		/// <summary>
		/// Gets a list of any components that are overlapping with the weapon hitbox.
		/// </summary>
		/// <returns></returns>
		private List<IDungeonPlayerAttackHandler> GetOverlappingAttackHandlers() {
			
			Vector3 targetCenter = this.transform.position + this.BoxCollider.center;
			Vector3 targetHalfExtents = this.BoxCollider.bounds.extents;
			Quaternion targetOrientation = Quaternion.identity;
			
			// Get a list of anything that was attackable.
			var interactionHandlers = Physics.OverlapBox(
					center: targetCenter,
					halfExtents: targetHalfExtents, 
					orientation: targetOrientation)
				.SelectMany(c => c.gameObject.GetComponents<IDungeonPlayerAttackHandler>())
				.ToList();

			return interactionHandlers;
			
		}
		#endregion
		
	}
}