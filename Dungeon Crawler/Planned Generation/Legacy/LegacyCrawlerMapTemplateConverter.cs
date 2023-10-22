using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grawly.DungeonCrawler.Generation.Legacy {
	
	/// <summary>
	/// Reads from a map asset file to build a map at runtime based on a set of pre-written maps.
	/// </summary>
	/// <remarks>
	/// i think i made this before but deleted it bc it was garbage so im making it again im so exhausted
	/// </remarks>
	public class LegacyCrawlerMapTemplateConverter : MonoBehaviour {
		
		public static LegacyCrawlerMapTemplateConverter Instance { get; private set; }

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The transform to parent spawned prefabs to.
		/// </summary>
		[SerializeField]
		private Transform spawnParentTransform;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			/*if (this.generateOnStart == true) {
				this.GenerateMap(mapTemplate: this.mapTemplate);
			}*/
		}
		#endregion

		#region GENERATION - GENERAL
		/// <summary>
		/// Generate a map based on the information contained within the passed template.
		/// </summary>
		/// <param name="floorTemplate">The template to use in map generation.</param>
		private void GenerateFloor(CrawlerFloorTemplate floorTemplate) {
			this.GenerateFloor(floorParams: floorTemplate.GenerateFloorParams());
		}
		/// <summary>
		///  Generate a map based on the parameters specified.
		/// </summary>
		/// <param name="floorParams">The parameters to use in building this map.</param>
		private void GenerateFloor(CrawlerFloorParams floorParams) {
			// Use the version of this function that can place spawned prefabs into a parent transform.
			this.GenerateFloor(
				floorParams: floorParams, 
				parentTransform: this.spawnParentTransform);
		}
		/// <summary>
		///  Generate a map based on the parameters specified.
		/// </summary>
		/// <param name="floorParams">The parameters to use in building this map.</param>
		/// <param name="parentTransform"></param>
		private void GenerateFloor(CrawlerFloorParams floorParams, Transform parentTransform) {
			
			// Grab all the children from the parent transform and destroy them.
			List<Transform> childTransforms = new List<Transform>();
			for (int i = 0; i < parentTransform.childCount; i++) {
				childTransforms.Add(parentTransform.GetChild(index: i));
				// DestroyImmediate(parentTransform.GetChild(index: i));
			}
			foreach (Transform childTransform in childTransforms) {
				DestroyImmediate(childTransform.gameObject);
			}
			
			// Iterate through each tile params...
			foreach (CrawlerMapTileParams tileParams in floorParams.AllMapTileParams) {
				// ...figure out the ID type...
				CrawlerTileIDType tileIDType = tileParams.TileIDType;
				// ...grab the associated prefab...
				GameObject tilePrefab = floorParams.MapThemeTemplate.GetPrefab(tileIDType: tileIDType);
				// ...and spawn it appropriately.
				this.SpawnTilePrefab(
					xPos: tileParams.XPos,
					yPos: tileParams.YPos, 
					tileSize: floorParams.MapThemeTemplate.TileSize,
					tilePrefab: tilePrefab, 
					parentTransform: parentTransform);
			}
		}
		#endregion

		#region GENERATION - PREFAB PLACEMENT
		/// <summary>
		/// Spawns a tile prefab at the provided x/y coordinates.
		/// </summary>
		/// <param name="xPos">The x-position to place the tile.</param>
		/// <param name="yPos">The y-position to place the tile.</param>
		/// <param name="tileSize">The amount to scale the placement of the tile.</param>
		/// <param name="tilePrefab">The prefab to actually spawn.</param>
		/// <param name="parentTransform">The transform to parent the instantiated prefabs to..</param>
		private void SpawnTilePrefab(int xPos, int yPos, float tileSize, GameObject tilePrefab, Transform parentTransform) {
			
			// Create a new vector which contains the target position.
			Vector3 targetPosition = new Vector3(
				x: xPos * tileSize,
				y: 0, 
				z: yPos * tileSize);
			
			// Instantiate the prefab.
			GameObject obj = GameObject.Instantiate(
				original: tilePrefab, 
				position: targetPosition, 
				rotation: Quaternion.identity);

			// Set the parent to be the passed in transform.
			obj.transform.SetParent(p: parentTransform);
			
		}
		#endregion

		#region ODIN HELPERS
		[Button]
		private void DebugClear() {
			// Grab all the children from the parent transform and destroy them.
			List<Transform> childTransforms = new List<Transform>();
			for (int i = 0; i < this.spawnParentTransform.childCount; i++) {
				childTransforms.Add(this.spawnParentTransform.GetChild(index: i));
			}
			foreach (Transform childTransform in childTransforms) {
				DestroyImmediate(childTransform.gameObject);
			}
		}
		/*[SerializeField]
		private CrawlerMapTemplate debugMapTemplate;
		[Button]
		private void DebugGenerate() {
			this.GenerateMap(mapTemplate: this.debugMapTemplate);
		}*/
		#endregion
		
	}
}