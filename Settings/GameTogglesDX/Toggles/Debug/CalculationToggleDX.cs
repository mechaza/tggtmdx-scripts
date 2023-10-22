using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.UI;

namespace Grawly.Toggles.Audio {

	/// <summary>
	/// Adjusts the master volume for the entire application.
	/// </summary>
	[System.Serializable, Title("Base Power Multiplier")]
	public class BasePowerMultiplier : StandardFloatToggle {
		
		public override void OnHorizontalMenuMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMenuMove(moveDir);
			GameController.Instance.DebugBasePowerMultiplier = this.GetToggleFloat();
		}

		#region FIELDS - METADATA
		public override string ToggleName => "Base Power Multiplier";
		public override string ToggleDescription => "Multiplies the power of moves in battle calculations.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Debug;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion


	}
	
	/// <summary>
	/// Adjusts the master volume for the entire application.
	/// </summary>
	[System.Serializable, Title("Final Amount Multiplier")]
	public class FinalAmountMultiplier : StandardFloatToggle {

		public override void OnHorizontalMenuMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMenuMove(moveDir);
			GameController.Instance.DebugFinalAmountMultiplier = this.GetToggleFloat();
		}
		
		#region FIELDS - METADATA
		public override string ToggleName => "Final Amount Multiplier";
		public override string ToggleDescription => "Multiplies the result of battle calculations at the very end.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Debug;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

	}


}