using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Toggles.Audio {

	/// <summary>
	/// Adjusts the master volume for the entire application.
	/// </summary>
	[System.Serializable, Title("Master Volume")]
	public class MasterVolume : StandardFloatToggle {

		#region FIELDS - METADATA
		public override string ToggleName => "Master Volume";
		public override string ToggleDescription => "Scales the volume of the entire application.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Audio;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

		/*public override void Shift(ToggleShiftType shiftType) {
			throw new System.NotImplementedException();
		}*/
	}


}