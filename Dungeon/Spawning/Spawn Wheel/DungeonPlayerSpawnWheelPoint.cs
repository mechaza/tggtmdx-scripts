using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Grawly.Chat;
using DG.Tweening;
using Sirenix.OdinInspector;
using Grawly.Story;
using Grawly.Toggles;
using Grawly.Toggles.Audio;

namespace Grawly.Dungeon  {
	
	/// <summary>
	/// A "point" on the spawn wheel that could potentially have an enemy spawn at its position.
	/// </summary>
	public class DungeonPlayerSpawnWheelPoint : MonoBehaviour, IDungeonSpawnPoint {

		#region FIELDS - TOGGLES
		
		#endregion

		#region PROPERTIES - IDUNGEONSPAWNPOINT
		/// <summary>
		/// The position associated with whatever this interface is attached to.
		/// </summary>
		public Vector3 SpawnPointPosition {
			get {
				return this.transform.position;
			}
		}
		/// <summary>
		/// The rotation associated with whatever this interface is attached to.
		/// </summary>
		public Quaternion SpawnPointRotation {
			get {
				return this.transform.rotation;
			}
		}
		#endregion
		
		#region STATUS
		/// <summary>
		/// Checks whether or not this wheel point can be used to spawn an enemy.
		/// </summary>
		/// <returns>Whether or not this wheel point can be used to spawn an enemy.</returns>
		public bool IsValid() {
			
			// Grab all the colliders that overlap with this point's position.
			var colliders = Physics.OverlapBox(center: this.SpawnPointPosition, halfExtents: Vector3.one);
			// Go through each one to make sure it has an area component.
			// If it does, that means this point is inside and is valid.
			foreach (var collider in colliders) {
				if (collider.GetComponent<DungeonEnemySpawnArea>() != null) {
					return true;
				}
			}
			// If the area has not been found, that means this point is outside boundaries.
			return false;
		}
		#endregion
		
	}
	
}