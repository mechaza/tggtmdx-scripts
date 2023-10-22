using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;


namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// A new version of the runtime dungeon that uses DungeonArchitect instead of DunGen.
	/// </summary>
	[RequireComponent(typeof(CrawlerController))]
	public class CrawlerDungeonArchitectDungeon : MonoBehaviour {

		public static CrawlerDungeonArchitectDungeon Instance { get; private set; }

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should the runtime dungeon be generated on starting the scene?
		/// </summary>
		[Header("Toggles")]
		[SerializeField]
		private bool generateOnStart = true;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The dungeon controlling all this shit.
		/// </summary>
		[Header("Scene References")]
		[SerializeField]
		private DungeonArchitect.Dungeon runtimeDungeon;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			if (this.generateOnStart == true) {
				this.RebuildDungeon();
				/*GameController.Instance.WaitThenRun(timeToWait: 0.1f, action: () => {
					this.RebuildDungeon();
				});*/
			}
		}
		#endregion

		#region DUNGEON GENERATION
		/// <summary>
		/// Completely and totally rebuilds the dungeon at runtime.
		/// </summary>
		[Button]
		private void RebuildDungeon() {
			// this.runtimeDungeon.RequestRebuild();
			this.runtimeDungeon.Config.Seed = (uint) Random.Range(minInclusive: 0, maxExclusive: 100000);
			this.runtimeDungeon.Build();
		}
		#endregion

		#region ODIN HELPERS
	
		#endregion
		
	}
}

