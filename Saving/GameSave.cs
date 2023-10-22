using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Sirenix.Serialization;
using Grawly.Calendar;
using System.Linq;
using Grawly.Battle.Equipment;
using Grawly.Battle.Equipment.Badges;
using Grawly.DungeonCrawler;
using Grawly.Friends;
using Grawly.MiniGames.ShuffleTime;
using Grawly.Playstyle;
using UnityEngine.Serialization;

namespace Grawly {

	/// <summary>
	/// Stores the save data assocaited with a particular play session.
	/// </summary>
	[System.Serializable]
	public class GameSave {

		#region FIELDS - METADATA
		/// <summary>
		/// Is this a new game? If so, everything else in this save file should be ignored.
		/// </summary>
		public bool newGame = true;
		/// <summary>
		/// The playstyle mode for this save.
		/// </summary>
		[FormerlySerializedAs("playstyleModeType")]
		public PlaystyleType playstyleType = PlaystyleType.None;
		/// <summary>
		/// The difficulty type for this save.
		/// </summary>
		public DifficultyType difficultyType = DifficultyType.Normal;
		#endregion

		#region FIELDS - NAMES
		/// <summary>
		/// Contains save data on the names for the girls.
		/// </summary>
		[OdinSerialize]
		public Dictionary<CharacterIDType, string> characterIDNameDict = new Dictionary<CharacterIDType, string>();
		#endregion
		
		#region FIELDS - COMBATANTS
		/// <summary>
		/// The Players themselves. These get converted back to actual players during the game.
		/// </summary>
		public List<Player.SerializablePlayer> players = new List<Player.SerializablePlayer>();
		/// <summary>
		/// The Personas themselves. These get converted back to actual personas during the game.
		/// </summary>
		public List<Persona.SerializablePersona> personas = new List<Persona.SerializablePersona>();
		#endregion
		
		#region FIELDS - MISC
		/// <summary>
		/// The items in posession by the player. You know this could just be a fucking kvp list.
		/// </summary>
		public List<string> items = new List<string>();
		/// <summary>
		/// The badge collection that holds all the partys badges.
		/// </summary>
		public SerializableBadgeCollectionSet badgeCollectionSet = new SerializableBadgeCollectionSet();
		/// <summary>
		/// The weapon collection that holds all the party's weapons.
		/// </summary>
		public SerializableWeaponCollectionSet weaponCollectionSet = new SerializableWeaponCollectionSet();
		/// <summary>
		/// MONEY.
		/// </summary>
		public int money;
		/// <summary>
		/// The amount of time spent playing this save.
		/// </summary>
		public float playTimeSeconds;
		/// <summary>
		/// The number of times this GameSave has been loaded.
		/// </summary>
		public int loadCount = 0;
		/// <summary>
		/// The number of times this GameSave has ben saved to disk.
		/// </summary>
		public int saveCount = 0;
		/// <summary>
		/// The number of times the party has died in this save.
		/// </summary>
		public int deathCount = 0;
		#endregion

		#region FIELDS - GAUNTLET
		/// <summary>
		/// The amount of experience currently awaiting to be cashed in.
		/// </summary>
		public int expBank;
		/// <summary>
		/// The number of times in a row the player has won.
		/// </summary>
		public int winStreak;
		#endregion

		#region FIELDS - DATE
		/// <summary>
		/// The current day number. 
		/// </summary>
		public int currentDayNumber;
		/// <summary>
		/// The current time of day.
		/// </summary>
		public TimeOfDayType currentTimeOfDay;
		#endregion

		#region FIELDS - STORY
		public List<StoryFlagKVP> storyFlagKVPs = new List<StoryFlagKVP>();
		/// <summary>
		/// The current friend data set.
		/// </summary>
		public FriendDataSet friendDataSet = new FriendDataSet();
		#endregion

		#region SHUFFLE CARDS
		/// <summary>
		/// A shuffle card deck.
		/// </summary>
		[OdinSerialize]
		public ShuffleCardDeck shuffleCardDeck = new ShuffleCardDeck();
		#endregion
		
