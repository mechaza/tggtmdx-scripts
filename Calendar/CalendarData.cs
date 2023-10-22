using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;
using System;
using System.IO;
using Grawly.Data;
using Grawly.Dungeon;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Grawly.Calendar {
	
	/// <summary>
	/// Contains the data that defines how the game should progress long term.
	/// Similar to how Persona structures its progression.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Calendar/Calendar Data")]
	[TypeInfoBox("Contains the data that defines how the game should progress long term. Similar to how Persona structures its progression.")]
	public class CalendarData : SerializedScriptableObject {
		#region FIELDS - CALENDAR START
		/// <summary>
		/// The year that this calendar should start.
		/// </summary>
		[SerializeField, TabGroup("General", "General"), Title("Start Date")]
		private int startYear = 1992;
		/// <summary>
		/// The month this calendar should start.
		/// </summary>
		[SerializeField, TabGroup("General", "General")]
		private int startMonth = 3;
		/// <summary>
		/// The day of the month this calendar should start.
		/// </summary>
		[SerializeField, TabGroup("General", "General")]
		private int startDay = 8;
		/// <summary>
		/// The total number of days for this calendar.
		/// </summary>
		[SerializeField, TabGroup("General", "General")]
		private int totalDays = 80;
		#endregion

		#region PROPERTIES - DATE
		/// <summary>
		/// The day that the calendar should start.
		/// </summary>
		private System.DateTime StartDate {
			get {
				return new System.DateTime(year: this.startYear, month: this.startMonth, day: this.startDay);
			}
		}
		#endregion

		#region FIELDS - LOCATION DEFAULTS
		/// <summary>
		/// A list of overworld definitions to use in the event a CalendarDay does not provide its own overrides.
		/// These take precidence over the static definitions.
		/// </summary>
		[OdinSerialize, TabGroup("Locations", "Locations"), Title("Overworld Definitions"), ListDrawerSettings(Expanded = true)]
		[PropertyTooltip("A list of overworld definitions to use in the event a CalendarDay does not provide its own overrides. These take precidence over the static definitions.")]
		[GUIColor(r: 0.8f, g: 1f, b: 0.8f, a: 1f)]
		private List<DynamicLocationDefinition> dynamicLocationDefinitions = new List<DynamicLocationDefinition>();
		/// <summary>
		/// A list of overworld definitions containing scenes to load for a given overworld type regardless of what time it is.
		/// The selection is made here if an entry was not found in the dynamic definitions.
		/// </summary>
		[OdinSerialize, TabGroup("Locations", "Locations"), ListDrawerSettings(Expanded = true)]
		[PropertyTooltip("A list of overworld definitions containing scenes to load for a given overworld type regardless of what time it is. The selection is made here if an entry was not found in the dynamic definitions.")]
		[GUIColor(r: 0.8f, g: 0.8f, b: 1f, a: 1f)]
		private List<StaticLocationDefinition> staticLocationDefinitions = new List<StaticLocationDefinition>();
		#endregion

		/*#region FIELDS - MAP SCREEN DEFAULTS
		/// <summary>
		/// A list of location definitions that the player can go to on the map screen.
		/// </summary>
		[OdinSerialize, TabGroup("Map", "Map"), Title("Map Screen Definitions"), ListDrawerSettings(Expanded = true)]
		[PropertyTooltip("A list of location definitions that the player can go to on the map screen.")]
		[GUIColor(r: 1f, g: 0.8f, b: 0.8f, a: 1f)]
		private List<MapScreenLocationDefinition> mapScreenLocations = new List<MapScreenLocationDefinition>();
		#endregion*/

		#region FIELDS - DAY TEMPLATES
		/// <summary>
		/// A list of day templates that contain story beats for each day.
		/// </summary>
		[SerializeField, TabGroup("Days", "Days"), ListDrawerSettings(Expanded = true, NumberOfItemsPerPage = 20)]
		[PropertyTooltip("A list of day templates that contain story beats for each day.")]
		private List<CalendarDayTemplate> calendarDayTemplates = new List<CalendarDayTemplate>();
		#endregion

		#region FIELDS - DAYS
		/// <summary>
		/// The data that contains the different days that dictate progression through the game.
		/// Days are automatically assigned dates based on the start date specified in the Defaults tab. Just focus on what should be happening 'so many days in.'
		/// </summary>
		private List<CalendarDay> calendarDays { get; set; } = new List<CalendarDay>();
		#endregion

		#region PROPERTIES - DAYS
		/// <summary>
		/// The number of CalendarDayTemplates that exist in this CalendarData.
		/// </summary>
		public int CalendarDayTemplateCount {
			get {
				return this.calendarDayTemplates.Count;
			}
		}
		/// <summary>
		/// The data that contains the different days that dictate progression through the game.
		/// Days are automatically assigned dates based on the start date specified in the Defaults tab. Just focus on what should be happening 'so many days in.'
		/// </summary>
		private List<CalendarDay> CalendarDays {
			get {
				// If the list wasn't initialized, do so.
				// I'm not a big fan of this but... whatever.
				if (this.calendarDays == null || this.calendarDays.Count == 0) {
					Debug.Assert(this.totalDays > 0);
					this.calendarDays = new List<CalendarDay>();
					for (int i = 0; i < this.totalDays; i++) {
						this.calendarDays.Add(new CalendarDay());
					}
				}

				return this.calendarDays;
			}
		}
		/// <summary>
		/// The CalendarDayTemplates. I use these to help me streamline generating presets.
		/// </summary>
		/// <exception cref="Exception"></exception>
		public List<CalendarDayTemplate> CalendarDayTemplates {
			get {
				if (Application.isEditor == false || Application.isPlaying == true) {
					throw new Exception("This should only be called from the editor when not in play mode!");
				}
				return this.calendarDayTemplates;
			}
			set {
				if (Application.isEditor == false || Application.isPlaying == true) {
					throw new Exception("This should only be called from the editor when not in play mode!");
				}

				this.calendarDayTemplates = value;
			}
		}
		#endregion

		#region SETTERS - DAY TEMPLATES
		/// <summary>
		/// Overwrites the calendar day templates stored in this CalendarData.
		/// Should only be used in the automation of CalendarData creation.
		/// This does not get used in any situation at runtime.
		/// </summary>
		/// <param name="calendarDayTemplates"></param>
		public void SetCalendarDayTemplates(List<CalendarDayTemplate> calendarDayTemplates) {
			if (Application.isEditor == false || Application.isPlaying) {
				throw new Exception("This should only be called from the editor and not during runtime.");
			}
			Debug.Log("OVERWRITING CALENDARDAYTEMPLATES.");
			this.calendarDayTemplates = calendarDayTemplates;
		}
		#endregion
		
		#region GETTERS - DAY TEMPLATES
		/// <summary>
		/// Gets the calendar day template for the specified day number.
		/// </summary>
		/// <param name="dayNumber">The day number of the template to retrieve.</param>
		/// <returns></returns>
		public CalendarDayTemplate GetCalendarDayTemplate(int dayNumber) {
			return this.calendarDayTemplates.Find(t => t.dayNumber == dayNumber);
		}
		#endregion

		#region GETTERS - SCENES
		/// <summary>
		/// Returns the name of the scene to load for the given combination of day number/overworld type/time of day.
		/// Definitions in the day template for the specified day are checked first,
		/// followed by dynamic definitons, then static.
		/// </summary>
		/// <param name="dayNumber">The day the scene will be loading on.</param>
		/// <param name="locationType">The kind of overworld to load.</param>
		/// <param name="timeOfDay">The time of day to check for.</param>
		/// <returns>The name of the scene to load for the specified combination of paramters.</returns>
		public string GetScene(int dayNumber, LocationType locationType, TimeOfDayType timeOfDay) {
			// Grab the day template for the given number.
			CalendarDayTemplate dayTemplate = this.GetCalendarDayTemplate(dayNumber: dayNumber);
			// If the template has an override, use that.
			if (dayTemplate.HasLocationOverride(locationType, timeOfDay) == true) {
				return dayTemplate.GetLocationOverride(locationType, timeOfDay);
			}

			// Check if there is a dynamic definition.
			if (this.dynamicLocationDefinitions.Exists(o => o.LocationType == locationType && o.TimeToOverride == timeOfDay)) {
				return this.dynamicLocationDefinitions.Find(o => o.LocationType == locationType && o.TimeToOverride == timeOfDay).SceneName;
			} else {
				// If there is no dynamic definition, use the static one.
				return this.staticLocationDefinitions.Find(o => o.LocationType == locationType).SceneName;
			}
		}
		#endregion

		#region GETTERS - STORY BEAT
		/// <summary>
		/// Gets the next day/time when passed a specific day number and time of day.
		/// </summary>
		/// <param name="currentDay">The current day number.</param>
		/// <param name="currentTime">The current time of day.</param>
		/// <returns></returns>
		public StoryBeat GetNextStoryBeat(int currentDay, TimeOfDayType currentTime) {
			// Find the template for the current day.
			CalendarDayTemplate currentDayTemplate = this.GetCalendarDayTemplate(dayNumber: currentDay);

			// Check if the template has more story beats.
			if (currentDayTemplate.HasMoreStoryBeats(currentTime) == true) {
				// If there are more beats, return the next beat for the current template.
				return this.GetNextStoryBeat(currentDayTemplate: currentDayTemplate, currentTime: currentTime);
			} else {
				// If that was the last story beat, probe the next template and return the first story beat.
				return this.GetCalendarDayTemplate(currentDay + 1).StoryBeats.First();
			}
		}
		/// <summary>
		/// Gets the next story beat when given the current time.
		/// </summary>
		/// <param name="currentDayTemplate"></param>
		/// <param name="currentTime"></param>
		/// <returns></returns>
		private StoryBeat GetNextStoryBeat(CalendarDayTemplate currentDayTemplate, TimeOfDayType currentTime) {
			int index = currentDayTemplate.StoryBeats.FindIndex(sb => sb.timeOfDay == currentTime);
			return currentDayTemplate.StoryBeats[index + 1];
		}
		/// <summary>
		/// Determines what scene to load based on the information in a story beat.
		/// </summary>
		/// <param name="storyBeat">The story beat to interpret.</param>
		/// <returns>The scene associated with this story beat.</returns>
		public string GetStoryBeatScene(StoryBeat storyBeat) {
			if (storyBeat.gameTransitionType == GameTransitionType.CustomTransition) {
				// If its a scene transition, use the scene stored.
				return storyBeat.sceneName;
			} else if (storyBeat.gameTransitionType == GameTransitionType.LocationTransition) {
				// If its an overworld transition, grab it from the overworld definitions.
				return this.GetScene(dayNumber: storyBeat.DayNumber, locationType: storyBeat.locationType, timeOfDay: storyBeat.timeOfDay);
			} else {
				throw new Exception("Something went wrong trying to get a scene from the story beat!");
			}
		}
		#endregion

		/*#region GETTERS - NPCS
		/// <summary>
		/// Gets the NPCs that are available during the specified day and time.
		/// </summary>
		/// <param name="currentDay">The day which respective NPCs should be available for.</param>
		/// <param name="currentTime">The time of day (in addition to day number) the respective NPCs should be available for.</param>
		/// <returns>A list of NPC templates that are available for this specific day and time.</returns>
		public List<DungeonNPCTemplate> GetNPCTemplates(int currentDay, TimeOfDayType currentTime) {
			throw new NotImplementedException("ADD THIS");
		}
		#endregion*/
		
		#region GETTERS - MAP SCREEN
		/// <summary>
		/// Gets a list of available map locations for the specified day number and the time of day.
		/// </summary>
		/// <param name="dayNumber">The current day number.</param>
		/// <param name="timeOfDay">The current time of day.</param>
		/// <returns>A list of available locations on the map.</returns>
		public List<MapScreenLocationDefinition> GetMapLocations(int dayNumber, TimeOfDayType timeOfDay) {
			// The calendar day template should hold the locations available on this day.
			return this.GetCalendarDayTemplate(dayNumber: dayNumber)
				.GetMapScreenDefinitions(timeOfDayType: timeOfDay);
		}
		#endregion

		#region GETTERS - CALENDAR DAYS
		/// <summary>
		/// Gets the specified day, but also grabs the 3 days before it and the 3 days after it.
		/// This is used in the Day Change screen. Think how Persona displays the calendar.
		/// </summary>
		/// <param name="dayOfFocus">The day that is being grabbed.</param>
		/// <param name="surroundingDays">The days that should be grabbed before and after the specified day.</param>
		/// <returns></returns>
		public List<CalendarDay> GetSurroundingDays(int dayOfFocus, int surroundingDays = 4) {
			//
			// If the range is too large, it will throw an error.
			// For this reason, make sure there's extra "padding" days in the calendar days list.
			// If push comes to shove, do what I was doing up above.
			//

			// Figure out the range of days that are needed.
			List<int> dayOffsets = Enumerable.Range(start: dayOfFocus - surroundingDays + 1, count: (surroundingDays * 2) + 1).ToList();
			// Use the GetDay function to prep the CalendarDays for use.
			return dayOffsets.Select(i => this.GetDay(dayOfFocus: i)).OrderBy(c => c.EpochDay).ToList();
		}
		/// <summary>
		/// Returns all of the days in the calendar data.
		/// </summary>
		/// <returns></returns>
		public List<CalendarDay> GetAllDays() {
			List<int> allDayIndicies = Enumerable.Range(start: 0, count: this.CalendarDays.Count).ToList();
			return allDayIndicies.Select(i => this.GetDay(dayOfFocus: i)).OrderBy(c => c.EpochDay).ToList();
		}
		/// <summary>
		/// Gets the day at the specified index.
		/// </summary>
		/// <param name="dayOfFocus">The index of the day to get.</param>
		/// <returns></returns>
		public CalendarDay GetDay(int dayOfFocus) {
			// Before getting the day, remember to modify its date.
			return this.SetDateOnCalendarDay(
				calendarDay: this.CalendarDays[dayOfFocus], 
				startDateTime: this.StartDate,
				dayOffset: dayOfFocus);
		}
		#endregion
		
		#region DATETIME MODIFICATIONS
		/// <summary>
		/// Processes a given CalendarDay and assigns it the information it needs for its dates.
		/// </summary>
		/// <param name="calendarDay">The CalendarDay to modify.</param>
		/// <param name="startDateTime">The day to start off from.</param>
		/// <param name="dayOffset">The days to offset from the given date.</param>
		/// <returns>The modified CalendarDay with the information it needs to get moving.</returns>
		private CalendarDay SetDateOnCalendarDay(CalendarDay calendarDay, System.DateTime startDateTime, int dayOffset) {
			// Add the days to the DateTime and then pass it to the regular ass version.
			return this.SetDateOnCalendarDay(calendarDay: calendarDay, dateTime: startDateTime.AddDays(value: dayOffset));
		}
		/// <summary>
		/// Processes a given CalendarDay and assigns it the information it needs for its dates.
		/// </summary>
		/// <param name="calendarDay">The CalendarDay to modify.</param>
		/// <param name="dateTime">The datetime with the information to populate it with.</param>
		/// <returns></returns>
		private CalendarDay SetDateOnCalendarDay(CalendarDay calendarDay, System.DateTime dateTime) {
			calendarDay.DayNumber = dateTime.Day;
			calendarDay.MonthNumber = dateTime.Month;
			// The integer values of my CalendarWeekdayType match up with the DayOfWeek enum in System so it should be fine.
			calendarDay.WeekdayType = ((CalendarWeekdayType) dateTime.DayOfWeek);
			// Figure out the amount of days since the start day. I really only need this for sorting purposes.
			// Note that this is actually being saved as a double.
			calendarDay.EpochDay = (int) dateTime.Subtract(this.StartDate).TotalDays;
			// FINALLY return that shit.
			return calendarDay;
		}
		#endregion

		#region ODIN HELPERS
		[TabGroup("Debug", "Debug"), Button]
		private void RenameDayTemplates(string prefix) {
#if UNITY_EDITOR
			prefix = string.IsNullOrEmpty(prefix) ? "Day " : prefix + " Day ";
			for (int i = 0; i < this.calendarDayTemplates.Count; i++) {
				string newName = prefix + i.ToString();
				CalendarDayTemplate template = this.calendarDayTemplates[i];
				string assetPath = AssetDatabase.GetAssetPath(template.GetInstanceID());

				AssetDatabase.RenameAsset(assetPath, newName);
				
			}
			AssetDatabase.SaveAssets();
#endif
		}
		#endregion
	}
}