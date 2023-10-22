using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Toggles;
using Grawly.Toggles.Gameplay;

namespace Grawly.Battle.DropParsers.Standard {

	/// <summary>
	/// Adds the items to the inventory but delays receiving exp until the player goes to a bank node.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Adds the items to the inventory but delays receiving exp until the player goes to a bank node.")]
	public class GauntletDropParser : BattleDropParser {
		

		public override BattleResultsSet ParseBattleDrops(BattleTemplate battleTemplate, BattleParams battleParams, GameVariables gameVariables) {

			// Create a new results set.
			BattleResultsSet resultsSet = new BattleResultsSet();
			
			// Increment the win streak.
			gameVariables.WinStreak += 1;

			// Only add the experience to the bank if it's set to do so.
			if (ToggleController.GetToggle<DisableExperience>().GetToggleBool() == false) {
				// Probe the BattleTemplate for the amount of EXP to be awarded, then add it to the bank and the results.
				int battleExperience = battleTemplate.GetExperience(gameVariables: gameVariables);
				gameVariables.ExpBank += battleExperience;
				resultsSet.TotalEXP += battleExperience;
				// gameVariables.expBank += battleTemplate.GetExperience(gameVariables: gameVariables);
			} else {
				Debug.Log("Experience has been disabled. Skipping.");
			}

			

			// Immediately add the money.
			int battleMoney = battleTemplate.GetMoney(gameVariables: gameVariables);
			gameVariables.Money += battleMoney;
			resultsSet.TotalMoney += battleMoney;
			// gameVariables.money += battleTemplate.GetMoney(gameVariables: gameVariables);

			// Also add the items.
			List<BattleBehavior> battleItems = battleTemplate.GetItems(gameVariables: gameVariables);
			gameVariables.AddItemsToInventory(items: battleItems);
			resultsSet.ItemsList = battleItems;
			// gameVariables.AddItemsToInventory(items: battleTemplate.GetItems(gameVariables: gameVariables));

			Debug.Log("EXP BANK: " + gameVariables.ExpBank.ToString());

			// Return the results set.
			return resultsSet;

		}

	}


}