using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Grawly.UI;
using Sirenix.OdinInspector;

namespace Grawly.Battle.BattleArena {

	/// <summary>
	/// Basically a quick and easy way for me to grab the position of the arena in any scene I want. As long as it exists.
	/// </summary>
	public class BattleArenaDXPosition : MonoBehaviour {

		public static BattleArenaDXPosition instance;

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Just some objects I can have in the scene to debug where exactly the arena should be oriented.
		/// </summary>
		[SerializeField]
		private GameObject dummyVisualsObject;
		/// <summary>
		/// Should this object also take care of loading up the battle scene?
		/// </summary>
		[SerializeField]
		private bool loadBattleSceneAtStart = false;
		/// <summary>
		/// Should the notification canvas be turned on?
		/// </summary>
		[SerializeField]
		private bool enableNotificationCanvas = true;
		/// <summary>
		/// The text that should be displayed on the notification canvas, if its turned on.
		/// </summary>
		[SerializeField, ShowIf("enableNotificationCanvas")]
		private string notificationCanvasLabelText = "";
		#endregion

		#region UNITY CALLS
		private void Awake() {
			this.dummyVisualsObject.SetActive(false);
			if (instance == null) {
				instance = this;
				// this.dummyVisualsObject.SetActive(false);
			} else {
				Debug.LogError("Instance of BattleArenaDXPosition is not null! Is there more than one in the scene?");
			}	
		}
		private void Start() {
			if (this.loadBattleSceneAtStart == true) {
				Debug.Log("BattleArenaDXPosition was set to load the battle scene on Start. Doing that now.");
				SceneManager.LoadScene(sceneName: "Battle Arena", mode: LoadSceneMode.Additive);
			}
			
			// If the notificaiton canvas is turned on, set it up.
			if (this.enableNotificationCanvas == true) {
				// NOTE: 
				// I need to do this in an if/else check because the null conditional doesn't seem to work
				// in situations where the NotificationController is in the previous scene,
				// but not the one being loaded into.
				if (NotificationController.Instance != null) {
					NotificationController.Instance?.RebuildAreaLabel(text: this.notificationCanvasLabelText);
				}
			} else {
				// If it's not supposed to be here, dismiss it.
				Debug.LogWarning("I SHOULD SNAP THIS TWEEN INSTEAD OF CALLING DISMISS() WHERE IT TAKES HALF A SECOND");
				NotificationController.Instance?.Dismiss();
			}
			
		}
		#endregion

	}


}