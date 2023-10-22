using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Grawly {

	/// <summary>
	/// A way to access the input given by a player. Useful so I can control how it's accessed and also use Rewired to do so.
	/// </summary>
	public class InputController : MonoBehaviour {

		public static InputController Instance { get; private set; }

		#region FIELDS
		/// <summary>
		/// The Reiwred Player that is used for input.
		/// </summary>
		private Rewired.Player rewiredPlayer;
		#endregion

		#region UNITY FUNCTIONS
		private void Awake() {
			if (Instance == null) {
				// Set the Instance so the input controller can be used as a singleton.
				Instance = this;
				DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			// If the RewiredPlayer hasn't been set, grab the first player.
			if (this.rewiredPlayer == null) {
				this.rewiredPlayer = Rewired.ReInput.players.GetPlayer(0);
			}
		}
		#endregion

		#region GETTING INPUT
		/// <summary>
		/// Gets whether or not the specified action has had its button pressed down.
		/// </summary>
		/// <param name="actionName">The name of the action to grab.</param>
		/// <returns></returns>
		public bool GetButton(string actionName) {
			return this.rewiredPlayer.GetButton(actionName: actionName);
		}
		/// <summary>
		/// Gets whether or not the specified action has had its button pressed down.
		/// </summary>
		/// <param name="actionName">The name of the action to grab.</param>
		/// <returns></returns>
		public bool GetButtonDown(string actionName) {
			return this.rewiredPlayer.GetButtonDown(actionName: actionName);
		}
		/// <summary>
		/// Gets whether or not the specified action has had its button released.
		/// </summary>
		/// <param name="actionName">The name of the action to grab.</param>
		/// <returns></returns>
		public bool GetButtonUp(string actionName) {
			return this.rewiredPlayer.GetButtonUp(actionName: actionName);
		}
		/// <summary>
		/// Gets the axis of the specified action.
		/// </summary>
		/// <param name="actionName">The name of the action to grab.</param>
		/// <returns></returns>
		public float GetAxis(string actionName) {
			return this.rewiredPlayer.GetAxis(actionName: actionName);
		}
		/// <summary>
		/// Gets whether or not an axis is being pressed in the positive direction.
		/// </summary>
		/// <param name="actionName">The name of the axis.</param>
		/// <param name="threshold">The amount to check.</param>
		/// <returns></returns>
		public bool GetAxisPositive(string actionName, float threshold = 0.5f) {
			return this.GetAxis(actionName) > threshold;
		}
		/// <summary>
		/// Gets whether or not an axis is being pressed in the negative direction.
		/// </summary>
		/// <param name="actionName">The name of the axis.</param>
		/// <param name="threshold">The amount to check.</param>
		/// <returns></returns>
		public bool GetAxisNegative(string actionName, float threshold = 0.5f) {
			return this.GetAxis(actionName)  < -threshold;
		}
		/// <summary>
		/// Gets the raw axis of the specified action.
		/// </summary>
		/// <param name="actionName">The name of the action to grab.</param>
		/// <returns></returns>
		public float GetAxisRaw(string actionName) {
			return this.rewiredPlayer.GetAxisRaw(actionName: actionName);
		}
		/// <summary>
		/// Gets the raw axis of the specified action and checks if it's above the threshold.
		/// </summary>
		/// <param name="actionName"></param>
		/// <param name="threshold"></param>
		/// <returns></returns>
		public bool GetAxisRawPositive(string actionName, float threshold = 0.5f) {
			return this.GetAxisRaw(actionName) > threshold;
		}
		/// <summary>
		/// Gets the raw axis of the specified action and checks if it's below the threshold.
		/// </summary>
		/// <param name="actionName"></param>
		/// <param name="threshold"></param>
		/// <returns></returns>
		public bool GetAxisRawNegative(string actionName, float threshold = 0.5f) {
			return this.GetAxisRaw(actionName) < -threshold;
		}
		/// <summary>
		/// Gets whether or not the specified button has been double pressed.
		/// </summary>
		/// <param name="actionName">The name of the action to grab.</param>
		/// <returns></returns>
		public bool GetDoubleButtonDown(string actionName, float inputTime = 1.0f) {
			return this.rewiredPlayer.GetButtonDoublePressDown(actionName: actionName, speed: inputTime);
		}
		#endregion

		
	}


}