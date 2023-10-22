using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grawly.Calendar;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Grawly.Menus.Map {
	
	/// <summary>
	/// The controller for the map screen.
	/// </summary>
	public class MapScreenController : SerializedMonoBehaviour {
	
		public static MapScreenController Instance { get; private set; }

		#region FIELDS - RESOURCES
		/// <summary>
		/// The different music tracks to use depending on what time of day it is.
		/// </summary>
		[SerializeField, Title("Music")]
		private Dictionary<TimeOfDayType, IntroloopAudio> mapScreenMusicDict = new Dictionary<TimeOfDayType, IntroloopAudio>();
		#endregion

		#region FIELDS - SCENE REFERENCES		
		[SerializeField, Title("Scene References")]
		private List<PrototypeMapButton> prototypeMapButtons = new List<PrototypeMapButton>();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {

			// Play the audio associated with the given time of day.
			IntroloopAudio audioToUse = this.mapScreenMusicDict[GameController.Instance.Variables.CurrentTimeOfDay];
			AudioController.instance.PlayMusic(audio: audioToUse, track: 0);
			
			
			// Grab the map locations to use.
			List<MapScreenLocationDefinition> mapLocations = CalendarController.Instance.CalendarData.GetMapLocations(
				dayNumber: CalendarController.Instance.CurrentDay.EpochDay, 
				timeOfDay: CalendarController.Instance.CurrentTimeOfDay);
			
			// Grab the appropriate number of buttons.
			List<PrototypeMapButton> availableButtons = this.prototypeMapButtons
				.Take(mapLocations.Count)
				.ToList();
			
			// Go through each location and turn on the respective button.
			for (int i = 0; i < mapLocations.Count; i++) {
				availableButtons[i].gameObject.SetActive(true);
				availableButtons[i].Prepare(mapLocations[i]);
			}
			
			// Dehighlight every button except the first.
			availableButtons.Skip(1).ToList().ForEach(b => b.Dehighlight());
			// Select the first, which will highlight it.
			EventSystem.current.SetSelectedGameObject(this.prototypeMapButtons.First(b => b.gameObject.activeInHierarchy == true).gameObject);
			
		}
		#endregion

	
		
	}

	
}