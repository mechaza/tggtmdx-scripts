using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Calendar {
	
	/// <summary>
	/// A data structure that associates an overworld type with a scene name.
	/// TimedOverworldDefinitions take precidence.
	/// </summary>
	[InlineProperty, HideReferenceObjectPicker, System.Serializable]
	[GUIColor(r: 0f, g: 1f, b: 0.64f, a: 1f)]
	public class StaticLocationDefinition {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The type of location to be associated with the provided scene name.
		/// </summary>
		[OdinSerialize]
		[PropertyTooltip("The type of location to be associated with the scene provided below.")]
		public LocationType LocationType { get; set; } = LocationType.None;
		/// <summary>
		/// The name of the scene that should be opened by default when loading the location above.
		/// </summary>
		[OdinSerialize]
		[PropertyTooltip("The name of the scene that should be opened by default when loading the location above.")]
		public string SceneName { get; set; } = "";
		#endregion

	}

	
}