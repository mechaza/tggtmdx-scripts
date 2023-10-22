using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Battle;
using Grawly.Battle.Equipment;
using Grawly.Battle.Equipment.Badges;
using Grawly.Calendar;
using Grawly.DungeonCrawler;
using Grawly.UI;
using Grawly.Gauntlet;
using Grawly.Toggles;
using Grawly.Toggles.Proto;
using Sirenix.Serialization;
using Grawly.Friends;
using Grawly.MiniGames.ShuffleTime;
using Grawly.Playstyle;
using Grawly.UI.MenuLists;
using UnityEngine.Serialization;

namespace Grawly {

	/// <summary>
	/// A class that is used to store all of the variables that get saved long term.
	/// E.x., the Personas in stock, inventory, calendar information, etc.
	/// </summary>
	public class GameVariables {

		#region FIELDS - MODE
		/// <summary>
		/// The kind of playstyle being used for this set of variables.
		/// </summary>
		public PlaystyleType PlaystyleType { get; set; } = PlaystyleType.None;
		/// <summary>
		/// The game's difficulty.
		/// </summary>
		public DifficultyType DifficultyType { get; set; } = DifficultyType.Normal;
		#endregion

		#region FIELDS - SAVES
		/// <summary>
		/// The number of times these variables have been loaded from disk.
		/// </summary>
		public int LoadCount { get; set; } = 0;
		/// <summary>
		/// The number of times these variables have been saved to disk.
		/// </summary>
		public int SaveCount { get; set; } = 0;
		/// <summary>
		/// The number of times a player has died on this save.
		/// </summary>
		public int DeathCount { get; set; } = 0;
		#endregion
		
		#region FIELDS - NAMES
		/// <summary>
		/// Contains the mapping of character IDs to their names.
		/// </summary>
		public CharacterIDMap CharacterIDMap { get; private set; } = new CharacterIDMap();
		#endregion

		#region FIELDS - COMBATANTS
		/// <summary>
		/// The player templates to use for this game state.
		/// </summary>
		public List<Player> Players { get; set; } = new List<Player>();
		/// <summary>
		/// The templates that should be used in addition to ones stored by players.
		/// </summary>
		public List<Persona> Personas { get; set; } = new List<Persona>();
		#endregion

		#region FIELDS - SHUFFLE CARDS
		/// <summary>
		/// The deck that contains cards drawn during shuffle time.
		/// </summary>
		public ShuffleCardDeck ShuffleCardDeck { get; private set; } = new ShuffleCardDeck();
		#endregion
		
		#region FIELDS - INVENTORY
		/// <summary>
		/// The items that the party has access to. The integer is the number available for that particular item.
		/// </summary>
		public Dictionary<BattleBehavior, int> Items { get; set; } = new Dictionary<BattleBehavior, int>();
		/// <summary>
		/// A collection that encapsulates all of the weapons available to the party.
		/// </summary>
		public WeaponCollectionSet WeaponCollectionSet { get; private set; } = new WeaponCollectionSet();
		/// <summary>
		/// The object which encapsulates all of the badges in the players possession.
		/// </summary>
		public BadgeCollectionSet BadgeCollectionSet { get; private set; } = new BadgeCollectionSet();
		/// <summary>
		/// The amount of money the player currently has.
		/// </summary>
		public int Money { get; set; } = 0;
		/// <summary>
		/// The amount of time the game has been played for.
		/// </summary>
		public float PlayTimeSeconds { get; set; } = 0f;
		#endregion

		#region FIELDS - CALENDAR
		/// <summary>
		/// The number of the current day. If you're looking for the CurrentDay itself, it gets computed in the CalendarController.
		/// </summary>
		public int CurrentDayNumber { get; set; } = 0;
		/// <summary>
		/// The current time of day. Can also be accessed in CalendarController because it just references this.
		/// </summary>
		public TimeOfDayType CurrentTimeOfDay { get; set; } = TimeOfDayType.EarlyMorning;
		#endregion

