using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// One of the atomic pieces that make up an actual board piece to be placed on the badge board.
	/// </summary>
	public class BadgeBoardPieceFill : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The coordinates of this fill in particular.
		/// </summary>
		[Title("Toggles"), SerializeField]
		private Vector2Int fillCoordinates = new Vector2Int();
		#endregion

		#region FIELDS - Colors
		/// <summary>
		/// The color to use when highlighting this fill.
		/// </summary>
		[Title("Colors"), SerializeField]
		private Color highlightColor = Color.red;
		#endregion
		
		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The coordinates of this fill in particular.
		/// </summary>
		public Vector2Int FillCoordinates {
			get {
				return this.fillCoordinates;
			}
		}
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the objects that make up this one as children.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The image representing the elemental icon associated with this piece.
		/// </summary>
		[SerializeField]
		private Image fillElementalIconImage;
		/// <summary>
		/// The image that should be used for the pulse effect.
		/// </summary>
		[SerializeField]
		private Image fillPulseImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Clears the fill so it is no longer visible.
		/// </summary>
		public void ClearFill() {
			// Just turn off all the objects.
			this.allObjects.SetActive(false);
		}
		#endregion
		
		#region BUILDING
		/// <summary>
		/// Builds this badge fill to be representetive of the specified elemental type.
		/// </summary>
		/// <param name="elementType">The element to associate with this fill.</param>
		public void BuildFill(ElementType elementType) {
			// Stop the pulse if one is going. I actually hope this works.
			this.StopPulseAnimation();
			// Turn on all of the objects.
			this.allObjects.SetActive(true);
			// Set the override sprite on the elemental icon.
			this.fillElementalIconImage.overrideSprite = DataController.GetDefaultElementalIcon(elementType);			
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Plays the pulse animation on this piece fill.
		/// </summary>
		public void PlayPulseAnimation() {
			this.fillPulseImage.color = this.highlightColor;
		}
		/// <summary>
		/// Stops the pulse animation on this piece fill.
		/// </summary>
		public void StopPulseAnimation() {
			this.fillPulseImage.color = Color.clear;
		}
		#endregion
		
	}
}