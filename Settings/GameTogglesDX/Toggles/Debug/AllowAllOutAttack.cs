using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Toggles.Proto {

	[System.Serializable, Title("Allow All Out Attack")]
	public class AllowAllOutAttack : StandardBoolToggle {

		#region FIELDS - METADATA
		public override string ToggleName => "Allow All Out Attack";
		public override string ToggleDescription => "Whether or not All Out Attacks should be allowed. Overrides story flag preference if set to true.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Debug;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion

	}


}