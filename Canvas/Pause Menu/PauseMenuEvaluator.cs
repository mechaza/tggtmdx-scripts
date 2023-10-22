using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;
using System.Linq;
using DG.Tweening;
using Grawly.Battle.BattleMenu;
using System;

namespace Grawly.UI {
	
	/// <summary>
	/// I want to separate the class that helps with evaluating actual operations I want to perform from the pause menu because the PauseMenuController is a bit bloated right now.
	/// This will basically be a place to pass in players/items and evaluate their effects and shit.
	/// </summary>
	[RequireComponent(typeof(PauseMenuController))]
	public class PauseMenuEvaluator : MonoBehaviour {






		// FOR THE RECORD I MADE THE SETSUBMITTABLES THING
		// AND ALSO SORT OF INTERCEPT TARGET PALYERS IN A DDIFFERENT WAY IN THE EVALUATOR.








		public static PauseMenuEvaluator instance;

		#region FIELDS - STATE
		/// <summary>
		/// The tween callback that should be run when the an asynchronous pause function is complete.
		/// </summary>
		private Action<string, int, bool> onAsynchronousFunctionComplete;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region EVALUATION - ITEMS
		/// <summary>
		/// THIS is what I'm calling from PlayMaker. I hate this so much.
		/// </summary>
		public void EvaluateItem() {
			// PlayMaker has been having trouble keeping track of the variables I tell it to keep track of so i gotta do everything my damn self.
			// Only pass in the target players who can actually use it.
			this.EvaluateItem(
				targetPlayers: PauseMenuController.instance.TargetPlayers.Where(p => p.IsAffectedByBehavior(PauseMenuController.instance.SelectedInventoryItem.behavior)).ToList(),
				inventoryItem: PauseMenuController.instance.SelectedInventoryItem);
		}
		/// <summary>
		/// Evaluates the effects of the battle behavior on the player. 
		/// </summary>
		/// <param name="player">The player who is being affected by the item.</param>
		/// <param name="inventoryItem">The battle behavior describing the item itself.</param>
		private void EvaluateItem(List<Player> targetPlayers, InventoryItem inventoryItem) {

			// Go through each target player and have it be passed through the function.
			// Asyncrhonous functions will call PauseFunctionComplete manually when they are done.
			targetPlayers.ForEach(p => {
				inventoryItem.behavior.PauseFunctions.ForEach(pf => pf.Execute(
				source: p,
				targets: new List<Combatant>() { p },
				self: inventoryItem.behavior));
			});

			// Call the quick rebuild on the player statuses.
			PlayerStatusDXController.instance?.QuickRebuild();
			
			this.onAsynchronousFunctionComplete = delegate (string str, int num, bool toggle) {
				// Decrement the inventory item count by the number that was passed into PauseFunctionComplete. Sometimes skill cards will not use it.
				inventoryItem.Count -= num;
				// Check whether there are any more of that particular item left.
				if (inventoryItem.Count == 0) {
					Debug.Log("PAUSE MENU: Inventory count is now at zero. Sending 'Can Not Use Item Again' event.");
					this.CannotUseItemAgain();
				} else {
					Debug.Log("PAUSE MENU: Inventory count is still above zero. Sending 'Can Use Item Again' event.");
					this.CanUseItemAgain(targetPlayersToKeepHighlighted: targetPlayers);
				}
			};

			// If there are no asynchronous functions, finish up.
			// Otherwise, this function will get called externally.
			if (inventoryItem.behavior.PauseFunctions.Where(pf => pf.IsAsynchronous == true).ToList().Count == 0) {
				Debug.Log("PAUSE MENU: Item has no asynchronous functions. Decrementing item count by default.");
				this.PauseFunctionComplete(num: 1);
			}

		}
		#endregion

