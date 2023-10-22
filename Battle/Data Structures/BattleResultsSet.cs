using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// BattleTemplate battleTemplate, BattleParams battleParams

namespace Grawly.Battle {
	
	/// <summary>
	/// Contains the data that should be used on the results screen.
	/// </summary>
	public class BattleResultsSet {
		
		#region FIELDS - BASIC DROPS
		/// <summary>
		/// The total amount of EXP that should be given to the party.
		/// </summary>
		public int TotalEXP { get; set; } = 0;
		/// <summary>
		/// The total amount of money that should be given to the party.
		/// </summary>
		public int TotalMoney { get; set; } = 0;
		/// <summary>
		/// The items to be given to the party, stored in a list.
		/// May contain duplicates.
		/// </summary>
		public List<BattleBehavior> ItemsList { get; set; } = new List<BattleBehavior>();
		#endregion

		#region FIELDS - COMBATANTS
		/// <summary>
		/// A dictionary that contains the levels of each player before the battle and after the battle.
		/// </summary>
		public Dictionary<Player, (int preBattleLevel, int postBattleLevel)> PlayerLevelDict { get; set; } = new Dictionary<Player, (int preBattleLevel, int postBattleLevel)>();
		/// <summary>
		/// A dictionary that contains the levels of each persona before the battle and after the battle.
		/// </summary>
		public Dictionary<Persona, (int preBattleLevel, int postBattleLevel)> PersonaLevelDict { get; set; } = new Dictionary<Persona, (int preBattleLevel, int postBattleLevel)>();
		#endregion
		
		#region PROPERTIES - BASIC DROPS
		/// <summary>
		/// The items to be given to the party.
		/// The keys are the items themselves, and the values are their count.
		/// </summary>
		public Dictionary<BattleBehavior, int> ItemDict {
			get {
				Dictionary<BattleBehavior, int> itemsDict = new Dictionary<BattleBehavior, int>();
				foreach (BattleBehavior item in this.ItemsList) {
					if (itemsDict.ContainsKey(item)) {
						itemsDict[item] += 1;
					} else {
						itemsDict.Add(item, 1);
					}
				}
				return itemsDict;
			}
		}
		#endregion
	
		#region PROPERTIES - COMBATANTS
		/// <summary>
		/// The players that have leveled up this battle.
		/// </summary>
		public List<Player> LeveledUpPlayers {
			get {
				// Go through the level dict and find the KVPs where the post battle level is higher than the pre battle level.
				return this.PlayerLevelDict
					.Where(kvp => kvp.Value.postBattleLevel > kvp.Value.preBattleLevel)
					.Select(kvp => kvp.Key)
					.ToList();
			}
		}
		/// <summary>
		/// The Pesronas that have leveled up this battle.
		/// </summary>
		public List<Persona> LeveledUpPersonas {
			get {
				// Go through the level dict and find the KVPs where the post battle level is higher than the pre battle level.
				return this.PersonaLevelDict
					.Where(kvp => kvp.Value.postBattleLevel > kvp.Value.preBattleLevel)
					.Select(kvp => kvp.Key)
					.ToList();
			}
		}
		/// <summary>
		/// Are there any leveled up players in this set?
		/// </summary>
		public bool HasLeveledUpPlayers => this.LeveledUpPlayers.Count > 0;
		/// <summary>
		/// Are there any leveled up personas in this set?
		/// </summary>
		public bool HasLeveledUpPersonas => this.LeveledUpPersonas.Count > 0;
		#endregion

		#region PROPERTIES - FLAGS
		/// <summary>
		/// Are there any items inside this results set?
		/// </summary>
		public bool ContainsItems => this.ItemsList.Count > 0;
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Contains the data that should be used on the results screen.
		/// </summary>
		public BattleResultsSet() {
			
		}
		/// <summary>
		/// Contains the data that should be used on the results screen.
		/// Initializes the level dictionaries with the default data from the players.
		/// </summary>
		/// <param name="players">The players to register by default in the results set.</param>
		public BattleResultsSet(List<Player> players) : this(players, players.Select(p => p.ActivePersona).ToList()) {
			// This version just grabs the personas from each player.
		}
		/// <summary>
		/// Contains the data that should be used on the results screen.
		/// Initializes the level dictionaries with the default data from the players.
		/// </summary>
		/// <param name="players">The players to register by default in the results set.</param>
		/// <param name="personas">The personas to register by default in the results set.</param>
		public BattleResultsSet(List<Player> players, List<Persona> personas) {
			// Add each player and persona to the level dictionary. Note that both entries are their CURRENT level.
			foreach (Player player in players) {
				this.PlayerLevelDict.Add(player, (player.Level, player.Level));
			}
			foreach (Persona persona in personas) {
				this.PersonaLevelDict.Add(persona, (persona.Level, persona.Level));
			}
		}
		#endregion
		
	}
	
}