using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
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
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grawly {

	/// <summary>
	/// A way of storing the "initial" state of a game. Mostly for debug purposes, but can be used in other situations as well.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Game Variables Template")]
	public class GameVariablesTemplate : SerializedScriptableObject {

		#region FIELDS - CONFIGURATION : TOGGLES
		/// <summary>
		/// The playstyle for this set of variables.
		/// </summary>
		[FormerlySerializedAs("PlaystyleModeType")]
		[TabGroup("Variables", "General"), Title("Difficulty")]
		public PlaystyleType playstyleType = PlaystyleType.None;
		/// <summary>
		/// The game's difficulty.
		/// </summary>
		[TabGroup("Variables","General"), Title("Difficulty")]
		public DifficultyType DifficultyType = DifficultyType.Normal;
		#endregion

		#region FIELDS - GENERAL : INVENTORY
		/// <summary>
		/// The amount of money the party has.
		/// </summary>
		[TabGroup("Variables","General"), Title("Inventory")]
		public int money = 0;
		/// <summary>
		/// A list of items to store in the inventory by default.
		/// </summary>
		[TabGroup("Variables","General"), InlineEditor]
		public List<BattleBehavior> items = new List<BattleBehavior>();
		/// <summary>
		/// The template that should be used to create a set of weapons for the party.
		/// </summary>
		[TabGroup("Variables", "General"), InlineEditor]
		public WeaponCollectionSetTemplate weaponCollectionSetTemplate;
		/// <summary>
		/// The template that should be used to create a set of badges for the party.
		/// </summary>
		[TabGroup("Variables", "General"), InlineEditor]
		public BadgeCollectionSetTemplate badgeCollectionSetTemplate;
		#endregion
		
		#region FIELDS - GENERAL : CALENDAR
		/// <summary>
		/// The day to start out on.
		/// </summary>
		[TabGroup("Variables","Configuration"), Title("Calendar")]
		public int startDay = 4;
		/// <summary>
		/// The time of day to use by default.
		/// </summary>
		[TabGroup("Variables","Configuration")]
		public TimeOfDayType startTime = TimeOfDayType.Afternoon;
		#endregion
		
		#region FIELDS - SAVING : TOTALS
		/// <summary>
		/// The number of times these variables have been saved to disk.
		/// </summary>
		[TabGroup("Variables","Configuration"), Title("Totals")]
		public int saveCount = 0;
		/// <summary>
		/// The number of times these variables have been loaded from disk.
		/// </summary>
		[TabGroup("Variables","Configuration")]
		public int loadCount = 0;
		/// <summary>
		/// The number of times the party has died on this save.
		/// </summary>
		[TabGroup("Variables","Configuration")]
		public int deathCount = 0;
		#endregion
		
		#region FIELDS - TOGGLES : GAUNTLET
		/// <summary>
		/// The number of battles the player has won in a row.
		/// </summary>
		[TabGroup("Variables","Configuration"), Title("Gauntlet")]
		public int winStreak = 0;
		/// <summary>
		/// The amount of experience currently awaiting to be cashed in.
		/// </summary>
		[TabGroup("Variables","Configuration")]
		public int expBank = 0;
		#endregion
		
		#region FIELDS - SAVING : NAMES
		/// <summary>
		/// Contains the names that get used for the girls in the game.
		/// </summary>
		[TabGroup("Variables","Configuration"), Title("Character IDs"), OdinSerialize]
		private CharacterIDMap characterIDMap = new CharacterIDMap();
		/// <summary>
		/// Contains the names that get used for the girls in the game.
		/// </summary>
		public CharacterIDMap CharacterIDMap {
			get {
				Debug.Log("Cloning names from template.");
				return this.characterIDMap.Clone();
			}
		}
		#endregion
		
		#region FIELDS - SHUFFLE CARDS
		/// <summary>
		/// A list of shuffle cards to create a deck from by default.
		/// </summary>
		[TabGroup("Variables","Configuration"), Title("Shuffle Cards")]
		public List<ShuffleCardTemplate> shuffleCardTemplates = new List<ShuffleCardTemplate>();
		#endregion
		
		#region FIELDS - TOGGLES : FLAGS
		/// <summary>
		/// The flags that should be used for this particular game state.
		/// </summary>
		[TabGroup("Variables","Configuration"), OdinSerialize, Title("Story"), HideReferenceObjectPicker, HideLabel]
		private StoryFlags storyFlags = new StoryFlags();
		/// <summary>
		/// The flags that should be used for this particular game state.
		/// </summary>
		public StoryFlags StoryFlags {
			get {
				Debug.Log("Cloning flags from template.");
				return this.storyFlags.Clone();
			}
		}
		#endregion
		
		#region FIELDS - COMBATANTS
		/// <summary>
		/// The player templates to use for this game state.
		/// </summary>
		[TabGroup("Variables","Combatants"), InlineEditor]
		public List<PlayerTemplate> playerTemplates = new List<PlayerTemplate>();
		/// <summary>
		/// The templates that should be used in addition to ones stored by players.
		/// </summary>
		[TabGroup("Variables","Combatants"), InlineEditor, ValidateInput(condition: "ValidateDebugPersonas", "Debug Personas cannot contain any personas which already belong to a Debug Player!", InfoMessageType.Warning)]
		public List<PersonaTemplate> personaTemplates = new List<PersonaTemplate>();
		#endregion

		#region FIELDS - FRIENDS
		/// <summary>
		/// The friend templates that should be used by default.
		/// </summary>
		[SerializeField, TabGroup("Variables","Configuration"), Title("Friends")]
		private List<FriendTemplate> friendTemplates = new List<FriendTemplate>();
		/// <summary>
		/// The friend templates that should be used by default.
		/// </summary>
		public List<FriendTemplate> FriendTemplates {
			get {
				return this.friendTemplates;
			}
			set {
				this.friendTemplates = value;
			}
		}
		#endregion
		
		#region FIELDS - DUNGEON PROGRESSION
		/// <summary>
		/// A list containing the data of how far a player has progressed in a dungeon.
		/// </summary>
		[TabGroup("Variables","Configuration"), OdinSerialize, Title("Dungeon")]
		private List<DungeonProgressionData> dungeonProgressions = new List<DungeonProgressionData>();
		/// <summary>
		/// A list containing the data of how far a player has progressed in a dungeon.
		/// </summary>
		public List<DungeonProgressionData> DungeonProgressions {
			get {
				return this.dungeonProgressions.Select(dp => dp.Clone()).ToList();
			}
		}
		#endregion

		#region FIELDS - CRAWLER PROGRESSION
		/// <summary>
		/// A list containing the data of how far a player has progressed in a crawler.
		/// </summary>
		[TabGroup("Variables", "Configuration"), OdinSerialize, Title("Crawler")]
		private List<CrawlerProgressionSet> crawlerProgressions = new List<CrawlerProgressionSet>();
		/// <summary>
		/// A list containing the data of how far a player has progressed in a crawler.
		/// </summary>
		public List<CrawlerProgressionSet> CrawlerProgressions {
			get {
				// Clone that shit.
				return this.crawlerProgressions.Select(cp => cp.Clone()).ToList();
			}
		}
		#endregion
		
		#region ODIN FUNCTIONS
		/// <summary>
		/// Makes sure none of the debug players contain any of the debug personas.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		private bool ValidateDebugPersonas(List<PersonaTemplate> value) {
			foreach (PersonaTemplate persona in value) {
				foreach (PlayerTemplate player in this.playerTemplates) {
					if (player.personaTemplate == persona) {
						return false;
					}
				}
			}
			return true;
		}
		#endregion

		
		
	}


}
