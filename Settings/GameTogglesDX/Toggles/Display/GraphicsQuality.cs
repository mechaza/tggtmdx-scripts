using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.UI;

namespace Grawly.Toggles.Display {

	[System.Serializable, Title("Graphics Quality")]
	public class GraphicsQuality : StandardEnumToggle<GraphicsQualityType>, GTGameBootHandler {

		#region FIELDS - METADATA
		public override string ToggleName => "Graphics Quality";
		public override string ToggleDescription => "The quality of the graphics of the game.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Display;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

		#region MAIN FUNCTIONALITY
		private void Execute() {
			GraphicsQualityType graphicsQualityType = this.GetToggleEnum();
			int index = (int)graphicsQualityType;
			switch (graphicsQualityType) {
				case GraphicsQualityType.Low:
					QualitySettings.SetQualityLevel(index: index, applyExpensiveChanges: true);
					break;
				case GraphicsQualityType.Medium:
					QualitySettings.SetQualityLevel(index: index, applyExpensiveChanges: true);
					break;
				case GraphicsQualityType.High:
					QualitySettings.SetQualityLevel(index: index, applyExpensiveChanges: true);
					break;
				case GraphicsQualityType.Golden:
					QualitySettings.SetQualityLevel(index: index, applyExpensiveChanges: true);
					break;
				default:
					Debug.LogError("Could not set the graphics quality! Defaulting to Low.");
					QualitySettings.SetQualityLevel(index: 0, applyExpensiveChanges: true);
					break;

			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		/// <summary>
		/// The quantity string should be whatever the current value is.
		/// </summary>
		public override string QuantityString => this.GetToggleEnum().ToString();
		#endregion

		#region INTERFACE IMPLEMENTATION - ON GAMECONTROLLER INITIALIZE
		public void OnGameInitialize() {
			this.Execute();
		}
		#endregion

		#region INTERFACE IMPLEMENTION - HORIZONTAL MOVE
		public override void OnHorizontalMenuMove(HorizontalMoveDirType moveDir) {
			
			
			
			base.OnHorizontalMenuMove(moveDir);
			this.Execute();
			
			
			
		}
		#endregion

	}
	public enum GraphicsQualityType {
		Low = 0,
		Medium = 1,
		High = 2,
		Golden = 3,
	}

}