		#region FIELDS - GAUNTLET VARIABLES
		/// <summary>
		/// The amount of EXP that can be cashed in.
		/// </summary>
		public int ExpBank { get; set; } = 0;
		/// <summary>
		/// The amount of battle wins the player has had in a row before cashing in their exp.
		/// </summary>
		public int WinStreak { get; set; } = 0;
		#endregion

		#region FIELDS - STORY
		/// <summary>
		/// Used to store flags that track progress throughout the game.
		/// </summary>
		public StoryFlags StoryFlags { get; set; } = new StoryFlags();
		/// <summary>
		/// The current friend data set.
		/// </summary>
		public FriendDataSet FriendDataSet { get; set; } = new FriendDataSet();
		/// <summary>
		/// A list containing the data of how far a player has progressed in a dungeon.
		/// </summary>
		public List<DungeonProgressionData> DungeonProgressions { get; set; } = new List<DungeonProgressionData>();
		/// <summary>
		/// A list containing the data of how far a player has progressed in a crawler.
		/// </summary>
		public List<CrawlerProgressionSet> CrawlerProgressions { get; set; } = new List<CrawlerProgressionSet>();
		#endregion

		#region FIELDS - ENEMY LOGGING
		/// <summary>
		/// In Persona, you don't know an enemy's weakness until you attack them with it.
		/// This keeps track of who has been attacked with what.
		/// String keys are the enemy's name, so don't change them in the middle of development if possible.
		/// </summary>
		[OdinSerialize]
		public Dictionary<string, Dictionary<ElementType, bool>> EnemyResistanceLogDict { get; private set; } = new Dictionary<string, Dictionary<ElementType, bool>>();
		#endregion

		#region PROPERTIES : GAUNTLET
		/// <summary>
		/// The Chaos amount. Affects things like exp multipliers and drop rates.
		/// Always between 0 and 1.
		/// </summary>
		public float Chaos {
			get {
				return Mathf.InverseLerp(a: 0f, b: 20f, value: (float)this.WinStreak);
			}
		}
		/// <summary>
		/// The amount to multiply EXP by when winning battles.
		/// </summary>
		public float ExpMultiplier {
			get {
				return Mathf.Lerp(a: 1f, b: 10f, t: this.Chaos);
			}
		}
		/// <summary>
		/// The amount to multiply money by when winning battles.
		/// </summary>
		public float MoneyMultiplier {
			get {
				return Mathf.Lerp(a: 1f, b: 10f, t: this.Chaos);
			}
		}
		/// <summary>
		/// The amount to multiply enemy attributes by.
		/// </summary>
		public float EnemyAttributeBoost {
			get {
				return Mathf.Lerp(a: 1f, b: 20f, t: this.Chaos);
			}
		}
		#endregion

		#region PROPERTIES : SCENE
		/// <summary>
		/// The name of the current location. Mostly just for convinience when saving.
		/// I actually may change this later.
		/// </summary>
		public string LocationName {
			get {
				try {
					return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
				} catch (System.Exception e) {
					Debug.LogError("Couldn't return the scene's current name! Returning an error string.");
					return "ERROR";
				}
			}
		}
		#endregion

