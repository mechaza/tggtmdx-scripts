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
	public abstract class MapDX : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that has all the visuals.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private GameObject allVisuals;
		/// <summary>
		/// The RawImage that displays the map.
		/// </summary>
		[SerializeField]
		private RawImage rawImage;
		/// <summary>
		/// The GameObject that has all the visuals.
		/// </summary>
		protected GameObject AllVisuals {
			get {
				return this.allVisuals;
			}
		}
		/// <summary>
		/// The RawImage that displays the map.
		/// </summary>
		protected RawImage RawImage {
			get {
				return this.rawImage;
			}
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Displays the map.
		/// </summary>
		public abstract void DisplayMap();
		/// <summary>
		/// Dismisses the map.
		/// </summary>
		public abstract void DismissMap();
		#endregion

	}


}