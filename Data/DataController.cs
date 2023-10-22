using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;
using Grawly.Dungeon;
using Grawly.Chat;
using Grawly.Calendar;
using System.Linq;
using Grawly.Battle.Equipment;
using Grawly.Battle.Equipment.Badges;
using Grawly.Data;
using Grawly.DungeonCrawler;
using Grawly.DungeonCrawler.Generation;
using Grawly.Friends;
using Grawly.Menus;
using Grawly.Toggles;

namespace Grawly {
	public class DataController : SerializedMonoBehaviour {

		public static DataController Instance { get; private set; }

		#region FIELDS - GLOBAL ASSET DATA
		/// <summary>
		/// The GlobalAssetData that stores data that should never be changed.
		/// If something isn't explicitly defined in the monobehaviour, it will default to the global asset data's contents. It's a ScriptableObject.
		/// I used to have all the assets as just part of a monobehaviour but thats kinda Uhh. lmao.
		/// </summary>
		private GlobalAssetData globalAssetData;
		#endregion

		#region UNITY FUNCTIONS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				// Load the default global asset data from resources. Because otherwise Unity includes it with each scene in the build which isn't incredibly good.
				this.globalAssetData = Resources.Load<GlobalAssetData>(path: "GlobalAssetData/DefaultGlobalAssetData");
			}
		}
		#endregion

		#region GETTERS - TOGGLES
		/// <summary>
		/// Gets a difficulty variables template from the asset data.
		/// </summary>
		/// <param name="difficultyType"></param>
		/// <returns></returns>
		public DifficultyVariablesTemplate GetDifficultyTemplate(DifficultyType difficultyType) {
			return this.globalAssetData.difficultyTemplates.First(t => t.difficultyType == difficultyType);
		}
		#endregion
		
		#region GETTERS - AUDIO
		/// <summary>
		/// Gets an SFX of a specific type.
		/// </summary>
		public AudioClip GetSFX(SFXType type) {
			return this.globalAssetData.GetSFX(type: type);
		}
		/// <summary>
		/// Gets an SFX of a specific name.
		/// </summary>
		public AudioClip GetSFX(string name) {
			return this.globalAssetData.GetSFX(name: name);
		}
		/// <summary>
		/// Gets the music for a specific type.
		/// </summary>
		/// <param name="type">The type of music to retrieve.</param>
		/// <returns></returns>
		public IntroloopAudio GetMusic(MusicType type) {
			// If no entry exists locally, use the default.
			return this.globalAssetData.GetMusic(type: type);
		}
		#endregion

		#region GETTERS - IMAGES
		/// <summary>
		/// Returns the menu icon for a given elemental type.
		/// </summary>
		/// <param name="type">The element associated with the desired icon.</param>
		/// <param name="useLargeIcon">Should the new, larger icon be used?</param>
		/// <returns></returns>
		public Sprite GetElementalIcon(ElementType type, bool useLargeIcon = false) {
			return this.globalAssetData.GetElementalIcon(type: type, useLargeIcon: useLargeIcon);
		}
		/// <summary>
		/// Returns a sprite for a given string.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Sprite GetSprite(string name) {
			return this.globalAssetData.GetSprite(name: name);
		}
		#endregion

		#region GETTERS - GAMEOBJECTS
		/// <summary>
		/// Returns a game object for a given name.
		/// </summary>
		public GameObject GetGameObject(string name) {
			return this.globalAssetData.GetGameObject(name: name);
		}
		#endregion

		#region GETTERS - BATTLE EFFECTS
		/// <summary>
		/// Gets the VFX associated with a particular affliction.
		/// </summary>
		/// <param name="afflictionType">The type of affliction.</param>
		/// <returns>The particle system assocaiated with the given affliction.</returns>
		public GameObject GetAfflictionVFX(AfflictionType afflictionType) {
			return this.globalAssetData.GetAfflictionVFX(afflictionType: afflictionType);
		}
		#endregion

		#region GETTERS - BATTLE BEHAVIORS
		/// <summary>
		/// Get a battle behavior for a given affliction type.
		/// </summary>
		public BattleBehavior GetBehavior(AfflictionType afflictionType) {
			return this.globalAssetData.GetBehavior(afflictionType: afflictionType);
		}
		/// <summary>
		/// Gets a "common" battle behavior associated with a specified type.
		/// </summary>
		/// <param name="commonBehaviorType"></param>
		/// <returns></returns>
		public BattleBehavior GetBehavior(CommonBattleBehaviorType commonBehaviorType) {
			return this.globalAssetData.GetBehavior(commonBehaviorType: commonBehaviorType);
		}
		/// <summary>
		/// Get a BattleBehavior specified for the given string.
		/// </summary>
		/// <param name="behaviorName">The name of the BattleBehavior to grab.</param>
		/// <returns></returns>
		public BattleBehavior GetBehavior(string behaviorName) {
			return this.globalAssetData.GetBehavior(behaviorName: behaviorName);
		}
		#endregion

		#region GETTERS - BFX
		/// <summary>
		/// ouh
		/// </summary>
		/// <param name="bFXType"></param>
		/// <returns></returns>
		public GameObject GetBFX(BFXType bFXType) {
			Debug.Log("DATA: Getting BFX of type " + bFXType.ToString());
			return this.globalAssetData.GetBFX(bFXType);
		}
		/// <summary>
		/// Gets a ScriptableBFX associated with the given BattleBehavior.
		/// </summary>
		/// <param name="battleBehavior">The battle behavior associated with the scriptablebfx</param>
		/// <returns></returns>
		public ScriptableBFX GetScriptableBFX(BattleBehavior battleBehavior) {
			Debug.Log("DATA: Getting ScriptableBFX for behavior " + battleBehavior.behaviorName);
			return this.globalAssetData.GetBFX(battleBehavior: battleBehavior);
		}
		#endregion

		#region GETTERS - ENEMIES
		/// <summary>
		/// Returns the placeholder enemy spawn dictionary in the event one inside of a MissionData cannot be used.
		/// </summary>
		/// <returns></returns>
		public Dictionary<int, List<BattleTemplate>> GetPlaceholderBattleTemplateDict() {
			Debug.LogWarning("NOTE: Using Placeholder Battle Template Template Dict");
			return this.globalAssetData.GetPlaceholderBattleTemplateDict();
		}
		#endregion

		#region GETTERS - MAPS
		/// <summary>
		/// Returns the Crawler Dungeon associated with the specified CrawlerDungeonIDType.
		/// </summary>
		/// <param name="crawlerDungeonIDType"></param>
		/// <returns></returns>
		public CrawlerDungeonTemplate GetCrawlerDungeonTemplate(CrawlerDungeonIDType crawlerDungeonIDType) {
			return this.globalAssetData.GetCrawlerDungeonTemplate(crawlerDungeonIDType);
		}
		#endregion
		
		#region GETTERS - REWARDS AND TREASURES
		/// <summary>
		/// Returns the placeholder treasure dictionary in the event one inside of a MissionData cannot be used.
		/// </summary>
		public Dictionary<int, List<BattleBehavior>> GetPlaceholderTreasureDict() {
			Debug.LogWarning("NOTE: Using Placeholder Enemy Spawn Template Dict");
			return this.globalAssetData.GetPlaceholderTreasureDict();
		}
		/// <summary>
		/// Returns the placeholder rewards list in the event one inside of a MissionData cannot be used.
		/// </summary>
		public List<BattleBehavior> GetPlaceholderRewards() {
			Debug.LogWarning("NOTE: Using Placeholder Rewards");
			return this.globalAssetData.GetPlaceholderRewards();
		}
		#endregion

		#region STATIC GETTERS - GAME PRESET TEMPLATES
		/// <summary>
		/// Returns the object that holds references to all the GamePresets to actually be used.
		/// </summary>
		/// <returns></returns>
		public static GamePresetTemplateReference GetGamePresetTemplateReference() {
			return Resources.Load<GamePresetTemplateReference>(path: "Game Presets/GamePresetReference");
		}
		#endregion
		
		#region STATIC GETTERS - GAME VARIABLES
		/// <summary>
		/// Returns the default GameVariablesTemplate. This is typically used for when the game is started for the first time.
		/// </summary>
		/// <returns></returns>
		public static GameVariablesTemplate GetDefaultGameVariablesTemplate() {
			return Resources.Load<GameVariablesTemplate>(path: "GameVariables/DefaultGameVariables");
		}
		/// <summary>
		/// Returns the GameVariablesTemplate of the specified name. Typically will only really do this if I'm debugging shit.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static GameVariablesTemplate GetGameVariablesTemplate(string str) {
			return Resources.Load<GameVariablesTemplate>(path: "GameVariables/" + str);
		}
		#endregion

		#region STATIC GETTERS - CALENDAR DATA
		/// <summary>
		/// Returns the default CalendarData.
		/// I use this when bootstrapping new GamePresets.
		/// </summary>
		/// <returns></returns>
		public static CalendarData GetDefaultCalendarData() {
			return Resources.Load<CalendarData>(path: "CalendarData/DefaultCalendarData");
		}
		#endregion
		
		#region STATIC GETTERS - GAME TOGGLES
		/// <summary>
		/// Returns the default game toggles template.
		/// This is what is used a lot during settings configurations and shit.
		/// </summary>
		/// <returns></returns>
		public static GameTogglesTemplateDX GetDefaultGameTogglesTemplate() {
			// Just find the specific string match.
			return GetGameTogglesTemplate(str: "DefaultConfigToggles");
		}
		/// <summary>
		/// Gets the GameTogglesTemplate with the specified file name.
		/// </summary>
		/// <param name="str">The name of the toggles template to grab.</param>
		/// <returns></returns>
		public static GameTogglesTemplateDX GetGameTogglesTemplate(string str) {
			return GetAllGameTogglesTemplates().Find(tt => tt.name == str);
		}
		/// <summary>
		/// Grabs all of the available GameTogglesTemplates in the resources folder.
		/// </summary>
		/// <returns></returns>
		private static List<GameTogglesTemplateDX> GetAllGameTogglesTemplates() {
			return Resources.LoadAll<GameTogglesTemplateDX>(path: "GameToggles/").ToList();
		}
		#endregion

		#region STATIC GETTERS - CHAT SPEAKERS
		/// <summary>
		/// i might remove this later
		/// </summary>
		private static AssetBundle commonChatSpeakerBundle;
		/// <summary>
		/// Returns a chat speaker with the given 
		/// </summary>
		/// <param name="speakerName"></param>
		/// <returns></returns>
		public static ChatSpeakerTemplate GetChatSpeaker(string speakerName) {

			// just doing this for now
			try {
				return Resources.LoadAll<ChatSpeakerTemplate>(path: "Chat Speakers/").First(cs => cs.rawSpeakerName.ToLower() == speakerName.ToLower());
			} catch (System.Exception e) {
				Debug.LogError("Couldn't find chat speaker with name " + speakerName + "! Returning blank speaker.");
				return Resources.LoadAll<ChatSpeakerTemplate>(path: "Chat Speakers/").First(cs => cs.rawSpeakerName == "");
			}
			

			Debug.Log("Getting speaker name: " + speakerName);

			if (Application.isEditor == true) {
				return Resources.LoadAll<ChatSpeakerTemplate>(path: "Chat Speakers/").First(cs => cs.rawSpeakerName.ToLower() == speakerName.ToLower());
			}

			if (commonChatSpeakerBundle == null) {
				commonChatSpeakerBundle = AssetBundle.LoadFromFile(path: System.IO.Path.Combine(Application.streamingAssetsPath, "chat-speakers/common-chat-speakers"));
			}
			// AssetBundle assetBundle = AssetBundle.LoadFromFile(path: System.IO.Path.Combine(Application.streamingAssetsPath, "chat-speakers/common-chat-speakers"));
			// return assetBundle.LoadAsset<ChatSpeaker>(name: speakerName);
			return commonChatSpeakerBundle.LoadAllAssets<ChatSpeakerTemplate>().First(cs => cs.rawSpeakerName.ToLower() == speakerName.ToLower());
		}
		#endregion

		#region STATIC GETTERS - CHAT PICTURES
		/// <summary>
		/// Gets a sprite associated with the provided ID for use in chat dialogues. 
		/// </summary>
		/// <param name="pictureID">The ID of the picture to retrieve.</param>
		/// <returns>The sprite that relates to the desired picture.</returns>
		public static Sprite GetChatPicture(string pictureID) {
			// Look at all the chat pictures and find the first one whose ID matches the one requested.
			return DataController.GetAllChatPictures()
				.First(pt => pt.ID == pictureID)
				.Sprite;
		}
		/// <summary>
		/// Grabs all of the chat pictures in the resources folder.
		/// </summary>
		/// <returns></returns>
		private static List<ChatPictureTemplate> GetAllChatPictures() {
			// Go into the Chat Pictures folder and grab all of that shit.
			return Resources.LoadAll<ChatPictureTemplate>(path: "Chat Pictures/").ToList();
		}
		#endregion
		
		#region STATIC GETTERS - FRIEND TEMPLATES
		/// <summary>
		/// Returns a friend template with the given arcana.
		/// </summary>
		/// <param name="arcanaType">The arcana to match.</param>
		/// <returns>A friend template with the given arcana.</returns>
		public static FriendTemplate GetFriendTemplate(ArcanaType arcanaType) {
			Debug.Log("Geting teplate for arcana " + arcanaType);
			return Resources.LoadAll<FriendTemplate>(path: "Friends/").First(ft => ft.arcanaType == arcanaType);
		}
		#endregion
		
		#region STATIC GETTERS - BATTLEBEHAVIORS
		/// <summary>
		/// Grabs a BattleBehavior when all else fails.
		/// </summary>
		/// <param name="behaviorName"></param>
		/// <returns></returns>
		public static BattleBehavior GetDefaultBattleBehavior(string behaviorName) {
			return Resources.Load<BattleBehavior>(path: "Behaviors/Defaults/" + behaviorName);
		}
		#endregion
		
		#region STATIC GETTERS - COMBATANTS
		/// <summary>
		/// Returns the Player Template with the specified player name.
		/// </summary>
		/// <param name="playerName"></param>
		/// <returns></returns>
		public static PlayerTemplate GetPlayerTemplate(string playerName) {
			return DataController.GetAllPlayerTemplates().Find(pt => pt.metaData.name == playerName);
		}
		/// <summary>
		/// Returns all of the PlayerTemplates stored in resources.
		/// </summary>
		/// <returns></returns>
		public static List<PlayerTemplate> GetAllPlayerTemplates() {
			return Resources.LoadAll<PlayerTemplate>(path: "Players/").ToList();
		}
		/// <summary>
		/// Returns the Persona Template with the specified name.
		/// </summary>
		/// <returns></returns>
		public static PersonaTemplate GetPersonaTemplate(string personaName) {
			return DataController.GetAllPersonaTemplates().Find(pt => pt.metaData.name == personaName);
		}
		/// <summary>
		/// Returns all of the PersonaTemplates stored in resources.
		/// </summary>
		/// <returns></returns>
		public static List<PersonaTemplate> GetAllPersonaTemplates() {
			return Resources.LoadAll<PersonaTemplate>(path: "Personas/").ToList();
		}
		/// <summary>
		/// Returns all of the battle behaviors stored in resources.
		/// </summary>
		/// <returns></returns>
		public static List<BattleBehavior> GetAllBattleBehaviors() {
			return Resources.LoadAll<BattleBehavior>(path: "Behaviors/").ToList();
		}
		/// <summary>
		/// Gets all of the enemy templates.
		/// </summary>
		/// <returns></returns>
		public static List<EnemyTemplate> GetAllEnemyTemplates() {
			// return UnityEditor.AssetDatabase.LoadAllAssetsAtPath("_TGGTMDX/Definitions/Enemies/")
			return Resources.LoadAll<EnemyTemplate>(path: "Enemies/").ToList();
		}
		/// <summary>
		/// Gets all of the enemy templates in the given subfolder.
		/// </summary>
		/// <param name="subfolder">The name of the subfolder to get the enemies from.</param>
		/// <returns></returns>
		public static List<EnemyTemplate> GetAllEnemyTemplates(string subfolder) {
			/*return UnityEditor.AssetDatabase
				.LoadAllAssetsAtPath("_TGGTMDX/Definitions/Enemies/" + subfolder)
				.Where(o => o is EnemyTemplate)
				.Cast<EnemyTemplate>()
				.ToList();*/
			return Resources.LoadAll<EnemyTemplate>(path: "Enemies/" + subfolder + "/").ToList();
		}
		#endregion

		#region STATIC GETTERS - BADGES
		/// <summary>
		/// Gets all of the BadgeTemplates stored in the resources folder.
		/// </summary>
		/// <returns>A list of all available badge templates.</returns>
		private static List<BadgeTemplate> GetAllBadgeTemplates() {
			return Resources.LoadAll<BadgeTemplate>(path: "Badge Templates/").ToList();
		}
		/// <summary>
		/// Gets the BadgeTemplate associated with the provided BadgeID.
		/// </summary>
		/// <param name="badgeID">The ID that links to the relevant BadgeTemplate.</param>
		/// <returns>The BadgeTemplate with the correct ID.</returns>
		public static BadgeTemplate GetBadgeTemplate(BadgeID badgeID) {
			// Go through all the badge templates and find the first one with a matching ID.
			return GetAllBadgeTemplates().First(bt => bt.BadgeID.Equals(badgeID));
		}
		#endregion

		#region STATIC GETTERS - WEAPONS
		/// <summary>
		/// Gets all of the WeaponTemplates stored in the resources folder.
		/// </summary>
		/// <returns>A list of all available weapon templates.</returns>
		private static List<WeaponTemplate> GetAllWeaponTemplates() {
			return Resources.LoadAll<WeaponTemplate>(path: "Weapon Templates/").ToList();
		}
		/// <summary>
		/// Gets a WeaponTemplate associated with the provided WeaponID.
		/// </summary>
		/// <param name="weaponID">The WeaponID to identify the desired template.</param>
		/// <returns>The WeaponTemplate with the associated ID.</returns>
		public static WeaponTemplate GetWeaponTemplate(WeaponID weaponID) {
			// Go through all the weapon templates and find the first one with a matching ID.
			return GetAllWeaponTemplates().First(wt => wt.WeaponID.Equals(weaponID));
		}
		#endregion
		
		#region STATIC GETTERS - AFFLICTION MATERIALS
		/// <summary>
		/// Gets the material that should be applied to a sprite for the specified affliction.
		/// </summary>
		/// <param name="afflictionType">The type of affliction that is associated with the given material.</param>
		/// <returns></returns>
		public static Material GetAfflictionMaterial(AfflictionType afflictionType) {
			return Resources.Load<Material>(path: "AfflictionMaterials/" + afflictionType.ToString());
		}
		#endregion

		#region STATIC GETTERS - AUDIO
		/// <summary>
		/// Gets an audio clip of the specified SFX type. This shouldn't be used frequently as it uses Resources.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static AudioClip GetDefaultSFX(SFXType type) {
			// Load up the default global asset data and use that to grab the sfx.
			return Resources.Load<GlobalAssetData>(path: "GlobalAssetData/DefaultGlobalAssetData").GetSFX(type: type);
		}
		#endregion

		#region STATIC GETTERS - ICONS
		/// <summary>
		/// Gets an icon for the specified elemental type.
		/// </summary>
		/// <param name="elementType">The type of element associated with the icon.</param>
		/// <param name="useLargeIcon">Should the new, large icon be used?</param>
		/// <returns></returns>
		public static Sprite GetDefaultElementalIcon(ElementType elementType, bool useLargeIcon = false) {
			// Load up the default global asset data and use that to grab the icon.
			return Resources
				.Load<GlobalAssetData>(path: "GlobalAssetData/DefaultGlobalAssetData")
				.GetElementalIcon(type: elementType, useLargeIcon: useLargeIcon);
		}
		/// <summary>
		/// Gets an icon for the specified weather type.
		/// </summary>
		/// <param name="weatherType"></param>
		/// <returns></returns>
		public static Sprite GetDefaultWeatherIcon(WeatherType weatherType) {
			return Resources
				.Load<GlobalAssetData>(path: "GlobalAssetData/DefaultGlobalAssetData")
				.GetWeatherIcon(weatherType: weatherType);
		}
		#endregion

		#region STATIC GETTERS - TEXT MATERIALS
		/// <summary>
		/// Grabs the material that should be used for a stm.
		/// </summary>
		/// <param name="materialName">The name of the material.</param>
		/// <returns></returns>
		public static Material GetDefaultSTMMaterial(string materialName) {
			return Resources.Load<Material>(path: "DefaultSTMMaterials/" + materialName);
		}
		#endregion

	}

}