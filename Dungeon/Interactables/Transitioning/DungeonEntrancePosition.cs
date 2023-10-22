using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon {

	/// <summary>
	/// When a scene is loaded and a spawn position is designated, this component marks where that location is.
	/// </summary>
	[RequireComponent(typeof(MeshRenderer))]
	public class DungeonEntrancePosition : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The ID of this spawn position.
		/// </summary>
		public DungeonSpawnType spawnPositionType = DungeonSpawnType.Default;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The mesh renderer that identifies where in the scene this marker is placed.
		/// Should get turned off upon Start.
		/// </summary>
		private MeshRenderer markerMeshRenderer;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			// Grab the mesh render so it can be turned off in a moment.
			this.markerMeshRenderer = this.GetComponent<MeshRenderer>();
		}
		private void Start() {
			// Turn the mesh renderer off.
			this.markerMeshRenderer.enabled = false;
		}
		#endregion
		
	}

}