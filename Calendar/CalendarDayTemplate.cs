using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grawly.Calendar.Behavior.General;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.Serialization;

namespace Grawly.Calendar {
	
	/// <summary>
	/// Contains story beats for individualized days.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Calendar/Calendar Day Template")]
	public class CalendarDayTemplate : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The day for this template to take place on.
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Toggles")]
		public int dayNumber = 0;
		#endregion

		#region FIELDS - SCENE OVERRIDES
		/// <summary>
		/// A list of definitions to override on this particular day.
		/// Sensitive to time.
		/// </summary>
		[OdinSerialize, TabGroup("Overrides", "Overrides"), ListDrawerSettings(Expanded = true)]
		[PropertyTooltip("A list of definitions to override on this particular day. Sensitive to time.")]
		[GUIColor(r: 0.8f, g: 1f, b: 0.8f, a: 1f)]
		private List<DynamicLocationDefinition> dynamicLocationOverrides = new List<DynamicLocationDefinition>();
		/// <summary>
		/// A list of definitions to override on this particular day.
		/// Will be used if nothing is available in dynamic.
		/// </summary>
		[OdinSerialize, TabGroup("Overrides", "Overrides"), ListDrawerSettings(Expanded = true)]
		[PropertyTooltip("A list of definitions to override on this particular day. Will be used if nothing is available in dynamic.")]
		[GUIColor(r: 0.8f, g: 0.8f, b: 1f, a: 1f)]
		private List<StaticLocationDefinition> staticLocationOverrides = new List<StaticLocationDefinition>();
		#endregion
		
		#region FIELDS - STORY BEATS
		/// <summary>
		/// A list of story beats for this given day.
		/// </summary>
		[OdinSerialize, ListDrawerSettings(Expanded = true), Space(10)]
		[InfoBox("Note that StoryBehaviors inside each StoryBeat are executed sequentially and can be blocking by design.")]
		private List<StoryBeat> storyBeats = new List<StoryBeat>();
		/// <summary>
		/// A list of story beats for this given day.
		/// </summary>
		public List<StoryBeat> StoryBeats {
			get {
				// I'm VERY nervous about doing it like this but this is the only way I can think of working with legacy code.
				this.storyBeats.ForEach(s => s.DayNumber = this.dayNumber);				
				return this.storyBeats;
			}
		}
		#endregion
		
		#region PROPERTIES - STORY BEATS
		/// <summary>
		/// Does this day template have ANY story beats?
		/// </summary>
		public bool HasStoryBeats {
			get {
				return this.StoryBeats.Count > 0;
			}
		}
		#endregion

		#region GETTERS - DEFINITIONS
		/// <summary>
		/// Get the locations to display for the time of day provided.
		/// </summary>
		/// <param name="timeOfDayType"></param>
		/// <returns></returns>
		public List<MapScreenLocationDefinition> GetMapScreenDefinitions(TimeOfDayType timeOfDayType) {
			// Find the story beat with the same time of day and return its definitions.
			return this.StoryBeats
				.First(sb => sb.timeOfDay == timeOfDayType)
				.mapScreenDefinitions;
		}
		#endregion
		
		#region GETTERS - OVERRIDES
		/// <summary>
		/// Gets the scene to load for the specified overworld type and time of day.
		/// This overrides what's defined in the Calendar Data.
		/// </summary>
		/// <param name="locationType">The location to check for.</param>
		/// <param name="timeOfDay">The time of day to check for.</param>
		/// <returns></returns>
		public string GetLocationOverride(LocationType locationType, TimeOfDayType timeOfDay) {
			Debug.Assert(this.HasLocationOverride(locationType, timeOfDay));
			
			// If there is a dynamic override, use that.
			if (this.dynamicLocationOverrides.Exists(o => o.LocationType == locationType && o.TimeToOverride == timeOfDay)) {
				return this.dynamicLocationOverrides
					.Find(o => o.LocationType == locationType && o.TimeToOverride == timeOfDay)
					.SceneName;
			} else {
				// If there is no dynamic override, use the static one.
				return this.staticLocationOverrides.Find(o => o.LocationType == locationType).SceneName;
			}
		}
		#endregion
		
