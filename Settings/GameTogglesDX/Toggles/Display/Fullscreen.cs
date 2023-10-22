using System.Collections;
using System.Collections.Generic;
using Grawly.UI;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Toggles.Display {

	[System.Serializable, Title("Fullscreen")]
	public class Fullscreen : StandardEnumToggle<ScreenModeType> {

		#region FIELDS - METADATA
		public override string ToggleName => "Fullscreen";
		public override string ToggleDescription => "Should the game run in Fullscreen mode?";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Display;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// Is this toggle set to be fullscreen?
		/// </summary>
		public bool IsFullScreen {
			get {
				return this.GetToggleEnum() == ScreenModeType.Fullscreen;
			}
		}
		#endregion
		
		#region EXECUTION
		private void Execute() {

			// Check whether this enum toggle is set to be fullscreen or not.
			bool fullScreen = this.IsFullScreen;
			
			// This is a bit... awkward, but go ahead and call the screen resolution toggle with this info.
			ToggleController.GetToggle<ScreenResolution>().Execute(fullScreen: fullScreen);
			
			
			// Screen.fullScreen = true;
		}
		#endregion
		
		#region OVERRIDES
		public override void OnHorizontalMenuMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMenuMove(moveDir);
			this.Execute();
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IMENUABLE
		/// <summary>
		/// The quantity string should be whatever the current value is.
		/// </summary>
		public override string QuantityString => this.GetToggleEnum().ToString();
		#endregion

	}

	/// <summary>
	/// The way to present the game's screen.
	/// </summary>
	public enum ScreenModeType {
		Windowed = 0,
		Fullscreen = 1,
	}

}