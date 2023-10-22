using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

namespace Grawly.Dungeon.UI {

	/// <summary>
	/// A map. Yeah.
	/// </summary>
	public class FullMapDX : MapDX {

		public static FullMapDX instance;

		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		private void OnDestroy() {
			instance = null;
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Displays the map.
		/// </summary>
		public override void DisplayMap() {
			this.AllVisuals.SetActive(true);
		}
		/// <summary>
		/// Dismisses the map.
		/// </summary>
		public override void DismissMap() {
			this.AllVisuals.SetActive(false);
		}
		#endregion

	}


}