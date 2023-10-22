using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI.Legacy;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Builders.GridFlow.Tilemap;

namespace Grawly.DungeonCrawler.Generation.Simple {
	/// <summary>
	/// The builder that actually handles building the simple crawler dungeon.
	/// </summary>
	public class SimpleCrawlerDungeonBuilder : DungeonBuilder {
		#region FIELDS - STATE : DUNGEON GENERATION
		/// <summary>
		/// The configuration for this simple crawler builder.
		/// </summary>
		private SimpleCrawlerDungeonConfig SimpleConfig { get; set; }
		/// <summary>
		/// The model for this simple crawler builder.
		/// </summary>
		private SimpleCrawlerDungeonModel SimpleModel { get; set; }
		#endregion

		#region FIELDS - STATE : RESOURCES
		/// <summary>
		/// The RNG for this builder.
		/// </summary>
		private System.Random RNG { get; set; }
		#endregion

		#region OVERRIDES : DUNGEON BUILDER
		/// <summary>
		/// Builds the dungeon layout.  In this method, you should build your dungeon layout and save it in your model file
		/// No markers should be emitted here.   (EmitMarkers function will be called later by the engine to do that)
		/// </summary>
		/// <param name="config">The builder configuration</param>
		/// <param name="model">The dungeon model that the builder will populate</param>
		public override void BuildDungeon(DungeonConfig config, DungeonModel model) {
			base.BuildDungeon(config, model);

			// Create a new random object using the seed in the config.
			this.RNG = new System.Random((int) config.Seed);

			// Save the config and model for later. Make sure to give the model a reference to the config too.
			this.SimpleConfig = config as SimpleCrawlerDungeonConfig;
			this.SimpleModel = model as SimpleCrawlerDungeonModel;
			this.SimpleModel.Config = this.SimpleConfig;

			// Generate the layout for the level and use it to prepare the model.
			var simpleModelParams = this.GenerateLevelLayout(config: this.SimpleConfig);
			this.SimpleModel.Prepare(simpleModelParams: simpleModelParams);
		}
		/// <summary>
		/// Override the builder's emit marker function to emit our own markers based on the layout that we built
		/// You should emit your markers based on the layout you have saved in the model generated previously
		/// When the user is designing the theme interactively, this function will be called whenever the graph state changes,
		/// so the theme engine can populate the scene (BuildDungeon will not be called if there is no need to rebuild the layout again)
		/// </summary>
		public override void EmitMarkers() {
			base.EmitMarkers();
			this.EmitLevelMarkers(model: this.SimpleModel, config: this.SimpleConfig);
			this.ProcessMarkerOverrideVolumes();
		}
		#endregion

		#region GENERATION : MAIN
		/// <summary>
		/// Generate the level layout based on the configuration provided.
		/// </summary>
		/// <param name="config">The configuration to use in generating this simple dungeon.</param>
		/// <returns>Parameters to use in building the model for this dungeon.</returns>
		private SimpleCrawlerDungeonModelParams GenerateLevelLayout(SimpleCrawlerDungeonConfig config) {
			// Create and initialize a 2D array containing what tiles have been visited.
			var visited = new bool[config.mazeWidth, config.mazeHeight];
			for (int x = 0; x < config.mazeWidth; x++) {
				for (int y = 0; y < config.mazeHeight; y++) {
					visited[x, y] = false;
				}
			}

			// Create a stack to keep track of the current run and visit the first tile.
			Stack<IntVector2> stack = new Stack<IntVector2>();
			var startPoint = new IntVector2(0, 2 + this.RNG.Next() % (config.mazeHeight - 4));
			visited[startPoint.x, startPoint.y] = true;
			stack.Push(startPoint);

			// For the current run, 
			while (stack.Count > 0) {
				// Get the point at the top of the stack.
				IntVector2 currentPoint = stack.Peek();

				// See if another neighbor can be found. 
				IntVector2 nextPoint;
				if (this.GetNextNeighbor(currentPoint: currentPoint, nextPoint: out nextPoint, visited: visited)) {
					// If a neighbor is found, visit it and push it onto the stack.
					visited[nextPoint.x, nextPoint.y] = true;
					stack.Push(nextPoint);
				} else {
					// If no neighbor is found, go backwards in the run by popping the stack.
					stack.Pop();
				}
			}

			// Initialize some simple model params, then prep its tile states with the visited created eariler.
			SimpleCrawlerDungeonModelParams simpleModelParams = new SimpleCrawlerDungeonModelParams();
			simpleModelParams.TileStates = new SimpleCrawlerTileStateType[config.mazeWidth, config.mazeHeight];
			for (int x = 0; x < config.mazeWidth; x++) {
				for (int y = 0; y < config.mazeHeight; y++) {
					// For this (x, y) coordinate, if it was visited, its blocked.
					SimpleCrawlerTileStateType targetState = visited[x, y] ? SimpleCrawlerTileStateType.Empty : SimpleCrawlerTileStateType.Blocked;
					// Pass this into the params.
					simpleModelParams.TileStates[x, y] = targetState;
				}
			}

			// Return the params.
			return simpleModelParams;
		}
		#endregion