		#region GETTERS - STATE CHECK
		/// <summary>
		/// Returns true if there are more story beats after the specified time of day.
		/// </summary>
		/// <param name="currentTimeOfDay">The current time of day.</param>
		/// <returns>Whether or not there are more story beats after the given time.</returns>
		public bool HasMoreStoryBeats(TimeOfDayType currentTimeOfDay) {

			// If there are NO story beats, just return false. 
			if (this.StoryBeats.Count == 0) {
				return false;
			}
			
			// Check if the time of day for the last story beat is the one passed in.
			bool lastStoryBeat = this.StoryBeats.Last().timeOfDay == currentTimeOfDay;

			// Return the opposite of that result.
			return !lastStoryBeat;

		}
		/// <summary>
		/// Does this day template have an override for the specified overworld type and time of day?
		/// </summary>
		/// <param name="locationType">The type of overworld to check against.</param>
		/// <param name="timeOfDay">The time of day.</param>
		/// <returns>Whether or not this day template has an override.</returns>
		public bool HasLocationOverride(LocationType locationType, TimeOfDayType timeOfDay) {
			
			// Check if there is a definition in either the dynamic or static overrides.
			bool hasDynamic = this.dynamicLocationOverrides.Exists(o => o.LocationType == locationType && o.TimeToOverride == timeOfDay);
			bool hasStatic = this.staticLocationOverrides.Exists(o => o.LocationType == locationType);
			
			// If either of them are true, say so.
			return hasDynamic || hasStatic;
			
		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// A quick way to add a story beat to the template.
		/// </summary>
		[Button]
		private void AddStoryBeat() {
			this.storyBeats.Add(new StoryBeat());
		}
		/// <summary>
		/// Getting annoyed having to make story beats by hand so why not just make a fucking button that auto fills it for me.
		/// </summary>
		[Button]
		private void GeneratePlaceholderStoryBeats() {

			// Create some hard coded story beats.
			
			StoryBeat morningStoryBeat = new StoryBeat();
			morningStoryBeat.locationType = LocationType.InsideHome;
			morningStoryBeat.timeOfDay = TimeOfDayType.Morning;
			morningStoryBeat.AddPlaceholderStoryBehavior(new AutoChat(
				inlineChatText: ": > This morning scene is not defined. Continuing to afternoon.",
				freePlayerOnClose: false,
				timeToWait: 1f));
			morningStoryBeat.AddPlaceholderStoryBehavior(new NextStoryBeat());
			
			StoryBeat afternoonStoryBeat = new StoryBeat();
			afternoonStoryBeat.locationType = LocationType.InsideHome;
			afternoonStoryBeat.timeOfDay = TimeOfDayType.Afternoon;
			afternoonStoryBeat.AddPlaceholderStoryBehavior(new AutoChat(
				inlineChatText: ": > This afternoon scene is not defined. The girls now have free time.",
				freePlayerOnClose: true,
				timeToWait: 1f));

			StoryBeat eveningStoryBeat = new StoryBeat();
			eveningStoryBeat.locationType = LocationType.InsideHome;
			eveningStoryBeat.timeOfDay = TimeOfDayType.Evening;
			eveningStoryBeat.AddPlaceholderStoryBehavior(new AutoChat(
				inlineChatText: ": > This evening scene is not defined. The girls now have free time.",
				freePlayerOnClose: true,
				timeToWait: 1f));

			// Create a new list with these story beats.
			this.storyBeats = new List<StoryBeat>() {
				morningStoryBeat, 
				afternoonStoryBeat,
				eveningStoryBeat
			};

			/*// Go through each time of day type and make a new story beat for ones that don't already have one.
			foreach (TimeOfDayType timeOfDayType in System.Enum.GetValues(typeof(TimeOfDayType))) {
				
				if (timeOfDayType == TimeOfDayType.FINISH) {
					// Don't bother with the FINISH type.
					continue;
				} else if (this.storyBeats.Any(sb => sb.timeOfDay == timeOfDayType)) {
					// Also don't bother if the time of day already exists as a story beat.
					continue;
				}

				// Create a new story beat and add it.
				StoryBeat storyBeat = new StoryBeat();
				storyBeat.timeOfDay = timeOfDayType;
				this.storyBeats.Add(storyBeat);
				
			}

			// Order the story beats by time of day.
			this.storyBeats = this.StoryBeats.OrderBy(sb => sb.timeOfDay).ToList();*/

		}
		/// <summary>
		/// Just a quick shortcut to adding map screen definitions.
		/// </summary>
		[Button]
		private void AddPlaceholderMapScreens() {
			foreach (StoryBeat storyBeat in this.storyBeats) {
				storyBeat.mapScreenDefinitions = new List<MapScreenLocationDefinition>() {
					new MapScreenLocationDefinition(){ MapButtonText = "Home", TargetLocationType = LocationType.InsideHome },
					new MapScreenLocationDefinition(){ MapButtonText = "Mall", TargetLocationType = LocationType.MiamiMallLobby },
					new MapScreenLocationDefinition(){ MapButtonText = "Downtown", TargetLocationType = LocationType.ShoppingDistrict },
					new MapScreenLocationDefinition(){ MapButtonText = "Prototype Map 1", TargetLocationType = LocationType.Proto1 },
					new MapScreenLocationDefinition(){ MapButtonText = "Prototype Map 2", TargetLocationType = LocationType.Proto2 },
					new MapScreenLocationDefinition(){ MapButtonText = "Prototype Map 3", TargetLocationType = LocationType.Proto3 },
					new MapScreenLocationDefinition(){ MapButtonText = "Prototype Map 4", TargetLocationType = LocationType.Proto4 },
				};
			}
		}
		#endregion
		
	}
	
}
