using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using System.Linq;
using Grawly.Battle.Functions;
using Grawly.UI;
using UnityEngine.EventSystems;
using Grawly.Battle.Results;
using Grawly.UI.Legacy;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// This is just to help me define routines for learning skill cards.
	/// </summary>
	public class AnalysisSkillCardUseHelper : MonoBehaviour {
		
		public static AnalysisSkillCardUseHelper Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// A queue of actions that can be used to enqueue things that need to be handled during the results screen.
		/// </summary>
		private Queue<Action> CurrentReactionQueue { get; set; } = new Queue<Action>();
		/// <summary>
		/// The card behavior that is currently being learned.
		/// </summary>
		private BattleBehavior CurrentCardBehavior { get; set; }
		/// <summary>
		/// The current state of the screen that lets you select a skill to learn.
		/// </summary>
		public LearnSkillStateType CurrentLearnSkillStateType { get; set; } = LearnSkillStateType.PreConfirmation;
		#endregion
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The callback set to use when showing a persona learning a new skill from their level up moves.
		/// I define all of this here so I don't clutter the controller.
		/// </summary>
		public AnalysisCallbackSet PersonaLearnSkillCallbackSet { get; private set; } = new AnalysisCallbackSet();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				this.PersonaLearnSkillCallbackSet.OnBehaviorItemSubmit = tuple => {
					this.BehaviorListItemSubmit(
						currentCombatant: tuple.combatant, 
						submittedBehavior: tuple.listItem.CurrentBattleBehavior, 
						submittedMenuItem: tuple.listItem);
				};
				this.PersonaLearnSkillCallbackSet.OnBehaviorItemCancel = tuple => {
					this.BehaviorListItemCancel(
						currentCombatant: tuple.combatant, 
						menuItemBehavior: tuple.listItem.CurrentBattleBehavior,
						menuItem: tuple.listItem);
				};
			}
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the skill card helper.
		/// </summary>
		private void ResetState() {
			Debug.Log("Resetting skill card helper state");
			this.CurrentLearnSkillStateType = LearnSkillStateType.PreConfirmation;
			this.CurrentReactionQueue.Clear();
			this.CurrentCardBehavior = null;
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the analysis screen so that a skill card can be used.
		/// </summary>
		/// <param name="persona">The persona who should learn the skill card.</param>
		/// <param name="cardBehavior">The actual card to learn.</param>
		/// <param name="onAnalysisCloseCallback">The callback to run on analysis close.</param>
		public void Present(Persona persona, BattleBehavior cardBehavior, Action onAnalysisCloseCallback) {
			
			// Reset the state.
			this.ResetState();
			
			// Remember the card.
			this.CurrentCardBehavior = cardBehavior;
			
			// Set up the reaction queue.
			this.CurrentReactionQueue = this.GenerateReactionQueue(
				persona: persona, 
				cardBehavior: cardBehavior,
				onAnalysisCloseCallback: onAnalysisCloseCallback);
			
			// Execute it.
			this.ExecuteNextReaction();
			
		}
		/// <summary>
		/// Flashes the analysis screen.
		/// </summary>
		private void FlashAnalysisScreen() {
			Flasher.instance.Flash();
			AudioController.instance.PlaySFX(SFXType.PlayerExploit);
		}
		#endregion
		
		#region REACTION QUEUE
		/// <summary>
		/// Generates a queue of all the things that need to be done on the results screen when provided with the given results.
		/// </summary>
		/// <param name="persona">The persona that should learn this skill card.</param>
		/// <param name="cardBehavior">The actual skill card to learn.</param>
		/// <param name="onAnalysisCloseCallback">The callback to run on analysis close.</param>
		/// <returns>A queue of actions that asynchronously handles the things that need to be done.</returns>
		private Queue<Action> GenerateReactionQueue(Persona persona, BattleBehavior cardBehavior, Action onAnalysisCloseCallback) {
			
			// Create a new queue.
			Queue<Action> reactionQueue = new Queue<Action>();
			
			// Add a reaction that presents the personas with these skills.
			reactionQueue.Enqueue(() => {
				this.PresentLearnSkillCard(
					persona: persona, 
					cardBehavior: cardBehavior, 
					onAnalysisCloseCallback: onAnalysisCloseCallback);
			});

			// Close out the analysis screen.
			reactionQueue.Enqueue(() => {
				CombatantAnalysisControllerDX.Instance.Close();	
			});	
			
			return reactionQueue;

		}
		/// <summary>
		/// Executes the next action in the reaction queue.
		/// </summary>
		public void ExecuteNextReaction() {
			Debug.Log("EXECUTING NEXT RESULTS REACTION.");
			this.CurrentReactionQueue.Dequeue().Invoke();
		}
		/// <summary>
		/// Executes the next action in the reaction queue after a delay.
		/// </summary>
		/// <param name="delay">The amount of time to wait before actually executing.</param>
		private void ExecuteNextReaction(float delay) {
			GameController.Instance.WaitThenRun(delay, () => {
				this.ExecuteNextReaction();
			});
		}
		#endregion
		
		#region PERSONA SKILL UNLOCK - MAIN CALLS
		/// <summary>
		/// Opens the combatant analysis screen so that skills can be shown being learned.
		/// </summary>
		/// <param name="persona">The persona to present.</param>
		/// <param name="cardBehavior">The actual skill card to learn.</param>
		/// <param name="onAnalysisCloseCallback">The callback to run on analysis close.</param>
		private void PresentLearnSkillCard(Persona persona, BattleBehavior cardBehavior, Action onAnalysisCloseCallback) {
			
			// Flash the analysis screen.
			this.FlashAnalysisScreen();
			
			// Open the analysis controller so the persona can learn their new move.
			CombatantAnalysisControllerDX.Instance.Open(
				analysisType: AnalysisScreenCategoryType.LearnSkillCard, 
				focusCombatant: persona,
				callbackSet: this.PersonaLearnSkillCallbackSet,
				presentCompleteCallback: () => {
					// Wait a few seconds before proceeding.
					GameController.Instance.WaitThenRun(timeToWait: 2f, () => {
						this.LearnSkillCard(currentPersona: persona, cardBehavior: cardBehavior);
						// this.ExecuteNextReaction();
					});
				},
				analysisDismissedCallback: () => {
					GameController.Instance.RunEndOfFrame(() => {
						onAnalysisCloseCallback.Invoke();
					});
				});
			
		}
		/// <summary>
		/// Begins the process of learning the next level skill for the provided persona.
		/// </summary>
		/// <param name="currentPersona">The persona who is learning a new move.</param>
		/// <param name="cardBehavior">The actual skill card to learn.</param>
		private void LearnSkillCard(Persona currentPersona, BattleBehavior cardBehavior) {
			
			// Reset the state on the learn skill state to pre confirmation.
			this.CurrentLearnSkillStateType = LearnSkillStateType.PreConfirmation;

			// Check if this behavior can be added.
			bool emptySlotAvailable = currentPersona.CanAddBehavior(behavior: cardBehavior);
			
			
			if (emptySlotAvailable == true) {
				// Update the state type.
				this.CurrentLearnSkillStateType = LearnSkillStateType.PostConfirmation;
				// Learn the behavior.
				currentPersona.AddBehavior(cardBehavior);
				// Append it to the list and focus on it.
				CombatantAnalysisControllerDX.Instance.BehaviorMenuList.Append(cardBehavior, focusOnAdd: true);
				// Grab the appended item. Note that since it was focused on the append above, it should be displayed.
				var appendedMenuItem = CombatantAnalysisControllerDX.Instance.BehaviorMenuList.GetFocusedMenuItem(cardBehavior);
				// Animate highlighting it. 
				AudioController.instance.PlaySFX(SFXType.PlayerBattleMenu, 1.5f);
				appendedMenuItem.PlayOverwriteAnimation();
				// Wait a few seconds then announce the skill as added. This will also select it.
				GameController.Instance.WaitThenRun(2f, () => {
					this.AnnounceAddedSkill(
						currentPersona: currentPersona,
						menuItem: appendedMenuItem, 
						announcementClosedCallback: () => {
							// If that was the last skill, select the menuItem.
							EventSystem.current.SetSelectedGameObject(appendedMenuItem.gameObject);
						});
				});
							
			} else {
				// If no slot is available, prompt for replacement.
				ChatControllerDX.GlobalOpen(
					scriptLine:": > Please select a skill to replace.; checker: true", 
					simpleClosedCallback: () => {
						CombatantAnalysisControllerDX.Instance.BehaviorMenuList.SelectFirstMenuListItem();
					});
			}
			
		}
		#endregion
		
		#region PERSONA SKILL UNLOCK - ANNOUNCEMENTS AND FLASHES
		/// <summary>
		/// Announces that the persona has just learned the specified item.
		/// </summary>
		/// <param name="currentPersona">The persona currently learning the skill.</param>
		/// <param name="menuItem">The menu item containing the skill that was learned.</param>
		/// <param name="announcementClosedCallback">The action to invoke upon closing the confirmation.</param>
		public void AnnounceAddedSkill(Persona currentPersona, CombatantAnalysisDXBehaviorListItem menuItem, Action announcementClosedCallback) {
			
			// Update the state to be post confirmation.
			this.CurrentLearnSkillStateType = LearnSkillStateType.PostConfirmation;
			
			// Prep the string. This is just for convinience.
			string confirmationStr = currentPersona.metaData.name + " has learned " + menuItem.CurrentBattleBehavior.behaviorName + "!";
			ChatControllerDX.GlobalOpen(
				scriptLine: ": > " + confirmationStr + "; checker: true", 
				chatClosedCallback: ((str, num, toggle) => {
					announcementClosedCallback.Invoke();
				}));
			
		}
		#endregion
		
		#region CALLBACK SETUP
		/// <summary>
		/// A callback to use in scenarios where an item on the behavior menu list is submitted
		/// while presenting a level up skill on the results screen's analysis menu.
		/// </summary>
		/// <param name="currentCombatant">The combatant currently displayed.</param>
		/// <param name="submittedBehavior">The behavior that was selected to be overwritten.</param>
		/// <param name="submittedMenuItem">The menu item that sent the event.</param>
		private void BehaviorListItemSubmit(Combatant currentCombatant, BattleBehavior submittedBehavior, CombatantAnalysisDXBehaviorListItem submittedMenuItem) {

			if (this.CurrentLearnSkillStateType ==  LearnSkillStateType.PostConfirmation) {
				this.ExecuteNextReaction();
				return;
			}
			
			// Open the chat to confirm.
			ChatControllerDX.GlobalOpen(
				promptLine: ": > Replace " + submittedBehavior.behaviorName + " with " 
				            + this.CurrentCardBehavior.behaviorName 
				            + " ?; checker: true", 
				optionOne: "Confirm",
				optionTwo: "Cancel",
				chatClosedCallback: ((str, num, toggle) => {
					
					// REPLACING MOVE.
					if (toggle == true) {
						
						// Update the confirmation state.
						this.CurrentLearnSkillStateType = LearnSkillStateType.PostConfirmation;
						
						// Add it to the persona.
						currentCombatant.AddBehavior(behavior: this.CurrentCardBehavior, toReplace: submittedBehavior);
						
						// Update the current behavior.
						submittedMenuItem.CurrentBattleBehavior =  this.CurrentCardBehavior;
						
						// Play the overwrite animation.
						AudioController.instance.PlaySFX(SFXType.PlayerBattleMenu, 1.5f);
						submittedMenuItem.PlayOverwriteAnimation();
						
						// Run this on the next frame. Nested chats do not work well.
						GameController.Instance.RunEndOfFrame(() => {
							// Announce the change.
							this.AnnounceAddedSkill(
								currentPersona: currentCombatant as Persona, 
								menuItem: submittedMenuItem, 
								announcementClosedCallback: () => {
									// If that was the last skill, select the menuItem.
									EventSystem.current.SetSelectedGameObject(submittedMenuItem.gameObject);
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
			if (this.CurrentLearnSkillStateType ==  LearnSkillStateType.PostConfirmation) {
				this.ExecuteNextReaction();
				return;
			}
			
			// Open the chat to confirm.
			ChatControllerDX.GlobalOpen(
				promptLine: ": > Stop learning " 
				            + this.CurrentCardBehavior.behaviorName 
				            + " ?; checker: true", 
				optionOne: "Confirm",
				optionTwo: "Cancel",
				chatClosedCallback: ((str, num, toggle) => {
					
					// MOVE NOT BEING LEARNED
					if (toggle == true) {
						// Update the confirmation state.
						this.CurrentLearnSkillStateType = LearnSkillStateType.PostConfirmation;
						GameController.Instance.RunEndOfFrame(() => {
							// If there is another skill that is ready, execute the next reaction.
							this.ExecuteNextReaction();
						});
						
						
						
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
