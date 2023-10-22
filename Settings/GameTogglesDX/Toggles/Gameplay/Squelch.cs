using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Toggles.Gameplay {
	
	[System.Serializable, Title("Squelch Limit")]
	public class Squelch : StandardFloatToggle {

		#region FIELDS - METADATA
		public override string ToggleName => "Squelch Limit";
		public override string ToggleDescription => "The amount of spungo and Grime Sludge the player is allowed to digest.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Gameplay;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

	}


}