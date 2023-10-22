using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle {

	// Interfaces specific to badges.

	/// <summary>
	/// Adds BattleBehaviors to a combatant's moveset.
	/// </summary>
	public interface IBehaviorAdder {
		/// <summary>
		/// Adds battle behavior's to a combatant's moveset.
		/// Usually means I have a badge that does this.
		/// </summary>
		/// <param name="self">The combatant currently in use, if needed.</param>
		/// <returns>The behaviors to add.</returns>
		List<BattleBehavior> GetBattleBehaviorsToAdd(Combatant self);
	}

	/// <summary>
	/// Gets called when the attached badge is added.
	/// </summary>
	public interface IBadgeAddHandler {
		/// <summary>
		/// Gets called when this badge is added to a combatant.
		/// </summary>
		/// <param name="self">The combatant who wants to wear the badge.</param>
		void OnBadgeAdd(Combatant self);
	}
	/// <summary>
	/// Gets called when this badge is removed from a combatant.
	/// </summary>
	public interface IBadgeRemoveHandler {
		/// <summary>
		/// Gets called when this badge is removed from a combatant.
		/// </summary>
		/// <param name="self">The combatant who wants to wear this badge.</param>
		void OnBadgeRemove(Combatant self);
	}

}