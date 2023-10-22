using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraTransitions;
using Cinemachine;
using Sirenix.OdinInspector;
using Grawly.Battle.WorldEnemies;
using System.Linq;
using Grawly.DungeonCrawler;
using Grawly.Gauntlet;
using Grawly.Menus;
using UnityStandardAssets.ImageEffects;

namespace Grawly.Battle {
	public class BattleCameraController : MonoBehaviour {

		public static BattleCameraController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// Only used to determine whether or not to grab cameraOne or cameraTwo when finding the MainCamera.
		/// </summary>
		private bool cameraSwap = false;
		/// <summary>
		/// A list of activated virtual cameras. Note that the Normal view is likely not in here.
		/// </summary>
		private List<CinemachineVirtualCamera> activatedVirtualCameras = new List<CinemachineVirtualCamera>();
		#endregion

		#region FIELDS - SCENE REFERENCES : MISC
		/// <summary>
		/// The third party asset that handles camera transition effects.
		/// </summary>
		[SerializeField, TabGroup("Controller","Misc References")]
		private CameraTransition cameraTransition;
		/// <summary>
		/// The parent that holds the two cameras.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Misc References")]
		private GameObject cameraParent;
		/// <summary>
		/// Contains the virtual cameras. Must get turned on at the start of a battle and turned off at the end.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Misc References")]
		private GameObject virtualCamerasParent;
		/// <summary>
		/// The target group for enemies.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Misc References")]
		private CinemachineTargetGroup enemyTargetGroup;
		[SerializeField, TabGroup("Controller", "Misc References")]
		private Camera cameraOne;
		[SerializeField, TabGroup("Controller", "Misc References")]
		private Camera cameraTwo;
		#endregion

		#region FIELDS - SCENE REFERENCES : VIRTUAL CAMERAS
		/// <summary>
		/// The camera to be used for the position at the start of a player's turn.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Virtual Cameras")]
		private CinemachineVirtualCamera normalBattleCamera;
		/// <summary>
		/// The camera for when looking "head on" at the enemies. Used for all out attack and multiple target attacks.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Virtual Cameras")]
		private CinemachineVirtualCamera headOnBattleCamera;
		/// <summary>
		/// A battle camera with severe dutch and shit. Used mostly for All Out Attacks.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Virtual Cameras")]
		private CinemachineVirtualCamera dynamicHeadOnBattleCamera;
		/// <summary>
		/// A battle camera with severe dutch and shit. Used mostly for All Out Attacks.
		/// Publically accessible because I'm sorta hard coding the all out attack.
		/// </summary>
		public CinemachineVirtualCamera DynamicHeadOnBattleCamera {
			get {
				return this.dynamicHeadOnBattleCamera;
			}
		}
		/// <summary>
		/// The camera for when I need to orbit around the enemies.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Virtual Cameras")]
		private CinemachineVirtualCamera orbitEnemyBattleCamera;
		/// <summary>
		/// The camera for when I need to orbit around the enemies.
		/// Publically accessible because I'm sorta hard coding the all out attack.
		/// </summary>
		public CinemachineVirtualCamera OrbitEnemyBattleCamera {
			get {
				return this.orbitEnemyBattleCamera;
			}
		}
		/// <summary>
		/// The camera used to for making the sweeping effect at the start of battles. Turn on when going back to dungeon, turn off when going into battle.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Virtual Cameras")]
		private CinemachineVirtualCamera battleStartVirtualCamera;
		/// <summary>
		/// The virtual camera that gets used for the soft battle transition.
		/// At the start, it assumes the position of the player cam and then will transition to the normal cam.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Virtual Cameras")]
		private CinemachineVirtualCamera softBattleStartVirtualCamera;
		#endregion

		#region FIELDS - COMPUTED
		/// <summary>
		/// The "Main" camera (i.e., the one that is active)
		/// </summary>
		public Camera MainCamera {
			get {
				if (cameraSwap == false) {
					return cameraOne;
				} else {
					return cameraTwo;
				}
			}
		}
		/// <summary>
		/// The "Secondary" camera (i.e., the one that is turned off)
		/// </summary>
		public Camera SecondaryCamera {
			get {
				if (cameraSwap == true) {
					return cameraOne;
				} else {
					return cameraTwo;
				}
			}
		}
		/// <summary>
		/// A dictionary to make it easier for me to store the camera references.
		/// </summary>
		private Dictionary<BattleCameraType, CinemachineVirtualCamera> battleCameraDict = new Dictionary<BattleCameraType, CinemachineVirtualCamera>();
		#endregion

