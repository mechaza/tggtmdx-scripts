using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.Battle.Modifiers;
using System.Linq;

namespace Grawly.Battle.Navigator {
	
	/// <summary>
	/// The different types of log entries that can exist.
	/// </summary>
	public enum NavigatorLogEntryType {
		None					= 0,
		
		NormalBattleStart		= 101,
		
		
	}
	
}