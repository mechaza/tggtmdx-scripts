using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Grawly.Chat;
using DG.Tweening;
using Sirenix.OdinInspector;
using Grawly.Story;
using Grawly.Toggles;
using Grawly.Toggles.Audio;

namespace Grawly.Dungeon {
	
	/// <summary>
	/// A "wheel" of colliders that circle around the DungeonPlayer
	/// and can detect if something is allowed to spawn next to them.
	/// </summary>
	public class DungeonPlayerSpawnWheel : MonoBehaviour {

		public static DungeonPlayerSpawnWheel Instance { get; private set; }

		
		
		#region FIELDS - TOGGLES
		
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// A list of available "points" that enemies can spawn at.
		/// </summary>
		[SerializeField]
		private List<DungeonPlayerSpawnWheelPoint> wheelPoints = new List<DungeonPlayerSpawnWheelPoint>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		#endregion

		#region GETTERS - STATE
		/// <summary>
		/// Checks whether or not the wheel has points that are valid for spawning.
		/// </summary>
		/// <returns>Whether or not the wheel has points valid for spawning.</returns>
		public bool HasValidPoints() {
			// Cascade down using the default points.
			return this.HasValidPoints(wheelPoints: this.wheelPoints);
		}
		/// <summary>
		/// Checks whether or not the wheel has valid points for spawning.
		/// </summary>
		/// <param name="wheelPoints">The points to check against.</param>
		/// <returns>Whether any of the points provided are valid.</returns>
		private bool HasValidPoints(List<DungeonPlayerSpawnWheelPoint> wheelPoints) {
			return this.GetValidPoints(wheelPoints: wheelPoints).Count > 0;
		}
		#endregion
		
		#region GETTERS - POINTS
		/// <summary>
		/// Gets a random point from a list of valid spawn points.
		/// </summary>
		/// <returns>A totally random valid point.</returns>
		public DungeonPlayerSpawnWheelPoint GetRandomValidPoint() {
			// Grab valid points using the default list. 
			var validPoints = this.GetValidPoints(wheelPoints: this.wheelPoints);
			// Pass this down and use a random point.
			return this.GetRandomValidPoint(validPoints: validPoints);
		}
		/// <summary>
		/// Gets a random point from a list of valid spawn points.
		/// </summary>
		/// <param name="validPoints">The points to pick from.</param>
		/// <returns>A totally random valid point.</returns>
		private DungeonPlayerSpawnWheelPoint GetRandomValidPoint(List<DungeonPlayerSpawnWheelPoint> validPoints) {
			// There should always be at least one point.
			Debug.Assert(validPoints.Count > 0);
			return validPoints.Random();
		}
		/// <summary>
		/// Returns a list of wheel points that are not "out of bounds" and can be used to spawn an enemy.
		/// Uses the default set of wheel points.
		/// </summary>
		/// <returns>A list of wheel points that can potentially spawn an enemy at its location.</returns>
		public List<DungeonPlayerSpawnWheelPoint> GetValidPoints() {
			// Just use the points referenced above.
			return this.GetValidPoints(wheelPoints: this.wheelPoints);
		}
		/// <summary>
		/// Returns a list of wheel points that are not "out of bounds" and can be used to spawn an enemy.
		/// </summary>
		/// <param name="wheelPoints">The points to check against.</param>
		/// <returns>A list of wheel points that can potentially spawn an enemy at its location.</returns>
		private List<DungeonPlayerSpawnWheelPoint> GetValidPoints(List<DungeonPlayerSpawnWheelPoint> wheelPoints) {
			// Find the points that are valid (usually means if they are in bounds)
			return wheelPoints
				.Where(wp => wp.IsValid() == true)
				.ToList();
		}
		#endregion
		
	}
	
}