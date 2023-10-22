using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Encapsulates the way a badge's current level is displayed on a list item.
	/// </summary>
	public class BadgeListItemExperienceInfo : MonoBehaviour {

		#region FIELDS - STATE
		
		#endregion
		
		#region FIELDS - SCENE REFERENCES : OBJECTS
		/// <summary>
		/// Contains all relevant objects as children.
		/// </summary>
		[SerializeField, TabGroup("Experience", "Scene References"), Title("GameObjects")]
		private GameObject allObjects;
		#endregion

		#region FIELDS - SCENE REFERENCES : LABELS
		/// <summary>
		/// The label that displays the text representing the badge's current level.
		/// </summary>
		[SerializeField, TabGroup("Experience", "Scene References"), Title("Labels")]
		private SuperTextMesh badgeLevelLabel;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image that is used for the backing of the fill for the level meter's front.
		/// </summary>
		[SerializeField, TabGroup("Experience", "Scene References"), Title("Images")]
		private Image badgeLevelBackingFrontImage;
		/// <summary>
		/// The image that is used for the backing of the fill for the level meter's dropshadow.
		/// </summary>
		[SerializeField, TabGroup("Experience", "Scene References")]
		private Image badgeLevelBackingDropshadowImage;
		/// <summary>
		/// The image that is used for the actual fill for the badge's level.
		/// </summary>
		[SerializeField, TabGroup("Experience", "Scene References")]
		private Image badgeLevelFillImage;
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Sets the visuals on this list item to be highlighted.
		/// </summary>
		/// <param name="badge">The badge containing the info to represent.</param>
		public void Highlight(Badge badge) {
			// TODO: ADD THIS
			// throw new NotImplementedException("Add this!");
		}
		/// <summary>
		/// Sets the visuals on this list item to be dehighlighted.
		/// </summary>
		/// <param name="badge">The badge containing the info to represent.</param>
		public void Dehighlight(Badge badge) {
			// TODO: ADD THIS
			// throw new NotImplementedException("Add this!");
		}
		#endregion
		
	}
}