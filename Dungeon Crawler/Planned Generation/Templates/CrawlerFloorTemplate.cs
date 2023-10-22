using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grawly.Battle;
using Grawly.Dungeon.Interactable;
using Grawly.DungeonCrawler.Events;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler.Generation {
	
	/// <summary>
	/// Contains the definitions of a crawler map that should be built at runtime.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Crawler/Floor Template")]
	public class CrawlerFloorTemplate : SerializedScriptableObject {

		#region FIELDS - MAP ASSETS
		/// <summary>
		/// All of the CSV files that should be used on this map.
		/// </summary>
		[SerializeField, Title("Map Files")]
		private List<TextAsset> mapCSVs = new List<TextAsset>();
		/// <summary>
		/// A list of items to make available to the player. Accessed by index.
		/// </summary>
		[SerializeField, Title("Treasure")]
		private List<TreasureTemplate> treasureItems = new List<TreasureTemplate>();
		/// <summary>
		/// An indexed list of battle templates for specific events. Not used in random encounters.
		/// </summary>
		[SerializeField, PropertyTooltip("An indexed list of battle templates for specific events. Not used in random encounters.")]
		private List<BattleTemplate> scriptedBattleTemplates = new List<BattleTemplate>();
		#endregion
		
		#region FIELDS - TOGGLES : ENCOUNTERS
		/// <summary>
		/// Are encounters enabled?
		/// </summary>
		[Title("Encounters")]
		[SerializeField]
		public bool enableEncounters = true;
		/// <summary>
		/// The number of steps to take (approximately) before incrementing the danger level.
		/// </summary>
		[SerializeField, ShowIf("enableEncounters")]
		public int baseStepsPerDangerLevel = 10;
		/// <summary>
		/// The number of steps to add/subtract to the base steps to create a range.
		/// </summary>
		[SerializeField, ShowIf("enableEncounters")]
		public int rangeStepsPerDangerLevel = 5;
		/// <summary>
		/// The battle templates to pick at random when encountering in the dungeon.
		/// </summary>
		[SerializeField, PropertyTooltip("A list of battles to pick at random when encountering in the dungeon."), ShowIf("enableEncounters")]
		private List<BattleTemplate> randomBattleTemplates = new List<BattleTemplate>();
		#endregion

		#region FIELDS - TOGGLES : EVENTS
		/// <summary>
		/// The events that should be ready to execute on this floor.
		/// Sorted by index.
		/// </summary>
		[SerializeField, Title("Events")]
		private List<CrawlerEventTemplate> floorEvents = new List<CrawlerEventTemplate>();
		#endregion
		
		#region FIELDS - THEME
		/// <summary>
		/// The theme to use for this map template.
		/// </summary>
		[SerializeField]
		private CrawlerMapThemeTemplate themeTemplate;
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Generates a set of map params from this template.
		/// </summary>
		/// <returns></returns>
		public CrawlerFloorParams GenerateFloorParams() {
			List<CrawlerMapTileParams> tileParams = this.ParseCSV(this.mapCSVs);
			return new CrawlerFloorParams(tileParams: tileParams, themeTemplate: this.themeTemplate);
		}
		#endregion

		#region PARSING - GENERAL
		/// <summary>
		/// Gets the TileIDType fro a specified value.
		/// These are usually used when parsing CSV files.
		/// </summary>
		/// <param name="value">The value to read out.</param>
		/// <returns>The TileID associated with the provided string.</returns>
		private CrawlerTileIDType GetTileIDType(string value) {
			// Parse the value from the string.
			int parsedValue = Int32.Parse(s: value);
			// Cascade down to the version of this function using an int.
			return this.GetTileIDType(value: parsedValue);
		}
		/// <summary>
		/// Get the TileID type from a specified value.
		/// These are usually used in CSV files.
		/// </summary>
		/// <param name="value">The value to read out.</param>
		/// <returns>The tile ID associated with the specified value.</returns>
		private CrawlerTileIDType GetTileIDType(int value) {
			// Cast the value to an enum and return it.
			CrawlerTileIDType castedValue = (CrawlerTileIDType) value;
			return castedValue;
		}
		/// <summary>
		/// Gets the size of a 2D array.
		/// </summary>
		/// <param name="grid"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private Vector2Int GetGridSize<T>(T[,] grid) {
			int width = grid.GetLength(0);
			int height = grid.GetLength(1);
			return new Vector2Int(x: width, y: height);
		}
		#endregion

		#region PARSING - CSV
		/// <summary>
		/// Parse a batch of CSV files into a set of tile map params.
		/// </summary>
		/// <param name="textAssets"></param>
		/// <returns></returns>
		private List<CrawlerMapTileParams> ParseCSV(List<TextAsset> textAssets) {
			List<CrawlerMapTileParams> mapParams = textAssets.SelectMany(ta => this.ParseCSV(ta)).ToList();
			return mapParams;
		}
		/// <summary>
		/// Parse a CSV file into a set of tile map params.
		/// </summary>
		/// <param name="textAsset"></param>
		/// <returns></returns>
		private List<CrawlerMapTileParams> ParseCSV(TextAsset textAsset) {

			// Create a new list of tile params.
			List<CrawlerMapTileParams> mapTileParams = new List<CrawlerMapTileParams>();
			
			// Grab the text from the text asset.
			string text = textAsset.text;
			
			// Split the text by newline.
			string[] alllines = text.Split("\n"[0]);

			// Begin iterating through each line.
			for (int x = 0; x < alllines.Length; x++) {
				// Split the current line, then iterate through that as well.
				string[] splitLine = (alllines[x].Trim()).Split(","[0]);
				for (int y = 0; y < splitLine.Length; y++) {
					// Grab the value for the current tile.
					string currentTileValue = splitLine[y];
					// If the string is empty, break out of the loop. That means we're done.
					if (string.IsNullOrWhiteSpace(currentTileValue) == true) {
						break;
					}
					
					// If there is a value, figure out the tile ID associated with this x/y coordinate.
					CrawlerTileIDType tileIDType = this.GetTileIDType(value: currentTileValue);
					// I may fuss around with the x/y coordinates so set those here.
					int xPos = y;
					int yPos = (alllines.Length - x);
					
					// Create a new map tile param and add it to the list.
					CrawlerMapTileParams tileParams = new CrawlerMapTileParams(
						xPos: xPos,
						yPos: yPos, 
						tileIDType: tileIDType);
					
					mapTileParams.Add(tileParams);
				}
			}

			return mapTileParams;

		}
		#endregion
		
		#region GETTERS - ASSETS
		/// <summary>
		/// Returns a random battle associated with this floor.
		/// </summary>
		/// <returns></returns>
		public BattleTemplate GetRandomBattle() {
			return this.randomBattleTemplates.Random();
		}
		/// <summary>
		/// Returns an event battle associated with this floor and the given index number.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public BattleTemplate GetEventBattle(int index) {
			return this.scriptedBattleTemplates[index];
		}
		/// <summary>
		/// Gets the TreasureTemplate stored at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public TreasureTemplate GetTreasure(int index) {
			TreasureTemplate treasureTemplate = this.treasureItems[index];
			return treasureTemplate;
		}
		/// <summary>
		/// Gets the EventTemplate stored at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public CrawlerEventTemplate GetEvent(int index) {
			CrawlerEventTemplate eventTemplate = this.floorEvents[index];
			return eventTemplate;
		}
		#endregion
		
	}
}