		#region PROPERTIES - MISC
		/// <summary>
		/// The items in a form that can be read by a menu that also needs to know their count.
		/// </summary>
		public List<IMenuable> MenuableItems {
			get {
				Debug.Log("GENERATING NEW LIST OF MENUABLE ITEMS.");
				// Sometimes there will be entries in the items dictionary that have a zero count. I obviously do not want those in the list.
				return this.Items
					.Select(kvp => new InventoryItem(behavior: kvp.Key, variables: this))
					.Where(ii => ii.Count > 0)
					.Cast<IMenuable>()
					.ToList();
			}
		}
		/// <summary>
		/// The amount of time the player has been playing this game, formatted as a string.
		/// Used in the SaveFileButton.
		/// </summary>
		public string PlayTimeString {
			get {
				int playMinutes = (int)this.PlayTimeSeconds / 60;
				return (playMinutes / 60) + "h " + (playMinutes % 60) + "m";
			}
		}
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Create the GameVariables from the specified template.
		/// </summary>
		/// <param name="template"></param>
		public GameVariables(GameVariablesTemplate template) {
			Debug.Log("Initializing GameVariables...");
			// Start out by clearing the GameVariables and making sure it has references to shit.
			this.Clear();

			this.DifficultyType = template.DifficultyType;
			this.PlaystyleType = template.playstyleType;
			
			// Add the nicknames.
			this.CharacterIDMap = template.CharacterIDMap;

			// Create a new deck from any card templates inside the variables template.
			this.ShuffleCardDeck = new ShuffleCardDeck(cardTemplates: template.shuffleCardTemplates);
			
			// Add the badge collection.
			this.BadgeCollectionSet = new BadgeCollectionSet(template.badgeCollectionSetTemplate);
			
			// Add the weapons. Note that this must be done AFTER making the badge collection; not before.
			// It must also be done BEFORE setting up the players, since they will need access to the weapons.
			this.WeaponCollectionSet = new WeaponCollectionSet(
				weaponCollectionSetTemplate: template.weaponCollectionSetTemplate,
				badgeCollectionSet: this.BadgeCollectionSet);
			
			// Add the Players/Personas.
			this.AddPlayers(templates: template.playerTemplates);
			this.AddPersonas(templates: template.personaTemplates);
			// Add the items.
			this.AddItemsToInventory(items: template.items);
			
			
			
			// Get a CLONE of the story flags from the template.
			this.StoryFlags = template.StoryFlags;

			// Create a new friend data set.
			this.FriendDataSet = new FriendDataSet(friendTemplates: template.FriendTemplates);
			
			// Get the dungeon progressions.
			this.DungeonProgressions = template.DungeonProgressions;
			this.CrawlerProgressions = template.CrawlerProgressions;
			
			// Set the current day and time.
			this.CurrentDayNumber = template.startDay;
			this.CurrentTimeOfDay = template.startTime;
			
			// Assign the money and exp.
			this.Money = template.money;
			
			// Set number of times the party has died.
			this.DeathCount = template.deathCount;
			
			// Also set the play time.
			// this.playTimeSeconds = template.playTimeSeconds;
			this.PlayTimeSeconds = 0;
			
			// Save the default save/load counts.
			this.SaveCount = template.saveCount;
			this.LoadCount = template.loadCount;
			
			// Create a new GauntletVariables from the template.
			this.ExpBank = template.expBank;
			this.WinStreak = template.winStreak;

		}
		/// <summary>
		/// Sets the game variables based on the game save passed in.
		/// </summary>
		/// <param name="gameSave">The game to create a GameSave from.</param>
		public GameVariables(GameSave gameSave) {

			this.DifficultyType = gameSave.difficultyType;
			this.PlaystyleType = gameSave.playstyleType;
			
			this.CharacterIDMap = new CharacterIDMap(dictionary: gameSave.characterIDNameDict);

			
			// The BadgeCollectionSet needs to be created first so that there is no mismatch when making the weapons.
			// They also need to be prepared here so that the players are able to retrieve their weapons.
			this.BadgeCollectionSet = new BadgeCollectionSet(gameSave.badgeCollectionSet);
			this.WeaponCollectionSet = new WeaponCollectionSet(
				serializableWeaponCollectionSet: gameSave.weaponCollectionSet,
				badgeCollectionSet: this.BadgeCollectionSet);
			
			this.Players = gameSave.players
				.Select(sp => new Player(
					sp: sp, 
					scd: GameSaveLoaderController.instance,
					gameVariables: this))
				.ToList();

			this.Personas = gameSave.personas
				// .Where(sp => gameSave.players.Select(x => x.persona).Contains(sp) == false)
				.Select(sp => new Persona(
					sp: sp, 
					scd: GameSaveLoaderController.instance,
					gameVariables: this))
				.ToList();

			this.ShuffleCardDeck = gameSave.shuffleCardDeck.Clone();

			this.Items = new Dictionary<BattleBehavior, int>();
			gameSave.items.ForEach(s => {
				BattleBehavior key = GameSaveLoaderController.instance.GetBattleBehavior(s);
				if (this.Items.ContainsKey(key)) { this.Items[key] += 1; }
				else { this.Items.Add(key: GameSaveLoaderController.instance.GetBattleBehavior(s), value: 1); }
			});
			
			
			
			
			this.Money = gameSave.money;
			this.PlayTimeSeconds = gameSave.playTimeSeconds;
			this.CurrentDayNumber = gameSave.currentDayNumber;
			this.CurrentTimeOfDay = gameSave.currentTimeOfDay;
			this.StoryFlags = new StoryFlags(storyFlagKVPs: gameSave.storyFlagKVPs);
			this.FriendDataSet = new FriendDataSet(gameSave.friendDataSet);
			
			this.DungeonProgressions = gameSave.dungeonProgressions.Select(dp => dp.Clone()).ToList();
			this.CrawlerProgressions = gameSave.crawlerProgressions.Select(cp => cp.Clone()).ToList();

			// Create a resistance dict from the enemy logs.
			this.EnemyResistanceLogDict = gameSave.enemyResistanceLogDict;

			// Also get the save/load count.
			this.SaveCount = gameSave.saveCount;
			this.LoadCount = gameSave.loadCount;
			
			// And the death count.
			this.DeathCount = gameSave.deathCount;
			
			// Create a new gauntlet variables as well.
			this.ExpBank = gameSave.expBank;
			this.WinStreak = gameSave.winStreak;

		}
		#endregion

