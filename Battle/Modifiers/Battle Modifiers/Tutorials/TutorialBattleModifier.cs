using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI;
using Grawly.Battle.BattleMenu;
using Grawly.Chat;

namespace Grawly.Battle.Modifiers.Tutorials {

	/// <summary>
	/// Shows a tutorial at the start of the battle.
	/// </summary>
	public class TutorialBattleModifier : BattleModifier, IOnBattleStart {

		#region FIELDS
		/// <summary>
		/// Sorta also shoehorning this in but... the chat that should be run prior to running this shit.
		/// </summary>
		[SerializeField]
		private TextAsset chatScript;
		/// <summary>
		/// The tutorial template to use when starting up this battle.
		/// </summary>
		[SerializeField]
		private TutorialTemplate tutorialTemplate;
		#endregion

		#region INTERFACE IMPLEMENTATION - IONBATTLESTART
		/// <summary>
		/// When the battle starts, it should bring up the tutorial screen and play the given tutorial.
		/// </summary>
		/// <returns></returns>
		public BattleReaction OnBattleStart() {
			return delegate (BattleReactionSequence battleReactionSequence) {

				// Tween the statuses out. I give them half a second because otherwise there is an issue with the animation.
				GameController.Instance.WaitThenRun(timeToWait: 0.5f, action: delegate {
					PlayerStatusDXController.instance.TweenVisible(status: false, inBattle: true);
				});

				// Open the chat where one of the characters says some weird shit like "OH NO CLOWNS"
				ChatControllerDX.GlobalOpen(textAsset: this.chatScript,
					chatOpenedCallback: delegate { },
					chatClosedCallback: delegate {          // When the chat closes, open up the tutorial.

						// Open up the tutorial and when it closes out, evaluate the next action.
						TutorialScreenController.OpenTutorial(
							tutorialTemplate: this.tutorialTemplate,
							objectToReselectOnClose: null,
							actionOnClose: delegate {
								// Tween the statuses in when the tutorial closes..
								PlayerStatusDXController.instance.TweenVisible(status: true, inBattle: true);
								// Evaluate the next action.
								GameController.Instance.WaitThenRun(timeToWait: 0.5f, action: delegate {
									battleReactionSequence.ExecuteNextReaction();
								});
							});

					});
			};
		}
		#endregion

	}


}