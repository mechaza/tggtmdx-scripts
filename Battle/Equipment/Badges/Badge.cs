using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.UI.MenuLists;

namespace Grawly.Battle.Equipment.Badges {

	/// <summary>
	/// Contains badge behavior (from the template) and the current experience accumulated for it.
	/// </summary>
	public class Badge : IBadgeIDHaver, IMenuable {

		#region FIELDS - STATE
		/// <summary>
		/// The experience attached to this badge.
		/// </summary>
		private int badgeExperience = 0;
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// The experience attached to this badge.
		/// </summary>
		public int BadgeExperience {
			get {
				return this.badgeExperience;
			} set {
				// Make sure to clamp the experience so that it doesn't go below zero.
				// this.badgeExperience = Mathf.Max(a: 0, b: value);
				this.badgeExperience = Mathf.Clamp(
					value: value, 
					min: 0,
					max: this.BadgeTemplate.MaxExperience);
			}
		}
		/// <summary>
		/// The badge's current level. Inferred from the experience.
		/// </summary>
		public int BadgeLevel {
			get {
				throw new NotImplementedException("Add this");
			}
		}
		#endregion
		
		#region FIELDS - GENERAL
		/// <summary>
		/// The template that is used for this badge.
		/// Contains most of the behavior.
		/// </summary>
		private BadgeTemplate BadgeTemplate { get; set; }
		#endregion

		#region PROPERTIES - GENERAL
		/// <summary>
		/// The elemental type associated with this badge.
		/// </summary>
		public ElementType ElementType => this.BadgeTemplate.ElementType;
		/// <summary>
		/// All of the facts for this badge.
		/// Used to help populate the BadgeInfo.
		/// </summary>
		public List<BadgeFact> BadgeFacts {
			get {
				return this.BadgeTemplate.BadgeBehaviors		// Go through the badge behaviors...
					.Where(bb => bb is IBadgeFactHaver)			// ...find the ones that implement IBadgeFactHaver...
					.Cast<IBadgeFactHaver>()
					.SelectMany(bfc => bfc.BadgeFacts)			// Get all of their facts collectively.
					.ToList();
			}
		}
		#endregion

		#region PROPERTIES - GRID
		/// <summary>
		/// A list of coordinates in the badge's local space where its filled.
		/// </summary>
		public List<Vector2Int> BaseLocalFillPositions => this.BadgeTemplate.BaseLocalFillPositions;
		#endregion
		
		#region PROPERTIES - IBADGEIDHAVER
		/// <summary>
		/// The ID that helps to identify the badge being represented by this class.
		/// </summary>
		/// <remarks>
		/// I can just use the template's ID for this one.
		/// </remarks>
		public BadgeID BadgeID => this.BadgeTemplate.BadgeID;
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Initializes a badge.
		/// </summary>
		/// <param name="badgeTemplate">The template for this badge.</param>
		/// <param name="initialExperience">The experience to initailize the badge with.</param>
		public Badge(BadgeTemplate badgeTemplate, int initialExperience = 0) {
			this.BadgeTemplate = badgeTemplate;
			this.BadgeExperience = initialExperience;
		}
		/// <summary>
		/// Creates a badge from one that was previously saved out to disk.
		/// </summary>
		/// <param name="serializableBadge"></param>
		public Badge(SerializableBadge serializableBadge) {
			// Use the ID in the serializable badge to grab the associated template from the DataController.
			this.BadgeTemplate = DataController.GetBadgeTemplate(badgeID: serializableBadge.BadgeID);
			// Save the experience as well.
			this.BadgeExperience = serializableBadge.currentExperience;
		}
		#endregion

		#region GETTERS - FILL/GRID
		/// <summary>
		/// Get the fill size of this badge with the provided rotation/flip transformations applied.
		/// </summary>
		/// <param name="badgeTransform">The transform containing the modifications applied to this badge.</param>
		/// <returns>The fill size of this badge with the transform applied.</returns>
		public Vector2Int GetFillSize(BadgeTransform badgeTransform) {
			// Cascade down with the information stored in the badge transform.
			return this.GetFillSize(
				rotationType: badgeTransform.RotationType,
				flipType: badgeTransform.FlipType);
		}
		/// <summary>
		/// Get the fill size of this badge with the provided rotation/flip transformations applied.
		/// </summary>
		/// <param name="rotationType">The rotation type to apply to the fill positions.</param>
		/// <param name="flipType">The flip type to apply to the fill positions.</param>
		/// <returns></returns>
		public Vector2Int GetFillSize(BadgeRotationType rotationType, BadgeFlipType flipType) {
			// Pass this information off to the template.
			return this.BadgeTemplate.GetFillPivotPosition(
				rotationType: rotationType,
				flipType: flipType);
		}
		/// <summary>
		/// Returns the local fill positions after they have had the given rotation/flip transformations applied to them.
		/// </summary>
		/// <param name="rotationType">The rotation type to apply to the fill positions.</param>
		/// <param name="flipType">The flip type to apply to the fill positions.</param>
		/// <returns>A list of local fill positions after being transformed.</returns>
		public List<Vector2Int> GetTransformedLocalFillPositions(BadgeRotationType rotationType, BadgeFlipType flipType) {
			// Ask the badge template.
			return this.BadgeTemplate.GetLocalFillPositions(
				rotationType: rotationType, 
				flipType: flipType);
		}
		#endregion

		#region GETTERS - MODIFIERS
		/// <summary>
		/// Returns a list of modifiers that implement the given interface.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public List<T> GetModifiers<T>() {
			// Probe the template's behaviors for ones of the specified type.
			return this.BadgeTemplate.BadgeBehaviors
				.Where(bb => bb is T)
				.Cast<T>()
				.ToList();
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public string PrimaryString => this.BadgeTemplate.BadgeName;
		public string QuantityString => throw new System.Exception("This is never used!");
		public string DescriptionString => throw new System.Exception("This is never used!");
		public Sprite Icon => DataController.GetDefaultElementalIcon(elementType: this.ElementType);
		#endregion
		
	}


}
