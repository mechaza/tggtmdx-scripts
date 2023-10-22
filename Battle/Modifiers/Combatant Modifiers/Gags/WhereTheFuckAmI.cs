using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Just brings up a prompt.
	/// Begins the battle if the close callback is false, ends the battle if the close callback is true.
	/// </summary>
	public class WhereTheFuckAmI : CombatantModifier, IOnBattleStart {

		#region FIELDS - RESOURCES
		/// <summary>
		/// The script that will be played at the start.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "Resources")]
		private TextAsset chatScript;
		#endregion

		#region ITERFACE IMPLEMENATION - IONBATTLESTART
		/// <summary>
		/// At the start of the battle... a little nasty boy will start talking.
		/// </summary>
		/// <returns></returns>
		public BattleReaction OnBattleStart() {
			return delegate (BattleReactionSequence battleReactionSequence) {
				
				Sequence seq = DOTween.Sequence();

				seq.AppendCallback(new TweenCallback(delegate {
					BattleCameraController.Instance.ActivateVirtualCamera(cam: (this.combatantOwner as Enemy).WorldEnemyDX.ZoomInCamera);
					BattleNotifier.DisplayNotifier("The clown began talking...", 3f);
				}));

				seq.AppendInterval(3f);

				seq.AppendCallback(new TweenCallback(delegate {
					BattleCameraController.Instance.SetVignette(amt: 5, time: 2f);
					// Open up the chat controller.
					ChatControllerDX.GlobalOpen(
						textAsset: this.chatScript,
						chatOpenedCallback: delegate {
							PlayerStatusDXController.instance.TweenVisible(status: false, inBattle: true);
							// Dungeon.DungeonHUDController.Instance.TweenPlayerStatuses(status: false);
						},

						// HERES WHERE THE FUN HAPPENS
						chatClosedCallback: delegate(string str, int num, bool toggle) {
							// If the toggle is false, just continue the battle.
							if (toggle == false) {
								BattleCameraController.Instance.SetVignette(amt: 0, time: 0.5f);
								PlayerStatusDXController.instance.TweenVisible(status: true, inBattle: true);
								// Dungeon.DungeonHUDController.Instance.TweenPlayerStatuses(status: true);
								BattleCameraController.Instance.PageTurnTransition();
								battleReactionSequence.ExecuteNextReaction();
							} else {
								// If its true... end the battle. 
								BattleCameraController.Instance.SetVignette(amt: 0, time: 4f);
								BattleController.Instance.FSM.SendEvent("Battle Complete");
							}
						});
				}));
				

				seq.Play();
			};
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "A test to see if I can get a chat running at the battle start.";
		/// <summary>
		/// A string that details how this function works.
		/// </summary>
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}		
		#endregion

	}


}