		#region GENERATION : VISITED AND NEIGHBOR CHECKS
		/// <summary>
		/// Checks if a provided (x, y) coordinate has been visted inside of a provided 2D Array.
		/// </summary>
		/// <param name="visited">A 2D array containing which tiles have or haven't been visited already.</param>
		/// <param name="x">The x-coordinate of the point to check.</param>
		/// <param name="y">The y-coordinate of the point to check.</param>
		/// <returns>Whether or not the (x, y) coordinate provided has been visited.</returns>
		private bool IsVisited(bool[,] visited, int x, int y) {
			
			// Return true if its beyond the borders.
			if (x < 0 || y < 0 || x >= visited.GetLength(0) || y >= visited.GetLength(1)) {
				return true;
			}
			
			// Otherwise, return the value stored in the array.
			return visited[x, y];
		}
		/// <summary>
		/// Checks if the point provided can be dug towards inside the 2D array of provided 'visited' point states.
		/// </summary>
		/// <param name="point">The point that may or may not be dug towards.</param>
		/// <param name="visited">A 2D array dictating which tiles have or haven't been visited.</param>
		/// <returns>Whether or not the provided point can be dug towards.</returns>
		private bool CanDigToPoint(IntVector2 point, bool[,] visited) {
			
			// If the provided point has already been visited, it cannot be dug towards.
			if (this.IsVisited(visited, point.x, point.y)) {
				return false;
			}
			
			// Count how many neighbors of this point have been visited.
			int neighborPathways = 0;
			neighborPathways += this.IsVisited(visited, point.x - 1, point.y + 0) ? 1 : 0;
			neighborPathways += this.IsVisited(visited, point.x + 1, point.y + 0) ? 1 : 0;
			neighborPathways += this.IsVisited(visited, point.x + 0, point.y + 1) ? 1 : 0;
			neighborPathways += this.IsVisited(visited, point.x + 0, point.y - 1) ? 1 : 0;
			
			// The provided point can only be dug to if there are two or more unvisited tiles beside it.
			return neighborPathways <= 1;
		}
		/// <summary>
		/// ???
		/// </summary>
		/// <param name="currentPoint"></param>
		/// <param name="nextPoint"></param>
		/// <param name="visited"></param>
		/// <returns></returns>
		private bool GetNextNeighbor(IntVector2 currentPoint, out IntVector2 nextPoint, bool[,] visited) {

			// Create a definition of what kinds of offsets can be used.
			var offsets = new List<IntVector2> {
				new IntVector2(-1, 0), 
				new IntVector2(1, 0),
				new IntVector2(0, -1), 
				new IntVector2(0, 1)
			};

			
			// Go through each offset in that list.
			while (offsets.Count > 0) {
				// Pluck a totally random offset from the list and remove it.
				int i = this.RNG.Next() % offsets.Count;
				var offset = offsets[i];
				offsets.RemoveAt(i);
				
				// Check if this random neighbor can be dug towards.
				if (this.CanDigToPoint(currentPoint + offset, visited)) {
					// If this neighbor can be dug towards, *assign it* to the nextPoint passed in.
					nextPoint = currentPoint + offset;
					// Return true.
					return true;
				}
				
			}

			// If there are no neighbors, *assign* the nextPoint the currentPoint, and return false.
			nextPoint = currentPoint;
			return false;
		}
		#endregion

		#region GENERATION : MARKERS
		/// <summary>
		/// Emits the markers for the level.
		/// </summary>
		/// <param name="model">The model for this dungeon.</param>
		/// <param name="config">The config for this dungeon.</param>
		private void EmitLevelMarkers(SimpleCrawlerDungeonModel model, SimpleCrawlerDungeonConfig config) {
			
			// Create a vector to help with scaling markers as they are emitted.
			var gridSize = new Vector3(config.gridSize.x, 0, config.gridSize.y);
			
			//  Go through each tile and add an emitter based on if its blocked or not.
			for (int x = 0; x < config.mazeWidth; x++) {
				for (int y = 0; y < config.mazeHeight; y++) {
					var position = Vector3.Scale(new Vector3(x + 0.5f, 0, y + 0.5f), gridSize);
					var markerTransform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
					var markerName = model.tileStates[x, y] == SimpleCrawlerTileStateType.Blocked
						? SimpleCrawlerDungeonConstants.WallBlock
						: SimpleCrawlerDungeonConstants.GroundBlock;
					this.EmitMarker(markerName, markerTransform, new IntVector(x, 0, y), -1);
				}
			}
			
		
		}
		#endregion
		
	}
}