		#region SETUP - PREPARATION
		/// <summary>
		/// Completely wipes the GameVariables and re-initializes all of its fields.
		/// </summary>
		private void Clear() {
			this.CharacterIDMap = new CharacterIDMap();
			this.Players = new List<Player>();
			this.Personas = new List<Persona>();
			this.Items = new Dictionary<BattleBehavior, int>();
			this.WeaponCollectionSet = new WeaponCollectionSet();
			// this.Weapons = new List<Weapon>();
			this.BadgeCollectionSet = new BadgeCollectionSet();
			this.ShuffleCardDeck = new ShuffleCardDeck();
			this.Money = 0;
			this.SaveCount = 0;
			this.LoadCount = 0;
			this.DeathCount = 0;
			this.StoryFlags = new StoryFlags();
			this.DungeonProgressions = new List<DungeonProgressionData>();
			this.CrawlerProgressions = new List<CrawlerProgressionSet>();
			this.CurrentTimeOfDay = TimeOfDayType.EarlyMorning;
			this.CurrentDayNumber = 4;
			this.ExpBank = 0;
			this.WinStreak = 0;
		}
		#endregion

		#region SETUP - NAMES
		/// <summary>
		/// Updates the CharacterIDMap with new names.
		/// </summary>
		/// <param name="nameTuples"></param>
		public void UpdateCharacterNames(List<(CharacterIDType idType, string name)> nameTuples) {
			Debug.Log("Updating Character Names.");
			// Reset the ID map.
			this.CharacterIDMap = new CharacterIDMap(nameTuples);
			// Go through each tuple, find the player who shares that ID, and update their name.
			nameTuples.ForEach(t => {
				this.Players.First(p => p.playerTemplate.characterIDType == t.idType).metaData.name = t.name;
			});
		}
		#endregion

