using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;

namespace Grawly.DungeonCrawler.Generation  {
	
	/// <summary>
	/// Contains the information regarding a floor that was just created.
	/// </summary>
	public class CrawlerFloorResultSet {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the tiles that were created for this floor.
		/// </summary>
		public List<CrawlerFloorTile> AllTiles { get; set; } = new List<CrawlerFloorTile>();
		#endregion

		#region TILE ADDITIONS
		/// <summary>
		/// Adds a tile to the list of floor tiles this set is keeping track of.
		/// </summary>
		/// <param name="floorTile">The tile to add to this set.</param>
		public void AddFloorTile(CrawlerFloorTile floorTile) {
			this.AllTiles.Add(floorTile);
		}
		/// <summary>
		/// Adds a list of floor tiles to the floor tiles this set is keeping track of.
		/// </summary>
		/// <param name="floorTiles"></param>
		public void AddFloorTiles(List<CrawlerFloorTile> floorTiles) {
			foreach (var floorTile in floorTiles) {
				this.AddFloorTile(floorTile);
			}
		}
		#endregion

		#region MODIFIERS
		/// <summary>
		/// Gets all of the modifiers of the specified type from the tiles that are stored in this results set.
		/// </summary>
		/// <typeparam name="T">The type to look for.</typeparam>
		/// <returns>A list of all the modifers that contain this component.</returns>
		public List<T> GetModifiers<T>() where T : ICrawlerComponent {
			return this.AllTiles.Where(t => t is T).Cast<T>().ToList();
		}
		#endregion
		
	}
	
}