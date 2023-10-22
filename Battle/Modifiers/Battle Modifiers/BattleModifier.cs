using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// A class similar to the CombatantModifier, but used explicitly for the battle as a whole and is not attached to any combatant in particular.
	/// </summary>
	public abstract class BattleModifier {
		
		/// <summary>
		/// Creates a clone of this BattleModifier. Usually used when it's being retrieved from a template.
		/// </summary>
		/// <returns></returns>
		public BattleModifier Clone() {
			Debug.Log("Cloning BattleModifier: " + this.ToString());
			return (BattleModifier)this.MemberwiseClone();
		}

	}


}