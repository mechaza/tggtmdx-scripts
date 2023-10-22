using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon.Legacy {

	/// <summary>
	/// The field of vision collider for the enemy. If a DungeonPlayer steps inside it, they'll react.
	/// </summary>
	public class DungeonEnemyFOV : MonoBehaviour, IPlayerCollisionHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time that it shoudl take for this enemy to go docile if the player exits the collider.
		/// </summary>
		[SerializeField]
		private float timeUntilDocile = 1f;
		/// <summary>
		/// The amount of seconds that must pass before the detection audio can be played again.
		/// </summary>
		[SerializeField]
		private float audioTimer = 3f;
		/// <summary>
		/// Can the audio clip that plays when this enemy sees a player be played at this moment?
		/// </summary>
		private bool canPlayDetectionClip = true;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The enemy this FOV is attached to.
		/// </summary>
		[SerializeField]
		private DungeonEnemy enemy;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this enemy's FOV.
		/// </summary>
		private void ResetState() {
			// this.CurrentChaseTime = 0f;
		}
		#endregion
		
		/*#region EVENTS
		public void PlayerEnterFOV() {
			// Debug.Log("GIVE CHASE");
			// Chase the player
			this.enemy.fsm.SendEvent("Alert");
			// Have the emotion sprite appear above the enemy.
			this.enemy.SetEmotionSprite(true);
			if (canPlayDetectionClip == true) {
				// Play the detection audio clip
				this.enemy.PlayDetectionAudio();
				// Start the countdown for the amout of time that has passed since the detetction clip was last played.
				StartCoroutine(BeginAudioTimer());
			}
		}
		public void PlayerExitFOV() {

			
			Debug.Log("STOP CHASE");
			enemy.fsm.SendEvent("Docile");
			enemy.SetEmotionSprite(false);

		}
		#endregion*/

		#region OTHER
		/// <summary>
		/// Plays a timer to reset the amount of time that has passed since the last shadow detection clip played.
		/// </summary>
		/// <returns></returns>
		private IEnumerator BeginAudioTimer() {
			canPlayDetectionClip = false;
			yield return new WaitForSeconds(audioTimer);
			canPlayDetectionClip = true;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IPLAYERCOLLISIONHANDLER
		/// <summary>
		/// Gets called when the DungeonPlayer enters a special collision.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		public void OnPlayerCollisionEnter(DungeonPlayer dungeonPlayer) {
			if (canPlayDetectionClip == true) {
				// Play the detection audio clip
				this.enemy.PlayDetectionAudio();
				// Start the countdown for the amout of time that has passed since the detetction clip was last played.
				StartCoroutine(BeginAudioTimer());
			}
		}
		/// <summary>
		/// Gets called when the DungeonPlayer exits a special collision.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		public void OnPlayerCollisionExit(DungeonPlayer dungeonPlayer) {
			enemy.SetEmotionSprite(false);
		}
		/// <summary>
		/// Gets called when the DungeonPlayer simply stays in an area.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		public void OnPlayerCollisionStay(DungeonPlayer dungeonPlayer){
			// DEBUG FOR NOW
			this.enemy.SetEmotionSprite(true);
			this.enemy.ChaseTarget(targetTransform: dungeonPlayer.transform);
		}
		#endregion
		
	}


}