		#region FIELDS - METHODS
		/// <summary>
		/// A coroutine that sets the vignette on the screen over time.
		/// </summary>
		private IEnumerator setVignetteOverTime;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
			// Set the references to the battle cameras. This dictionary is mostly for convinience.
			this.battleCameraDict.Add(key: BattleCameraType.NormalCamera, value: this.normalBattleCamera);
			this.battleCameraDict.Add(key: BattleCameraType.HeadOnCamera, value: this.headOnBattleCamera);
			this.battleCameraDict.Add(key: BattleCameraType.DynamicHeadOn, value: this.dynamicHeadOnBattleCamera);
			this.battleCameraDict.Add(key: BattleCameraType.OrbitEnemies, value: this.orbitEnemyBattleCamera);
			this.battleCameraDict.Add(key: BattleCameraType.BattleStart, value: this.battleStartVirtualCamera);

			// this.cameraTransition.RenderTextureSize = CameraTransition.RenderTextureSizes.HalfVideo;
			this.cameraTransition.RenderTextureDepth = CameraTransition.RenderTextureDepths._16Bits;

		}
		#endregion

		#region CINEMACHINE VIRTUAL CAMERA FUNCTIONS
		/// <summary>
		/// Swap the Main and Secondary cameras with each other.
		/// </summary>
		private void SwapCameras() {
			// Save the clear flags/culling mask
			CameraClearFlags clearFlags = MainCamera.clearFlags;
			int cullingMask = MainCamera.cullingMask;
			// Give the main camera the secondary camera's info
			MainCamera.clearFlags = SecondaryCamera.clearFlags;
			MainCamera.cullingMask = SecondaryCamera.cullingMask;

			// Reverse the enabled status of the listener.
			MainCamera.GetComponent<AudioListener>().enabled = false;
			SecondaryCamera.GetComponent<AudioListener>().enabled = true;

			// Fix up the secondary camera
			SecondaryCamera.clearFlags = clearFlags;
			SecondaryCamera.cullingMask = cullingMask;

			// Swap the brains. Swap the brains!
			// (Do this because I need to make sure the camera being swapped doesn't follow a target virtual camera.)
			MainCamera.GetComponent<CinemachineBrain>().enabled = false;
			SecondaryCamera.GetComponent<CinemachineBrain>().enabled = true;

			// Swap the two
			cameraSwap = !cameraSwap;

		}
		/// <summary>
		/// Activates the camera of the specified type.
		/// </summary>
		/// <param name="cameraType">The type of camera to activate.</param>
		public void ActivateVirtualCamera(BattleCameraType cameraType) {
			// Grab the camera from its entry in the dictionary and pass it over to the method that actually does activate it.
			this.ActivateVirtualCamera(cam: this.battleCameraDict[cameraType]);
		}
		/// <summary>
		/// Activates the game object that SHOULD have a virtual camera.
		/// </summary>
		public void ActivateVirtualCamera(CinemachineVirtualCamera cam) {
			Debug.Log("Activating " + cam.gameObject.ToString());
			cam.gameObject.SetActive(true);
			activatedVirtualCameras.Add(cam);
		}
		/// <summary>
		/// Disables a given list of virtual cameras. Mostly using this when I need to do things like,
		/// restore the normal view, etc. 
		/// </summary>
		/// <param name="toDisable"></param>
		private void DisableVirtualCameras(List<CinemachineVirtualCamera> toDisable) {
			// I have to make a copy because most of the time toDisable is the same as activatedVirtualCameras.
			List<CinemachineVirtualCamera> toDisableCopy = new List<CinemachineVirtualCamera>(toDisable);
			foreach (CinemachineVirtualCamera cam in toDisableCopy) {
				cam.gameObject.SetActive(false);
				activatedVirtualCameras.Remove(cam);
			}

		}
		#endregion

		#region CINEMACHINE TARGET GROUP FUNCTIONS
		/// <summary>
		/// Updates the cinemachine target group with new targets containing only alive enemies. Also gives "focus" to a given enemy, if desired.
		/// </summary>
		public void UpdateCinemachineTargetGroup(List<WorldEnemyDX> worldEnemies, WorldEnemyDX focusEnemy = null) {
			// Set the target group to be computed as a result of the enemies passed in.
			this.enemyTargetGroup.m_Targets = worldEnemies
				.Select(we => new CinemachineTargetGroup.Target {
					target = we.CinemachineTarget.transform,						// The target transform is the transform of the enemy's game object.
					radius = (Mathf.Clamp((5f / worldEnemies.Count) * 2f, 1f, 3f)),	// The radius depends on how many enemies were passed in and the magnitude
					weight = (we == focusEnemy) ? 1.6f : 1f })						// The focus enemy should have a higher weight.
				.ToArray();															// Cast it as an array, because the group needs that.
		}
		#endregion

