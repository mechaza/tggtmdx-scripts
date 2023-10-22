using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Dungeon;
using Grawly.Toggles;
using UnityEngine;
using UnityEngine.SceneManagement;
	
namespace Grawly {

	public class ResetController : MonoBehaviour {

		#region STATIC FIELDS - STATE
		/// <summary>
		/// A list of game objects that have been registered as "do not destory."
		/// If this script runs its Start function, the list gets cleared and everything inside gets reset.
		/// </summary>
		private static List<GameObject> DontDestroyList = new List<GameObject>();

		#endregion
		
		#region UNITY CALLS
		private IEnumerator Start() {
			yield return new WaitForSeconds(2f);
			
			Destroy(GameObject.Find("[DOTween]"));
			
			yield return new WaitForSeconds(0.1f);
			
			DestroyCachedObjects();

			// Wait a bit.
			yield return new WaitForSeconds(1f);
			// Load the title screen.
			SceneManager.LoadScene("Initialization");
		}
		#endregion

		#region MAIN CALLS

		/// <summary>
		/// Registers the game object to the list of game objects to destroy on reset and also tells it to not destory anyway.
		/// </summary>
		/// <param name="obj"></param>
		public static void AddToDontDestroy(GameObject obj) {
			Debug.Log("Adding GameObject to ResetController: " + obj.name);
			if (DontDestroyList.Contains(obj) == false) {
				DontDestroyList.Add(obj);
				GameObject.DontDestroyOnLoad(obj);
			}
			
		}

		public static void DestroyCachedObjects() {
		
			
			string str = "DESTROYING: ";
			
			foreach (var obj in DontDestroyList) {
				if (obj != null) {
					str += obj.name + ", ";
				}
				
			}
			foreach (var obj in DontDestroyList) {
				if (obj != null) {
					Destroy(obj);
				}
				
			}
			DontDestroyList.Clear();

			if (GameController.Instance != null) {
				// Debug.Log("Destroying GameController instance.");
				GameObject.Destroy(GameController.Instance.gameObject);
			}

			/*if (DungeonPlayer.Instance != null) {
				Debug.Log("Destroying DungeonPlayer instance.");
				Destroy(DungeonPlayer.Instance.gameObject);
			}*/
			
		}
		#endregion
		
	}
	
}
