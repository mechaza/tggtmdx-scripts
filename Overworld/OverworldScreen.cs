using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Overworld {
	public class OverworldScreen : MonoBehaviour {

		/// <summary>
		/// The bounds for this screen. Gets used by ProCamera2D so that the cursor doesn't make the screen go wonky.
		/// </summary>
		public Vector4 bounds;

		/// <summary>
		/// Draws the bounds. Helpful for debugging.
		/// </summary>
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.yellow;
			// Top
			Gizmos.DrawLine(new Vector3(-20f, bounds.x), new Vector3(20f, bounds.x));
			// Right
			Gizmos.DrawLine(new Vector3(bounds.y, -20f), new Vector3(bounds.y, 20f));
			// Bottom
			Gizmos.DrawLine(new Vector3(-20f, bounds.z), new Vector3(20f, bounds.z));
			// Left
			Gizmos.DrawLine(new Vector3(bounds.w, -20f), new Vector3(bounds.w, 20f));
		}

	}
}