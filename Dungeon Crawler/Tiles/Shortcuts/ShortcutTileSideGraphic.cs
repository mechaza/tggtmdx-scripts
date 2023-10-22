using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Dungeon;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// A graphic to display on the side of a shortcut tile.
	/// Helps signal if a side is an entrance/exit, and if it is able to be passed through.
	/// </summary>
	public class ShortcutTileSideGraphic : MonoBehaviour {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The side of the shortcut tile this graphic is attached to.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private ShortcutSideType shortcutSideType = ShortcutSideType.None;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The side of the shortcut tile this graphic is attached to.
		/// </summary>
		public ShortcutSideType ShortcutSideType => this.shortcutSideType;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The ShortcutTile that this tile side belongs to.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private ShortcutTile parentShortcutTile;
		/// <summary>
		/// Contains both the open/closed graphics as children.
		/// </summary>
		/// <remarks>
		/// If this side of the shortcut is used as either entrance or exit, this is enabled.
		/// If this side is not used, its turned off completely.
		/// </remarks>
		[SerializeField]
		private GameObject allGraphics;
		/// <summary>
		/// The GameObject containing the graphics to display when this side of the shortcut is open.
		/// </summary>
		[SerializeField]
		private GameObject shortcutOpenGraphic;
		/// <summary>
		/// The GameObject containing the graphics to display when this side of the shortcut is closed.
		/// </summary>
		[SerializeField]
		private GameObject shortcutClosedGraphic;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The ShortcutTile that this tile side belongs to.
		/// </summary>
		public ShortcutTile ParentShortcutTile => this.parentShortcutTile;
		#endregion

		#region FIELDS - RESOURCES 
		
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this graphic.
		/// </summary>
		public void ResetState() {
			// Just turn off all the graphics.
			this.allGraphics.SetActive(false);
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Sets the graphic to reflect the status of the shortcut tile its attached to.
		/// </summary>
		/// <param name="entranceSide">The side of the shortcut tile that should be entered.</param>
		/// <param name="exitSide">The side of the shortcut tile that will be exited.</param>
		/// <param name="isOpen">Is the pathway between the entrance and exit open?</param>
		public void SetShortcutGraphic(ShortcutSideType entranceSide, ShortcutSideType exitSide, bool isOpen) {

			// Figure out if this side of the shortcut is used at all and set the visibility based on that.
			bool isUsed = (this.ShortcutSideType == entranceSide || this.ShortcutSideType == exitSide);
			this.allGraphics.SetActive(isUsed);
			
			// If not used, just back out.
			if (isUsed == false) {
				return;
			}

			// Determine if open graphics should be used. This happens if this is the entrance or if the path is open.
			bool useOpenGraphics = (this.ShortcutSideType == entranceSide) || isOpen == true;
			// Set the visibility based on that.
			this.shortcutOpenGraphic.SetActive(value: useOpenGraphics);
			this.shortcutClosedGraphic.SetActive(value: !useOpenGraphics);
			
		}
		#endregion

		#region INTERFACE IMPLEMENTATION
		
		#endregion
		
	}
	
}