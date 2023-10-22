using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Toggles.Proto {

	[System.Serializable, Title("Show Enemy Debug Info")]
	public class ShowEnemyDebugInfo : StandardBoolToggle {

		#region FIELDS - METADATA
		public override string ToggleName => "Show Enemy Debug Info";
		public override string ToggleDescription => "Display debug information on enemies when selecting them. This also unlocks their info in the analysis screen.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Debug;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

	}


}