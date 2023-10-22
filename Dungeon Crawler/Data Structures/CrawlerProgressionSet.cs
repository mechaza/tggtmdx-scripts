using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System.Linq;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// Contains the data that keeps track of the Player's progression through a Crawler Dungeon.
	/// </summary>
	[System.Serializable]
	public class CrawlerProgressionSet {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The ID associated with this progression data.
		/// </summary>
		public CrawlerDungeonIDType crawlerDungeonIDType = CrawlerDungeonIDType.None;
		#endregion

		#region FIELDS - STATE
		/// <summary>
		/// Contains entries describing the progress of each crawler floor.
		/// </summary>
		public List<CrawlerFloorProgress> floorProgressList = new List<CrawlerFloorProgress>();
		#endregion
		
		#region PROPERTIES - STATE
		/// <summary>
		/// The number of the highest floor the player has reached so far.
		/// </summary>
		public int CurrentHighestFloorReached {
			get {
				// Comb through all the floors that are marked as reached and return the highest one.
				return this.floorProgressList
					.Where(fp => fp.floorReached == true)
					.Max(fp => fp.floorNumber);
			}
		}
		#endregion

		#region CONSTRUCTORS
		public CrawlerProgressionSet() {
			// Create 200 entries for the floor progress (not all will be used in a given dungeon)
			// and initialize the floor number on each.
			for (int i = 0; i < 50; i++) {
				this.floorProgressList.Add(new CrawlerFloorProgress() {
					floorNumber = i,
				});
			}
		}
		#endregion
		
		#region CHECKERS
		/// <summary>
		/// Checks whether a treasure on a given floor is available for obtaining.
		/// </summary>
		/// <param name="floorNumber">The floor to check the treasures on.</param>
		/// <param name="treasureIndex">The index of the treasure on the floor provided.</param>
		/// <returns>Whether or not the specified treasure can still be obtained.</returns>
		public bool CheckTreasureAvailability(int floorNumber, int treasureIndex) {
			// Get the appropriate floor and then check its treasure availability.
			return this
				.GetFloorProgress(floorNumber: floorNumber)
				.CheckTreasureAvailability(treasureIndex: treasureIndex);
		}
		/// <summary>
		/// Checks whether an event on a given floor is available for invokation.
		/// </summary>
		/// <param name="floorNumber">The floor to check the event for.</param>
		/// <param name="eventIndex">The index associated with the desired event.</param>
		/// <returns>Whether the event located at this index is able to be invoked.</returns>
		public bool CheckEventAvailability(int floorNumber, int eventIndex) {
			// Get the appropriate floor and then check its event availability.
			return this
				.GetFloorProgress(floorNumber)
				.CheckEventAvailability(eventIndex: eventIndex);
		}
		/// <summary>
		/// Checks whether a minimaptile at the specified location is visible on this floor.
		/// </summary>
		/// <param name="floorNumber"></param>
		/// <param name="tileCoordinates"></param>
		/// <returns></returns>
		public bool CheckMiniMapTileVisibility(int floorNumber, Vector2Int tileCoordinates) {
			return this
				.GetFloorProgress(floorNumber)
				.CheckMiniMapTileVisibility(tileCoordinates: tileCoordinates);
		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets the progress associated with the floor specified.
		/// </summary>
		/// <param name="floorNumber">The floor progress to check for.</param>
		/// <returns></returns>
		public CrawlerFloorProgress GetFloorProgress(int floorNumber) {
			return this.floorProgressList[floorNumber];
		}
		/// <summary>
		/// Gets the numbers of all the floors where a teleporter has been activated.
		/// </summary>
		/// <returns>A list of floors where a teleporter has been activated.</returns>
		public List<int> GetActivatedTeleporterFloors() {
			throw new NotImplementedException("Add this!");
		}
		#endregion

		#region SETTERS
		/// <summary>
		/// Updates the progressions saved in this set so that the floor number passed in is marked as the highest reached.
		/// </summary>
		/// <param name="floorNumber">The floor number that should be set as the highest.</param>
		public void SetHighestFloorReached(int floorNumber) {
			// Go through each FloorProgress object...
			foreach (CrawlerFloorProgress floorProgress in this.floorProgressList) {
				// If the floor is less or equal to the one passed in, mark it as reached.
				if (floorProgress.floorNumber <= floorNumber) {
					floorProgress.floorReached = true;
				} else {
					// If the floor in this iteration of the loop is greater than the one passed in, break out.
					break;
				}
			}
		}
		#endregion
		
		#region CLONING
		/// <summary>
		/// Creates a clone of this dungeon progression.
		/// </summary>
		/// <returns></returns>
		public CrawlerProgressionSet Clone() {
			return new CrawlerProgressionSet() {
				crawlerDungeonIDType = this.crawlerDungeonIDType,
				floorProgressList = this.floorProgressList.Select(fp => fp.Clone()).ToList(),
			};
		}
		#endregion
		
	}
}