using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;
using Grawly.Chat;
using Grawly.Dungeon;
using Sirenix.Serialization;
using System.Linq;
using Grawly.Calendar;
using Grawly.Data;
using Grawly.DungeonCrawler;
using Grawly.DungeonCrawler.Generation;
#if UNITY_EDITOR	
using UnityEditor;
#endif
namespace Grawly {
	/// <summary>
	/// Contains assets that should always remain in memory.
	/// This replaces how I used to handle the DataController where all the assets were just stored in a MonoBehaviour.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Global Asset Data")]
	public class GlobalAssetData : SerializedScriptableObject {
		#region FIELDS - TOGGLES
		/// <summary>
		/// A list containing the difficulty templates.
		/// </summary>
		[TabGroup("Toggles", "Toggles"), SerializeField]
		public List<DifficultyVariablesTemplate> difficultyTemplates = new List<DifficultyVariablesTemplate>();
		#endregion

		#region FIELDS - AUDIO
		/// <summary>
		/// Stores introloop audio for music of any given type.
		/// </summary>
		[TabGroup("Audio", "Music"), SerializeField]
		public Dictionary<MusicType, IntroloopAudio> musicDict = new Dictionary<MusicType, IntroloopAudio>();
		/// <summary>
		/// Stores sound effects by their type.
		/// </summary>
		[TabGroup("Audio", "SFX"), SerializeField]
		public Dictionary<SFXType, AudioClip> sfxTypeDict = new Dictionary<SFXType, AudioClip>();
		/// <summary>
		/// Stores sound effects by a name.
		/// </summary>
		[TabGroup("Audio", "SFX"), SerializeField]
		public Dictionary<string, AudioClip> sfxNameDict = new Dictionary<string, AudioClip>();
		#endregion

		#region FIELDS - IMAGES
		/// <summary>
		/// Stores the different elemental icons.
		/// </summary>
		[TabGroup("Images", "Icons"), SerializeField]
		public Dictionary<ElementType, Sprite> elementIconDict = new Dictionary<ElementType, Sprite>();
		/// <summary>
		/// Stores the different elemental icons, in a larger size.
		/// </summary>
		[TabGroup("Images", "Icons"), SerializeField]
		public Dictionary<ElementType, Sprite> elementLargeIconDict = new Dictionary<ElementType, Sprite>();
		/// <summary>
		/// Stores the different icons that represent the weather types.
		/// </summary>
		[TabGroup("Images", "Icons"), SerializeField]
		public Dictionary<WeatherType, Sprite> weatherIconDict = new Dictionary<WeatherType, Sprite>();
		/// <summary>
		/// Stores sprites by a given name.
		/// </summary>
		[TabGroup("Images", "Sprites"), SerializeField]
		public Dictionary<string, Sprite> spriteNameDict = new Dictionary<string, Sprite>();
		#endregion

		#region FIELDS - GAME OBJECTS
		/// <summary>
		/// A dictionary which stores certain BFX by name.
		/// </summary>
		[TabGroup("GameObjects", "BFX"), SerializeField]
		public Dictionary<BFXType, GameObject> BFXDict = new Dictionary<BFXType, GameObject>();
		/// <summary>
		/// A list of scriptableBFXs that contain lots of sh*t... i gues...... lol
		/// </summary>
		[TabGroup("GameObjects", "BFX"), SerializeField]
		public List<ScriptableBFX> scriptableBFXes = new List<ScriptableBFX>();
		/// <summary>
		/// A dictionary containing all of the particle systems used for animating afflictions.
		/// </summary>
		[TabGroup("GameObjects", "Affliction"), SerializeField]
		public Dictionary<AfflictionType, GameObject> afflictionVFXDict = new Dictionary<AfflictionType, GameObject>();
		/// <summary>
		/// A dictionary which stores game objects by name.
		/// </summary>
		[TabGroup("GameObjects", "Game Objects"), SerializeField]
		public Dictionary<string, GameObject> gameObjectDict = new Dictionary<string, GameObject>();
		#endregion

		#region FIELDS - MAPS
		/// <summary>
		/// A list of crawler dungeon templates. Whoa!
		/// </summary>
		[TabGroup("Maps", "Crawler"), SerializeField]
		private List<CrawlerDungeonTemplate> crawlerDungeonTemplates = new List<CrawlerDungeonTemplate>();
		#endregion
		
