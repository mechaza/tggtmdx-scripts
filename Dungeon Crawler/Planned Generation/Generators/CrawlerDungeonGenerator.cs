using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grawly.DungeonCrawler.Generation {
	public class CrawlerDungeonGenerator : MonoBehaviour {
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The transform to parent spawned prefabs to.
		/// </summary>
		[SerializeField]
		private Transform spawnParentTransform;
		#endregion
		
		#region GENERATION - PREPARATION
		/// <summary>
		/// Clears out all of the child objects belonging to the transform that is responsible for parenting the tiles of this dungeon.
		/// </summary>
		private void ClearCurrentMap() {
			// Cascade down.
			this.ClearCurrentMap(parentTransform: this.spawnParentTransform);
		}
		/// <summary>
		/// Clears out all of the child objects belonging to the transform that is responsible for parenting the tiles of this dungeon.
		/// </summary>
		/// <param name="parentTransform">The transform containing the objects to clear out.</param>
		private void ClearCurrentMap(Transform parentTransform) {
			// Grab all the children from the parent transform and destroy them.
			List<Transform> childTransforms = new List<Transform>();
			for (int i = 0; i < parentTransform.childCount; i++) {
				childTransforms.Add(parentTransform.GetChild(index: i));
			}
			foreach (Transform childTransform in childTransforms) {
				DestroyImmediate(childTransform.gameObject);
			}
		}
		#endregion
		
		#region GENERATION - GENERAL
		/// <summary>
		/// Generates a floor based on the information provided in the dungeon template.
		/// </summary>
		/// <param name="dungeonTemplate">The crawler dungeon template that contains the information for the floor to be generated.</param>
		/// <param name="floorNumber">The floor number to actually access.</param>
		public CrawlerFloorResultSet GenerateFloor(CrawlerDungeonTemplate dungeonTemplate, int floorNumber) {
			
			// Grab the template associated with the provided floor.
			CrawlerFloorTemplate floorTemplate = dungeonTemplate.GetFloorTemplate(floorNumber: floorNumber);
			
			// Generate the floor that was retrieved and grab the resulting tiles.
			List<CrawlerFloorTile> spawnedTiles = this.GenerateFloor(floorTemplate: floorTemplate);
			
			// Return the result set.
			return new CrawlerFloorResultSet() {
				AllTiles = spawnedTiles
			};

		}
		/// <summary>
		/// Generate a map based on the information contained within the passed template.
		/// </summary>
		/// <param name="floorTemplate">The template to use in map generation.</param>
		private List<CrawlerFloorTile> GenerateFloor(CrawlerFloorTemplate floorTemplate) {
			// Cascade down.
			return this.GenerateFloor(floorParams: floorTemplate.GenerateFloorParams());
		}
		/// <summary>
		///  Generate a map based on the parameters specified.
		/// </summary>
		/// <param name="floorParams">The parameters to use in building this map.</param>
		private List<CrawlerFloorTile> GenerateFloor(CrawlerFloorParams floorParams) {
			// Use the version of this function that can place spawned prefabs into a parent transform.
			return this.GenerateFloor(
				floorParams: floorParams, 
				parentTransform: this.spawnParentTransform);
		}
		/// <summary>
		///  Generate a map based on the parameters specified.
		/// </summary>
		/// <param name="floorParams">The parameters to use in building this map.</param>
		/// <param name="parentTransform"></param>
		private List<CrawlerFloorTile> GenerateFloor(CrawlerFloorParams floorParams, Transform parentTransform) {

			// Clear out the existing tiles.
			this.ClearCurrentMap(parentTransform: this.spawnParentTransform);
			
			// Create a list to help keep track of the floor tiles that were created.
			List<CrawlerFloorTile> allSpawnedTiles = new List<CrawlerFloorTile>();
			
			// Iterate through each tile params...
			foreach (CrawlerMapTileParams tileParams in floorParams.AllMapTileParams) {
				
				// ...figure out the ID type...
				CrawlerTileIDType tileIDType = tileParams.TileIDType;

				// If the ID is None, that means nothing should be spawned. Continue the loop.
				if (tileIDType == CrawlerTileIDType.None) {
					continue;
				}
				
				// ...but if it's NOT None, grab the associated prefab...
				GameObject tilePrefab = floorParams.MapThemeTemplate.GetPrefab(tileIDType: tileIDType);
				
				// Do a quick debug check on the prefab that was grabbed to make sure it actually exists.
				if (tilePrefab == null) {
					throw new System.Exception("Could not get prefab for tile ID type " + tileIDType);
				}
				
				// ...and spawn it appropriately.
				List<CrawlerFloorTile> spawnedTiles = this.SpawnTilePrefab(
					xPos: tileParams.XPos,
					yPos: tileParams.YPos, 
					tileSize: floorParams.MapThemeTemplate.TileSize,
					tilePrefab: tilePrefab, 
					parentTransform: parentTransform);
				
				// Add the spawned tiles to the list that's keeping track of it.
				allSpawnedTiles.AddRange(spawnedTiles);
			}
			
			// After going through all of the tile params, return the list of the tiles that were created.
			return allSpawnedTiles;

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
		private List<CrawlerFloorTile> SpawnTilePrefab(int xPos, int yPos, float tileSize, GameObject tilePrefab, Transform parentTransform) {
			
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

			// Grab all of the floor tiles that have just been spawned and tell them their coordinates.
			List<CrawlerFloorTile> spawnedTiles = obj.GetComponents<CrawlerFloorTile>().ToList();
			foreach (var spawnedTile in spawnedTiles) {
				spawnedTile.CurrentCoordinates = new Vector2Int(x: xPos, y: yPos);
			}
			
			// Return all of the components in this game object that inherit from the floor tile class.
			// return obj.GetComponents<CrawlerFloorTile>().ToList();
			return spawnedTiles;
		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// Generates a floor based on the floor number given and the template in the sister CrawlerController component.
		/// </summary>
		/// <param name="floorNumber">The floor from the dungeon template to generate.</param>
		/// <returns></returns>
		[Button, HideInPlayMode]
		private CrawlerFloorResultSet GenerateFloor(int floorNumber) {
			// Grab the template from the CrawlerController.
			CrawlerDungeonTemplate dungeonTemplate = this.GetComponent<CrawlerController>().CurrentCrawlerTemplate;
			// Pass this along down.
			return this.GenerateFloor(dungeonTemplate: dungeonTemplate, floorNumber: floorNumber);
		}
		#endregion
		
	}
}