		#region SETUP - COMBATANTS
		/// <summary>
		/// Adds player to the players list.
		/// </summary>
		public void AddPlayers(List<PlayerTemplate> templates) {
			templates.ForEach(pt => this.Players.Add(new Player(template: pt, gameVariables: this)));
			Debug.Log("PLAYER COUNT: " + Players.Count);
		}
		/// <summary>
		/// Adds a player to the variables from a name passed in.
		/// </summary>
		/// <param name="playerName">The name of the player to add.</param>
		public void AddPlayer(string playerName) {
			// Probe the data controller for the player to add and then add it.
			this.AddPlayers(templates: new List<PlayerTemplate>() { DataController.GetPlayerTemplate(playerName: playerName) });
		}
		/// <summary>
		/// Adds players to the pool of personas.
		/// </summary>
		/// <param name="templates">The persona templates to add.</param>
		public void AddPersonas(List<PersonaTemplate> templates) {
			templates.ForEach(pt => this.Personas.Add(new Persona(template: pt, gameVariables: this)));
		}
		#endregion

		#region SETUP - INVENTORY : ITEMS
		/// <summary>
		/// Adds the items stored in a treasure template to the inventory.
		/// </summary>
		/// <param name="treasureTemplate"></param>
		public void AddItemsToInventory(TreasureTemplate treasureTemplate) {
			this.AddItemsToInventory(items: treasureTemplate.AllItems);
		}
		/// <summary>
		/// Adds a collection of items to the inventory at once.
		/// </summary>
		/// <param name="items"></param>
		public void AddItemsToInventory(List<BattleBehavior> items) {
			// Just go through the list and add each item one by one.
			items.ForEach(bb => this.AddItemToInventory(item: bb, quantity: 1));
		}
		/// <summary>
		/// Adds an item to the inventory.
		/// </summary>
		/// <param name="item"></param>
		public void AddItemToInventory(BattleBehavior item, int quantity = 1) {
			// Check if the inventory has the item already. If so, just increment the count.
			if (Items.ContainsKey(item) == true) {
				Items[item] += quantity;
			} else {
				// If not, create a new entry.
				Items.Add(item, quantity);
			}
		}
		/// <summary>
		/// Adds an item to the inventory via its name.
		/// </summary>
		/// <param name="item"></param>
		public void AddItemToInventory(string itemName, int quantity = 1) {
			// this.AddItemToInventory(item: this.allMoves.Find(i => i.behaviorName == itemName), quantity: quantity);
			this.AddItemToInventory(
				item: DataController.Instance.GetBehavior(behaviorName: itemName), 
				quantity: quantity);
		}
		/// <summary>
		/// Removes an item of a certain kind from the dictionary.
		/// </summary>
		/// <param name="item"></param>
		public void RemoveItemFromInventory(BattleBehavior item) {
			if (Items.ContainsKey(item) == true) {
				Items[item] -= 1;
				// If the count has reached zero, delete the key.
				if (Items[item] == 0) { Items.Remove(item); }
			} else {
				Debug.LogWarning("Cannot remove item from dictionary!");
			}
		}
		#endregion