		#region EFFECT FUNCTIONS
		/// <summary>
		/// Sets a vignette on the main camera. Gets used for the secondary screen.
		/// </summary>
		public void SetVignette(float amt, float time = 0f) {
			if (setVignetteOverTime != null) {
				StopCoroutine(setVignetteOverTime);
			}
			// StopCoroutine("SetVignetteOverTime");       // Stop the old vignette if it's still going.
			if (time > 0f) {
				setVignetteOverTime = SetVignetteOverTime(amt, time);
				StartCoroutine(setVignetteOverTime);
				// StartCoroutine(SetVignetteOverTime(amt, time));
			} else {
				MainCamera.GetComponent<VignetteAndChromaticAberration>().intensity = amt;
				
			}
		}
		/// <summary>
		/// Sets the vignette over time, if needed.
		/// </summary>
		private IEnumerator SetVignetteOverTime(float amt, float time) {
			
			float t = 0f;
			VignetteAndChromaticAberration comp = MainCamera.GetComponent<VignetteAndChromaticAberration>();
			float old = comp.intensity;
			while (t < time) {
				comp.intensity = Mathf.Lerp(old, amt, t / time);
				t += Time.deltaTime;
				yield return null;
			}
			comp.intensity = amt;
		}
		/// <summary>
		/// Tells the two main cameras to copy the culling color of the camera passed in, as well as other details if needed.
		/// Helpful for when I need to assign a camera to match like, fog or something.
		/// </summary>
		/// <param name="cam">The camera's </param>
		public void CopyCameraSettings(Camera cam) {
			this.cameraOne.clearFlags = cam.clearFlags;
			this.cameraTwo.clearFlags = cam.clearFlags;
			this.cameraOne.backgroundColor = cam.backgroundColor;
			this.cameraTwo.backgroundColor = cam.backgroundColor;
		}
		#endregion

