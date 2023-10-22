using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Invector.vCharacterController;
using System.Linq;
using DG.Tweening;
using Grawly.Dungeon.Generation;
using Grawly.Dungeon.Legacy;
using Grawly.UI;
using UnityStandardAssets.ImageEffects;

namespace Grawly.Dungeon {

	/// <summary>
	/// A representation of the player as they exist in the dungeon. Analogous to a CharacterController but more for states and whatnot.
	/// </summary>
	public class DungeonPlayer : MonoBehaviour {

		public static DungeonPlayer Instance { get; private set; }
		
		#region PROPERTIES - STATE
		/// <summary>
		/// The current state of the DungeonPlayer.
		/// Helpful for knowing if it's "locked" or not.
		/// </summary>
		public DungeonPlayerStateType CurrentState => this.GetFSMState();
		/// <summary>
		/// Is the functionality to set wait/
		/// </summary>
		public bool CurrentStateLocked { get; private set; } = false;
		/// <summary>
		/// Is the DungeonPlayer ready to attack?
		/// </summary>
		public bool ReadyToAttack {
			get {
				// For right now, just return the FSM state.
				return this.GetFSMState() == DungeonPlayerStateType.Free;
			}
		}
		#endregion
		
		#region FIELDS - STATIC VARIABLES
		/// <summary>
		/// The camera for the player.
		/// </summary>
		public static Camera playerCamera;
		/// <summary>
		/// The actual player game object.
		/// </summary>
		public static GameObject playerModel;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The state machine that drives certain functionality for this DungeonPlayer.
		/// </summary>
		[HideInInspector]
		public PlayMakerFSM fsm;
		/// <summary>
		/// The camera that tracks the player. This is the same as playerCamera.
		/// </summary>
		[SerializeField]
		private Camera playerCamRef;
		/// <summary>
		/// The visuals that represent the player's weapon.
		/// Used in attacks.
		/// </summary>
		[SerializeField]
		private GameObject weaponVisuals;
		/// <summary>
		/// The GameObject that contains the weapon's hitbox.
		/// This is separate from the visuals because it needs to be toggled asynchronously from it.
		/// </summary>
		[SerializeField]
		private GameObject weaponHitbox;
		/// <summary>
		/// I'm only gonna use one node label from now on. Easier.
		/// </summary>
		public DungeonNodeLabel nodeLabel;
		/// <summary>
		/// The third party asset that controls input for the dungeon player.
		/// </summary>
		private vMeleeCombatInput combatInput;
		/// <summary>
		/// The spawn wheel attached to this DungeonPlayer.
		/// Just referencing the instance for right now.
		/// </summary>
		public DungeonPlayerSpawnWheel SpawnWheel => DungeonPlayerSpawnWheel.Instance;
		/// <summary>
		/// Contains multiple DungeonEnemies that need to be dealt with if there was more than one around the player.
		/// </summary>
		public List<DungeonEnemy> DungeonEnemiesInRadius {
			get {
				// Debug.Log("Looking for dungeon enemies in radius.");
				// Make a new blank list.
				float radius = 5f;
				List<DungeonEnemy> dungeonEnemies = new List<DungeonEnemy>();
				// Go through each DungeonEnemy in the scene.
				foreach (DungeonEnemy dungeonEnemy in GameObject.FindObjectsOfType<DungeonEnemy>()) {
					// If the distance is less than the threshold (and its not the enemy that was just encountered), add it to the queue.
					float distance = (DungeonPlayer.Instance.gameObject.transform.position - dungeonEnemy.transform.position).magnitude;
					// Debug.Log("Center: " + DungeonPlayer.Instance.gameObject.transform.position + ", Enemy: " + dungeonEnemy.transform.position + ". Distance: " + distance);
					if (distance < radius) {
						Debug.Log("DUNGEON ENEMY FOUND IN RADIUS");
						dungeonEnemies.Add(dungeonEnemy);
					}
				}
				return dungeonEnemies;
			}
		}
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				playerCamera = playerCamRef;
				playerModel = this.gameObject;
				combatInput = GetComponent<vMeleeCombatInput>();
				fsm = GetComponent<PlayMakerFSM>();
			}
		}
		private void Update() {

			// Back out if the state is set to wait.
			if (this.GetFSMState() == DungeonPlayerStateType.Wait) {
				return;
			}

			// If pausing, well, call the pause contorller.
			if (InputController.Instance.GetButtonDown("Pause")) {
				this.SetFSMState(DungeonPlayerStateType.Wait);
				PauseMenuController.Open(onExitCallback: delegate {
					this.SetFSMState(DungeonPlayerStateType.Free);
				});
			} else if (InputController.Instance.GetButtonDown("Action")) {
				Debug.Log("Submit button pressed. Alerting DungeonPlayerTrigger.");
				DungeonPlayerTrigger.instance.CallPlayerInteractable();
			} 
			
			//
			// The code below is commented bc I needed to add an else-if. I think.
			//
			
			/*// If pausing, well, call the pause contorller.
			if (InputController.Instance.GetButtonDown("Pause")) {
				this.SetFSMState(DungeonPlayerStateType.Wait);
				PauseMenuController.Open(onExitCallback: delegate {
					this.SetFSMState(DungeonPlayerStateType.Free);
				});
			}
			
			if (InputController.Instance.GetButtonDown("Action")) {
				Debug.Log("Submit button pressed. Alerting DungeonPlayerTrigger.");
				DungeonPlayerTrigger.instance.CallPlayerInteractable();
			}*/
			
		}
		#endregion

		#region STATE MACHINE RELATED
		/// <summary>
		/// Sends the event to the DungeonPlayer's FSM, converted to a string, of course.
		/// </summary>
		/// <param name="state"></param>
		public void SetFSMState(DungeonPlayerStateType state) {
			switch (state) {
				case DungeonPlayerStateType.Free:
					Debug.Log("DungeonPlayer FSM State set to free. Unlocking input.");
					this.combatInput.SetLockBasicInput(value: false);
					this.combatInput.SetLockCameraInput(value: false);
					this.combatInput.SetLockMeleeInput(value: false);
					break;
				case  DungeonPlayerStateType.Wait:
					Debug.Log("DungeonPlayer FSM State set to wait. Locking input.");
					this.combatInput.SetLockBasicInput(value: true);
					this.combatInput.SetLockCameraInput(value: true);
					this.combatInput.SetLockMeleeInput(value: true);
					break;
				case  DungeonPlayerStateType.CameraOnly:
					Debug.Log("DungeonPlayer FSM State set to CameraOnly. Locking input.");
					this.combatInput.SetLockBasicInput(value: true);
					this.combatInput.SetLockCameraInput(value: false);
					this.combatInput.SetLockMeleeInput(value: true);
					break;
				default:
					Debug.LogError("Couldn't determine DungeonPlayer FSM state!");
					break;
			}

			string str = state.ToString();
			fsm.SendEvent(str);
		}
		/// <summary>
		/// Returns the state of the DungeonPlayer's FSM as an enum.
		/// </summary>
		public DungeonPlayerStateType GetFSMState() {
			switch (fsm.ActiveStateName) {
				case "Free":
					return DungeonPlayerStateType.Free;
				case "Wait":
					return DungeonPlayerStateType.Wait;
				case "CameraOnly":
					return DungeonPlayerStateType.CameraOnly;
				default:
					Debug.LogError("Couldn't determine DungeonPlayer FSM state!");
					return DungeonPlayerStateType.ERROR;
			}
		}
		#endregion

		#region EFFECTS - ATTACKS
		/// <summary>
		/// Sets the graphics on the dungeon player's weapon.
		/// Used when attacking.
		/// </summary>
		/// <param name="active"></param>
		public void SetWeaponGraphics(bool active) {
			this.weaponVisuals.SetActive(active);
		}
		/// <summary>
		/// Sets the hitbox on the dungeon player's weapon.
		/// Used when attacking.
		/// </summary>
		/// <param name="active"></param>
		public void SetWeaponHitbox(bool active) {
			this.weaponHitbox.SetActive(active);
		}
		#endregion
		
		#region EFFECTS - TRANSITIONS
		/// <summary>
		/// A (potentially) debug method for playing the effect that blurs the player camera when they hit an enemy in a dungeon.
		/// </summary>
		public IEnumerator PlayBattleTransitionEffectRoutine() {
		
			// Blur the camera.
			playerCamera.GetComponent<BlurOptimized>().enabled = true;
			DOTween.To(
				getter: () => playerCamera.GetComponent<BlurOptimized>().blurSize,
				setter: x => playerCamera.GetComponent<BlurOptimized>().blurSize = x,
				endValue: 3f,
				duration: 1f);
			yield return new WaitForSeconds(4f);
			playerCamera.GetComponent<BlurOptimized>().blurSize = 0f;
			playerCamera.GetComponent<BlurOptimized>().enabled = false;
		}
		/// <summary>
		/// Debug function that resets whatever the effect routine above did.
		/// </summary>
		public void ResetEffect() {
			CameraFilterPack_Blur_DitherOffset effect = playerCamera.GetComponent<CameraFilterPack_Blur_DitherOffset>();
			effect.Distance = Vector2.zero;
			effect.enabled = false;
		}
		#endregion

		#region DUNGEON PLACEMENT
		/// <summary>
		/// Move the DungeonPlayer to a new location. Handy when moving up a floor.
		/// </summary>
		/// <param name="pos"></param>
		public void Relocate(Transform pos) {
			Debug.Log("RELOCATING THE PLAYER");
			this.transform.SetPositionAndRotation(pos.position, pos.rotation);
		}
		/// <summary>
		/// Move the dungeon player to a new location.
		/// Done over a set amount of time.
		/// Good for tweening.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="tweenTime"></param>
		/// <param name="easeType"></param>
		public void Relocate(Transform pos, float tweenTime, Ease easeType = Ease.InOutCirc) {
			this.transform.DOMove(endValue: pos.position, duration: tweenTime).SetEase(ease: easeType);
			this.transform.DORotateQuaternion(endValue: pos.rotation, duration: tweenTime).SetEase(ease: easeType);
		}
		#endregion

		#region MOVEMENT
		public void ZeroMomentum() {
			Debug.Log("WUOW");
			// GetComponent<Rigidbody>().velocity = Vector3.zero;
			// combatInput.cc.
		}
		#endregion

	}
}