		#region EVALUATION - SKILLS
		/// <summary>
		/// Evaluates the skill selected.
		/// </summary>
		public void EvaluateSkill() {
			// Evaluate the skill, but only use players where they can be targetd by that skill. 
			this.EvaluateSkill(
				sourcePlayer: PauseMenuController.instance.SourcePlayer,
				targetPlayers: PauseMenuController.instance.TargetPlayers.Where(p => p.IsAffectedByBehavior(PauseMenuController.instance.SelectedSkill)).ToList(), 
				behavior: PauseMenuController.instance.SelectedSkill);
		}
		/// <summary>
		/// Evaluates the skill selected.
		/// </summary>
		private void EvaluateSkill(Player sourcePlayer, List<Player> targetPlayers, BattleBehavior behavior) {

			// Deduct the cost.
			sourcePlayer.DeductBehaviorCost(behavior: behavior);

			// Go through each target player and have it be passed through the function.
			targetPlayers.ForEach(p => {
				behavior.PauseFunctions.ForEach(pf => pf.Execute(
				source: sourcePlayer,
				targets: new List<Combatant>() { p },
				self: behavior));
			});

			// Call the quick rebuild on the player statuses.
			PlayerStatusDXController.instance.QuickRebuild();

			// Prep the callback.
			/*this.onAsynchronousFunctionComplete = new TweenCallback(delegate {
				// If the source player is capable of using the move again, send the Can Use Item Again event.
				if (sourcePlayer.CanUseBehavior(behavior: behavior) == true) {
					Debug.Log("PAUSE MENU: Can still use skill. Sending 'Can Use Item Again' event.");
					this.CanUseItemAgain(targetPlayersToKeepHighlighted: targetPlayers);
				} else {
					Debug.Log("PAUSE MENU: Cannot use skill any more. Sending 'Can Not Use Item Again' event.");
					this.CannotUseItemAgain();
				}
			});*/

			this.onAsynchronousFunctionComplete = delegate (string str, int num, bool toggle) {
				// If the source player is capable of using the move again, send the Can Use Item Again event.
				if (sourcePlayer.HasResourcesForBehavior(behavior: behavior) == true) {
					Debug.Log("PAUSE MENU: Can still use skill. Sending 'Can Use Item Again' event.");
					this.CanUseItemAgain(targetPlayersToKeepHighlighted: targetPlayers);
				} else {
					Debug.Log("PAUSE MENU: Cannot use skill any more. Sending 'Can Not Use Item Again' event.");
					this.CannotUseItemAgain();
				}
			};

			// If there are no asynchronous functions, run the callback.
			if (behavior.PauseFunctions.Where(pf => pf.IsAsynchronous == true).ToList().Count == 0) {
				this.PauseFunctionComplete();
			}


		}
		#endregion

		#region EVALUATION - BERSONA
		/// <summary>
		/// Switches over to the new persona.
		/// </summary>
		public void EvaluateBersona() {
			this.EvaluateBersona(targetPlayers: PauseMenuController.instance.TargetPlayers, personaToSwitchTo: PauseMenuController.instance.PersonaToSwitchTo);
		}
		/// <summary>
		/// Switches over to the new persona.
		/// </summary>
		private void EvaluateBersona(List<Player> targetPlayers, Persona personaToSwitchTo) {
			targetPlayers[0].AssignPlayerPersona(persona: personaToSwitchTo);
			this.CanUseItemAgain(targetPlayersToKeepHighlighted: targetPlayers);
		}
		#endregion

		#region EVENTS TO CALL WHEN DONE
		/// <summary>
		/// Gets called when the pause functions have completed execution.
		/// </summary>
		public void PauseFunctionComplete(string str = "", int num = 0, bool toggle = false) {
			// Run the callback.
			this.onAsynchronousFunctionComplete(arg1: str, arg2: num, arg3: toggle);
			// Nullify the callback because I don't want it to be present any more.
			this.onAsynchronousFunctionComplete = null;
		}
		#endregion

		#region FINISHERS TO SEND EVENTS TO FSM
		/// <summary>
		/// A function I'm separating out because I found myself repeating myself.
		/// </summary>
		/// <param name="targetPlayersToKeepHighlighted">The platyers to keep highlighted.</param>
		private void CanUseItemAgain(List<Player> targetPlayersToKeepHighlighted) {
			// Also rebuild the list.
			PauseMenuController.instance.RebuildMenuList();
			// Rebuild the player statuses. Maybe I wanna do this in the FSM but... idk. I'm having more fun doing it in code.
			PauseMenuController.instance.RebuildPauseMenuPlayerStatuses(toKeepHighlighted: targetPlayersToKeepHighlighted);
			// Send the event that tells them it can rebuild again.
			PauseMenuController.instance.FSM.SendEvent("Can Use Item Again");
			
		}
		/// <summary>
		/// A function I'm separating out because I found myself repeating myself.
		/// Doesnt take any parameters becuase I don't need to keep anyone highlighted.
		/// </summary>
		private void CannotUseItemAgain() {
			// Rebuild the statuses but don't keep them highlighted.
			PauseMenuController.instance.RebuildPauseMenuPlayerStatuses();
			PauseMenuController.instance.FSM.SendEvent("Cannot Use Item Again");
		
		}
		#endregion

		

	}


}