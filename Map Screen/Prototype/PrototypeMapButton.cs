using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Calendar;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Grawly.Menus.Map {

	/// <summary>
	/// A prototype button that can be used for the map screen.
	/// </summary>
	public class PrototypeMapButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler {

		/*#region FIELDS - TOGGLES
		/// <summary>
		/// The text to put onto the button.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private string buttonText = "";
		/// <summary>
		/// The name of the scene to load when this button is hit.
		/// </summary>
		[SerializeField]
		private string sceneToLoad = "";
		#endregion*/

		#region FIELDS - STATE
		/// <summary>
		/// The current map location being used for this button.
		/// </summary>
		private MapScreenLocationDefinition currentMapLocation;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label for the button.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private SuperTextMesh buttonLabel;
		/// <summary>
		/// The "highlight bar" that gets displayed when this button is on screen.
		/// </summary>
		[SerializeField]
		private GameObject highlightBar;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Totally resets the state of this button.
		/// </summary>
		public void ResetState() {
			this.gameObject.SetActive(false);
		}
		/// <summary>
		/// Preps this button to be used with the specified map location.
		/// </summary>
		/// <param name="mapLocation">The location to use.</param>
		public void Prepare(MapScreenLocationDefinition mapLocation) {
			this.currentMapLocation = mapLocation;
		}
		#endregion
		
		#region PRESENTATION
		public void Highlight() {
			this.highlightBar.gameObject.SetActive(true);
			this.buttonLabel.textMaterial = DataController.GetDefaultSTMMaterial("UIDefault");
			this.buttonLabel.Text = "<c=white>" + this.currentMapLocation.MapButtonText;
		}
		public void Dehighlight() {
			this.highlightBar.gameObject.SetActive(false);
			this.buttonLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2 White");
			this.buttonLabel.Text = "<c=black>" + this.currentMapLocation.MapButtonText;
		}
		#endregion
		
		#region EVENT HANDLERS
		public void OnSelect(BaseEventData eventData) {
			this.Highlight();
		}
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Hover);
			this.Dehighlight();
		}
		public void OnSubmit(BaseEventData eventData) {
			
			// Assert that the time of day in the calendar controller is the same as the map location's.
			// Debug.Assert(CalendarController.Instance.CurrentTimeOfDay == this.currentMapLocation.TimeOfAvailability);
			
			EventSystem.current.SetSelectedGameObject(null);
			AudioController.instance?.PlaySFX(SFXType.Select);
			
			SceneController.instance.LoadScene(
				dayNumber: CalendarController.Instance.CurrentDay.EpochDay, 
				locationType: this.currentMapLocation.TargetLocationType,
				timeOfDay: CalendarController.Instance.CurrentTimeOfDay,
				dontStopMusic: true);
			
			/*SceneController.instance.LoadScene(
				dayNumber: CalendarController.Instance.CurrentDay.EpochDay, 
				locationType: this.currentMapLocation.TargetLocationType,
				timeOfDay: this.currentMapLocation.TimeOfAvailability,
				dontStopMusic: true);*/
			
		}
		#endregion
		
	}
	
}
