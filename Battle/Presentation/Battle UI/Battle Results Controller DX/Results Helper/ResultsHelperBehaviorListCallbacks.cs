using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using System.Linq;
using DG.Tweening.Core;
using Grawly.Battle.Functions;
using Grawly.UI;
using UnityEngine.EventSystems;
using Grawly.Battle.Analysis;
using Grawly.Toggles.Proto;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;

namespace Grawly.Battle.Results {
	
	/// <summary>
	/// Literally just meant to help with setting up things in the results controller.
	/// I define things like callback sets for the analysis screen and junk.
	/// </summary>
	public partial class BattleResultsHelperDX {
		
		
		#region CALLBACK SETUP
		/// <summary>
		/// A callback to use in scenarios where an item on the behavior menu list is submitted
		/// while presenting a level up skill on the results screen's analysis menu.
		/// </summary>
		/// <param name="currentCombatant">The combatant currently displayed.</param>
		/// <param name="submittedBehavior">The behavior that was selected to be overwritten.</param>
		/// <param name="submittedMenuItem">The menu item that sent the event.</param>
		private void BehaviorListItemSubmit(Combatant currentCombatant, BattleBehavior submittedBehavior, CombatantAnalysisDXBehaviorListItem submittedMenuItem) {

			if (BattleResultsControllerDX.Instance.CurrentLearnSkillStateType ==  LearnSkillStateType.PostConfirmation) {
				BattleResultsControllerDX.Instance.ExecuteNextReaction();
				return;
			}
			
			// Open the chat to confirm.
			ChatControllerDX.GlobalOpen(
				promptLine: ": > Replace " + submittedBehavior.behaviorName + " with " 
				            + (currentCombatant as Persona).levelUpMoves.Peek().behavior.behaviorName 
				            + " ?; checker: true", 
				optionOne: "Confirm",
				optionTwo: "Cancel",
				chatClosedCallback: ((str, num, toggle) => {
					
					// REPLACING MOVE.
					if (toggle == true) {
						
						// Update the confirmation state.
						BattleResultsControllerDX.Instance.CurrentLearnSkillStateType = LearnSkillStateType.PostConfirmation;
						
						// Dequeue the behavior.
						BattleBehavior dequeuedBehavior = (currentCombatant as Persona).levelUpMoves.Dequeue().behavior;
						
						// Add it to the persona.
						currentCombatant.AddBehavior(behavior: dequeuedBehavior, toReplace: submittedBehavior);
						
						// Update the current behavior.
						submittedMenuItem.CurrentBattleBehavior = dequeuedBehavior;
						
						// Play the overwrite animation.
						AudioController.instance.PlaySFX(SFXType.PlayerBattleMenu, 1.5f);
						submittedMenuItem.PlayOverwriteAnimation();
						
						// Rebuild the auxiliary item.
						CombatantAnalysisControllerDX.Instance.BehaviorAuxiliaryItem.Rebuild(
							analysisParams: CombatantAnalysisControllerDX.Instance.CurrentAnalysisParams);
						
						// Run this on the next frame. Nested chats do not work well.
						GameController.Instance.RunEndOfFrame(() => {
							// Announce the change.
							BattleResultsControllerDX.Instance.AnnounceAddedSkill(
								currentPersona: currentCombatant as Persona, 
								menuItem: submittedMenuItem, 
								announcementClosedCallback: () => {
									if ((currentCombatant as Persona).IsNextSkillReady == true) {
										// If there is another skill that is ready, execute the next reaction.
										BattleResultsControllerDX.Instance.ExecuteNextReaction();
									} else {
										// If that was the last skill, select the menuItem.
										EventSystem.current.SetSelectedGameObject(submittedMenuItem.gameObject);
									}
								});
						});
						
						
						
					// NOT REPLACING MOVE.
					} else {
						// Reselect the move again.
						EventSystem.current.SetSelectedGameObject(submittedMenuItem.gameObject);
					}
				}));
		}
		/// <summary>
		/// A callback to use in scenarios where an item on the behavior menu list is submitted
		/// while presenting a level up skill on the results screen's analysis menu.
		/// </summary>
		/// <param name="currentCombatant">The combatant currently displayed.</param>
		/// <param name="menuItemBehavior">The behavior that was selected to be overwritten.</param>
		/// <param name="menuItem">The menu item that sent the event.</param>
		private void BehaviorListItemCancel(Combatant currentCombatant, BattleBehavior menuItemBehavior, CombatantAnalysisDXBehaviorListItem menuItem) {
			
			// If on post confirmation, it literally does not matter.
			if (BattleResultsControllerDX.Instance.CurrentLearnSkillStateType ==  LearnSkillStateType.PostConfirmation) {
				BattleResultsControllerDX.Instance.ExecuteNextReaction();
				return;
			}
			
			// Open the chat to confirm.
			ChatControllerDX.GlobalOpen(
				promptLine: ": > Forget " 
				            + (currentCombatant as Persona).levelUpMoves.Peek().behavior.behaviorName 
				            + " ?; checker: true", 
				optionOne: "Confirm",
				optionTwo: "Cancel",
				chatClosedCallback: ((str, num, toggle) => {
					
					// MOVE NOT BEING LEARNED
					if (toggle == true) {
						// Update the confirmation state.
						BattleResultsControllerDX.Instance.CurrentLearnSkillStateType = LearnSkillStateType.PostConfirmation;
						// Dequeue the move from the Persona. This just discards it.
						(currentCombatant as Persona).levelUpMoves.Dequeue();
						// Rebuild the auxiliary item.
						CombatantAnalysisControllerDX.Instance.BehaviorAuxiliaryItem.Rebuild(
							analysisParams: CombatantAnalysisControllerDX.Instance.CurrentAnalysisParams);
						if ((currentCombatant as Persona).IsNextSkillReady == true) {
							// If there is another skill that is ready, execute the next reaction.
							BattleResultsControllerDX.Instance.ExecuteNextReaction();
						} else {
							// If that was the last skill, select the menuItem.
							EventSystem.current.SetSelectedGameObject(menuItem.gameObject);
						}
						
						
						// STILL SELECTING
					} else {				
						// Reselect the move again.
						EventSystem.current.SetSelectedGameObject(menuItem.gameObject);
					}
					
					
				}));
		}
		#endregion
		
	}

}
