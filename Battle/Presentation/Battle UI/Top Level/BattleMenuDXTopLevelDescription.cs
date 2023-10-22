using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Battle.BattleMenu {


	/// <summary>
	/// Describes the current top level selection.
	/// </summary>
	public class BattleMenuDXTopLevelDescription : SerializedMonoBehaviour {

		public static BattleMenuDXTopLevelDescription instance;

		#region FIELDS - TOGGLES
		/// <summary>
		/// Contains descriptions of the different top level buttons.
		/// </summary>
		[Title("Toggles"), SerializeField]
		private Dictionary<BattleMenuDXTopLevelSelectionType, string> descriptionDict = new Dictionary<BattleMenuDXTopLevelSelectionType, string>();
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the visuals for this thingy.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allVisuals;
		/// <summary>
		/// The STM that represents the description tag.
		/// </summary>
		[SerializeField]
		private SuperTextMesh descriptionTextLabel;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region DISPLAY
		/// <summary>
		/// Displays the information relevant to the given top level type.
		/// </summary>
		/// <param name="topLevelType">The type of selection being highlighted..</param>
		public void Display(BattleMenuDXTopLevelSelectionType topLevelType) {
			// Use the associated string stored in the dictionary.
			this.Display(descriptionText: this.descriptionDict[topLevelType] ?? "NO DESCRIPTION AVAILABLE");
		}
		/// <summary>
		/// Displays the passed in information on the description.
		/// </summary>
		/// <param name="descriptionText">Description text.</param>
		public void Display(string descriptionText) {
			this.allVisuals.SetActive(true);
			this.descriptionTextLabel.Text = descriptionText;
		}
		/// <summary>
		/// Hides all the visuals.
		/// </summary>
		public void Hide() {
			this.allVisuals.SetActive(false);
		}
		#endregion

	}


}