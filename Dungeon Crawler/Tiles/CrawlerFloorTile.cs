using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grawly.Dungeon;
using UnityEngine;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// A tile that a crawler character can step on.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public abstract class CrawlerFloorTile : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The coordinates of this tile.
		/// I primarily am just using this to help keep track of minimap tiles
		/// so I can activate them when revisiting a map.
		/// </summary>
		public Vector2Int CurrentCoordinates { get; set; } = new Vector2Int();
		#endregion
		
		#region STATE CHECKS
		/// <summary>
		/// Checks a specific point to see if the specified CrawlerPlayer overlaps with it.
		/// </summary>
		/// <param name="crawlerPlayer">The CrawlerPlayer to check for.</param>
		/// <param name="pointToCheck">The specific point to look for the CrawlerPlayer at.</param>
		/// <param name="tileSize">The current tile size in the dungeon.</param>
		/// <returns>The side of the tile the crawler player is on, if any.</returns>
		private bool CheckPointForPlayer(CrawlerPlayer crawlerPlayer, Vector3 pointToCheck, int tileSize) {
			
			// Find the colliders that overlap with the target point.
			var overlappingColliders = Physics.OverlapBox(
				center: pointToCheck, 
				halfExtents: (this.transform.localScale * (tileSize * 0.9f)) / 2f, 
				orientation: Quaternion.identity);

			// Iterate through the colliders and check if any of them are the player being looked for.
			foreach (var collider in overlappingColliders) {
				// Create a variable to store whatever player was found.
				CrawlerPlayer potentialPlayer = null;
				// Attempt to get the CrawlerPlayer component from the collider.
				bool playerFound = collider.TryGetComponent<CrawlerPlayer>(component: out potentialPlayer);
				// If a player was found and the player found matches the one specified, return true.
				if (playerFound && potentialPlayer == crawlerPlayer) {
					return true;
				}
			}

			// If reaching this point, it means the player was not found.
			return false;
			
		}
		/// <summary>
		/// Checks the sides of this tile to see if the player is hanging along any of them.
		/// If the player is not around, the result will be None.
		/// </summary>
		/// <returns></returns>
		public CrawlerTileSideType CheckSidesForPlayer() {
			// Use the current instance for the player and also use their tile size.
			return this.CheckSidesForPlayer(
				crawlerPlayer: CrawlerPlayer.Instance, 
				tileSize: CrawlerPlayer.Instance.TileSize);
		}
		/// <summary>
		/// Checks the sides of this tile to see if the player is hanging along any of them.
		/// If the player is not around, the result will be None.
		/// </summary>
		/// <param name="crawlerPlayer">The CrawlerPlayer to look for.</param>
		/// <param name="tileSize">The current size of the tiles.</param>
		/// <returns>The side of the tile the player is currently next to.</returns>
		public CrawlerTileSideType CheckSidesForPlayer(CrawlerPlayer crawlerPlayer, int tileSize) {

			// For the sake of readability, grab the current tile position.
			Vector3 tilePosition = this.transform.position;
			
			// Declare different offsets to check against.
			Vector3 offsetN = new Vector3(x: 0f, y: 0f, z: 1f);
			Vector3 offsetE = new Vector3(x: 1f, y: 0f, z: 0f);
			Vector3 offsetS = new Vector3(x: 0f, y: 0f, z: -1f);
			Vector3 offsetW = new Vector3(x: -1f, y: 0f, z: 0f);
			
			// Calculate the points to actually check.
			Vector3 targetPointN = tilePosition + (offsetN * tileSize);
			Vector3 targetPointE = tilePosition + (offsetE * tileSize);
			Vector3 targetPointS = tilePosition + (offsetS * tileSize);
			Vector3 targetPointW = tilePosition + (offsetW * tileSize);
			
			// Now check to see if the player is on any of the sides.
			// NOTE: The order of this is actually important for *some* fucking reason.
			// I had to swap the west and south checks because South kept succeeding even though I wanted West.
			if (this.CheckPointForPlayer(crawlerPlayer: crawlerPlayer, pointToCheck: targetPointN, tileSize: tileSize)) {
				return CrawlerTileSideType.N;
			} else if (this.CheckPointForPlayer(crawlerPlayer: crawlerPlayer, pointToCheck: targetPointE, tileSize: tileSize)) {
				return CrawlerTileSideType.E;
			} else if (this.CheckPointForPlayer(crawlerPlayer: crawlerPlayer, pointToCheck: targetPointW, tileSize: tileSize)) {
				return CrawlerTileSideType.W;
			} else if (this.CheckPointForPlayer(crawlerPlayer: crawlerPlayer, pointToCheck: targetPointS, tileSize: tileSize)) {
				return CrawlerTileSideType.S;
			}

			// If at this point, none of the checks were successful.
			// Return 'none' to say the player is not along any of the sides.
			return CrawlerTileSideType.None;

		}
		#endregion
		
	}

	/// <summary>
	/// The different sides of a tile in the crawler dungeon.
	/// I found out I'm having situations where I need to check what side the player is standing on.
	/// </summary>
	public enum CrawlerTileSideType {
		None = 0,
		N = 1,
		E = 2,
		S = 3,
		W = 4,
	}
}