using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Toggles.Gameplay {
	
	[System.Serializable, Title("Player Attack Scale")]
	public class PlayerAttackScale : StandardFloatToggle {

		#region FIELDS - METADATA
		public override string ToggleName => "Player Attack Scale";
		public override string ToggleDescription => "Scales the player attack by the specified amount.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Debug;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

	}


}