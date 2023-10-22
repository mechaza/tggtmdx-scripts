using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Calendar {
	
	/// <summary>
	/// A data structure that associates a time of day and overworld location type with a given scene.
	/// The CalendarData has set of these for default scenes to load,
	/// but CalendarDays can override it if they so choose.
	/// </summary>
	[InlineProperty, HideReferenceObjectPicker, System.Serializable]
	[GUIColor(r: 0f, g: 1f, b: 0.64f, a: 1f)]
	public class DynamicLocationDefinition {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The time of day that this dynamic definition should take place.
		/// </summary>
		[OdinSerialize]
		[PropertyTooltip("The time of day that this dynamic location should be used, specifically.")]
		public TimeOfDayType TimeToOverride { get; set; } = TimeOfDayType.EarlyMorning;
		/// <summary>
		/// The type of location that should be overridden.
		/// </summary>
		[OdinSerialize]
		[PropertyTooltip("The type of location that should be overridden.")]
		public LocationType LocationType { get; set; } = LocationType.None;
		/// <summary>
		/// The scene to load for the combined pair of time of day and location.
		/// </summary>
		[OdinSerialize]
		[PropertyTooltip("The scene to load for the combined pair of time of day and location.")]
		public string SceneName { get; set; } = "";
		#endregion

	}

	
}