		#region ENEMY LOGGING
		/// <summary>
		/// Gets the enemy's resistance to a given element type.
		/// Will return null if the player has not obtained it yet.
		/// </summary>
		/// <param name="enemy">The enemy who is having their weakness probed.</param>
		/// <param name="behavior">The behavior that may be used aginst the enemy.</param>
		/// <returns></returns>
		public ResistanceType? GetKnownEnemyResistance(Enemy enemy, BattleBehavior behavior) {
			// Just use the version that checks the element type.
			return this.GetKnownEnemyResistance(enemy: enemy, elementType: behavior.elementType);
		}
		/// <summary>
		/// Marks this enemy as having their resistance to the given element known.
		/// </summary>
		/// <param name="enemy">The enemy who was just probed.</param>
		/// <param name="elementType">The element type to set.</param>
		/// <param name="resistanceKnown">Should the resistance be set to known?</param>
		public void SetKnownEnemyResistance(Enemy enemy, ElementType elementType, bool resistanceKnown) {
			// If there is no entry for this enemy, make one.
			if (this.EnemyResistanceLogDict.ContainsKey(enemy.metaData.name) == false) {
				this.EnemyResistanceLogDict.Add(key: enemy.metaData.name, value: new Dictionary<ElementType, bool>());
				// Also make sure to initialize it with False values for each element.
				foreach (ElementType element in System.Enum.GetValues(typeof(ElementType))) {
					this.EnemyResistanceLogDict[enemy.metaData.name].Add(key: element, value: false);
				}
			}

			// Overwrite the entry.
			this.EnemyResistanceLogDict[enemy.metaData.name][elementType] = resistanceKnown;

		}
		/// <summary>
		/// Gets the enemy's resistance to a given element type.
		/// Will return null if the player has not obtained it yet.
		/// </summary>
		/// <param name="enemy">The enemy who is having their weakness probed.</param>
		/// <param name="elementType">The element to ask this enemy what their resistance is to.</param>
		/// <returns></returns>
		public ResistanceType? GetKnownEnemyResistance(Enemy enemy, ElementType elementType) {

			// If there is no entry for this enemy, make one.
			if (this.EnemyResistanceLogDict.ContainsKey(enemy.metaData.name) == false) {
				this.EnemyResistanceLogDict.Add(key: enemy.metaData.name, value: new Dictionary<ElementType, bool>());
				// Also make sure to initialize it with False values for each element.
				foreach (ElementType element in System.Enum.GetValues(typeof(ElementType))) {
					this.EnemyResistanceLogDict[enemy.metaData.name].Add(key: element, value: false);
				}
			}
			
			// Check if the toggle that overrides this check is active.
			bool toggleOverride = ToggleController.GetToggle<ShowEnemyDebugInfo>().GetToggleBool(); 
			
			if (toggleOverride || this.EnemyResistanceLogDict[enemy.metaData.name][elementType] == true) {
				// If the override is set or the resistance is already known, return it.
				return enemy.CheckResistance(elementType);
			} else {
				// Return NULL if it is not known.
				return null;
			}
		}
		#endregion

