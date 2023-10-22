using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;
using Grawly.Calendar.Behavior;

namespace Grawly.Calendar {

	/// <summary>
	/// Provides access to ways to manipulate the calendar progression. 
	/// Replaces the old LegacyStoryController.
	/// </summary>
	public class CalendarController : MonoBehaviour {

		public static CalendarController Instance { get; private set; }

		#region FIELDS - ASSETS
		/// <summary>
		/// The CalendarData to use throughout the game.
		/// </summary>
		[SerializeField]
		private CalendarData calendarData;
		/// <summary>
		/// The CalendarData to use throughout the game.
		/// </summary>
		public CalendarData CalendarData {
			get {
				return this.calendarData;
			}
		}
		#endregion
		
		#region PROPERTIES - DATE
		/// <summary>
		/// The CalendarDay that represents the day the player is currently in.
		/// </summary>
		public CalendarDay CurrentDay {
			get {
				CalendarDay currentDay = this.CalendarData.GetDay(dayOfFocus: GameController.Instance.Variables.CurrentDayNumber); 
				return currentDay;
			}
		}
		/// <summary>
		/// The current time of day.
		/// </summary>
		public TimeOfDayType CurrentTimeOfDay {
			get {
				// Just get it from the GameController.
				return GameController.Instance.Variables.CurrentTimeOfDay;
			}
		}
		/// <summary>
		/// The current weather!
		/// </summary>
		public WeatherType CurrentWeatherType {
			get {
				Debug.LogWarning("FIX THIS LATER");
				return new List<WeatherType>() {
					WeatherType.Cloudy, WeatherType.Sunny
				}
					.Random();
			}
		}
		#endregion

		#region UNITY FUNCTIONS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
			}
			
			// Make sure to initialize the CalendarData.
			// this.CalendarData.Initialize();
			
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Updates the CalendarData with the data passed in.
		/// Helpful for when I need to prototype things with Presets or whatever.
		/// </summary>
		/// <param name="calendarData">The data to use.</param>
		public void SetCalendarData(CalendarData calendarData) {
			Debug.Log("Updating CalendarData...");
			this.calendarData = calendarData;
		}
		#endregion
		
