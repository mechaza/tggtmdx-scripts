using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Dungeon {

	/// <summary>
	/// This is what should be attached to a tile so that a tile will show up on the minimap.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class DungeonTileCollider : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// A list of GameObjects to enable when the player intersects with this tile's collider.
		/// </summary>
		[SerializeField]
		private List<GameObject> gameObjectsToEnable = new List<GameObject>();
		#endregion

		#region EVENTS
		/// <summary>
		/// Check if it's the player that just collided and if so, enable everything in to-enable.
		/// </summary>
		/// <param name="collision"></param>
		private void OnCollisionEnter(Collision collision) {
			if (collision.transform.tag == "Player") {
				Debug.Log("AHH");
				this.gameObjectsToEnable.ForEach(go => go.SetActive(true));
			}
		}
		#endregion

	}


}