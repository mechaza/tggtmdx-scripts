using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.Modifiers;

namespace Grawly.Battle.Navigator {

	/// <summary>
	/// The way the navigator will behave most of the time.
	/// </summary>
	[System.Serializable]
	public class StandardNavigator : NavigatorBehavior, IOnBattleStart, IOnPreTurn, IOnTurnStart, IOnTurnReady, IOnBehaviorEvaluated, IOnTurnEnd {

		#region FIELDS - STATE
		/// <summary>
		/// The BattleTemplate this navigator was cloned from.
		/// </summary>
		private BattleTemplate currentBattleTemplate;
		/// <summary>
		/// The voice this navigator should use.
		/// </summary>
		private BattleNavigatorVoice currentNavigatorVoice;
		/// <summary>
		/// The canvas this navigator should use.
		/// </summary>
		private BattleNavigatorCanvasController currentNavigatorCanvas;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Preps the navigator for whatever context it needs when the specified battle is going to be run.
		/// </summary>
		/// <param name="battleTemplate">The BattleTemplate that this navigator will be commentating on.</param>
		/// <param name="navigatorVoice">The voice this navigator will be outputting to, if any.</param>
		/// <param name="navigatorCanvas">The canvas this navigator will be using, if any.</param>
		public override void Prepare(BattleTemplate battleTemplate, BattleNavigatorVoice navigatorVoice = null, BattleNavigatorCanvasController navigatorCanvas = null) {
			this.currentBattleTemplate = battleTemplate;
			this.currentNavigatorVoice = navigatorVoice;
			this.currentNavigatorCanvas = navigatorCanvas;
		}
		#endregion

		#region NAVIGATOR LINES
		/// <summary>
		/// Requests a line from the navigator in the form of a BattleReaction.
		/// </summary>
		/// <param name="navigatorParams">The params used to construct the phrase to be said.</param>
		/// <param name="navigatorVoice">The voice this navigator should use.</param>
		/// <param name="navigatorCanvas">The canvas this navigator should use.</param>
		/// <returns>A battle reaction containing the line to be said by the navigator, if any.</returns>
		public override BattleReaction RequestReactionLine(BattleNavigatorParams navigatorParams, BattleNavigatorVoice navigatorVoice = null, BattleNavigatorCanvasController navigatorCanvas = null) {
			return delegate (BattleReactionSequence battleReactionSequence) {
				// Tell the canvas to show the line with the generated parameters.
				navigatorCanvas.PresentNavigator(navigatorParams: navigatorParams);
				// Allow the voice to speak.
				navigatorVoice.Speak(navigatorParams: navigatorParams);
				// Wait a few moments, then dismiss the navigator.
				GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
					navigatorCanvas.DismissNavigator();
				});
				// Execute the next reaction.
				battleReactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region BATTLE EVENTS
		public BattleReaction OnBattleStart() {

			int enemyCount = BattleController.Instance.AliveEnemies.Count;
			string str = enemyCount.ToString() + " shadows up ahead! Go get em, girls!";

			return this.RequestReactionLine(
				navigatorParams: new BattleNavigatorParams() {
					dialogueText = str
				}, 
				navigatorVoice: this.currentNavigatorVoice,
				navigatorCanvas: this.currentNavigatorCanvas);
		}
		public BattleReaction OnPreTurn() {
			return NavigatorBehavior.GenerateBlankReaction();
			// throw new System.NotImplementedException();
		}
		public BattleReaction OnTurnStart() {
			return NavigatorBehavior.GenerateBlankReaction();
		}
		public BattleReaction OnTurnReady() {
			return NavigatorBehavior.GenerateBlankReaction();
		}
		public BattleReaction OnBehaviorEvaluated() {
			return NavigatorBehavior.GenerateBlankReaction();
		}
		public BattleReaction OnTurnEnd() {
			return NavigatorBehavior.GenerateBlankReaction();
		}
		#endregion

		/*#region INTERNAL COMPUTATION
		/// <summary>
		/// Generates a set of BattleNavigatorParams based on the request being made.
		/// </summary>
		/// <param name="requestType">The requested line type.</param>
		/// <returns></returns>
		protected override BattleNavigatorParams GenerateBattleNavigatorParams(BattleNavigatorRequestType requestType) {
			throw new System.NotImplementedException("");
		}
		#endregion*/

	}


}