		#region FIELDS - BEHAVIORS
		/// <summary>
		/// Contains the behaviors to be used for a given affliction.
		/// </summary>
		[TabGroup("Behaviors", "Affliction Behaviors"), SerializeField]
		public Dictionary<AfflictionType, BattleBehavior> afflictionBehaviorDict = new Dictionary<AfflictionType, BattleBehavior>();
		/// <summary>
		/// Contains the behaviors to be associated for a common behavior.
		/// </summary>
		[TabGroup("Behaviors", "Affliction Behaviors"), SerializeField]
		public Dictionary<CommonBattleBehaviorType, BattleBehavior> commonBehaviorDict = new Dictionary<CommonBattleBehaviorType, BattleBehavior>();
		/// <summary>
		/// A list of all behaviors, more or less as a fallback.
		/// </summary>
		[TabGroup("Behaviors", "All Behaviors"), SerializeField]
		public List<BattleBehavior> allBehaviors = new List<BattleBehavior>();
		#endregion

		#region FIELDS - CHAT SPEAKERS
		/// <summary>
		/// Contains a globally available resource of chat speakers that may be commonly used.
		/// </summary>
		[TabGroup("Chat", "Chat Speakers"), SerializeField]
		private List<ChatSpeakerTemplate> chatSpeakers = new List<ChatSpeakerTemplate>();
		#endregion

