using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Calendar {
	
	/// <summary>
	/// A data structure that is kind of like a dynamic location but for the map screen.
	/// </summary>
	[InlineProperty, HideReferenceObjectPicker, System.Serializable]
	public class MapScreenLocationDefinition {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The location type.
		/// </summary>
		[OdinSerialize]
		public LocationType TargetLocationType { get; set; } = LocationType.None;
		/// <summary>
		/// The text to use on the label.
		/// </summary>
		[OdinSerialize]
		public string MapButtonText { get; set; } = "";
		#endregion

	}

	
}