		#region FIELDS - DUNGEON PROGRESSION
		/// <summary>
		/// A list containing the data of how far a player has progressed in a dungeon.
		/// </summary>
		public List<DungeonProgressionData> dungeonProgressions = new List<DungeonProgressionData>();
		/// <summary>
		/// A list containing the data of how far a player has progressed in a crawler.
		/// </summary>
		public List<CrawlerProgressionSet> crawlerProgressions = new List<CrawlerProgressionSet>();
		#endregion

		#region FIELDS - ENEMY LOGGING
		/// <summary>
		/// Keeps track of which enemies have had which elements tested on them.
		/// </summary>
		[OdinSerialize]
		public Dictionary<string, Dictionary<ElementType, bool>> enemyResistanceLogDict = new Dictionary<string, Dictionary<ElementType, bool>>();
		#endregion

		#region FIELDS - LOCATION
		/// <summary>
		/// The location where this save was done.
		/// </summary>
		public string locationName;
		/// <summary>
		/// The scene to load. For now I guess I'm just storing this alongside the location name.
		/// </summary>
		public string DefaultSceneToLoad {
			get {
				return this.locationName;
			}
		}
		#endregion

		#region PROPERTIES - MISC
		/// <summary>
		/// The amount of time the player has been playing this game, formatted as a string.
		/// Used in the SaveFileButton.
		/// </summary>
		public string PlayTimeString {
			get {
				int playMinutes = (int)this.playTimeSeconds / 60;
				return (playMinutes / 60) + "h " + (playMinutes % 60) + "m";
			}
		}
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Creates a game save from a set of variables.
		/// </summary>
		/// <param name="vars"></param>
		public GameSave(GameVariables vars) {

			// New Game needs to be turned off.
			this.newGame = false;
			this.difficultyType = vars.DifficultyType;
			this.playstyleType = vars.PlaystyleType;
			
			this.players = vars.Players.Select(p => new Player.SerializablePlayer(p)).ToList();
			// I can only add the personas NOT in use because when this is remade into a GameVariables, it also adds the personas list.
			// When a player is constructed, it also makes its persona and adds it to the list. Remember: this list is for personas NOT in use.
			this.personas = vars.Personas.Where(p => p.IsInUse(variables: vars) == false).Select(p => new Persona.SerializablePersona(p)).ToList();
			foreach (BattleBehavior item in new List<BattleBehavior>(vars.Items.Keys)) {
				for (int i = 0; i < vars.Items[item]; i++) {
					this.items.Add(item.behaviorName);
				}
			}
			this.weaponCollectionSet = new SerializableWeaponCollectionSet(weaponCollectionSet: vars.WeaponCollectionSet);
			// this.weapons = vars.Weapons.Select(w => new SerializableWeapon(w)).ToList();
			this.badgeCollectionSet = new SerializableBadgeCollectionSet(vars.BadgeCollectionSet);
			this.money = vars.Money;
			this.playTimeSeconds = vars.PlayTimeSeconds;
			this.expBank = vars.ExpBank;
			this.winStreak = vars.WinStreak;
			this.saveCount = vars.SaveCount;
			this.loadCount = vars.LoadCount;
			this.deathCount = vars.DeathCount;
			this.currentDayNumber = vars.CurrentDayNumber;
			this.currentTimeOfDay = vars.CurrentTimeOfDay;
			this.storyFlagKVPs = vars.StoryFlags.GetStoryFlagKVPs();
			this.shuffleCardDeck = vars.ShuffleCardDeck.Clone();
			this.friendDataSet = new FriendDataSet(vars.FriendDataSet);
			
			this.dungeonProgressions = new List<DungeonProgressionData>(vars.DungeonProgressions.Select(dp => dp.Clone()));
			this.crawlerProgressions = new List<CrawlerProgressionSet>(vars.CrawlerProgressions.Select(cp => cp.Clone()));

			this.enemyResistanceLogDict = vars.EnemyResistanceLogDict;

			this.characterIDNameDict = vars.CharacterIDMap.CharacterIDNameDict;

			this.locationName = vars.LocationName;

		}
		/// <summary>
		/// Creates an empty game save.
		/// </summary>
		public GameSave() {
			this.newGame = true;
		}
		#endregion

	}
}