		#region FIELDS - PLACEHOLDERS
		/// <summary>
		/// In the event a mission/difficulty combo does not have data available for a spawn dictionary, use this dcitionary as a fail safe.
		/// </summary>
		[TabGroup("Placeholder", "Placeholder Enemies"), OdinSerialize]
		public Dictionary<int, List<BattleTemplate>> placeholderBattleTemplateDict = new Dictionary<int, List<BattleTemplate>>();
		/// <summary>
		/// In the event a mission/difficulty combo does not have data available for a treasure dictionary, use this dcitionary as a fail safe.
		/// </summary>
		[TabGroup("Placeholder", "Placeholder Treasure"), SerializeField]
		public Dictionary<int, List<BattleBehavior>> placeholderTreasureDict = new Dictionary<int, List<BattleBehavior>>();
		/// <summary>
		/// In the event a mission/difficulty combo does not have data available for rewards, use this list as a fail safe.
		/// </summary>
		[TabGroup("Placeholder", "Placeholder Rewards"), SerializeField]
		public List<BattleBehavior> placeholderRewards = new List<BattleBehavior>();
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets an SFX of a specific type.
		/// </summary>
		public AudioClip GetSFX(SFXType type) {
			return sfxTypeDict[type];
		}
		/// <summary>
		/// Gets an SFX of a specific name.
		/// </summary>
		public AudioClip GetSFX(string name) {
			return sfxNameDict[name];
		}
		public IntroloopAudio GetMusic(MusicType type) {
			return this.musicDict[type];
		}
		/// <summary>
		/// Returns the menu icon for a given elemental type.
		/// </summary>
		/// <param name="type">The type of element assocaited with the icon to get.</param>
		/// <param name="useLargeIcon">Should the new, large icon be used?</param>
		/// <returns></returns>
		public Sprite GetElementalIcon(ElementType type, bool useLargeIcon) {
			
			// If using the new large icon, do that.
			if (useLargeIcon == true) {
				return this.elementLargeIconDict[type];
			} else {
				// Otherwise, just use the normal one.
				return elementIconDict[type];
			}
			
		}
		/// <summary>
		/// Gets the sprite associated with the provided weather type.
		/// </summary>
		/// <param name="weatherType"></param>
		/// <returns></returns>
		public Sprite GetWeatherIcon(WeatherType weatherType) {
			return this.weatherIconDict[weatherType];
		}
		/// <summary>
		/// Returns a sprite for a given string.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Sprite GetSprite(string name) {
			return spriteNameDict[name];
		}
		/// <summary>
		/// Returns a game object for a given name.
		/// </summary>
		public GameObject GetGameObject(string name) {
			return gameObjectDict[name];
		}
		/// <summary>
		/// Returns a BFX of a given type.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public GameObject GetBFX(BFXType type) {
			return BFXDict[type];
		}
		/// <summary>
		/// Returns a BFX associated with a given battle behavior.
		/// </summary>
		/// <param name="battleBehavior"></param>
		/// <returns></returns>
		public ScriptableBFX GetBFX(BattleBehavior battleBehavior) {
			return this.scriptableBFXes.First(sb => sb.elementType == battleBehavior.elementType);
		}
		/// <summary>
		/// Gets the VFX associated with a particular affliction.
		/// </summary>
		/// <param name="afflictionType">The type of affliction.</param>
		/// <returns>The particle system assocaiated with the given affliction.</returns>
		public GameObject GetAfflictionVFX(AfflictionType afflictionType) {
			if (afflictionVFXDict.ContainsKey(afflictionType)) {
				return afflictionVFXDict[afflictionType];
			} else {
				Debug.LogWarning("Affliction VFX not in dictionary! Returning Burn as default.");
				return afflictionVFXDict[AfflictionType.Burn];
			}
		}
		/// <summary>
		/// Get a battle behavior for a given affliction type.
		/// </summary>
		public BattleBehavior GetBehavior(AfflictionType afflictionType) {
			return afflictionBehaviorDict[afflictionType];
		}
		/// <summary>
		/// Gets a BattleBehavior for a given common type. E.x., attack.
		/// </summary>
		/// <param name="commonBehaviorType"></param>
		/// <returns></returns>
		public BattleBehavior GetBehavior(CommonBattleBehaviorType commonBehaviorType) {
			return this.commonBehaviorDict[commonBehaviorType];
		}
		/// <summary>
		/// Get a BattleBehavior specified for the given string.
		/// </summary>
		/// <param name="behaviorName">The name of the BattleBehavior to grab.</param>
		/// <returns></returns>
		public BattleBehavior GetBehavior(string behaviorName) {
			try {
				return this.allBehaviors.Find(b => b.behaviorName == behaviorName);
			} catch (System.Exception e) {
				Debug.LogError("Couldn't find behavior with name " + behaviorName + "! Crashing.");
				throw new System.Exception();
			}

			// return this.namedBehaviorDict[behaviorName];
		}
		/// <summary>
		/// Get a chat speaker based on the name passed in.
		/// </summary>
		/// <param name="speakerName">The name of the speaker as it is defined in the asset.</param>
		/// <returns>The Chat Speaker.</returns>
		public ChatSpeakerTemplate GetChatSpeaker(string speakerName) {
			// Find the speaker whose name matches that of the one passed in.
			// Use ToLower as a safeguard.
			return this.chatSpeakers.Find(s => s.rawSpeakerName.ToLower() == speakerName.ToLower());
		}
		/// <summary>
		/// Returns the placeholder enemy spawn dictionary in the event one inside of a MissionData cannot be used.
		/// </summary>
		/// <returns></returns>
		public Dictionary<int, List<BattleTemplate>> GetPlaceholderBattleTemplateDict() {
			return placeholderBattleTemplateDict;
		}
		/// <summary>
		/// Returns the placeholder treasure dictionary in the event one inside of a MissionData cannot be used.
		/// </summary>
		public Dictionary<int, List<BattleBehavior>> GetPlaceholderTreasureDict() {
			return placeholderTreasureDict;
		}
		/// <summary>
		/// Returns the placeholder rewards list in the event one inside of a MissionData cannot be used.
		/// </summary>
		public List<BattleBehavior> GetPlaceholderRewards() {
			return placeholderRewards;
		}
		/// <summary>
		/// Returns a CrawlerDungeonTemplate associated with the specified type provided.
		/// </summary>
		/// <param name="crawlerDungeonIDType"></param>
		/// <returns></returns>
		public CrawlerDungeonTemplate GetCrawlerDungeonTemplate(CrawlerDungeonIDType crawlerDungeonIDType) {
			return this.crawlerDungeonTemplates.First(dt => dt.CrawlerDungeonIDType == crawlerDungeonIDType);
		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// Cleans the allBehaviors list of any empty elements.
		/// </summary>
		[TabGroup("Behaviors", "All Behaviors"), Button, HideInPlayMode]
		private void RemoveEmptyBattleBehaviors() {
			Debug.Log("Removing empty elements from GlobalAssetData's BattleBehavior list.");
			this.allBehaviors = this.allBehaviors.Where(bb => bb != null).ToList();
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
#endif
		}
		#endregion
	}
}