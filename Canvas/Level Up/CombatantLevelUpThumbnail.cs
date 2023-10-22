using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Grawly.Gauntlet;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;

namespace Grawly.UI {

	/// <summary>
	/// Just a simple class to store references to the GameObjects that make up a thumbnail on the level up screen.
	/// </summary>
	public class CombatantLevelUpThumbnail : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the visuals for this thumbnail.
		/// </summary>
		[SerializeField, TabGroup("Thumbnail", "Scene References")]
		private GameObject allVisuals;
		/// <summary>
		/// The image that shows the icon of the combatant.
		/// </summary>
		[SerializeField, TabGroup("Thumbnail", "Scene References")]
		private Image combatantIconFrontImage;
		/// <summary>
		/// The image that shows the dropshadow of the icon of the combatant.
		/// </summary>
		[SerializeField, TabGroup("Thumbnail", "Scene References")]
		private Image combatantIconDropshadowImage;
		/// <summary>
		/// The SuperTextMesh that shows the description of the level up.
		/// </summary>
		[SerializeField, TabGroup("Thumbnail", "Scene References")]
		private SuperTextMesh levelUpLabel;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Sets whether the thumbnail should be visible or not.
		/// </summary>
		/// <param name="status"></param>
		public void SetVisualsActive(bool status) {
			this.allVisuals.SetActive(status);
		}
		/// <summary>
		/// Builds the thumbnail with the results of a level up operation.
		/// </summary>
		/// <param name="levelUpResults">The results associated with a particular level up operation.</param>
		public void Build(CombatantLevelUpResults levelUpResults) {
			// Get the icons from the combatant and set them on the thumbnail.
			this.combatantIconFrontImage.sprite = levelUpResults.combatant.Icon;
			this.combatantIconDropshadowImage.sprite = levelUpResults.combatant.Icon;
			// Set the level up text.
			this.levelUpLabel.Text = this.GetLevelUpLabelText(oldLevel: levelUpResults.oldLevel, newLevel: levelUpResults.newLevel);
			// Fade out the icon a bit if no level up occured.
			this.combatantIconFrontImage.color = (levelUpResults.LeveledUp == true) ? Color.white : Color.gray;
		}
		/// <summary>
		/// Gets the text that shows the old/new levels beneath the combatant.
		/// </summary>
		/// <param name="oldLevel">The combatant's old level.</param>
		/// <param name="newLevel">The combatant's new level.</param>
		/// <returns></returns>
		private string GetLevelUpLabelText(int oldLevel, int newLevel) {
			// Return a formatted string if a level up occured.
			if (newLevel > oldLevel) {
				return "<size=32>Lv</size><size=40>" + oldLevel.ToString() + "</size>   >   <size=52>Lv</size><size=64>" + newLevel.ToString() + "</size>";
			} else {
				return "";
			}
			
		}
		#endregion

	}

}