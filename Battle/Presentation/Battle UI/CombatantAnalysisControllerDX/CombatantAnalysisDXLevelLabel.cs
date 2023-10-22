using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// Manages the level label at the bottom of the screen.
	/// </summary>
	public class CombatantAnalysisDXLevelLabel : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The text mesh that displays the combatant's current level.
		/// </summary>
		[SerializeField, TabGroup("Label", "Scene References")]
		private SuperTextMesh currentLevelValueLabel;
		/// <summary>
		/// The text mesh that displays how much EXP until the next level.
		/// </summary>
		[SerializeField, TabGroup("Label", "Scene References")]
		private SuperTextMesh nextLevelValueLabel;
		/// <summary>
		/// The GameObject that serves as the parent for the level labels.
		/// </summary>
		[SerializeField, TabGroup("Label", "Scene References")]
		private GameObject currentLevelLabelsGameObject;
		/// <summary>
		/// The GameObject that serves as the parent for the next level labels
		/// </summary>
		[SerializeField, TabGroup("Label", "Scene References")]
		private GameObject nextLevelEXPLabelsGameObject;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this element.
		/// </summary>
		public void ResetState() {
			this.currentLevelValueLabel.Text = "";
			this.nextLevelValueLabel.Text = "";
			this.currentLevelLabelsGameObject.SetActive(false);
			this.nextLevelEXPLabelsGameObject.SetActive(false);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Presents this element with the parameters specified.
		/// </summary>
		/// <param name="analysisParams">The parameters that should be used to build this element.</param>
		public void Present(CombatantAnalysisParams analysisParams) {
			this.Rebuild(analysisParams: analysisParams);
		}
		/// <summary>
		/// Dismisses this element with the parameters specified.
		/// </summary>
		/// <param name="analysisParams">The parameters that should be used to build this element.</param>
		public void Dismiss(CombatantAnalysisParams analysisParams) {
			this.ResetState();
		}
		#endregion
		
		#region BUILDING
		/// <summary>
		/// Builds the level label with the parameters passed in.
		/// </summary>
		/// <param name="analysisParams">The parameters that should be used to build the label.</param>
		public void Rebuild(CombatantAnalysisParams analysisParams) {
			
			// Set the labels active based on if the analysis params calls for it or not.
			this.currentLevelLabelsGameObject.SetActive(analysisParams.CanDisplayCurrentLevel);
			this.nextLevelEXPLabelsGameObject.SetActive(analysisParams.CanDisplayNextLevelEXP);
			
			// Set the text. Yes I know that these will get built even if they're turned off.
			this.currentLevelValueLabel.Text = "" + analysisParams.CurrentCombatant.Level;
			this.nextLevelValueLabel.Text = "" + analysisParams.CurrentCombatant.ExpForNextLevel();
			
		}
		#endregion
		
	}
}