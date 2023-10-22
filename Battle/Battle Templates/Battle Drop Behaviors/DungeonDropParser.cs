using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Toggles;
using Grawly.Toggles.Gameplay;

namespace Grawly.Battle.DropParsers.Standard {

	/// <summary>
	/// Adds all drops to the player inventory immediately.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Adds the items to the inventory but delays receiving exp until the player goes to a bank node.")]
	public class DungeonDropParser : BattleDropParser {


		public override BattleResultsSet ParseBattleDrops(BattleTemplate battleTemplate, BattleParams battleParams, GameVariables gameVariables) {

			
			// Create a new results set.
			BattleResultsSet resultsSet = new BattleResultsSet(
				players: gameVariables.Players, 
				personas: gameVariables.Players.Select(p=> p.ActivePersona).ToList());

			// Add the EXP to the results set.
			resultsSet.TotalEXP += battleTemplate.EnemyTemplates.Sum(et => et.drops.exp);
			
			// Go through all alive players, parse the results, and add it to the results set.
			gameVariables.Players.Where(p => p.IsDead == false).ToList().ForEach(p => {
				if (ToggleController.GetToggle<DisableExperience>().GetToggleBool() == false) {	
					p.ParseBattleResults(battleTemplate: battleTemplate, gameVariables: gameVariables);
					var playerLevels = resultsSet.PlayerLevelDict[p];
					var personaLevels = resultsSet.PersonaLevelDict[p.ActivePersona];
					playerLevels.postBattleLevel = p.Level;
					personaLevels.postBattleLevel = p.ActivePersona.Level;
					resultsSet.PlayerLevelDict[p] = playerLevels;
					resultsSet.PersonaLevelDict[p.ActivePersona] = personaLevels;
				} else {
					Debug.Log("Experience has been disabled. Skipping.");
				}
			});

			
			// Immediately add the money.
			int battleMoney = battleTemplate.GetMoney(gameVariables: gameVariables);
			gameVariables.Money += battleMoney;
			resultsSet.TotalMoney += battleMoney;
			
			// Also add the items.
			var battleItems = battleTemplate.GetItems(gameVariables: gameVariables);
			gameVariables.AddItemsToInventory(items: battleItems);
			resultsSet.ItemsList = battleItems;

			// Return the results set.
			return resultsSet;

		}

	}


}