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
	/// A representation of an entire badge's shape and size as it exists on the badge board.
	/// </summary>
	public class BadgeBoardPiece : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The badge placement that was assigned to this board piece when it was generated.
		/// </summary>
		public BadgePlacement AssignedBadgePlacement { get; private set; }
		#endregion
		
		#region PROPERTIES - STATE
		/// <summary>
		/// Is this piece currently in use?
		/// </summary>
		public bool IsInUse {
			get {
				// If an ID has been assigned to this piece, that means its in use.
				return this.AssignedBadgePlacement != null;
			}
		}
		#endregion
		
		#region FIELDS - SCENE REFERENCES : GENERAL
		/// <summary>
		/// Contains all of the objects that make up this piece as children.
		/// </summary>
		[SerializeField, TabGroup("Piece", "Scene References"), Title("General")]
		private GameObject allObjects;
		/// <summary>
		/// The RectTransform that should be manipulated when placing this piece onto the board.
		/// </summary>
		[SerializeField, TabGroup("Piece", "Scene References")]
		private RectTransform targetRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : COMPONENTS
		/// <summary>
		/// A list of the fills that are used to build this board piece.
		/// </summary>
		[SerializeField, TabGroup("Piece", "Scene References"), Title("Components")]
		private List<BadgeBoardPieceFill> pieceFills = new List<BadgeBoardPieceFill>();
		#endregion
		
		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The RectTransform that should be manipulated when placing this piece onto the board.
		/// </summary>
		public RectTransform TargetRectTransform => this.targetRectTransform;
		#endregion
		
		#region GETTERS - FILLS
		/// <summary>
		/// Gets all of the fills that are used by the specified badge placement.
		/// </summary>
		/// <param name="badgePlacement"></param>
		/// <returns></returns>
		private List<BadgeBoardPieceFill> GetUsedFills(BadgePlacement badgePlacement) {
			// Probe the placement for the local fill coordinates and return the fills that coorespond to those locations.
			return badgePlacement.LocalFillCoordinates
				.Select(v => this.GetPieceFill(localFillPos: v))
				.ToList();
		}
		/// <summary>
		/// Gets the piece fill located at the specified local coordinates.
		/// </summary>
		/// <param name="localFillPos">The position of the desired fill in local space.</param>
		/// <returns>The fill located at the provided position, in local space.</returns>
		private BadgeBoardPieceFill GetPieceFill(Vector2Int localFillPos) {
			// Find the first fill whose coordinates match the ones passed in.
			return this.pieceFills.First(f => f.FillCoordinates == localFillPos);
		}
		#endregion
		
		#region BUILDING
		/// <summary>
		/// Clears the board piece of any visual representation it might have in association to an actual Badge.
		/// </summary>
		public void ClearPiece() {
			
			// Null out the badge placement assigned to this piece.
			this.AssignedBadgePlacement = null;
			
			// Go through all the fills and clear them out as well.
			this.pieceFills.ForEach(f => f.ClearFill());
			
			// Hide the piece's visuals.
			this.HidePiece();
			
		}
		/// <summary>
		/// Generates the board piece based on the badge and its orientation that is specified in the parameters.
		/// </summary>
		/// <param name="badgePlacement">The class containing the badge as well as its orientation.</param>
		public void BuildPiece(BadgePlacement badgePlacement) {
			
			// Unhide the piece.
			this.UnhidePiece();
			
			// Save the placement assigned to this piece.
			this.AssignedBadgePlacement = badgePlacement;
			
			// Get all of the fills that should be used and build them out.
			List<BadgeBoardPieceFill> usedFills = this.GetUsedFills(badgePlacement: badgePlacement);
			foreach (BadgeBoardPieceFill usedFill in usedFills) {
				usedFill.BuildFill(elementType: badgePlacement.Badge.ElementType);
			}

		}
		#endregion

		#region PRESENTATION - HIDING
		/// <summary>
		/// Makes this piece disappear temporarily while the crane is manipulating the dummy piece.
		/// </summary>
		public void HidePiece() {
			this.allObjects.SetActive(false);
		}
		/// <summary>
		/// Makes this piece reappear after being manipulated by the crane.
		/// </summary>
		public void UnhidePiece() {
			this.allObjects.SetActive(true);
		}
		#endregion
		
		#region PRESENTATION - PULSE ANIMATION
		/// <summary>
		/// Begins the animation that plays when this piece should be "pulsing" on the board.
		/// Usually when its being moved or highlighted in the selection list.
		/// </summary>
		public void BeginPulseAnimation() {
			// Go through each piece fill and play the pulse animation.
			foreach (BadgeBoardPieceFill pieceFill in this.pieceFills) {
				pieceFill.PlayPulseAnimation();
			}
		}
		/// <summary>
		/// Stops the animation that plays when the piece was previously pulsing.
		/// </summary>
		public void StopPulseAnimation() {
			// Go through each piece fill and play the pulse animation.
			foreach (BadgeBoardPieceFill pieceFill in this.pieceFills) {
				pieceFill.StopPulseAnimation();
			}
		}
		#endregion
		
	}
}