		#region EVENT FUNCTIONS
		/// <summary>
		/// The routine that sets up the camera for the beginning of a battle.
		/// </summary>
		public IEnumerator TransitionToBattle(Camera playerCamera, float delayTime = 2.95f) {
			// Disable the brain on the player's camera so that it doesn't transition tot he battle.
			playerCamera.GetComponent<CinemachineBrain>().enabled = false;

			// Disable any virtual cameras that may still be active from the last battle.
			this.DisableVirtualCameras(activatedVirtualCameras);
			// Turn on the battle camera and transition.
			this.cameraParent.SetActive(true);
			// Make sure the soft cam is off.
			this.softBattleStartVirtualCamera.gameObject.SetActive(false);
			// Also turn on the virtual cameras
			this.virtualCamerasParent.SetActive(true);

			/*// Get each enemy to look at the camera and update its visuals so no text is shwoing.
			foreach (LegacyWorldEnemySprite worldEnemySprite in GameObject.FindObjectsOfType<LegacyWorldEnemySprite>()) {
				worldEnemySprite.LookAtCamera(normalBattleCamera.gameObject);
				worldEnemySprite.UpdateStatusModifierVisuals();
			}*/

			this.cameraTransition.DoTransition(CameraTransitionEffects.WarpWave, playerCamera, MainCamera, 2f);
			// Turn off the battle start camera so that cinemachine can do the transition properly.

			yield return new WaitForSeconds(0.05f);
			battleStartVirtualCamera.gameObject.SetActive(false);

			// Wait a few seconds.
			// yield return new WaitForSeconds(2.95f);
			yield return new WaitForSeconds(delayTime);
		}
		/// <summary>
		/// The routine that transitions to the battle without the harsh transition effect.
		/// </summary>
		/// <param name="playerCamera"></param>
		/// <returns></returns>
		public IEnumerator SoftTransitionToBattle(Camera playerCamera, float delayTime) {

			// Disable the brain on the player's camera so that it doesn't transition tot he battle.
			playerCamera.GetComponent<CinemachineBrain>().enabled = false;

			// Disable any virtual cameras that may still be active from the last battle.
			this.DisableVirtualCameras(this.activatedVirtualCameras);

			// Turn on the battle camera and transition.
			this.cameraParent.SetActive(true);

			// Immediately turn off the battle start virtual cam before turning on the parent. Just in case.
			this.battleStartVirtualCamera.gameObject.SetActive(false);

			// Reposition the soft transition cam to the location of the player cam.
			this.softBattleStartVirtualCamera.transform.SetPositionAndRotation(
				position: playerCamera.transform.position,
				rotation: playerCamera.transform.rotation);

			// Turn on the virtual cameras
			this.virtualCamerasParent.SetActive(true);

			this.MainCamera.gameObject.SetActive(true);

			// Wait a frame.
			yield return new WaitForEndOfFrame();

			// Turn the soft camera off.
			this.softBattleStartVirtualCamera.gameObject.SetActive(false);

			// Wait for the amount of time specified.
			yield return new WaitForSeconds(delayTime);

		}
		/// <summary>
		/// Swaps the cameras and resets to the normal view, while playing the Page Turn Transition.
		/// </summary>
		public void PageTurnTransition() {
			this.SwapCameras();
			this.cameraTransition.DoTransition(CameraTransitionEffects.PageCurl, this.SecondaryCamera, this.MainCamera, 0.2f);
			this.DisableVirtualCameras(this.activatedVirtualCameras);
			this.UpdateCinemachineTargetGroup(worldEnemies: BattleArena.BattleArenaControllerDX.instance.ActiveWorldEnemyDXs);
			// this.UpdateCinemachineTargetGroup(BattleController.Instance.worldEnemies);
		}
		/// <summary>
		/// Gets called from the DungeonController's own BackToDungeon routine and disables the battle camera.
		/// </summary>
		public void BackToDungeon() {
			// Turn the player's brain back on.
			Dungeon.DungeonPlayer.playerCamera.GetComponent<CinemachineBrain>().enabled = true;
			Dungeon.DungeonPlayer.Instance.ResetEffect();
			// Clear out the list of virtual cameras. If any get referenced in a new battle, it could crash.
			this.DisableVirtualCameras(this.activatedVirtualCameras);

			this.cameraParent.SetActive(false);
			this.virtualCamerasParent.SetActive(false);
			// The parent will be disabled but this should be enabled.
			this.battleStartVirtualCamera.gameObject.SetActive(true);
		}
		/// <summary>
		/// Sort of similar to BackToDungeon but specifically for the gauntlet.
		/// </summary>
		public void BackToGauntlet() {

			
			// Gauntlet.Legacy.LegacyBattleGauntletLevelSelectController.Instance.mainCam.GetComponent<CinemachineBrain>().enabled = true;
			GauntletController.instance.MainCamera.GetComponent<CinemachineBrain>().enabled = true;

			// Clear out the list of virtual cameras. If any get referenced in a new battle, it could crash.
			this.DisableVirtualCameras(this.activatedVirtualCameras);

			this.cameraParent.SetActive(false);
			this.virtualCamerasParent.SetActive(false);
			// The parent will be disabled but this should be enabled.
			this.battleStartVirtualCamera.gameObject.SetActive(true);
			this.softBattleStartVirtualCamera.gameObject.SetActive(true);
		}
		/// <summary>
		/// The routine to run when going back to the dungeon crawler.
		/// </summary>
		public void BackToCrawler() {
			CrawlerPlayer.Instance.PlayerCamera.GetComponent<CinemachineBrain>().enabled = true;
			// Clear out the list of virtual cameras. If any get referenced in a new battle, it could crash.
			this.DisableVirtualCameras(this.activatedVirtualCameras);
			
			this.cameraParent.SetActive(false);
			this.virtualCamerasParent.SetActive(false);
			// The parent will be disabled but this should be enabled.
			this.battleStartVirtualCamera.gameObject.SetActive(true);
		}
		/// <summary>
		/// Gets called when going back to the battle test menu.
		/// Wow I really am stacking this shit on top of each other huh.
		/// </summary>
		public void BackToBattleTestMenu() {
			BattleTestController.Instance.MenuCamera.GetComponent<CinemachineBrain>().enabled = true;
			// Clear out the list of virtual cameras. If any get referenced in a new battle, it could crash.
			this.DisableVirtualCameras(this.activatedVirtualCameras);
			
			this.cameraParent.SetActive(false);
			this.virtualCamerasParent.SetActive(false);
			// The parent will be disabled but this should be enabled.
			this.battleStartVirtualCamera.gameObject.SetActive(true);
		}
		#endregion

		/// <summary>
		/// The different types of cameras that I may want to activate.
		/// </summary>
		public enum BattleCameraType {
			NormalCamera = 0,       // The camera to be used for the position at the start of a player's turn.
			HeadOnCamera = 1,		// The camera for looking "head on" at multiple targets.
			DynamicHeadOn = 2,		// Has severe dutch/angle. Mostly for all out attacks.
			OrbitEnemies = 3,		// Orbits enemies. 
			BattleStart = 4,		// Where the camrea begins when its making its sweep at the start of the battle.
		}

	}

}