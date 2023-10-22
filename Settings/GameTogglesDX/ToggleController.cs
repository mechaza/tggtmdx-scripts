using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles.Display;

namespace Grawly.Toggles {

	/// <summary>
	/// This is what I use whenever I need something to access a toggle from anywhere.
	/// As of now, it's mostly talking to the SaveController, but I prefer this in case I change where things are located.
	/// </summary>
	public static class ToggleController {

		#region FIELDS - STATE
		/// <summary>
		/// Were the boot handlers executed?
		/// </summary>
		public static bool ExecutedBootHandlers { get; private set; } = false;
		#endregion
		
		#region GETTERS
		/// <summary>
		/// Gets the GameToggle of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of toggle to get.</typeparam>
		/// <returns>The toggle to return.</returns>
		public static T GetToggle<T>() where T : GameToggleDX {
			// Probe the SaveController for the desired Toggle.
			return SaveController.CurrentGameToggleSet.GetToggle<T>();
		}
		/// <summary>
		/// Finds a list of GameToggles that have the specified interface.
		/// This is usually good for events, like the GameController just initialized.
		/// </summary>
		/// <typeparam name="T">The type of interface to look for.</typeparam>
		/// <returns>All of the toggles that implement the given interface.</returns>
		public static List<T> GetToggles<T>() where T : GTComponent {
			return SaveController.CurrentGameToggleSet.GetToggles<T>();
		}
		#endregion

		#region BOOT HANDLERS
		/// <summary>
		/// Grabs GameToggles that should be executed on boot and executes them accordingly.
		/// </summary>
		public static void ProcessBootToggles() {

			// Update the flag that marks the boot toggles as processed.
			ToggleController.ExecutedBootHandlers = true;
			
			// This one is a bit different. Grab the full screen toggle and use that with the screen resolution toggle.
			bool fullScreen = ToggleController.GetToggle<Fullscreen>().GetToggleEnum() == ScreenModeType.Fullscreen;
			ToggleController.GetToggle<ScreenResolution>().Execute(fullScreen: fullScreen);
			
			// Call all the toggles that have initialization handlers.
			List<GTGameBootHandler> bootHandlerToggles = ToggleController.GetToggles<GTGameBootHandler>();
			foreach (GTGameBootHandler bootHandlerToggle in bootHandlerToggles) {
				bootHandlerToggle.OnGameInitialize();
			}
		}
		#endregion
		
	}


}