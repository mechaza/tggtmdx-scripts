using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon {

	/// <summary>
	/// Does exactly what it says. Enables the mesh renderer when the player enters this collider.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(MeshRenderer))]
	public class EnableMeshRendererOnTriggerEnter : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// Was this collider... colided with?
		/// </summary>
		private bool activated = false;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// A list of other game objects this script should also enable.
		/// If there are any.
		/// </summary>
		[SerializeField]
		private List<GameObject> childGameObjects = new List<GameObject>();
		#endregion

		#region UNITY CALLS
		private void Start() {
			// DunGen may enable certain children on start. If this happens, I need to reset it.
			// This is relevant for things like the Minimap Icon Hooks.
			foreach (GameObject go in this.childGameObjects) {
				// Check if it's null. The reference is removed if DunGen tosses it away.
				if (go != null) {
					go.SetActive(false);
				}
			}
		}
		private void OnTriggerEnter(Collider other) {
			if (activated == false && other.gameObject.CompareTag(tag: "Player") == true) {
				this.GetComponent<MeshRenderer>().enabled = true;
				foreach (GameObject go in this.childGameObjects) {
					if (go != null) {
						go.SetActive(true);
					}
				}
				this.activated = true;
				
			}
		}
		#endregion

	}


}