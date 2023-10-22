using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Gets called halfway between a player's attack animation, if any of the enemies being targeted implement such a thing.
	/// </summary>
	public interface IInterceptOpponentAttackAnimation {
		/// <summary>
		/// The function to be called when interrupting the player animation.
		/// </summary>
		/// <param name="dcs">The DamageCalculationSet that was generated.</param>
		/// <param name="continueOnComplete">Should the routine continue when this routine is complete?</param>
		/// <returns>A routine that gets called inside the animation routine.</returns>
		IEnumerator OnInterceptOpponentAttackAnimation(DamageCalculationSet dcs);
	}

}