		#region CALENDAR CONTROL
		/// <summary>
		/// Goes to the next day/time that is defined in the calendar data.
		/// Uses the variables stored in the GameController.
		/// </summary>
		public void GoToNextStoryBeat() {
			// Grab the Variables from the GameController and use those.
			this.GoToNextStoryBeat(gameVariables: GameController.Instance.Variables);
		}
		/// <summary>
		/// Goes to the next day/time that is defined in the calendar data.
		/// </summary>
		/// <param name="gameVariables">The GameVariables to use for referencing the current day/time.</param>
		public void GoToNextStoryBeat(GameVariables gameVariables) {
			
			// Ask the calendar data what the next story beat is by passing the current day number and time.
			StoryBeat nextStoryBeat = this.CalendarData.GetNextStoryBeat(
				currentDay: gameVariables.CurrentDayNumber,
				currentTime: gameVariables.CurrentTimeOfDay);

			Debug.Log("(NEXT STORY BEAT) DAY: " + nextStoryBeat.DayNumber + " -- TIME: " + nextStoryBeat.timeOfDay);
			// Debug.Break();
			
			// If the next story beat has a camera override, go ahead and set it.
			if (nextStoryBeat.HasCameraOverride == true) {
				GlobalFlagController.Instance.SetCameraOverride(cameraTagType: nextStoryBeat.cameraTagOverride);
			}
			
			// Figure out the next scene to load up.
			string nextScene = this.CalendarData.GetStoryBeatScene(storyBeat: nextStoryBeat);
			Debug.Log("NEXT SCENE: " + nextScene);
			
			// Prepare a callback to run which runs all story behaviors.
			System.Action sceneLoadedCallback = () => {
				
				// Begin by making a new sequence and also grabbing the relevant reactions.
				StoryBeatReactionSequence storyBeatReactionSequence = new StoryBeatReactionSequence();
				List<StoryBeatReaction> storyBeatReactions = nextStoryBeat.StoryBehaviors
					.Select(sb => sb.OnStoryBeatLoad())
					.ToList();
				
				// Prepare the sequence and add the reactions.
				storyBeatReactionSequence.Prepare(()=>{});
				storyBeatReactionSequence.AddToSequence(storyBeatReactions);
				
				// Begin execution.
				storyBeatReactionSequence.ExecuteNextReaction();
				
			}; 
			
			// Determine whether to use the date change transition or the time change transition based on the day number.
			if (nextStoryBeat.DayNumber == gameVariables.CurrentDayNumber) {
				this.GoToTime(
					gameVariables: gameVariables,										// Use the variables passed in.
					startTime: gameVariables.CurrentTimeOfDay,							// Start time is the current time.
					endTime: nextStoryBeat.timeOfDay,									// End time is the time of day in the story beat.
					sceneName: nextScene,												// Scene name is the scene stored in the story beat.
					onSceneLoad: sceneLoadedCallback);									

			} else if (nextStoryBeat.DayNumber > gameVariables.CurrentDayNumber) {
				this.GoToDay(
					gameVariables: gameVariables,											// Use the variables passed in.
					calendarDayNumber: nextStoryBeat.DayNumber,								// New day number is in the story beat.
					timeOfDay: nextStoryBeat.timeOfDay,										// Time of day is in the story beat.
					sceneName: nextScene, 													// Scene name is in the story beat.
					onSceneLoad: sceneLoadedCallback);													
				
			} else {
				// If the day number in the story beat is LESS than the current day, thats an issue.
				throw new Exception("The next story beat day number is "
				                    + nextStoryBeat.DayNumber
				                    + ", but the current day in the variables is " 
				                    + gameVariables.CurrentDayNumber);
			}
			
		}
		/// <summary>
		/// Loads the CalendarTransitionScene and goes to the specified day.
		/// </summary>
		/// <param name="gameVariables">The GameVariables to update with the new time/day.</param>
		/// <param name="calendarDayNumber">The day number to use when transitioning.</param>
		/// <param name="timeOfDay">The time of day that should be loaded up on.</param>
		/// <param name="sceneName">The scene to load up next.</param>
		/// <param name="onSceneLoad">A callback to run when the scene is loaded.</param>
		private void GoToDay(GameVariables gameVariables, int calendarDayNumber, TimeOfDayType timeOfDay, string sceneName, Action onSceneLoad) {

			string str = "DAY TRANSITION - ";
			str += "FROM (DAY: " + gameVariables.CurrentDayNumber + ", TIME: " + gameVariables.CurrentTimeOfDay + ")     ";
			str += "TO (DAY: " + calendarDayNumber + ", TIME: " + timeOfDay + ")";
			Debug.Log(str);
			
			// First of all, update the day in the Variables.
			gameVariables.CurrentDayNumber = calendarDayNumber;
			
			// Also update the time as the first time of the day. This can be computed with the new current day, which was updated after that previous line.
			gameVariables.CurrentTimeOfDay = timeOfDay;

			// Tell the calendar scene controller to load up the given scene when it is ready.
			CalendarSceneController.SceneToLoad = sceneName;
			CalendarSceneController.OnSceneLoadCallback = onSceneLoad;
			
			// Load up the transition scene. When it gets there, it will call the function in the CalendarController that unpacks a day and decides how to proceed.
			SceneController.instance.LoadScene(sceneName: "DateChange");
			
		}
		/// <summary>
		/// Goes to the specified time of day.
		/// </summary>
		/// <param name="gameVariables">The GameVariables to update with the new time/day.</param>
		/// <param name="startTime">The time currently being transitioned from.</param>
		/// <param name="endTime">The destination time to go to.</param>
		/// <param name="sceneName">The name of the scene to load.</param>
		/// <param name="onSceneLoad">A callback to run when the scene is loaded.</param>
		private void GoToTime(GameVariables gameVariables, TimeOfDayType startTime, TimeOfDayType endTime, string sceneName, Action onSceneLoad) {
			
			// Just tell the scene controller where to go.
			SceneController.instance?.LoadSceneWithTimeTransition(
				sceneName: sceneName,
				startTime: startTime,
				endTime: endTime, 
				onSceneLoaded: onSceneLoad);

			// Set the time of day to go to.
			gameVariables.CurrentTimeOfDay = endTime;
	
		}
		#endregion
		
	}


}