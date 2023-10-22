using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.DungeonCrawler.Generation {
	
	/// <summary>
	/// The theme that contains the prefabs to use when building a crawler map.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Crawler/Crawler Theme")]
	public class CrawlerMapThemeTemplate : SerializedScriptableObject {

		#region FIELDS
		/// <summary>
		/// The size of each tile's width and height.
		/// Used when figuring out where to place each prefab.
		/// </summary>
		[OdinSerialize]
		public float TileSize { get; private set; } = 3f;
		/// <summary>
		/// The dictionary containing the prefabs to use when building a crawler map from a template.
		/// </summary>
		[OdinSerialize]
		private Dictionary<CrawlerTileIDType, GameObject> TilePrefabDict { get; set; } = new Dictionary<CrawlerTileIDType, GameObject>();
		#endregion

		#region GETTERS
		/// <summary>
		/// Returns a prefab associated with the specified tile ID type.
		/// </summary>
		/// <param name="tileIDType">The tile ID type associated with the desired prefab.</param>
		/// <returns>The prefab for the given tile ID type.</returns>
		public GameObject GetPrefab(CrawlerTileIDType tileIDType) {
			try {
				return this.TilePrefabDict[tileIDType];
			} catch (System.Exception e) {
				throw new System.Exception("Could not find prefab for TileIDType " + tileIDType.ToString());
			}
		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// Adds entries int he dictionary for anything that doesnt have a key.
		/// </summary>
		[Button]
		private void AddEmptyTiles() {
			
			// Go through each kind of tile id.
			foreach (CrawlerTileIDType tileIDType in System.Enum.GetValues(typeof(CrawlerTileIDType))) {
				// If it doesn't have an entry, add a blank one.
				if (this.TilePrefabDict.ContainsKey(tileIDType) == false) {
					this.TilePrefabDict.Add(key: tileIDType, value: null);
				}
			}
		}
		#endregion
		
	}
	
}