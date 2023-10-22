﻿using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Dungeon.UI;
using UnityEngine;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// The minimap for the dungeon crawler.
	/// </summary>
	public class CrawlerFullMap : MapDX {

		private static CrawlerFullMap instance { get; set; }
		public static CrawlerFullMap Instance {
			get {
				if (instance == null) {
					return null;
				} else {
					return instance;
				}
			}
		}
		
		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		private void Start() {
			this.DismissMap();
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Displays the map.
		/// </summary>
		public override void DisplayMap() {
			this.AllVisuals.SetActive(true);
			// Doing this because there's some weird error I don't know if I have time to debug.
			/*try {
				this.AllVisuals.SetActive(true);
			} catch (System.Exception e) {
				Debug.LogError("There was an error displaying the map! " + e.Message);
			}*/
			
		}
		/// <summary>
		/// Dismisses the map.
		/// </summary>
		public override void DismissMap() {
			this.AllVisuals.SetActive(false);
			/*try {
				this.AllVisuals.SetActive(false);
			} catch (System.Exception e) {
				Debug.LogError("There was an error dismissing the map! " + e.Message);
			}*/
		}
		#endregion
		
	}

}