using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.UI;

namespace Grawly.Toggles.Proto {
	
	[System.Serializable, Title("Skill Slot Cap")]
	public class CombatantSkillSlotCap : StandardIntToggle {

		
		
		#region FIELDS - METADATA
		public override string ToggleName => "Skill Slot Cap";
		public override string ToggleDescription => "Caps the number of skill slots on a combatant.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Debug;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion
		
	}
	
}

