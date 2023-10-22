using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.DungeonCrawler.Generation {
	
	/// <summary>
	/// Encapsulates multiple crawler map templates so that a complete dungeon can be created.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Crawler/Dungeon Template")]
	public class CrawlerDungeonTemplate : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The ID of the Crawler Dungeon assocaited with this template.
		/// </summary>
		[OdinSerialize]
		public CrawlerDungeonIDType CrawlerDungeonIDType { get; private set; } = CrawlerDungeonIDType.None;
		[OdinSerialize]
		public bool GeneratedAtRuntime { get; private set; } = true;
		/// <summary>
		/// All of the templates that should be used for this dungeon.
		/// </summary>
		[SerializeField]
		private List<CrawlerFloorTemplate> allMapTemplates = new List<CrawlerFloorTemplate>();
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets the floor template associated with the given floor number.
		/// </summary>
		/// <param name="floorNumber">The template for the provided floor number.</param>
		public CrawlerFloorTemplate GetFloorTemplate(int floorNumber) {
			
			// Make sure it's like. Actually in range.
			if (floorNumber >= this.allMapTemplates.Count) {
				throw new System.Exception("The floor asked for exceeds the number of templates available!");
			}
			
			// The floor number should line up with the index. Return that.
			return this.allMapTemplates[floorNumber];
			
		}
		#endregion
		
	}

	
	
}