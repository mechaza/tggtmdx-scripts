using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Grawly.Battle;
using Grawly.Gauntlet.Nodes;
using DG.Tweening;

namespace Grawly.Gauntlet {

	/// <summary>
	/// Basically what controls the UI for the level select in the gauntlet.
	/// </summary>
	public class GauntletMenuController : MonoBehaviour {

		public static GauntletMenuController instance;
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The class that shows the graphics for the node title.
		/// </summary>
		[SerializeField, TabGroup("Menu", "Scene References")]
		private GauntletNodeTitle nodeTitle;
		/// <summary>
		/// The class that shows the graphics for the node title.
		/// </summary>
		public GauntletNodeTitle NodeTitle {
			get {
				return this.nodeTitle;
			}
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		#endregion

	}


}