using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.UI.SubItem;
using Grawly.UI;

namespace Grawly.Toggles.Gameplay {
	
	
	[System.Serializable, Title("Difficulty Override")]
	public class DifficultyOverride : StandardEnumToggle<DifficultyOverrideType> {

		#region FIELDS - METADATA
		public override string ToggleName => "Difficulty Override";
		public override string ToggleDescription => "Overrides the difficulty in the save file. Reset the game to see effects.";
		public override string QuantityString => this.GetToggleEnum().ToString();
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Gameplay;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

		
	}


}