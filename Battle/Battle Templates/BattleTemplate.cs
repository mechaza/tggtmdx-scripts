using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.UI.Legacy;
using System.Linq;
using System.Reflection;
using System.Text;
using System;
using Grawly.UI;
using Grawly.Battle.Intros;
using Grawly.Battle.Outros;
using Grawly.Battle.BattleArena;
using Grawly.Battle.BattleArena.Setup;
using Grawly.Battle.BattleArena.Setup.Standard;
using Grawly.Battle.Modifiers;
using Sirenix.Serialization;
using Grawly.Battle.DropParsers;
using Grawly.Battle.Intros.Standard;
using Grawly.Battle.Navigator;
using Grawly.MiniGames.ShuffleTime;
using Grawly.UI.MenuLists;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Grawly.Battle {

    /// <summary>
    /// Basically replaces the old EnemySpawnTemplate with something that's more obvious and versitile.
    /// </summary>
    [CreateAssetMenu(menuName = "Grawly/Battle Template")]
    public class BattleTemplate : SerializedScriptableObject, IMenuable {

		#region ODIN HELPERS

		#endregion

		#region FIELDS - METADATA
		/// <summary>
		/// The name of the battle. Used primarily for organization/help purposes.
		/// </summary>
		[SerializeField, TabGroup("General", "General"), LabelWidth(width: 120f)]
		private string battleName = "";
		/// <summary>
		/// The name of the battle. Used primarily for organization/help purposes.
		/// </summary>
		public string BattleName {
			get {
				return this.battleName;
			}
		}
		/// <summary>
		/// Just some extra information about the battle.
		/// </summary>
		[SerializeField, TabGroup("General", "General"), LabelWidth(width: 120f)]
		private string battleDescription = "";
		/// <summary>
		/// Just some extra information about the battle.
		/// </summary>
		public string BattleDescription {
			get {
				return this.battleDescription;
			}
		}
		/// <summary>
		/// The sprite that should be used to represent this battle template if its used in the Battle Test.
		/// </summary>
		[SerializeField, TabGroup("General", "General")]
		public Sprite previewScreenshot;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should this battle use the battle music? If not, the background music that was playing before the battle started will continue.
		/// </summary>
		[SerializeField, TabGroup("General", "General")]
		public BattleMusicType battleMusicType { get; private set; } = BattleMusicType.Default;
		/// <summary>
		/// The music to use for this template, if being overridden.
		/// </summary>
		[SerializeField, TabGroup("General", "General"), ShowIf("OverridesBattleMusic")]
		private IntroloopAudio customBattleMusic;
		/// <summary>
		/// Is this battle affected by the chaos amount?
		/// </summary>
		[SerializeField, TabGroup("General", "General")]
		private bool affectedByChaos = true;
		/// <summary>
		/// Should this BattleTemplate provide its own drops?
		/// If false, the drops will be calculated as a result of what is stored in the enemies.
		/// </summary>
		[SerializeField, TabGroup("General", "General")]
		private bool overrideBattleDrops = false;
		/// <summary>
		/// The BattleDrops this BattleTemplate provides instead of using the enemies. 
		/// Not used if overrideBattleDrops is false.
		/// </summary>
		[SerializeField, TabGroup("General", "General"), ShowIf("overrideBattleDrops")]
		private Drops battleTemplateDrops = new Drops();
		#endregion

		#region FIELDS - PLAYERS
		/// <summary>
		/// Should this battle override the players that get used?
		/// </summary>
		[SerializeField, TabGroup("General", "General")]
		public bool overridePlayers = false;
		/// <summary>
		/// The templates used to spawn the players. May or may not be used.
		/// </summary>
		[SerializeField,TabGroup("General", "General"), ShowIf("overridePlayers")]
		private List<PlayerTemplate> playerTemplates = new List<PlayerTemplate>();
		/// <summary>
		/// The templates used to spawn the players. May or may not be used.
		/// </summary>
		public List<PlayerTemplate> PlayerTemplates {
			get {
				return this.playerTemplates;
			}
		}
		#endregion
		
		#region PROPERTIES - TOGGLES
		/// <summary>
		/// Does this template play music?
		/// </summary>
		public bool PlaysBattleMusic {
			get {
				return this.battleMusicType == BattleMusicType.Override
				       || this.battleMusicType == BattleMusicType.Default;
			}
		}
		/// <summary>
		/// Does this template play an override battle music?
		/// </summary>
		public bool OverridesBattleMusic {
			get {
				return this.battleMusicType == BattleMusicType.Override;
			}
		}
		/// <summary>
		/// The custom music to use for this battle template.
		/// </summary>
		/// <exception cref="Exception"></exception>
		public IntroloopAudio CustomBattleMusic {
			get {
				if (this.OverridesBattleMusic == false) {
					throw new System.Exception("Trying to get custom music for a template that does not allow it!");
				}

				return this.customBattleMusic;
			}
		}
		#endregion
		
		#region FIELDS - ENEMIES
		/// <summary>
		/// The templates used to spawn the enemies.
		/// </summary>
		[SerializeField, TabGroup("Combatants", "Enemies"), ListDrawerSettings(Expanded = true)]
		private List<EnemyTemplate> enemyTemplates = new List<EnemyTemplate>();
		/// <summary>
		/// The templates used to spawn the enemies.
		/// </summary>
		public List<EnemyTemplate> EnemyTemplates {
			get {
				return this.enemyTemplates;
			}
		}
		#endregion

		#region FIELDS - BEHAVIORS
		/// <summary>
		/// The behavior that should be used when setting up the battle arena.
		/// </summary>
		[OdinSerialize, TabGroup("Behaviors", "Behaviors"), Title("Arena Setup"), InlineProperty, HideLabel]
		private BattleArenaSetup BattleArenaSetup { get; set; }
		/// <summary>
		/// The behavior this battle should use for the Battle Navigator.
		/// </summary>
		[OdinSerialize, TabGroup("Behaviors", "Behaviors"), Title("Navigator"), InlineProperty, HideLabel]
		private NavigatorBehavior NavigatorBehavior { get; set; }
		/// <summary>
		/// The intro that should be used for this battle.
		/// E.x., how the game transitions from dungeon to battle, or otherwise.
		/// </summary>
		[OdinSerialize, TabGroup("Behaviors", "Behaviors"), Title("Intro"), InlineProperty, HideLabel]
		private BattleIntro BattleIntro { get; set; }
		/// <summary>
		/// The outro that should be used for this battle.
		/// Handles if I'm transitioning to like, the results screen or whatever.
		/// </summary>
		[OdinSerialize, TabGroup("Behaviors", "Behaviors"), Title("Outro"), InlineProperty, HideLabel]
		private BattleOutro BattleOutro { get; set; }
		/// <summary>
		/// The DropParser to use when parsing. The results of the battle.
		/// </summary>
		[OdinSerialize, TabGroup("Behaviors", "Behaviors"), Title("Drop Parser"), InlineProperty, HideLabel]
		private BattleDropParser BattleDropParser { get; set; }
		#endregion
		
		#region FIELDS - MODIFIERS
		/// <summary>
		/// A list of battle modifiers to use by default.
		/// </summary>
		[OdinSerialize, TabGroup("Behaviors", "Modifiers"), ListDrawerSettings(Expanded = true)]
		private List<BattleModifier> battleModifiers = new List<BattleModifier>();
		/// <summary>
		/// A list of battle modifiers to use by default.
		/// Clones the list in the template so it doesnt affect the scriptableobject.
		/// </summary>
		private List<BattleModifier> BattleModifiers {
			get {
				Debug.Log("Cloning BattleModifiers...");
				return this.battleModifiers.Select(m => m.Clone()).ToList();
			}
		}
		#endregion
	
		#region FIELDS - SHUFFLE TIME
		/// <summary>
		/// Should Shuffle Time be enabled?
		/// </summary>
		[OdinSerialize, TabGroup("Behaviors", "Shuffle Time")]
		public bool EnableShuffleTime { get; private set; } = false;
		/// <summary>
		/// The shuffle card templates to use for this battle template, if shuffle time is available.
		/// </summary>
		[OdinSerialize, TabGroup("Behaviors", "Shuffle Time"), ListDrawerSettings(Expanded = true), ShowIf("EnableShuffleTime")]
		private List<ShuffleCardTemplate> shuffleCardTemplates = new List<ShuffleCardTemplate>();
		/// <summary>
		/// The deck to use when starting a shuffle time session with this battle template.
		/// </summary>
		public ShuffleCardDeck ShuffleCardDeck {
			get {
				// Just create a new deck from the card templates.
				return new ShuffleCardDeck(cardTemplates: this.shuffleCardTemplates);
			}
		}
		#endregion

		#region FIELDS - COMPUTED : ENEMIES
		/// <summary>
		/// The sum of the level of all the enemies divided by their count.
		/// </summary>
		public int AverageLevel {
			get {
				return (int)((float)(this.EnemyTemplates.Sum(et => et.Level)) / (float)(this.EnemyTemplates.Count));
			}
		}
		#endregion

		#region GETERS - PARAMS
		/// <summary>
		/// Generates a template behavior set from the behaviors inside this template.
		/// </summary>
		/// <param name="advantageType">The advantage type to use for these params.</param>
		public BattleParams GenerateBattleParams() {
			
			// Return a new set 
			return new BattleParams() {
				ArenaSetup = this.BattleArenaSetup,
				BattleIntro = this.BattleIntro,
				BattleOutro = this.BattleOutro,
				DropParser = this.BattleDropParser,
				NavigatorBehavior = this.NavigatorBehavior.Clone(),
				BattleModifiers = this.BattleModifiers
			};

		}
		#endregion
		
		#region GETTERS - BATTLE DROPS
		/// <summary>
		/// Gets the experience that this BattleTemplate awards upon completion.
		/// </summary>
		/// <param name="gameVariables">The GameVariables associated with the current game.</param>
		/// <returns></returns>
		public int GetExperience(GameVariables gameVariables) {
			// Calculate the base amount of experience. This depends on whether it is being overridden or not.
			int baseExp = this.overrideBattleDrops == true ? this.battleTemplateDrops.exp : this.enemyTemplates.Sum(et => et.drops.exp);
			// If this battle template is affected by chaos, multiply it by the exp multiplier.
			baseExp = this.affectedByChaos == true ? (int)(baseExp * gameVariables.ExpMultiplier) : baseExp;
			// Return the exp, as everything is calculated.
			return baseExp;
		}
		/// <summary>
		/// Gets the money that this BattleTemplate awards upon completion.
		/// </summary>
		/// <param name="gameVariables">The GameVariables associated with the current game.</param>
		/// <returns></returns>
		public int GetMoney(GameVariables gameVariables) {
			// Calculate the base amount of experience. This depends on whether it is being overridden or not.
			int baseMoney = this.overrideBattleDrops == true ? this.battleTemplateDrops.money : this.enemyTemplates.Sum(et => et.drops.money);
			// If this battle template is affected by chaos, multiply it by the exp multiplier.
			baseMoney = this.affectedByChaos == true ? (int)(baseMoney * gameVariables.MoneyMultiplier) : baseMoney;
			// Return the exp, as everything is calculated.
			return baseMoney;
		}
		/// <summary>
		/// Gets the items that this BattlTemplate awards upon completion.
		/// </summary>
		/// <param name="gameVariables">The GameVariables associated with the current game.</param>
		/// <returns></returns>
		public List<BattleBehavior> GetItems(GameVariables gameVariables) {
			// Return the items stored in this template if overriding it. If not, return the ones the enemies have.
			return this.overrideBattleDrops == true ? this.battleTemplateDrops.items : this.enemyTemplates.SelectMany(et => et.drops.items).ToList();
		}
		#endregion

		#region INTERFACE IMPLEMENATION - IMENUABLE
		public string PrimaryString {
			get {
				return this.BattleName;
			}
		}
		public string QuantityString {
			get {
				return "";
			}
		}
		public string DescriptionString {
			get {
				return "";
			}
		}
		public Sprite Icon {
			get {
				return this.EnemyTemplates[0].iconSprite;
			}
		}
		#endregion

		#region ENUM DEFINITIONS
		/// <summary>
		/// Defines the different kinds of ways a battle template can play music.
		/// </summary>
		public enum BattleMusicType {
			Default 			= 0,		// Plays the normal battle theme.
			None 				= 1,		// Plays whatever is still currently playing.
			Override			= 2,		// Overrides the battle theme with its own theme.
			Silent				= 3,		// Should be used for silent battles.
		}
		#endregion
		
	}


}