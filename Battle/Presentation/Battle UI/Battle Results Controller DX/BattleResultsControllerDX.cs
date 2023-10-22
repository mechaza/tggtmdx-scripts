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
using Grawly.Battle.Analysis;
using Grawly.UI.Legacy;

namespace Grawly.Battle.Results {
	
	/// <summary>
	/// The new way that the results should be displayed.
	/// </summary>
	public class BattleResultsControllerDX : MonoBehaviour {

		public static BattleResultsControllerDX Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The current BattleRewards being used in this results controller.
		/// </summary>
		private BattleResultsSet CurrentResultsSet { get; set; }
		/// <summary>
		/// A queue of actions that can be used to enqueue things that need to be handled during the results screen.
		/// </summary>
		private Queue<Action> CurrentReactionQueue { get; set; } = new Queue<Action>();
		/// <summary>
		/// The current state of the screen that lets you select a skill to learn.
		/// </summary>
		public LearnSkillStateType CurrentLearnSkillStateType { get; set; } = LearnSkillStateType.PreConfirmation;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all other objects.
		/// </summary>
		[SerializeField, TabGroup("Results", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The borders that are used on the results screen.
		/// </summary>
		[SerializeField, TabGroup("Results", "Scene References")]
		private ChatBorders battleResultsBorders;
		/// <summary>
		/// The bust up to use on the results screen.
		/// </summary>
		[SerializeField, TabGroup("Results", "Scene References")]
		private BattleResultsDXCombatantBustUp combatantBustUp;
		/// <summary>
		/// A list containing the elements to display results with.
		/// </summary>
		[SerializeField, TabGroup("Results", "Scene References")]
		private List<BattleResultsDXElement> battleResultsElementList = new List<BattleResultsDXElement>();
		/// <summary>
		/// The Selectable that should be used to advance when the screen is done being read from.
		/// </summary>
		[SerializeField, TabGroup("Results", "Scene References")]
		private Selectable hiddenSelectable;
		/// <summary>
		/// The Image that displays the diamond pattern.
		/// </summary>
		[SerializeField, TabGroup("Results", "Scene References")]
		private Image diamondBackgroundImage;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				ResetController.AddToDontDestroy(this.gameObject);	
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the results controller.
		/// </summary>
		private void ResetState() {
			Debug.Log("Resetting results controller state");
			this.allObjects.SetActive(false);
			this.CurrentLearnSkillStateType = LearnSkillStateType.None;
			this.hiddenSelectable.gameObject.SetActive(false);
			this.CurrentResultsSet = null;
			this.CurrentReactionQueue.Clear();
			this.battleResultsBorders.ResetState();
			this.combatantBustUp.ResetState();
			this.battleResultsElementList.ForEach(re => re.ResetState());
		}
		#endregion

		#region REACTION QUEUE
		/// <summary>
		/// Generates a queue of all the things that need to be done on the results screen when provided with the given results.
		/// </summary>
		/// <param name="resultsSet">The results containing the data that needs to be handled.</param>
		/// <returns>A queue of actions that asynchronously handles the things that need to be done.</returns>
		private Queue<Action> GenerateReactionQueue(BattleResultsSet resultsSet) {
			
			// Create a new queue.
			Queue<Action> reactionQueue = new Queue<Action>();
			// Get the personas that have skills that need to be learned.
			List<Persona> personasWithUnlockedSkills = resultsSet.LeveledUpPersonas
				.Where(p => p.IsNextSkillReady)
				.ToList();

			
			// If there are ANY personas with skills unlocked, start learning them.
			if (personasWithUnlockedSkills.Count > 0) {
				// Add a reaction that presents the personas with these skills.
				reactionQueue.Enqueue(() => {
					this.PresentLearnSkillScreen(allCombatants: personasWithUnlockedSkills.Cast<Combatant>().ToList());
				});
				
				// Go through each persona and enqueue their reactions.
				foreach (Persona persona in personasWithUnlockedSkills) {
					// If this persona isn't the first, enqueue a reaction that focuses.
					if (persona != personasWithUnlockedSkills.First()) {
						reactionQueue.Enqueue(() => {
							this.FocusNewPersona(focusPersona: persona);
						});
					}
					// Generate a list of reactions for the provided persona and enqueue them.
					var reactionList = this.GenerateLearnSkillReactions(persona: persona);
					foreach (Action reaction in reactionList) {
						reactionQueue.Enqueue(reaction);
					}
				}
				
				// Close out the analysis screen.
				reactionQueue.Enqueue(() => {
					CombatantAnalysisControllerDX.Instance.Close();	
				});	
			}
			
			// At the end, add a reaction that transfers control.
			reactionQueue.Enqueue(() => {
				Gauntlet.GauntletController.instance?.SetFSMState(Gauntlet.GauntletStateType.Free);
				BattleController.Instance.CurrentBattleParams.BattleOutro.ReturnToCaller(
					template: BattleController.Instance.CurrentBattleTemplate, 
					battleParams: BattleController.Instance.CurrentBattleParams);
			});

			
			return reactionQueue;

		}
		/// <summary>
		/// Generates a list of reactions that allow you to learn all of the level skills.
		/// </summary>
		/// <param name="persona">The persona whos skills need to be learned.</param>
		/// <returns>A list of reactions to enqueue into the reaction queue.</returns>
		private List<Action> GenerateLearnSkillReactions(Persona persona) {
			
			// Create a new list of actions.
			List<Action> reactionList = new List<Action>();
			// Also grab the number of skills this persona is gonna learn.
			int skillsToLearn = persona.AvailableLevelSkillCount;
			
			// Add a reaction for each skill to learn.
			for (int j = 0; j < skillsToLearn; j++) {
				reactionList.Add(() => {
					this.LearnLevelSkill(currentPersona: persona);
				});
			}

			// Return the list.
			return reactionList;
			
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
		
		#region PRESENTATION
		/// <summary>
		/// Presents the BattleRewards onto the screen.
		/// </summary>
		/// <param name="battleResultsSet">The battle rewards to use.</param>
		public void Present(BattleResultsSet battleResultsSet) {
			
			// Reset the state.
			this.ResetState();
			
			// Turn it back on whoops!
			this.allObjects.SetActive(true);
			
			// Save the rewards and generate the reaction queue.
			this.CurrentResultsSet = battleResultsSet;
			this.CurrentReactionQueue = this.GenerateReactionQueue(resultsSet: battleResultsSet);
			
			// Present the borders on the next frame. I'm not sure why, but it doesn't fire otherwise.
			GameController.Instance.RunEndOfFrame(() => {
				this.battleResultsBorders.PresentBorders();
			});
			
			// Also present the bust up.
			this.combatantBustUp.Present(resultsSet: battleResultsSet);
			
			// Grab the elements that display results info and present them. Only present items if they exist.
			this.GetResultsElement(BattleResultsDXElementType.Experience).Present(resultsSet: battleResultsSet);
			this.GetResultsElement(BattleResultsDXElementType.Money).Present(resultsSet: battleResultsSet);
			if (battleResultsSet.ContainsItems == true) {
				this.GetResultsElement(BattleResultsDXElementType.Item).Present(resultsSet: battleResultsSet);
			}
			
			// After a second, turn the hidden selectable on.
			GameController.Instance.WaitThenRun(timeToWait:1f, () => {
				this.hiddenSelectable.gameObject.SetActive(true);
				EventSystem.current.SetSelectedGameObject(this.hiddenSelectable.gameObject);
			});
			
		}
		/// <summary>
		/// Dismisses the results controller from view.
		/// </summary>
		public void TotalHide() {
			this.ResetState();
		}
		/// <summary>
		/// Flashes the analysis screen.
		/// </summary>
		private void FlashAnalysisScreen() {
			Flasher.instance.Flash();
			AudioController.instance.PlaySFX(SFXType.PlayerExploit);
		}
		#endregion

		#region PERSONA SKILL UNLOCK - MAIN CALLS
		/// <summary>
		/// Opens the combatant analysis screen so that skills can be shown being learned.
		/// </summary>
		/// <param name="allCombatants">The parameters to use in opening the screen.</param>
		private void PresentLearnSkillScreen(List<Combatant> allCombatants) {
			
			// Flash the analysis screen.
			this.FlashAnalysisScreen();
			
			// Open the analysis controller so the persona can learn their new move.
			CombatantAnalysisControllerDX.Instance.Open(
				analysisType: AnalysisScreenCategoryType.PersonaLevelUp, 
				combatants: allCombatants,
				callbackSet: BattleResultsHelperDX.Instance.PersonaLearnSkillCallbackSet,
				presentCompleteCallback: () => {
					// Wait a few seconds before proceeding.
					GameController.Instance.WaitThenRun(timeToWait: 2f, () => {
						this.ExecuteNextReaction();
					});
				},
				analysisDismissedCallback: () => {
					GameController.Instance.WaitThenRun(timeToWait:1f, () => {
						this.hiddenSelectable.gameObject.SetActive(true);
						EventSystem.current.SetSelectedGameObject(this.hiddenSelectable.gameObject);
					});
				});
			
		}
		/// <summary>
		/// Rebuilds the analysis screen to show the provided Persona.
		/// This is usually when I'm done with learning skills for one
		/// but need to switch to another without closing the analysis screen.
		/// </summary>
		/// <param name="focusPersona">The persona to focus on.</param>
		private void FocusNewPersona(Persona focusPersona) {
			
			// Flash the screen.
			this.FlashAnalysisScreen();
			
			// Tell the analysis controller to focus on the provided persona. This effectively rebuilds the screen.
			CombatantAnalysisControllerDX.Instance.SetFocusCombatant(combatant: focusPersona);
			
			// Wait two seconds, then execute the next reaction.
			GameController.Instance.WaitThenRun(timeToWait: 2f, () => {
				this.ExecuteNextReaction();
			});
			
		}
		/// <summary>
		/// Begins the process of learning the next level skill for the provided persona.
		/// </summary>
		/// <param name="currentPersona">The persona who is learning a new move.</param>
		private void LearnLevelSkill(Persona currentPersona) {
			
			// Reset the state on the learn skill state to pre confirmation.
			this.CurrentLearnSkillStateType = LearnSkillStateType.PreConfirmation;

			// Grab the upcoming skill from the persona.
			BattleBehavior nextLevelSkill = currentPersona.levelUpMoves.Peek().behavior;
			
			// Check if this behavior can be added.
			bool emptySlotAvailable = currentPersona.CanAddBehavior(behavior: nextLevelSkill);
			
			// Flash the auxiliary item.
			AudioController.instance.PlaySFX(SFXType.PlayerBattleMenu, 1.5f);
			CombatantAnalysisControllerDX.Instance.BehaviorAuxiliaryItem.PlayOverwriteAnimation();
			
			// Wait two seconds, then begin learning.
			GameController.Instance.WaitThenRun(2f, () => {
				if (emptySlotAvailable == true) {
					// // If there is an empty slot, let the persona learn its new skill.
					currentPersona.AddNextLevelSkill();
					// Append it to the list and focus on it.
					CombatantAnalysisControllerDX.Instance.BehaviorMenuList.Append(nextLevelSkill, focusOnAdd: true);
					// Grab the appended item. Note that since it was focused on the append above, it should be displayed.
					var appendedMenuItem = CombatantAnalysisControllerDX.Instance.BehaviorMenuList.GetFocusedMenuItem(nextLevelSkill);
					// Animate highlighting it. 
					AudioController.instance.PlaySFX(SFXType.PlayerBattleMenu, 1.5f);
					appendedMenuItem.PlayOverwriteAnimation();
					// Also rebuild the auxiliary item. The analysis params should be different now.
					CombatantAnalysisControllerDX.Instance.BehaviorAuxiliaryItem.Rebuild(
						analysisParams: CombatantAnalysisControllerDX.Instance.CurrentAnalysisParams);
					// Wait a few seconds then announce the skill as added. This will also select it.
					GameController.Instance.WaitThenRun(2f, () => {
						this.AnnounceAddedSkill(
							currentPersona: currentPersona,
							menuItem: appendedMenuItem, 
							announcementClosedCallback: () => {
								if (currentPersona.IsNextSkillReady == true) {
									// If there is another skill that is ready, execute the next reaction.
									BattleResultsControllerDX.Instance.ExecuteNextReaction();
								} else {
									// If that was the last skill, select the menuItem.
									EventSystem.current.SetSelectedGameObject(appendedMenuItem.gameObject);
								}
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
			});
			
		}
		#endregion

		#region PERSONA SKILL UNLOCK - ANNOUNCEMENTS AND FLASHES
		/// <summary>
		/// Announces that the persona has just learned the specified item.
		/// </summary>
		/// <param name="currentPersona">The persona currently learning the skill.</param>
		/// <param name="menuItem">The menu item containing the skill that was learned.</param>
		public void AnnounceAddedSkill(Persona currentPersona, CombatantAnalysisDXBehaviorListItem menuItem) {
			// If nothing is passed for the callback, assume I want to select the item itself.
			this.AnnounceAddedSkill(
				currentPersona: currentPersona, 
				menuItem: menuItem, 
				announcementClosedCallback: () => {
					EventSystem.current.SetSelectedGameObject(menuItem.gameObject);
				});
		}
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
		
		#region EVENTS
		/// <summary>
		/// Gets called when the hidden advance button is hit.
		/// </summary>
		public void AdvanceButtonHit() {
			
			// Turn the selectable off and null out the current selected object.
			this.hiddenSelectable.gameObject.SetActive(false);
			EventSystem.current.SetSelectedGameObject(null);
			
			// Execute the next reaction in the queue.
			this.ExecuteNextReaction();
			
		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Grabs the results element associated with the specified results type.
		/// </summary>
		/// <param name="resultsElementType">The results element to retrieve.</param>
		/// <returns>The results element that displays information.</returns>
		private BattleResultsDXElement GetResultsElement(BattleResultsDXElementType resultsElementType) {
			return this.battleResultsElementList.First(e => e.ResultsElementType == resultsElementType);
		}
		#endregion
		
		/*#region PROTOTYPING
		/// <summary>
		/// A very basic way of presenting the battle rewards.
		/// </summary>
		/// <param name="battleResultsSet"></param>
		private void ProtoPresent(BattleResultsSet battleResultsSet) {
			
			// Turn everything on.
			this.allObjects.SetActive(true);
			
			// Set the text.
			this.placeholderEXPLabel.Text = "EXP: " + battleResultsSet.TotalEXP;
			this.placeholderMoneyLabel.Text = "Money: $" + battleResultsSet.TotalMoney;
			
			if (battleResultsSet.ItemsList.Count > 0) {
				this.placeholderItemsLabel.Text = "Items:";
				foreach (BattleBehavior item in battleResultsSet.ItemDict.Keys) {
					int count = battleResultsSet.ItemDict[item];
					this.placeholderItemsLabel.Text += "\nx" + count + "\t" + item.behaviorName;
				}
			} else {
				this.placeholderItemsLabel.Text = "";
			}

			// Get a list of combatants that leveled up.
			this.placeholderLeveledUpLabel.Text = "";
			var leveledUpCombatants = battleResultsSet
				.LeveledUpPersonas.Cast<Combatant>()
				.Concat(battleResultsSet.LeveledUpPlayers.Cast<Combatant>())
				.ToList();	
			foreach (Combatant c in leveledUpCombatants) {
				this.placeholderLeveledUpLabel.Text += "\n" + c.metaData.name + "\tis now Level " + c.Level;
			}
			
		}
		#endregion*/
		
	}
	
}