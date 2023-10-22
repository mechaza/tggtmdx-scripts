using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Battle.Modifiers.Afflictions;
using Grawly.UI;
using UnityEngine.Serialization;
using UnityStandardAssets.ImageEffects;

namespace Grawly.DungeonCrawler {
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(Collider))]
	public class CrawlerPlayer : MonoBehaviour {
		public static CrawlerPlayer Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The number of steps the player has taken.
		/// </summary>
		public int Steps { get; private set; } = 0;
		/// <summary>
		/// The current state of the crawler player.
		/// </summary>
		public CrawlerPlayerState PlayerState { get; private set; } = CrawlerPlayerState.Free;
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// Is this character in the middle of moving?
		/// </summary>
		public bool IsTweening {
			get { return DOTween.IsTweening(this.transform); }
		}
		/// <summary>
		/// Is this controller locked?
		/// If locked, input is blocked.
		/// </summary>
		public bool IsLocked {
			get {
				// If set to wait, or is currently tweening, input is locked.
				return this.PlayerState == CrawlerPlayerState.Wait || this.IsTweening;
			}
		}
		#endregion

		#region FIELDS - TOGGLES : MODE
		/// <summary>
		/// The current 'mode' for determining tile size.
		/// Tile size can be defined in multiple places, so this sets where that is.
		/// </summary>
		[Title("Mode")]
		[SerializeField, TabGroup("Player", "Toggles"), Tooltip("The current 'mode' for determining tile size. Tile size can be defined in multiple places, so this sets where that is.")]
		private CrawlerTileModeType tileModeType = CrawlerTileModeType.None;
		#endregion

		#region FIELDS - TOGGLES : TILES
		/// <summary>
		/// The tile size to use if overriding.
		/// </summary>
		[Title("Tiles")]
		[SerializeField, TabGroup("Player", "Toggles"), ShowIf("UsingTileSizeOverride")]
		private int tileSizeOverride = 3;
		/// <summary>
		/// The length of a tile.
		/// </summary>
		public int TileSize {
			get {
				switch (this.tileModeType) {
					case CrawlerTileModeType.None:
						// This probably will never get used.
						throw new NotImplementedException("This should theoretically never get used.");
					case CrawlerTileModeType.Player:
						// If set to player, use this override.
						return this.tileSizeOverride;
					case CrawlerTileModeType.Controller:
						// If set to controller, probe the crawler controller.
						throw new NotImplementedException("Grab the tile size from the crawler controller!");
					case CrawlerTileModeType.Dungeon:
						// If set to dungeon, probe the runtime crawler dungeon.
						throw new NotImplementedException("Grab the tile size from the crawler dungeon!");
					// return RuntimeCrawlerDungeonDX.Instance.TileSize;
					default:
						throw new System.Exception("Couldn't determine tile size!");
				}
			}
		}
		#endregion

		#region FIELDS - MOVEMENT
		/// <summary>
		/// The speed at which to walk.
		/// </summary>
		[Title("Movement")]
		[SerializeField, TabGroup("Player", "Toggles")]
		private float walkTime = 0.2f;
		/// <summary>
		/// The speed at which to run.
		/// </summary>
		private float runTime = 0.1f;
		/// <summary>
		/// The speed at which to turn.
		/// </summary>
		[SerializeField, TabGroup("Player", "Toggles")]
		private float turnTime = 0.2f;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The camera attached to the player.
		/// </summary>
		[SerializeField, TabGroup("Player", "Scene References")]
		private Camera playerCamera;
		/// <summary>
		/// The camera attached to the player.
		/// </summary>
		public Camera PlayerCamera {
			get { return this.playerCamera; }
		}
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// Is the tile size override being used?
		/// Is true only if grid positioning is not being used and the override is off.
		/// </summary>
		private bool UsingTileSizeOverride => this.tileModeType == CrawlerTileModeType.Player;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			// Add the player step event when this script is loaded.
			CrawlerController.Instance.PlayerStepEvent.AddListener(this.OnPlayerStep);
		}
		private void Update() {
			// If not locked and the pause button is pressed,
			if (this.IsLocked == false && InputController.Instance.GetButtonDown("Pause")) {
				// Set the player to wait.
				this.SetState(CrawlerPlayerState.Wait);
				// Open the pause controller. 
				PauseMenuController.Open(onExitCallback: delegate {
					// Upon exit, free up.
					this.SetState(CrawlerPlayerState.Free);
				});
			}

			// Generate player input.
			CrawlerInput p = new CrawlerInput();

			// If not in the middle of a tween, move.
			if (this.IsLocked == false && p.HasAnyInput) {
				this.ProcessInput(p);
			}
		}
		private void OnDestroy() {
			// Upon destruction of this component, remove it from the unity events in the crawler controller/dungeon.
			CrawlerController.Instance?.PlayerStepEvent.RemoveListener(this.OnPlayerStep);
		}
		#endregion

		#region STATE MANIPULATION
		/// <summary>
		/// Sets the state of the player.
		/// </summary>
		/// <param name="state"></param>
		public void SetState(CrawlerPlayerState state) {
			this.PlayerState = state;
		}
		#endregion

		#region MOVEMENT : PLAYER INPUT
		
		/// <summary>
		/// The routine that should move the crawler.
		/// </summary>
		/// <param name="p"></param>
		private void ProcessInput(CrawlerInput p) {
			if (p.HasActionInput) {
				this.Interact(p);
				return;
			}

			// If a turn is requested, do that.
			if (p.HasTurnInput) {
				this.Turn(p);
				return;
			}

			if (p.HasStrafeInput || p.HasWalkInput) {
				this.Move(p);
				return;
			}
		}
		/// <summary>
		/// Handles situations where the player needs to interact with whatever is in front of them.
		/// </summary>
		/// <param name="p">The input for this frame.</param>
		private void Interact(CrawlerInput p) {
			Debug.Assert(p.HasActionInput);

			// this.GetInteractableTiles().ForEach(t => t.OnInteract());
			this.GetForwardTiles<IPlayerInteractHandler>()
				.ForEach(t => t.OnInteract(
					crawlerProgressionSet: CrawlerController.Instance.CurrentProgressionSet, 
					floorNumber: CrawlerController.Instance.CurrentFloorNumber));
		}
		/// <summary>
		/// The actual routine to handle movement.
		/// </summary>
		/// <param name="p"></param>
		private void Move(CrawlerInput p) {
			
			// Assert that there is strafe/walk input.
			Debug.Assert(p.HasStrafeInput || p.HasWalkInput);

			// ...get the new target and move.
			Vector3 targetPos;
			if (p.HasStrafeInput) {
				targetPos = this.GetStrafeTargetPosition(sourceTransform: this.transform, p: p, tileSize: this.TileSize);
			} else {
				targetPos = this.GetWalkTargetPosition(sourceTransform: this.transform, p: p, tileSize: this.TileSize);
			}

			// If able to move to the given position, do so.
			if (this.CanMoveToPosition(sourceTransform: this.transform, targetPos: targetPos, tileSize: this.TileSize) == true) {
				// Make sure to call the look away tiles before doing so.
				this.GetForwardTiles<IPlayerLookAwayHandler>().ForEach(t => t.OnLookAway());

				// Determine the duration of the movement.
				float duration = (p.RunInput == true) ? this.runTime : this.walkTime;

				// Perform the move operation. When done, run OnStep().
				this.transform.DOMove(endValue: targetPos, duration: duration).SetEase(Ease.Linear).OnComplete(() => {
					CrawlerController.Instance.PlayerStepEvent.Invoke();
				});
			}
		}
		/// <summary>
		/// The function for turning.
		/// </summary>
		/// <param name="p"></param>
		private void Turn(CrawlerInput p) {
			// Assert that the orientation is either left or right.
			Debug.Assert(condition: p.HasTurnInput);

			// Call the look away tiles and run those.
			this.GetForwardTiles<IPlayerLookAwayHandler>().ForEach(t => t.OnLookAway());

			// Get the rotation direction.
			Vector3 rotateDir = (p.TurnOrientation == CrawlerOrientation.Left) ? new Vector3(0f, -90f, 0f) : new Vector3(0f, 90f, 0f);

			// Rotate it. When done, run OnTurn().
			this.transform.DOLocalRotate(endValue: rotateDir, duration: this.turnTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).OnComplete(() => {
				this.OnTurnComplete();
			});
		}
		#endregion

		#region MOVEMENT : TELEPORTATION
		/// <summary>
		/// Teleports the crawler player to the specified position/rotation.
		/// </summary>
		/// <param name="targetPos"></param>
		/// <param name="targetRot"></param>
		/// <param name="instantaneous"></param>
		private void Teleport(Vector3 targetPos, Quaternion targetRot, bool instantaneous) {
			// If not instantaneous, fade out.
			if (instantaneous == false) {
				throw new NotImplementedException("Add implementation for fading teleports!");
			} else {
				this.transform.SetPositionAndRotation(position: targetPos, rotation: targetRot);
			}
		}
		/// <summary>
		/// Teleports the crawler player to the specified transform.
		/// </summary>
		/// <param name="targetAnchor">The transform to teleport to.</param>
		/// <param name="heightOffset">The amount to offset the y-coordinate by..</param>
		/// <param name="instantaneous">Should this happen instantly?</param>
		public void Teleport(Transform targetAnchor, float heightOffset, bool instantaneous = false) {
			this.Teleport(
				targetPos: targetAnchor.position + new Vector3(0, heightOffset, 0), 
				targetRot: targetAnchor.rotation,
				instantaneous: instantaneous);
		}
		/// <summary>
		/// Teleports the crawler player to the specified transform.
		/// </summary>
		/// <param name="targetAnchor">The transform to teleport to.</param>
		/// <param name="instantaneous">Should this happen instantly?</param>
		public void Teleport(Transform targetAnchor, bool instantaneous = false) {
			this.Teleport(
				targetPos: targetAnchor.position, 
				targetRot: targetAnchor.rotation,
				instantaneous: instantaneous);
		}
		/// <summary>
		/// Teleports the crawler player to the specified position.
		/// </summary>
		/// <param name="targetPos">The position to teleport to.</param>
		/// <param name="instantaneous">Should this happen instantly?</param>
		public void Teleport(Vector3 targetPos, bool instantaneous = false) {
			// Teleport to the target position while using the rotation currently held by the crawler player.
			this.Teleport(
				targetPos: targetPos, 
				targetRot: this.transform.rotation,
				instantaneous: instantaneous);
		}
		/// <summary>
		/// Teleports the crawler player to the specified position.
		/// </summary>
		/// <param name="targetPos">The position to teleport to.</param>
		/// <param name="heightOffset">The amount to offset the y-coordinate by.</param>
		/// <param name="instantaneous">Should this happen instantly?</param>
		public void Teleport(Vector3 targetPos, float heightOffset, bool instantaneous = false) {
			// Teleport to the target position while using the rotation currently held by the crawler player.
			this.Teleport(
				targetPos: targetPos + new Vector3(0, heightOffset, 0), 
				targetRot: this.transform.rotation,
				instantaneous: instantaneous);
		}
		#endregion
		
		#region EVENTS - STEP
		/// <summary>
		/// Gets called when the dungeon craweler player steps.
		/// </summary>
		public void OnPlayerStep() {
			// Increment the steps.
			this.Steps += 1;

			// Go through each of the stepped tiles (usually just one) and perform the OnStep().
			this.GetSteppedTiles()
				.ForEach(t => t.OnLand(
					crawlerProgressionSet: CrawlerController.Instance.CurrentProgressionSet, 
					floorNumber: CrawlerController.Instance.CurrentFloorNumber));
			this.GetForwardTiles<IPlayerApproachHandler>()
				.ForEach(t => t.OnApproach(
					crawlerProgressionSet: CrawlerController.Instance.CurrentProgressionSet,
					floorNumber: CrawlerController.Instance.CurrentFloorNumber));
		}
		/// <summary>
		/// Gets called when the player turns.
		/// </summary>
		private void OnTurnComplete() {
			// this.GetApproachTiles().ForEach(t => t.OnApproach());
			this.GetForwardTiles<IPlayerApproachHandler>()
				.ForEach(t => t.OnApproach(
					crawlerProgressionSet: CrawlerController.Instance.CurrentProgressionSet,
					floorNumber: CrawlerController.Instance.CurrentFloorNumber));
		}
		#endregion

		#region CALCULATIONS - INTERACTABLES
		/// <summary>
		/// Gets any overlapping CrawlerComponents the player may be standing in.
		/// </summary>
		/// <typeparam name="T">The kind of CrawlerComponent to check for.</typeparam>
		/// <returns></returns>
		public List<T> GetOverlappingTiles<T>() where T : ICrawlerComponent {
			return Physics.OverlapBox(
					center: this.transform.position, 
					halfExtents: (this.transform.localScale * (this.TileSize * 0.9f)) / 2f, 
					orientation: Quaternion.identity)
				.SelectMany(c => c.gameObject.GetComponents<T>())
				.ToList();
		}
		/// <summary>
		/// Returns a list of floor tiles that were just stepped on.
		/// </summary>
		/// <returns></returns>
		public List<IPlayerLandHandler> GetSteppedTiles() {
			return Physics.OverlapBox(
				center: this.transform.position, 
				halfExtents: (this.transform.localScale * (this.TileSize * 0.9f)) / 2f, 
				orientation: Quaternion.identity)
				.SelectMany(c => c.gameObject.GetComponents<IPlayerLandHandler>())
				.ToList();
		}
		/// <summary>
		/// Returns a list of the tiles in front of the player that implement the given interface.
		/// </summary>
		/// <typeparam name="T">The type of component the tiles should implement.</typeparam>
		/// <returns>A list of tiles in front of the player that implement the given interface.</returns>
		private List<T> GetForwardTiles<T>() where T : ICrawlerComponent {
			
			Vector3 targetPosition = this.GetForwardTilePosition(
				sourceTransform: this.transform, 
				tileSize: this.TileSize);
			
			return Physics.OverlapSphere(
				position: targetPosition, 
				radius: 0.1f)
				.SelectMany(c => c.gameObject.GetComponents<T>())
				.ToList();
			
		}
		/// <summary>
		/// Gets all the tiles surrounding the player.
		/// </summary>
		/// <param name="includeCurentPosition">Should the tile the player is currently standing on also be included?</param>
		/// <typeparam name="T">The kind of tiles to look for.</typeparam>
		/// <returns>The tiles surrounding the player.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public List<T> GetSurroundingTiles<T>(bool includeCurentPosition = true) where T : ICrawlerComponent {
			
			// Create a list of directions to check.
			List<Vector3> cardinalDirections = new List<Vector3>() {
				new Vector3(x: 1f, y: 0f, z: 0f),		// Right
				new Vector3(x: 1f, y: 0f, z: 1f),		// Up-Right
				new Vector3(x: 0f, y: 0f, z: 1f),		// Up
				new Vector3(x: -1f, y: 0f, z: 1f),		// Up-Left
				new Vector3(x: -1f, y: 0f, z: 0f),		// Left
				new Vector3(x: -1f, y: 0f, z: -1f),		// Down-Left
				new Vector3(x: 0f, y: 0f, z: -1f),		// Down
				new Vector3(x: 1f, y: 0f, z: -1f),		// Down-Right
			};
			
			// If including the current position, add an empty vector.
			if (includeCurentPosition == true) {
				cardinalDirections.Add(new Vector3(x: 0, y: 0, z: 0));
			}

			// Convert those directions into positions to check against.
			List<Vector3> targetPositions = cardinalDirections
				.Select(d => this.GetSurroundingTilePosition(
					sourceTransform: this.transform,
					direction: d, 
					tileSize: this.TileSize))
				.ToList();

			return targetPositions														// Go through each of the target positions...
				.SelectMany(tp => Physics.OverlapSphere(position: tp, radius: 0.1f))	// ...grab all the colliders at those points...
				.SelectMany(c => c.gameObject.GetComponents<T>())						// ...then grab the appropriate components.
				.ToList();
			
		}
		#endregion

		#region CALCULATIONS - POSITIONING
		/// <summary>
		/// Gets the position of the tile in the direction specified from the source transform.
		/// </summary>
		/// <param name="sourceTransform">The source transform.</param>
		/// <param name="direction">The direction in which to check.</param>
		/// <param name="tileSize">The size of the tiles in the map.</param>
		/// <returns>The tile located in the direction specified.</returns>
		private Vector3 GetSurroundingTilePosition(Transform sourceTransform, Vector3 direction, float tileSize) {
			Vector3 currentPosition = sourceTransform.position;
			// Vector3 normalizedDirection = direction.normalized;
			// Vector3 targetPosition = currentPosition + (normalizedDirection * (tileSize * 0.6f));
			Vector3 targetPosition = currentPosition + (direction * (tileSize * 0.6f));
			return targetPosition;
		}
		/// <summary>
		/// Gets the position of the tile in front of the source transform.
		/// </summary>
		/// <param name="sourceTransform">The source transform.</param>
		/// <param name="tileSize">The size of the tiles in the map.</param>
		/// <returns>The center of the tile in front of the transform.</returns>
		private Vector3 GetForwardTilePosition(Transform sourceTransform, float tileSize) {
			/*Vector3 currentPosition = sourceTransform.position;
			Vector3 forwardDirection = sourceTransform.forward.normalized;
			Vector3 targetPosition = currentPosition + (forwardDirection * (tileSize * 0.6f));
			return targetPosition;*/
			return this.GetSurroundingTilePosition(
				sourceTransform: sourceTransform, 
				direction: sourceTransform.forward, 
				tileSize: tileSize);
		}
		/// <summary>
		/// Gets the target position when given a specified movement type.
		/// </summary>
		/// <param name="sourceTransform">The transform who is going to be walking.</param>
		/// <param name="p">The input for this frame.</param>
		/// <param name="tileSize">The size of the tiles on the grid.</param>
		/// <returns>The postion the target is going to walk to.</returns>
		private Vector3 GetWalkTargetPosition(Transform sourceTransform, CrawlerInput p, float tileSize) {
			// Assert that there is walk input.
			Debug.Assert(condition: p.HasWalkInput);

			// Grab the current position and forward direction.
			Vector3 currentPosition = sourceTransform.position;
			Vector3 forwardDirection = sourceTransform.forward.normalized;

			// If the back input is hit, negate the forward direction.
			if (p.BackInput == true) {
				forwardDirection = -forwardDirection;
			}

			// Return the current position plus all that jazz.
			return currentPosition + (forwardDirection * tileSize);
		}
		/// <summary>
		/// Gets the target position when given a specified movement type.
		/// </summary>
		/// <param name="sourceTransform">The transform who is going to be strafing.</param>
		/// <param name="p">The input for this frame.</param>
		/// <param name="tileSize">The size of the tiles on the grid.</param>
		/// <returns>The postion the target is going to strafe to.</returns>
		private Vector3 GetStrafeTargetPosition(Transform sourceTransform, CrawlerInput p, float tileSize) {
			// Assert that there is strafe input.
			Debug.Assert(condition: p.HasStrafeInput);

			// Grab the current position and forward direction.
			Vector3 currentPosition = sourceTransform.position;

			// ...determine the direction to strafe...
			float strafeDir = (p.StrafeOrientation == CrawlerOrientation.Left) ? -1f : 1f;

			// ...and return the updated postiion.
			return currentPosition + (sourceTransform.right * (strafeDir * tileSize));
		}
		#endregion

		#region STATE CHECKS
		/// <summary>
		/// Whether or not a transform can move to a given target position.
		/// </summary>
		/// <param name="sourceTransform">The transform who needs to move.</param>
		/// <param name="targetPos">The position the transform wants to move to.</param>
		/// <param name="tileSize">The size of the tiles on the grid.</param>
		/// <returns>Whether or not the transform can move to the target, unobstructed.</returns>
		private bool CanMoveToPosition(Transform sourceTransform, Vector3 targetPos, float tileSize) {
			Vector3 dir = (targetPos - sourceTransform.position).normalized;
			RaycastHit hitInfo;
			Physics.Raycast(origin: sourceTransform.position, direction: dir, maxDistance: tileSize, hitInfo: out hitInfo);

			// If there was nothing hit, return true.
			if (hitInfo.collider == null) {
				return true;
				// If there is a trigger, return true.
			} else if (hitInfo.collider.isTrigger == true) {
				return true;
				// Otherwise, return false.
			} else {
				return false;
			}
		}
		#endregion

		#region EFFECTS
		/// <summary>
		/// A (potentially) debug method for playing the effect that blurs the player camera when they hit an enemy in a dungeon.
		/// </summary>
		public IEnumerator PlayBattleTransitionEffectRoutine() {
			// Blur the camera.
			playerCamera.GetComponent<BlurOptimized>().enabled = true;
			DOTween.To(getter: () => playerCamera.GetComponent<BlurOptimized>().blurSize, setter: x => playerCamera.GetComponent<BlurOptimized>().blurSize = x, endValue: 3f, duration: 1f);
			yield return new WaitForSeconds(4f);
			playerCamera.GetComponent<BlurOptimized>().blurSize = 0f;
			playerCamera.GetComponent<BlurOptimized>().enabled = false;
		}
		#endregion
	}

	/// <summary>
	/// The input the player gives on any given frame.
	/// </summary>
	public readonly struct CrawlerInput {
		#region FIELDS - STATE
		public bool ForwardInput => InputController.Instance.GetAxisRawPositive("Vertical");
		public bool BackInput => InputController.Instance.GetAxisRawNegative("Vertical");
		public bool StrafeLeftInput => InputController.Instance.GetAxisRawNegative("Strafe");
		public bool StrafeRightInput => InputController.Instance.GetAxisRawPositive("Strafe");
		public bool TurnLeftInput => InputController.Instance.GetAxisRawNegative("Horizontal");
		public bool TurnRightInput => InputController.Instance.GetAxisRawPositive("Horizontal");
		public bool RunInput => InputController.Instance.GetButton("Sprint");
		public bool ActionInput => InputController.Instance.GetButtonDown("Submit");
		/*public bool StrafeLeftInput => InputController.instance.GetButton("Walk Left");
		public bool StrafeRightInput => InputController.instance.GetButton("Walk Right");*/
		#endregion

		#region PROPERTIES - FLAGS
		/// <summary>
		/// Is an action being attempted?
		/// </summary>
		public bool HasActionInput {
			get { return this.ActionInput; }
		}
		/// <summary>
		/// Is a strafe being attempted?
		/// </summary>
		public bool HasStrafeInput {
			get {
				// Are either of the strafe keys hit?
				return this.StrafeLeftInput || this.StrafeRightInput;
			}
		}
		/// <summary>
		/// Is a turn being attempted?
		/// </summary>
		public bool HasTurnInput {
			get { return this.TurnLeftInput || this.TurnRightInput; }
		}
		/// <summary>
		/// Is this player trying to move to a new tile?
		/// </summary>
		public bool HasWalkInput {
			get { return this.ForwardInput == true || this.BackInput == true; }
		}
		/// <summary>
		/// Was any input received?
		/// </summary>
		public bool HasAnyInput {
			get { return this.HasStrafeInput || this.HasTurnInput || this.HasWalkInput || this.HasActionInput; }
		}
		#endregion

		#region PROPERTIES - ORIENTATION
		/// <summary>
		/// The "best" strafe orientation for this input.
		/// </summary>
		public CrawlerOrientation StrafeOrientation {
			get {
				if (this.StrafeLeftInput) {
					return CrawlerOrientation.Left;
				}

				if (this.StrafeRightInput) {
					return CrawlerOrientation.Right;
				}

				return CrawlerOrientation.None;
			}
		}
		/// <summary>
		/// The "best" walk orientation for this input.
		/// </summary>
		public CrawlerOrientation WalkOrientation {
			get {
				if (this.ForwardInput) {
					return CrawlerOrientation.Forward;
				}

				if (this.BackInput) {
					return CrawlerOrientation.Back;
				}

				return CrawlerOrientation.None;
			}
		}
		/// <summary>
		/// The "best" turn orientation for this input.
		/// </summary>
		public CrawlerOrientation TurnOrientation {
			get {
				if (this.TurnLeftInput) {
					return CrawlerOrientation.Left;
				}

				if (this.TurnRightInput) {
					return CrawlerOrientation.Right;
				}

				return CrawlerOrientation.None;
			}
		}
		#endregion
	}

	public enum CrawlerOrientation {
		None = 0,
		Forward = 1,
		Right = 2,
		Back = 3,
		Left = 4,
	}

	/// <summary>
	/// The different states for the crawler player.
	/// </summary>
	public enum CrawlerPlayerState {
		Wait = 0,
		Free = 1,
	}
}