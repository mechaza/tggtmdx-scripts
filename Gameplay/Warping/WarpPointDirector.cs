using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Gameplay {
	
	/// <summary>
	/// Can be used to manage the different points which the player can warp to.
	/// Usable in all kinds of player modes. (Dungeon, Crawler, etc)
	/// </summary>
	public class WarpPointDirector : MonoBehaviour {
		
		public static WarpPointDirector Instance { get; private set; }

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// A list of available warp points.
		/// </summary>
		[SerializeField]
		private List<WarpPoint> warpPoints = new List<WarpPoint>();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Gets a warp point of a specified type.
		/// </summary>
		/// <param name="warpPointType">The warp point to find.</param>
		/// <returns>The warp point associated with the specified type.</returns>
		public WarpPoint GetWarpPoint(WarpPointType warpPointType) {
			
			// Find the first warp point whose type matches the one passed in.
			var warpPoint = this.warpPoints.First(wp => wp.WarpPointType == warpPointType) ?? null;
			
			// If it was not found, throw an exception.
			if (warpPoint == null) {
				throw new System.Exception("Could not find warp point of type " + warpPointType);
			} else {
				return warpPoint;
			}
		}
		#endregion
		
	}
	
}