		#region DUNGEON HELPERS
		/// <summary>
		/// Gets the DungeonProgessionData for the specified DungeonIDType.
		/// Will create a new one if it does not exist.
		/// </summary>
		/// <param name="dungeonIDType">The DungeonIDType of the dungeon being requested.</param>
		/// <returns></returns>
		private DungeonProgressionData GetDungeonProgressionData(DungeonIDType dungeonIDType) {
			try {
				// Attempt to find the dungeon by its ID.
				return this.DungeonProgressions.First(dp => dp.dungeonIDType == dungeonIDType);
			} catch (System.Exception e) {
				// If it does not exist, log out as much.
				Debug.LogWarning("Dungeon of ID " + dungeonIDType.ToString() + " was not found! Making a new one and adding it to the variables.");
				// Create a new progression data.
				DungeonProgressionData newDungeonProgressionData = new DungeonProgressionData() {
					dungeonIDType = dungeonIDType,
					highestFloorReached = 0,
				};
				// Add it to the list.
				this.DungeonProgressions.Add(newDungeonProgressionData);
				// Return the new one.
				return newDungeonProgressionData;
			}
		}
		/// <summary>
		/// Signals to the DungeonProgressions that the player has reached the specified floor.
		/// Will update to the passed in value if it's higher than whatever is saved.
		/// </summary>
		/// <param name="dungeonIDType">The ID of the dungeon currently in play.</param>
		/// <param name="currentFloor">The current floor the player is on.</param>
		/// <returns>Whether the highest floor was updated or not.</returns>
		public bool SignalDungeonFloorLevel(DungeonIDType dungeonIDType, int currentFloor) {
			// If the current floor is higher than what is stored...
			if (currentFloor > this.GetDungeonProgressionData(dungeonIDType).highestFloorReached) {
				// ... update it and return true.
				Debug.Log("Highest floor for " + dungeonIDType.ToString() + " is now " + currentFloor);
				this.GetDungeonProgressionData(dungeonIDType).highestFloorReached = currentFloor;
				return true;
			} else {
				// If it's not the highest floor, return false.
				return false;
			}
		}
		/// <summary>
		/// Increments the progress of the given dungeon type.
		/// </summary>
		/// <param name="dungeonIDType">The dungeon that has been traversed.</param>
		/// <param name="addCount">The number of floors to progress.</param>
		private void IncrementDungeonTopFloor(DungeonIDType dungeonIDType, int addCount = 1) {
			try {
				this.DungeonProgressions.First(dp => dp.dungeonIDType == dungeonIDType).highestFloorReached += addCount;
			} catch (System.Exception e) {
				Debug.LogError("Couldn't increment dungeon progression for Dungeon with ID " + dungeonIDType.ToString());
			}
		}
		/// <summary>
		/// Checks if the floor specified was reached for the given dungeon.
		/// </summary>
		/// <param name="dungeonIDType">The dungeon to check for.</param>
		/// <param name="floorNumber">The floor to check if it has been traversed.</param>
		/// <returns></returns>
		public bool CheckIfDungeonFloorWasReached(DungeonIDType dungeonIDType, int floorNumber) {
			try {
				// Check if the floor specified is higher than the highest floor reached.
				return floorNumber > this.DungeonProgressions.First(dp => dp.dungeonIDType == dungeonIDType).highestFloorReached;
			} catch (System.Exception e) {
				Debug.LogError("Couldn't find dungeon with ID " + dungeonIDType + "! Returning false.");
				return false;
			}
		}
		#endregion

		#region CRAWLER HELPERS
		/// <summary>
		/// Get the progression data associated with the provided crawler dungeon ID.
		/// If it does not exist already, a blank set will be made and added to the variables.
		/// </summary>
		/// <param name="crawlerDungeonIDType">The ID of the crawler dungeon progression to retrieve.</param>
		/// <returns></returns>
		public CrawlerProgressionSet GetCrawlerProgressionSet(CrawlerDungeonIDType crawlerDungeonIDType) {
			
			// Check if these variables actually have any progressions saved for the specified ID type.
			bool hasProgressionForID = this.CrawlerProgressions.Any(cp => cp.crawlerDungeonIDType == crawlerDungeonIDType);
			
			// If there are no progressions for the provided ID, make one.
			if (hasProgressionForID == false) {
				this.CrawlerProgressions.Add(new CrawlerProgressionSet() {
					crawlerDungeonIDType = crawlerDungeonIDType, 
				});
			}
			
			// Return the associated progression. It should exist now.
			return this.CrawlerProgressions.First(cp => cp.crawlerDungeonIDType == crawlerDungeonIDType);
			
		}
		/// <summary>
		/// Updates the CrawlerProgressionSet associated with the given ID type to have the specified floor number as the highest floor.
		/// Note that nothing will happen if the floor passed in is equal to or less than the current highest saved.
		/// </summary>
		/// <param name="crawlerDungeonIDType">The ID of the crawler dungeon..</param>
		/// <param name="currentFloor">The floor the player is currently on.</param>
		public void UpdateHighestCrawlerFloor(CrawlerDungeonIDType crawlerDungeonIDType, int currentFloor) {
			
		}
		#endregion
		
		#region GAUNTLET HELPERS
		/// <summary>
		/// This is what should get called when the player has cashed in their experience.
		/// It clears the win streak (and resets the chaos) and clears the experience.
		/// </summary>
		public void BankedExperience() {
			this.WinStreak = 0;
			this.ExpBank = 0;
		}
		#endregion

	}




}