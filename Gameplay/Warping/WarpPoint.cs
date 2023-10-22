using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Gameplay {
	
	/// <summary>
	/// Marks a point that the player can warp to during gameplay.
	/// This is mostly for custscenes or special events.
	/// </summary>
	public class WarpPoint : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The type of warp point associated with this object.
		/// </summary>
		[SerializeField]
		private WarpPointType warpPointType = WarpPointType.None;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The type of warp point associated with this object.
		/// </summary>
		public WarpPointType WarpPointType => this.warpPointType;
		#endregion

	}
}