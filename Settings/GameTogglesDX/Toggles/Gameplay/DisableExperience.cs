using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Toggles.Gameplay {
	[System.Serializable, Title("Disable Experience")]
	public class DisableExperience : StandardBoolToggle {

		#region FIELDS - METADATA
		public override string ToggleName => "Disable Experience";
		public override string ToggleDescription => "Disables experience gain at the end of battles.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Gameplay;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

	}


}