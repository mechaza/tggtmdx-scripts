using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle.Navigator {

	/// <summary>
	/// The way in which a BattleNavigator should behave.
	/// </summary>
	public abstract class NavigatorBehavior {

		#region PREPARATION
		/// <summary>
		/// Preps the navigator for whatever context it needs when the specified battle is going to be run.
		/// </summary>
		/// <param name="battleTemplate">The BattleTemplate that this navigator will be commentating on.</param>
		/// <param name="navigatorVoice">The voice this navigator will be outputting to, if any.</param>
		/// <param name="navigatorCanvas">The canvas this navigator will be using, if any.</param>
		public abstract void Prepare(BattleTemplate battleTemplate, BattleNavigatorVoice navigatorVoice = null, BattleNavigatorCanvasController navigatorCanvas = null);
		#endregion

		#region NAVIGATOR LINES
		/// <summary>
		/// Requests a line from the navigator in the form of a BattleReaction.
		/// </summary>
		/// <param name="navigatorParams">The params used to construct the phrase to be said.</param>
		/// <param name="navigatorVoice">The voice this navigator should use.</param>
		/// <param name="navigatorCanvas">The canvas this navigator should use.</param>
		/// <returns>A battle reaction containing the line to be said by the navigator, if any.</returns>
		public abstract BattleReaction RequestReactionLine(BattleNavigatorParams navigatorParams, BattleNavigatorVoice navigatorVoice = null, BattleNavigatorCanvasController navigatorCanvas = null);
		/// <summary>
		/// Just returns a blank reaction in the event that the navigator behavior currently in use does not implement a desired interface.
		/// </summary>
		/// <returns></returns>
		public static BattleReaction GenerateBlankReaction() {
			return delegate (BattleReactionSequence battleReactionSequence) {
				Debug.Log("Navigator not equipped to deal with this situation. Evaluating next reaction.");
				battleReactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region CLONING
		public NavigatorBehavior Clone() {
			return (this.MemberwiseClone() as NavigatorBehavior);
		}
		#endregion

	}


}