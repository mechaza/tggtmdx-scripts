using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Grawly.UI {

	/// <summary>
	/// I need something to help me handle UI events or something.
	/// There are multiples of these I guess. Think of it like an invisible dpad. lol.
	/// I really really hate this but I'll never use it outside the save screen.
	/// Contextually this should only be running when there is no other prompts open.
	/// </summary>
	public class SaveScreenEventHandler : MonoBehaviour, IMoveHandler, ISubmitHandler, ICancelHandler {

		#region FIELDS - SCENE REFERENCES
		[SerializeField]
		private GameObject centerButton;
		#endregion

		#region UNITY EVENTS
		private void OnEnable() {
			// When this object gets enabled by PlayMaker, check whether it knows its the "center button" or not.
			// If it is, its safe to enable it, probably.
			// The reason this gets enabled is because its the child of a parent that playmaker adjusts.
			if (this.gameObject == this.centerButton) {
				Debug.Log("The SaveScreenEventHandler was selected. It is now selected by the Event System.");
				EventSystem.current.SetSelectedGameObject(this.gameObject);
			}
		}
		#endregion

		#region EVENTS
		public void OnCancel(BaseEventData eventData) {
			// If this is inside the initialzation screen, call the function that cancels too.
			// LegacyInitializationControllerDX.Instance?.LoadMenuCancelled();
			SaveScreenController.instance.GetComponent<PlayMakerFSM>().SendEvent("Back");
		}
		public void OnMove(AxisEventData eventData) {
			// When OnMove gets called, it should tell the save screen controller to move either up or down.
			switch (eventData.moveDir) {
				case MoveDirection.Up:
					// If i hit up on the DPad, theoretically, the list should be moving down.
					SaveScreenController.instance.MoveListDown();
					break;
				case MoveDirection.Down:
					SaveScreenController.instance.MoveListUp();
					break;
				default:
					break;
			}
			// After moving, the "center" button should be reselected.
			EventSystem.current.SetSelectedGameObject(this.centerButton);
		}
		public void OnSubmit(BaseEventData eventData) {

			// Get the current screen type and if that slot has save data.
			SaveScreenType currentScreenType = SaveScreenController.instance.ScreenType;
			bool currentSlotHasSave = SaveScreenController.instance.CurrentSlotHasSave;

			// If the screen type is loading and there is a save available, send the event.
			if (currentScreenType == SaveScreenType.Load && currentSlotHasSave) {
				SaveScreenController.instance.GetComponent<PlayMakerFSM>().SendEvent("Slot Selected");
			} else if (currentScreenType == SaveScreenType.Save) {
				SaveScreenController.instance.GetComponent<PlayMakerFSM>().SendEvent("Slot Selected");
			} else {
				// This should be reached if the specified save is a new game and youre trying to load.
				AudioController.instance?.PlaySFX(type: SFXType.Invalid);
			}
			
			
		}
		#endregion

	}


}