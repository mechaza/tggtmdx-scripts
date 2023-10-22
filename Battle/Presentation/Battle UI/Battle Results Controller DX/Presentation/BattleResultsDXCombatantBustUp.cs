using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using System.Linq;
using Grawly.Battle.Functions;
using Grawly.UI;
using UnityEngine.EventSystems;
using Grawly.Battle.Analysis;
using Grawly.UI.Legacy;

namespace Grawly.Battle.Results {
	
	/// <summary>
	/// Shows a graphic of a combatant after the battle is complete.
	/// </summary>
	public class BattleResultsDXCombatantBustUp : MonoBehaviour {
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the objects as children.
		/// </summary>
		[SerializeField, TabGroup("Bust", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The main pivot to move this bust up around on.
		/// </summary>
		[SerializeField, TabGroup("Bust", "Scene References")]
		private RectTransform mainPivot;
		/// <summary>
		/// The RectTransform that moves the front image on the bust up around.
		/// </summary>
		[SerializeField, TabGroup("Bust", "Scene References")]
		private RectTransform frontImagePivot;
		/// <summary>
		/// The RectTransform that moves the dropshadow image on the bust up around.
		/// </summary>
		[SerializeField, TabGroup("Bust", "Scene References")]
		private RectTransform dropshadowImagePivot;
		/// <summary>
		/// The front image of the bust up.
		/// </summary>
		[SerializeField, TabGroup("Bust", "Scene References")]
		private Image bustUpFrontImage;
		/// <summary>
		/// The front image of the bust up.
		/// </summary>
		[SerializeField, TabGroup("Bust", "Scene References")]
		private Image bustUpDropshadowImage;
		#endregion

		#region UNITY CALLS
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this bustup.
		/// </summary>
		public void ResetState() {
			this.allObjects.SetActive(false);
		}
		#endregion

		#region PRESENTATION
		public void Present(BattleResultsSet resultsSet) {
			
		}
		#endregion
		
	}
	
}