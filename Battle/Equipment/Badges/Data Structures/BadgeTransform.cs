using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment.Badges  {
	
	/// <summary>
	/// Contains the data regarding the coordinates and rotation of a badge.
	/// </summary>
	[System.Serializable]
	public class BadgeTransform {
		
		#region FIELDS - POSITION
		/// <summary>
		/// The X-Position of where the pivot of this badge is on a grid.
		/// </summary>
		[OdinSerialize]
		public int XPos { get; set; } = 0;
		/// <summary>
		/// The Y-Position of where the pivot of this badge is on a grid.
		/// </summary>
		[OdinSerialize]
		public int YPos { get; set; } = 0;
		#endregion

		#region FIELDS - TRANSFORMATIONS
		/// <summary>
		/// The rotation applied to this badge.
		/// </summary>
		[OdinSerialize]
		public BadgeRotationType RotationType { get; set; } = BadgeRotationType.Twelve;
		/// <summary>
		/// The flip applied to this badge.
		/// </summary>
		[OdinSerialize]
		public BadgeFlipType FlipType { get; set; } = BadgeFlipType.Normal;
		#endregion

		#region CLONING
		/// <summary>
		/// Creates a clone of this badge transform.
		/// Mostly using this for when I edit the dummy piece.
		/// </summary>
		/// <returns></returns>
		public BadgeTransform Clone() {
			return new BadgeTransform() {
				XPos = this.XPos,
				YPos = this.YPos,
				RotationType = this.RotationType,
				FlipType = this.FlipType
			};
		}
		#endregion
		
	}
	
}