using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;
using Grawly.Battle.Equipment.Badges.Behaviors;
using Sirenix.Utilities;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Grawly.Battle.Equipment.Badges {

	/// <summary>
	/// Contains the data needed to initialize a badge.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Badges/Badge Template")]
	public class BadgeTemplate : SerializedScriptableObject, IBadgeIDHaver {

		#region PROPERTIES - IBADGEIDHAVER
		/// <summary>
		/// The ID that helps to identify the badge being represented by this class.
		/// </summary>
		public BadgeID BadgeID {
			get {
				// Generate a new ID from the raw ID stored in this template.
				return new BadgeID() {
					IDNumber = this.RawIDNumber
				};
			}
		}
		#endregion
		
		#region FIELDS - GENERAL
		/// <summary>
		/// The name of this badge.
		/// </summary>
		[OdinSerialize, TitleGroup("General", order: 101)]
		public string BadgeName { get; set; } = "";
		/// <summary>
		/// The "raw" ID number that should be used in creating the proper BadgeID.
		/// </summary>
		[OdinSerialize, TitleGroup("General"), ReadOnly]
		private int RawIDNumber { get; set; } = 0;
		/// <summary>
		/// The elemental type to use for this badge.
		/// </summary>
		[OdinSerialize, TitleGroup("General")]
		public ElementType ElementType { get; private set; } = ElementType.None;
		/// <summary>
		/// Should a custom sprite be used for this badge?
		/// The default is to use the associated elemental icon.
		/// </summary>
		[OdinSerialize, TitleGroup("General")]
		private bool UseCustomSprite { get; set; } = false;
		/// <summary>
		/// The sprite attached to this badge.
		/// </summary>
		[OdinSerialize, TitleGroup("General"), PreviewField, ShowIf("UseCustomSprite")]
		private Sprite CustomBadgeSprite { get; set; }
		#endregion

		#region PROPERTIES - GENERAL
		/// <summary>
		/// The sprite to use for this badge.
		/// By default, it will be associated with the elemental icon,
		/// but some badges can have custom sprites.
		/// </summary>
		public Sprite BadgeSprite {
			get {
				// If using a custom sprite, return that.
				if (this.UseCustomSprite == true) {
					return this.CustomBadgeSprite;
				} else {
					// If using the default, just grab the elemental icon from the data controller.
					// TODO: Make sure this is what I want to actually be doing.
					return DataController.GetDefaultElementalIcon(elementType: this.ElementType);
				}
			}
		}
		#endregion
		
		#region FIELDS - EXPERIENCE
		/// <summary>
		/// Should a custom experience value be used for this badge instead of relying on a predetermined value?
		/// </summary>
		[SerializeField, TitleGroup("Experience", order: 201)]
		private bool useCustomMasteryExperience = false;
		/// <summary>
		/// The custom experience amount required to master, if one is defined.
		/// </summary>
		[OdinSerialize, TitleGroup("Experience"), ShowIf("useCustomMasteryExperience")]
		private int CustomExperienceAmount { get; set; } = 0;
		/// <summary>
		/// The mastery type to use for this badge.
		/// </summary>
		[OdinSerialize, TitleGroup("Experience"), HideIf("useCustomMasteryExperience")]
		private BadgeMasteryType MasteryType = BadgeMasteryType.Medium;
		#endregion

		#region PROPERTIES - EXPERIENCE
		/// <summary>
		/// The amount of experience needed to master this badge.
		/// </summary>
		public int MaxExperience {
			get {
				// Get the experience needed from the difficulty toggles.
				return GameController.Instance.DifficultyToggles.GetBadgeMasteryCeiling(badgeMasteryType: this.MasteryType);
			}
		}
		#endregion
		
		#region FIELDS - GRID
		/// <summary>
		/// The "fill" of this badge.
		/// </summary>
		[OdinSerialize, FoldoutGroup("Fill", order: 301), Title("Baseline"), TableMatrix(DrawElementMethod = "DrawColoredEnumElement"), OnValueChanged("RefreshFillRotations")]
		private BadgeFillType[,] RawBadgeFill { get; set; } = new BadgeFillType[3, 3];
		/// <summary>
		/// A cache storing the 90-degree rotation of the raw badge fill.
		/// </summary>
		[OdinSerialize, FoldoutGroup("Fill", order: 302), Title("90 Degrees"), TableMatrix(DrawElementMethod = "DrawColoredEnumElement"), ReadOnly]
		private BadgeFillType[,] RawBadgeFillRotation90Cache { get; set; } = new BadgeFillType[3, 3];
		/// <summary>
		/// A cache storing the 180-degree rotation of the raw badge fill.
		/// </summary>
		[OdinSerialize, FoldoutGroup("Fill", order: 303), Title("180 Degrees"), TableMatrix(DrawElementMethod = "DrawColoredEnumElement"), ReadOnly]
		private BadgeFillType[,] RawBadgeFillRotation180Cache { get; set; } = new BadgeFillType[3, 3];
		/// <summary>
		/// A cache storing the 270-degree rotation of the raw badge fill.
		/// </summary>
		[OdinSerialize, FoldoutGroup("Fill", order: 304), Title("270 Degrees"), TableMatrix(DrawElementMethod = "DrawColoredEnumElement"), ReadOnly]
		private BadgeFillType[,] RawBadgeFillRotation270Cache { get; set; } = new BadgeFillType[3, 3];
		#endregion

		#region PROPRETIES - GRID (FILLS)
		
		#endregion
		
		#region PROPERTIES - GRID (GENERAL)
		/// <summary>
		/// A list of coordinates in the badge's local space where its filled.
		/// </summary>
		public List<Vector2Int> BaseLocalFillPositions {
			get {

				// Get the list of positions with no transformations applied.
				return this.GetLocalFillPositions(
					rotationType: BadgeRotationType.Twelve,
					flipType: BadgeFlipType.Normal);

				/*// Create a new placeholder list.
				List<Vector2Int> fillList = new List<Vector2Int>();
				
				// Go through the fill matrix and add coordinates that arent empty.
				for (int xPos = 0; xPos < this.FillWidth; xPos++) {
					for (int yPos = 0; yPos < this.FillLength; yPos++) {
						BadgeFillType fillType = this.BadgeFill[xPos, yPos];
						if (fillType == BadgeFillType.Filled) {
							fillList.Add(new Vector2Int(x: xPos, y: yPos));
						}
					}
				}
				
				// When done iterating, return the list.
				return fillList;*/

			}
		}
		#endregion
		
		#region FIELDS - BEHAVIORS
		/// <summary>
		/// The behaviors that dictate how this badge template operates.
		/// </summary>
		[OdinSerialize, TitleGroup("Behaviors", order: 401), ListDrawerSettings(Expanded = true)]
		public List<BadgeBehavior> BadgeBehaviors { get; private set; } = new List<BadgeBehavior>();
		#endregion

		#region GETTERS - FILLS
		/// <summary>
		/// Gets the list of local fill positions with the given rotation/flips applied.
		/// </summary>
		/// <param name="rotationType"></param>
		/// <param name="flipType"></param>
		/// <returns></returns>
		public List<Vector2Int> GetLocalFillPositions(BadgeRotationType rotationType, BadgeFlipType flipType) {
			
			// Create a new placeholder list.
			List<Vector2Int> fillList = new List<Vector2Int>();
			
			// Grab the grid that needs to be used.
			BadgeFillType[,] targetFillGrid = this.GetBadgeFillGrid(
				rotationType: rotationType,
				flipType: flipType);
			
			// Go through the fill matrix and add coordinates that arent empty.
			for (int xPos = 0; xPos < targetFillGrid.GetLength(0); xPos++) {
				for (int yPos = 0; yPos < targetFillGrid.GetLength(1); yPos++) {
					BadgeFillType fillType = targetFillGrid[xPos, yPos]; // BadgeFillType fillType = this.BadgeFill[xPos, yPos];
					if (fillType == BadgeFillType.Filled) {
						fillList.Add(new Vector2Int(x: xPos, y: yPos));
					}
				}
			}
				
			// When done iterating, return the list.
			return fillList;
			
		}
		/// <summary>
		/// Gets the fill grid with the specified rotation/flip transformations applied.
		/// </summary>
		/// <param name="rotationType"></param>
		/// <param name="flipType"></param>
		/// <returns></returns>
		private BadgeFillType[,] GetBadgeFillGrid(BadgeRotationType rotationType, BadgeFlipType flipType) {
			// Return a different grid depending on what rotation is specified.
			switch (rotationType) {
				case BadgeRotationType.Twelve:
					return this.RawBadgeFill;
				case BadgeRotationType.Three:
					return this.RawBadgeFillRotation90Cache;
				case BadgeRotationType.Six:
					return this.RawBadgeFillRotation180Cache;
				case BadgeRotationType.Nine:
					return this.RawBadgeFillRotation270Cache;
				default:
					throw new System.Exception("This should never be reached!");
			}
		}
		#endregion

		#region GETTERS - SIZES/PIVOTS
		/// <summary>
		/// Get the position of the fill pivot with the provided rotation/flip transformations applied.
		/// </summary>
		/// <param name="rotationType"></param>
		/// <param name="flipType"></param>
		/// <returns></returns>
		public Vector2Int GetFillPivotPosition(BadgeRotationType rotationType, BadgeFlipType flipType) {

			// Get the width/length of the fill grid with the transformations applied.
			int finalWidth = this.GetFillWidth(rotationType, flipType);
			int finalLength = this.GetFillLength(rotationType, flipType);
			
			// Return a new vector with those applied.
			return new Vector2Int(x: finalWidth, y: finalLength);
			
		}
		/// <summary>
		/// Get the width of the fill pivot with the provided rotation/flip transformations applied.
		/// </summary>
		/// <param name="rotationType"></param>
		/// <param name="flipType"></param>
		/// <returns></returns>
		public int GetFillWidth(BadgeRotationType rotationType, BadgeFlipType flipType) {
			// Figure out the appropriate grid to use based on the transformations passed in and get the coorect dimension.
			return this.GetBadgeFillGrid(rotationType: rotationType, flipType: flipType).GetLength(0);
		}
		/// <summary>
		/// Get the length of the fill pivot with the provided rotation/flip transformations applied.
		/// </summary>
		/// <param name="rotationType"></param>
		/// <param name="flipType"></param>
		/// <returns></returns>
		public int GetFillLength(BadgeRotationType rotationType, BadgeFlipType flipType) {
			// Figure out the appropriate grid to use based on the transformations passed in and get the coorect dimension.
			return this.GetBadgeFillGrid(rotationType: rotationType, flipType: flipType).GetLength(1);
		}
		#endregion
		
		#region UTILITIES
		/// <summary>
		/// Creates a badge from this template.
		/// </summary>
		/// <returns>A new badge with this template insertted into it.</returns>
		public Badge ComputeDefaultBadge() {
			return new Badge(badgeTemplate: this, initialExperience: 0);
		}
		/// <summary>
		/// Rotates the badge fill grid by 90 degrees.
		/// </summary>
		/// <param name="originalGrid"></param>
		/// <returns></returns>
		private BadgeFillType[,] RotateBadgeFillGrid(BadgeFillType[,] originalGrid) {
			// https://stackoverflow.com/questions/42519/how-do-you-rotate-a-two-dimensional-array
			BadgeFillType[,] ret = new BadgeFillType[3, 3];

			for (int i = 0; i < 3; ++i) {
				for (int j = 0; j < 3; ++j) {
					ret[i, j] = originalGrid[3 - j - 1, i];
				}
			}

			return ret;
		}
		#endregion

		#region EDITOR HELPERS
		/// <summary>
		/// Sets the raw ID.
		/// ONLY USE THIS IN EDITOR PROGRAMMING.
		/// </summary>
		/// <param name="rawIDNumber"></param>
		public void SetRawID(int rawIDNumber) {
			if (Application.isEditor == false) {
				throw new System.Exception("This should not be called anywhere other than the editor!");
			}
			this.RawIDNumber = rawIDNumber;
		}
		#endregion
		
		#region ODIN HELPERS - ROUTINES : GENERAL
		[TitleGroup("Grid", order: 503), Button]
		private void RefreshFillRotationsButton() {
			// Cascade down.
			this.RefreshFillRotations(value: this.RawBadgeFill);
		}
		/// <summary>
		/// Creates a cache of the rotated fill grids.
		/// </summary>
		/// <param name="value"></param>
		private void RefreshFillRotations(BadgeFillType[,] value) {
			// Rotate the caches. It can be assumed the value passed in is the raw fill itself.
			this.RawBadgeFillRotation90Cache = this.RotateBadgeFillGrid(originalGrid: value);
			this.RawBadgeFillRotation180Cache = this.RotateBadgeFillGrid(originalGrid: this.RawBadgeFillRotation90Cache);
			this.RawBadgeFillRotation270Cache = this.RotateBadgeFillGrid(originalGrid: this.RawBadgeFillRotation180Cache);
		}
		
		[TitleGroup("Grid", order: 504), Button]
		private void RenameAssetFile() {
#if UNITY_EDITOR
			string currentAssetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
			string currentAssetFolder = currentAssetPath.Replace(Path.GetFileName(currentAssetPath), "").TrimEnd('/');
			AssetDatabase.MoveAsset(oldPath: currentAssetPath, newPath: currentAssetFolder + "/" + this.RawIDNumber + " - " + this.BadgeName + ".asset");
			AssetDatabase.Refresh();
#endif
		}
		/// <summary>
		/// Generates a new fill for this badge template.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		[TitleGroup("Grid", order: 501), Button]
		private void GenerateNewFill(int width, int height) {
			this.RawBadgeFill = new BadgeFillType[width, height];
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					this.RawBadgeFill[i, j] = BadgeFillType.Empty;
				}
			}
		}
		private static BadgeFillType DrawColoredEnumElement(Rect rect, BadgeFillType value) {

			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
				int typesCount = System.Enum.GetValues(typeof(BadgeFillType)).Length;
				value = (BadgeFillType) (((int) value + 1) % typesCount);
				// value = !value;
				GUI.changed = true;
				Event.current.Use();
			}

#if UNITY_EDITOR
			bool isFilled = value == BadgeFillType.Filled;
			UnityEditor.EditorGUI.DrawRect(rect.Padding(1), isFilled ? new Color(0.1f, 0.8f, 0.2f) : new Color(0, 0, 0, 0.5f));
#endif

			return value;
		}
		#endregion
		
	
		
	}


}
