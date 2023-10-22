using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Grawly.Overworld {

	public class OverworldCursor2D : MonoBehaviour {

		public static OverworldCursor2D instance;

		// Helps me to maintain how certain animations play out when raycasting.
		// E.x., Entering new node, leaving node, etc.
		[HideInInspector]
		public ICursorInteractable currentInteractable;
		private ICursorInteractable previousInteractable;

		[Header("Visuals")]
		[SerializeField]
		private SpriteRenderer innerSprite;
		[SerializeField]
		private SpriteRenderer outerSprite;

		[Space(5)]
		[Header("Config")]
		[SerializeField]
		private float speed = 1f;
		[SerializeField]
		private float maxSplit = .4f;
		[SerializeField]
		private CharacterController characterController;

		private void Awake() {
			instance = this;
		}
		private void Update() {
			// Find out if the cursor is hovering over anything and handle situations where it is/is not/etc.
			CursorRaycast();
			// Figure out if the cursor is being moved or not.
			if (GetInputDirection().magnitude > 0.01f) {
				// If it is, move it as normal.
				MoveCursor();
			} else if (currentInteractable != null) {
				// If its not, see if you can orbit to the current interactable (if its not null)
				GravitateToInteractable(currentInteractable);
			} else {
				// innerSprite.transform.localScale = Vector3.one;
			}

			// I need to adjust the inner sprite in here because of how I handle when and where MoveCursor is called.
			innerSprite.transform.localPosition = (GetInputDirection() * Mathf.Clamp01(GetInputDirection().magnitude) * maxSplit);

			if (Input.GetButtonDown("Submit")) {
				Debug.Log("Opening node: " + currentInteractable.GetInteractableName());
				OpenInteractable(currentInteractable);
			} else if (Input.GetButtonDown("Cancel")) {
				
			}
		}

		#region MOVEMENT
		/// <summary>
		/// Moves the cursor based on the input recieved.
		/// </summary>
		private void MoveCursor() {
			// Kill any tweens that were in progress.
			transform.DOKill();

			float x = Input.GetAxis("Horizontal");
			float y = Input.GetAxis("Vertical");
			Vector3 dir = new Vector3(x, y);
			float mag = Mathf.Clamp01(dir.magnitude);   // Clamp the mag between 0 and 1 because if both x and y are 1, the mag might be something like 1.41
			dir.Normalize();

			// Move the inner sprite
			// innerSprite.transform.localPosition = (dir * mag * maxSplit);
			// Move the cursor
			characterController.Move(dir * Time.deltaTime * speed * mag);
			// transform.Translate(dir * Time.deltaTime * speed * mag);
		}
		/// <summary>
		/// Gets the magnitude of the input.
		/// </summary>
		/// <returns></returns>
		private Vector3 GetInputDirection() {
			float x = Input.GetAxis("Horizontal");
			float y = Input.GetAxis("Vertical");
			Vector3 dir = new Vector3(x, y);
			// return dir.magnitude;
			return dir;
		}
		/// <summary>
		/// Makes the cursor gravitate towards a given interactable.
		/// </summary>
		/// <param name="interactable"></param>
		private void GravitateToInteractable(ICursorInteractable interactable) {
			Vector2 p1 = interactable.GetGravitationPoint();
			Vector3 gravPoint = new Vector3(p1.x, p1.y, transform.position.z);
			transform.DOMove(gravPoint, 0.5f);
		}
		#endregion

		/// <summary>
		/// Tweaks the status of the overworld cursor.
		/// </summary>
		/// <param name="status"></param>
		public void Active(bool status) {
			gameObject.SetActive(status);
		}
		/// <summary>
		/// Processes events such as entering new node, leave old node, etc. 
		/// Raycasts from the cursor to determine if a new cursor has been hit. 
		/// </summary>
		/// <returns></returns>
		private void CursorRaycast() {
			// Move the current node into the previous node.
			previousInteractable = currentInteractable;
			// Start the raycast.
			RaycastHit hit;
			Ray ray = new Ray(this.transform.position, Vector3.forward);
			// Ray ray = Camera.main.ViewportPointToRay(this.transform.position);
			// Ray ray = Camera.main.ScreenPointToRay(this.transform.position);
			// If a node was hit, assign it to current node. If not, it has to be null.
			if (Physics.Raycast(ray, out hit, 50f)) {
				if (hit.transform.GetComponent<ICursorInteractable>() != null) {
					currentInteractable = hit.transform.GetComponent<ICursorInteractable>();
				}
			} else {
				currentInteractable = null;
			}

			// If the current node is not null and the previous node is not the same as the current,
			// that means the cursor has entered a new node.
			if (currentInteractable != null && previousInteractable != currentInteractable) {
				Debug.Log("Entering node: " + currentInteractable.GetInteractableName());
				currentInteractable.OnCursorEnter();
			} else if (previousInteractable != null && currentInteractable == null) {
				Debug.Log("Leaving node: " + previousInteractable.GetInteractableName());
				previousInteractable.OnCursorExit();
			} else if (previousInteractable == null && currentInteractable == null) {
				// Cursor just floating around.
			} else if (previousInteractable != null && previousInteractable == currentInteractable) {
				// Cursor just hanging around inside the same node.
			} else {
				Debug.LogError("I theoretically should never reach this state.");
			}

			Debug.DrawRay(ray.origin, ray.direction * 50, Color.yellow);
		}
		/// <summary>
		/// Sends a signal to the OverworldNode to open up.
		/// </summary>
		private void OpenInteractable(ICursorInteractable interactable) {
			interactable.OnCursorSubmit();
		}

	}


}