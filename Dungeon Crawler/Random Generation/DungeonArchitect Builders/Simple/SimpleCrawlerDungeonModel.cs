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
	/// A container class to make it easy to transfer parameters required for dungeon models.
	/// </summary>
	/// <remarks>
	/// The only reason this exists is because I didn't want to modify the model
	/// from inside GenerateLevelLayout() in the builder class.
	/// </remarks>
	public class SimpleCrawlerDungeonModelParams {
		public SimpleCrawlerTileStateType[,] TileStates { get; set; }
	}
	
	/// <summary>
	/// The model to use when building the basic crawler dungeon.
	/// </summary>
	public class SimpleCrawlerDungeonModel : DungeonModel {
		
		#region FIELDS - STATE
		[HideInInspector]
		public SimpleCrawlerDungeonConfig Config;
		[HideInInspector]
		public SimpleCrawlerTileStateType[,] tileStates;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Preps this model with the parameters passed in.
		/// </summary>
		/// <param name="simpleModelParams">The parameters to use in preparing this model.</param>
		public void Prepare(SimpleCrawlerDungeonModelParams simpleModelParams) {
			// Clone the tile states from the parameters and use those.
			this.tileStates = (SimpleCrawlerTileStateType[,])simpleModelParams.TileStates.Clone();
		}
		#endregion
		
	}

}
