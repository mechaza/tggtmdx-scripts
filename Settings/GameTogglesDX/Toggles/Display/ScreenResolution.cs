using System;
using UnityEngine;
using System.Collections;
using Grawly.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Toggles.Display {

	[System.Serializable, Title("Screen Resolution")]
	public class ScreenResolution : StandardEnumToggle<ScreenResolutionType>, GTHorizontalMoveHandler {

		#region FIELDS - METADATA
		public override string ToggleName => "Screen Resolution";
		public override string ToggleDescription => "The resolution to play the game at.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Display;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

		#region MAIN CALLS

		public void Execute(bool fullScreen) {
			
			// Grab the enum for if this should be in fullscreen or not.
			ScreenResolutionType screenResolutionType = this.GetToggleEnum();
			
			int width = this.GetWidth(screenResolutionType);
			int height = this.GetHeight(screenResolutionType);
			
			Screen.SetResolution(width: width, height: height, fullScreen);
			
		}
		public void Execute() {
			// this.Execute(fullScreen: Screen.fullScreen);
			this.Execute(fullScreen: ToggleController.GetToggle<Fullscreen>().IsFullScreen);
		}
	
		#endregion
		
		#region INTERFACE IMPLEMENTATION - GAMEBOOTHANDLER
		public override void OnHorizontalMenuMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMenuMove(moveDir);
			this.Execute();
		}
		#endregion

		#region GETTERS

		private int GetWidth(ScreenResolutionType resolutionType) {
			switch (resolutionType) {
				case ScreenResolutionType.None:
					return 1280;
				case ScreenResolutionType.HD:
					return 1280;
				case ScreenResolutionType.FullHD:
					return 1920;
				case ScreenResolutionType.QuadHD:
					return 2560;
				case ScreenResolutionType.UltraHD:
					return 1920 * 2;
				default:
					throw new NotImplementedException("i need to fix this");
			}
		}
		private int GetHeight(ScreenResolutionType resolutionType) {
			switch (resolutionType) {
				case ScreenResolutionType.None:
					return 720;
				case ScreenResolutionType.HD:
					return 720;
				case ScreenResolutionType.FullHD:
					return 1080;
				case ScreenResolutionType.QuadHD:
					return 1440;
				case ScreenResolutionType.UltraHD:
					return 1080 * 2;
				// return 720;
				default:
					throw new NotImplementedException("i need to fix this");
			}
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IMENUABLE
		/// <summary>
		/// The quantity string should be whatever the current value is.
		/// </summary>
		public override string QuantityString => this.GetToggleEnum().ToString();
		#endregion

	}
	public enum ScreenResolutionType {
		None = 0,
		HD = 1,			// 720p
		FullHD = 2,		// 1080p
		QuadHD = 3,		// 2560 x 1440
		UltraHD = 4,	// 4K
	}

}