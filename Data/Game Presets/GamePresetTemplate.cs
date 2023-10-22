using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grawly.Calendar;
using Grawly.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.Battle;
using Grawly.Chat;
using Grawly.Friends;
using Grawly.MiniGames.ShuffleTime;
using Grawly.Toggles;
using Grawly.UI.MenuLists;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using System.IO;
using UnityEditor;
#endif

namespace Grawly {
	
	/// <summary>
	/// A template that can be loaded up and run to create a game state.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Presets/Preset Template")]
	[TypeInfoBox("A template that can be loaded up and run to create a game state.")]
	public class GamePresetTemplate : SerializedScriptableObject, IMenuable {

		#region PROPERTIES - PATHS
		/// <summary>
		/// A simple way for me to reference the path this template is inside of.
		/// </summary>
		private string PresetFolderPath {
			get {
#if UNITY_EDITOR
				string currentAssetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
				string currentAssetFolder = currentAssetPath.Replace(Path.GetFileName(currentAssetPath), "").TrimEnd('/');
				return currentAssetFolder;
#endif
				throw new System.Exception("This should never be reached!");
			}
		}
		/// <summary>
		/// A simple way for me to reference the path to the chat scripts.
		/// </summary>
		private string ChatScriptFolderPath {
			get {
				return this.PresetFolderPath + "/Chat Scripts";
			}
		}
		/// <summary>
		/// A simple way for me to reference the path to the battle templates
		/// </summary>
		private string BattleTemplateFolderPath {
			get {
				return this.PresetFolderPath + "/Battle Templates";
			}
		}
		/// <summary>
		/// Is there a dedicated folder that stores chat scripts for this preset?
		/// </summary>
		public bool HasChatScriptsFolder {
			get {
#if UNITY_EDITOR
				// Confirm that the chat script folder actually exists.
				bool folderExists = AssetDatabase.IsValidFolder(path: ChatScriptFolderPath);
				return folderExists;
#endif
				throw new System.Exception("This should never be reached!");
			}
		}
		/// <summary>
		/// Is there a dedicated folder that stores battle templates for this preset?
		/// </summary>
		public bool HasBattleTemplatesFolder {
			get {
#if UNITY_EDITOR
				// Confirm that the chat script folder actually exists.
				bool folderExists = AssetDatabase.IsValidFolder(path: BattleTemplateFolderPath);
				return folderExists;
#endif
				throw new System.Exception("This should never be reached!");
			}
		}
		#endregion
		
		#region FIELDS - METADATA
		/// <summary>
		/// The name of this game preset.
		/// </summary>
		[Title("Metadata")]
		[OdinSerialize, TabGroup("Preset","Settings")]
		public string PresetName { get; private set; } = "";
		/// <summary>
		/// The description for this preset.
		/// </summary>
		[OdinSerialize, TabGroup("Preset","Settings")]
		public string PresetDescription { get; private set; } = "";
		/// <summary>
		/// A screenshot to use as a preview.
		/// </summary>
		[OdinSerialize, TabGroup("Preset","Settings")]
		public Sprite PreviewScreenshot { get; private set; }
		/// <summary>
		/// The color to use when highlighting this preset.
		/// Mostly for visual flair in the menus.
		/// </summary>
		[OdinSerialize, TabGroup("Preset", "Settings")]
		public GrawlyColorTypes ColorType = GrawlyColorTypes.Red;
		#endregion
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// Is saving allowed with this preset?
		/// Sometimes saving might break a preset so its worth having this in here.
		/// </summary>
		[Title("Toggles")]
		[OdinSerialize, ShowInInspector, TabGroup("Preset", "Settings")]
		[PropertyTooltip("Should saving be allowed for this preset?")]
		public bool AllowSaving { get; private set; } = false;
		/// <summary>
		/// The 'mode' to figure out what scene to load.
		/// </summary>
		[OdinSerialize, TabGroup("Preset","Settings")]
		[PropertyTooltip("The 'mode' to load a scene from on start up. You can either use an explicit scene, or a story beat.")]
		public GamePresetSceneType PresetSceneType { get; private set; } = GamePresetSceneType.SceneName;
		/// <summary>
		/// The scene to load when selecting this preset.
		/// </summary>
		[OdinSerialize, TabGroup("Preset","Settings"), HideIf("UseFirstStoryBeat")]
		[PropertyTooltip("The name of the scene to load upon starting this preset, if not using the first story beat.")]
		private string CustomSceneName { get; set; } = "";
		/// <summary>
		/// The game variables template to use for instansiating this.
		/// </summary>
		[OdinSerialize, TabGroup("Preset","Settings")]
		[PropertyTooltip("The GameVariablesTemplate to use for preparing this preset.")]
		public GameVariablesTemplate VariablesTemplate { get; private set; }
		/// <summary>
		/// The calendar data to use for this preset.
		/// </summary>
		[OdinSerialize, TabGroup("Preset","Settings")]
		[PropertyTooltip("The CalendarData for this preset. Contains information like what scenes to load on what days.")]
		public CalendarData CalendarData { get; private set; }
		/// <summary>
		/// The toggles that contain the settings to use by default for this preset.
		/// </summary>
		[OdinSerialize, TabGroup("Preset","Settings")]
		[PropertyTooltip("Contains the settings to use on this preset by default.")]
		public GameTogglesTemplateDX GameTogglesTemplate { get; private set; }
		#endregion

		#region PROPERTIES
		/// <summary>
		/// The color to use for this template when building it.
		/// </summary>
		public Color PrimaryColor {
			get {
				return GrawlyColors.colorDict[GrawlyColorTypes.Red];
			}
		}
		/// <summary>
		/// The name of the scene to load for this preset.
		/// </summary>
		public string SceneName {
			get {
				
				// If using a custom scene, return that.
				if (this.PresetSceneType == GamePresetSceneType.SceneName) {
					return this.CustomSceneName;
				} else {
					throw new System.Exception("This shouldn't be referenced when the scene type is read from a story beat!");
				}
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public string PrimaryString => this.PresetName;
		public string QuantityString => "";
		public string DescriptionString  => this.PresetDescription;
		public Sprite Icon  => this.PreviewScreenshot;
		#endregion

		#region GETTERS - SPECIFIC
		/// <summary>
		/// Gets all of the chat scripts associated with the provided day number.
		/// </summary>
		/// <param name="dayNumber">The number of the day to retrieve the chat scripts for.</param>
		/// <returns></returns>
		public List<SerializedChatScriptDX> GetAllSerializedChatScripts(int dayNumber) {
			
			// Generate the string of the folder to target for this day number.
			string targetFolder = this.ChatScriptFolderPath + "/Day " + dayNumber;

			// Get all the serialized scripts for this day's folder.
			List<SerializedChatScriptDX> scriptsForDay = this.GetAllAssets<SerializedChatScriptDX>(targetFolder: targetFolder);

			return scriptsForDay;

		}
		/// <summary>
		/// Gets all of the chat scripts associated with the provided day number.
		/// </summary>
		/// <param name="dayNumber">The number of the day to retrieve the chat scripts for.</param>
		/// <returns></returns>
		public List<SimpleChatScriptDX> GetAllSimpleChatScripts(int dayNumber) {
			
			// Generate the string of the folder to target for this day number.
			string targetFolder = this.ChatScriptFolderPath + "/Day " + dayNumber;

			// Get all the serialized scripts for this day's folder.
			List<SimpleChatScriptDX> scriptsForDay = this.GetAllAssets<SimpleChatScriptDX>(targetFolder: targetFolder);

			return scriptsForDay;

		}
		#endregion

		#region GETTERS - GENERIC
		/// <summary>
		/// Returns all of the assets of a specific type.
		/// </summary>
		/// <param name="targetFolder">The file path to look inside of.</param>
		/// <param name="safeMode">If safe mode is off, an exception will be thrown if no folder exists..</param>
		/// <typeparam name="T">The type of asset to retrieve.</typeparam>
		/// <returns></returns>
		private List<T> GetAllAssets<T>(string targetFolder, bool safeMode = true) where T : Object {
#if UNITY_EDITOR
			
			// Just make sure the folder actually exists.
			bool folderExists = AssetDatabase.IsValidFolder(path: targetFolder);
			if (folderExists == false) {
				if (safeMode == true) {
					return new List<T>();
				} else {
					throw new System.Exception("Path for " + targetFolder + " does not exist!");
				}
				
			}

			// https://answers.unity.com/questions/486545/getting-all-assets-of-the-specified-type.html
			List<T> assets = new List<T>();
			string[] guids = AssetDatabase.FindAssets(string.Format(format: "t:{0}", typeof(T)), searchInFolders: new string[]{ targetFolder});
			for( int i = 0; i < guids.Length; i++ ) {
				string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
				T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
				if( asset != null ){
					assets.Add(asset);
				}
			}
			return assets;
			
			/* List<T> assets = new List<T>();
     string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
     for( int i = 0; i < guids.Length; i++ )
     {
         string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
         T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
         if( asset != null )
         {
             assets.Add(asset);
         }
     }
     return assets;*/
			
			// Load up all the assets at the target folder and only retrieve the ones that exist.
			/*List<T> targetAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath: targetFolder)
				.Where(a => a is T)
				.Cast<T>()
				.ToList();*/

			// Return them.
			
#endif
			throw new Exception("This should never be reached!");
		}
		#endregion
		
		#region ODIN HELPERS - GENERATION
		/// <summary>
		/// Generates the assets required for a new preset and ensures they're all segregated from the rest of the project.
		/// </summary>
		[Button(ButtonStyle.Box), TabGroup("Preset", "Generation")]
		private void GeneratePreset() {
			// I'm baby?
			this.GeneratePreset(
				duplicateBattleBehaviors:false, 
				createScene:true, 
				addToPresetReference: true);
		}
		/// <summary>
		/// Generates the assets required for a new preset and ensures they're all segregated from the rest of the project.
		/// </summary>
		/// <param name="duplicateBattleBehaviors">Should any used BattleBehaviors be cloned specifically for this preset?</param>
		/// <param name="createScene">Should a scene be created specifically for this template?</param>
		/// <param name="addToPresetReference">Should this preset be added to the preset reference?</param>
		private void GeneratePreset(bool duplicateBattleBehaviors, bool createScene, bool addToPresetReference) {
			
#if UNITY_EDITOR
			
			if (this.PresetName == null || this.PresetName == "") {
				Debug.LogError("This template needs a PresetName before it can generate the required assets!");
				return;
			}
			
			// Create paths for everything needed. This takes a bit of work.
			string currentAssetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
			string currentAssetFolder = currentAssetPath.Replace(Path.GetFileName(currentAssetPath), "").TrimEnd('/');
			string targetFolderGUID = AssetDatabase.CreateFolder(parentFolder: currentAssetFolder, newFolderName: this.PresetName);
			string targetFolderPath = AssetDatabase.GUIDToAssetPath(targetFolderGUID);
			
			// Copy over the default templates so they can be used with this preset.
			GameVariablesTemplate newVariablesTemplate = this.CopyAsset<GameVariablesTemplate>(
				sourceAsset: DataController.GetDefaultGameVariablesTemplate(), 
				targetFolder: targetFolderPath, 
				fileName: this.PresetName + " Variables Template.asset");
			CalendarData newCalendarData = this.CopyAsset<CalendarData>(
				sourceAsset: DataController.GetDefaultCalendarData(), 
				targetFolder: targetFolderPath, 
				fileName: this.PresetName + " CalendarData.asset");
			List<CalendarDayTemplate> newCalendarDayTemplates = this.CopyAssetsWithIndices<CalendarDayTemplate>(
				sourceAssets: newCalendarData.CalendarDayTemplates,
				parentFolder: targetFolderPath, 
				subFolder: "Calendar Day Templates", 
				fileName: this.PresetName + " Day ")
				.ToList();
			GameTogglesTemplateDX newGameTogglesTemplate = this.CopyAsset<GameTogglesTemplateDX>(
				sourceAsset: DataController.GetDefaultGameTogglesTemplate(),
				targetFolder: targetFolderPath, 
				fileName: this.PresetName + " GameTogglesTemplate.asset");
			
			List<PlayerTemplate> newPlayerTemplates = this.CopyAssets<PlayerTemplate>(
				sourceAssets: newVariablesTemplate.playerTemplates,
				parentFolder: targetFolderPath, 
				subFolder: "Player Templates",
				suffixString: this.PresetName);
			IEnumerable<PersonaTemplate> oldPersonaTemplates = newVariablesTemplate.personaTemplates
				.Concat(newVariablesTemplate.playerTemplates.Select(pt => pt.personaTemplate));
			List<PersonaTemplate> newPersonaTemplates = this.CopyAssets<PersonaTemplate>(
				sourceAssets: oldPersonaTemplates, 
				parentFolder: targetFolderPath, 
				subFolder: "Persona Templates", 
				suffixString: this.PresetName);
			List<FriendTemplate> newFriendTemplates =  this.CopyAssets<FriendTemplate>(
					sourceAssets: newVariablesTemplate.FriendTemplates, 
					parentFolder: targetFolderPath,
					subFolder: "Friend Templates",
					suffixString: this.PresetName);
			List<ShuffleCardTemplate> newShuffleCardTemplates =  this.CopyAssets<ShuffleCardTemplate>(
				sourceAssets: newVariablesTemplate.shuffleCardTemplates, 
				parentFolder: targetFolderPath,
				subFolder: "Shuffle Card Templates",
				suffixString: this.PresetName);

			// Give the new players references to their cloned persona templates.
			foreach (PlayerTemplate newPlayerTemplate in newPlayerTemplates) {
				// Find the new persona template that matches the name of the old one.
				// Note that the new player template should still actually have a reference to the old one,
				// so this should work.
				PersonaTemplate newPersonaTemplate = newPersonaTemplates
					.First(t => t.metaData.name == newPlayerTemplate.personaTemplate.metaData.name);
				newPlayerTemplate.personaTemplate = newPersonaTemplate;
			}

			if (duplicateBattleBehaviors == true) {
				throw new System.Exception("I don't want to duplicate behaviors anymore.");
			}
			
			if (duplicateBattleBehaviors == true) {
				List<BattleBehavior> oldBattleBehaviors = new List<BattleBehavior>();
				oldBattleBehaviors = oldBattleBehaviors
					.Concat(newVariablesTemplate.playerTemplates.SelectMany(t => t.battleBehaviors))
					.Concat(newVariablesTemplate.personaTemplates.SelectMany(t => t.battleBehaviors))
					.Concat(newVariablesTemplate.personaTemplates.Select(t => t.standardAttackBehavior ?? DataController.GetDefaultBattleBehavior("Attack")))
					.Concat(newVariablesTemplate.playerTemplates.Select(t => t.personaTemplate).SelectMany(t => t.battleBehaviors))
					.Concat(newVariablesTemplate.playerTemplates.Select(t => t.personaTemplate).Select(t => t.standardAttackBehavior ?? DataController.GetDefaultBattleBehavior("Attack")))
					.Concat(newVariablesTemplate.items)
					.Where(bb => bb != null)
					.Distinct()
					.ToList();
				
				List<BattleBehavior> newBattleBehaviors = this.CopyAssets<BattleBehavior>(
					sourceAssets: oldBattleBehaviors, 
					parentFolder: targetFolderPath, 
					subFolder: "Battle Behaviors", 
					suffixString: this.PresetName);
				
				List<CombatantTemplate> allNewCombatantTemplates = newPlayerTemplates
					.Cast<CombatantTemplate>()
					.Concat(newPersonaTemplates)
					.ToList();
				foreach (CombatantTemplate combatantTemplate in allNewCombatantTemplates) {
					combatantTemplate.battleBehaviors = combatantTemplate.battleBehaviors
						.Select(bb => bb.behaviorName)
						.Select(bn => newBattleBehaviors.First(nbb => nbb.behaviorName == bn))
						.ToList();
					EditorUtility.SetDirty(target: combatantTemplate);
				}
				newVariablesTemplate.items = newVariablesTemplate.items
					.Select(bb => bb.behaviorName)
					.Select(bn => newBattleBehaviors.First(nbb => nbb.behaviorName == bn))
					.ToList();
			}
			
			// Create a series of folders to store dialogue, then make a new folder for each calendar day.
			string chatDialogueFolderGUID = AssetDatabase.CreateFolder(parentFolder: targetFolderPath, newFolderName: "Chat Scripts");
			string chatDialogueFolderPath = AssetDatabase.GUIDToAssetPath(chatDialogueFolderGUID).TrimEnd('/');
			foreach (CalendarDayTemplate calendarDayTemplate in newCalendarDayTemplates) {
				string subFolderName = "Day " + calendarDayTemplate.dayNumber;
				AssetDatabase.CreateFolder(parentFolder: chatDialogueFolderPath, newFolderName: subFolderName);
			}
			
			// Now assign everything.
			newVariablesTemplate.playerTemplates = newPlayerTemplates;
			newVariablesTemplate.personaTemplates = newPersonaTemplates.Except(newPlayerTemplates.Select(pt => pt.personaTemplate)).ToList();
			newVariablesTemplate.FriendTemplates = newFriendTemplates;
			newVariablesTemplate.shuffleCardTemplates = newShuffleCardTemplates;
			newCalendarData.CalendarDayTemplates = newCalendarDayTemplates;
			this.VariablesTemplate = newVariablesTemplate;
			this.CalendarData = newCalendarData;
			this.GameTogglesTemplate = newGameTogglesTemplate;
			
			// This is bad state management but create a variable that can exist out of the scope of this if-statement.
			string destinationScenePath = "";
			// If a scene should be created for this preset, also do that.
			if (createScene == true) {
				
				// Create a new folder for the scenes and grab its path.
				string sceneFolderGUID = AssetDatabase.CreateFolder(parentFolder: currentAssetFolder, newFolderName: "_Scenes");
				string sceneFolderPath = AssetDatabase.GUIDToAssetPath(sceneFolderGUID);
				
				// Get the path of the boilerplate scene and load that scene up.
				string boilerplateScenePath = "Assets/_TGGTMDX/Resources/Game Presets/Boilerplate Scene.unity";
				Scene boilerplateScene = EditorSceneManager.OpenScene(scenePath: boilerplateScenePath, mode: OpenSceneMode.Single);
				// Figure out what to name the file and also OVERWRITE THE DESTINATION SCENE PATH VARIABLE.
				string fileName = this.PresetName + " Generated Scene";
				destinationScenePath = sceneFolderPath + "/" + fileName + ".unity";	
				bool saveOK = EditorSceneManager.SaveScene(scene: boilerplateScene, dstScenePath: destinationScenePath);
				// destinationScenePath = targetFolderPath + "/" + fileName + ".unity";
				// bool saveOK = EditorSceneManager.SaveScene(scene: boilerplateScene, dstScenePath: targetFolderPath + "/" + fileName + ".unity");
				if (saveOK == true) {
					Debug.Log("Scene generated successfully.");
					this.PresetSceneType = GamePresetSceneType.SceneName;
					this.CustomSceneName = fileName;
				} else {
					Debug.LogError("Could not generate new scene for preset!");
				}
			}
			
			EditorUtility.SetDirty(newVariablesTemplate);
			EditorUtility.SetDirty(newCalendarData);
			EditorUtility.SetDirty(this);

			// If adding this preset to the reference, do so (but also make sure you add the scene path if that was made too.)
			if (addToPresetReference == true) {
				GamePresetTemplateReference templateReference = DataController.GetGamePresetTemplateReference();
				if (createScene == true) {
					templateReference.AddPresetTemplate(presetTemplate: this, scenePath: destinationScenePath);
				} else {
					templateReference.AddPresetTemplate(presetTemplate: this);
				}
				EditorUtility.SetDirty(templateReference);
			}
			
			AssetDatabase.SaveAssets();
			AssetDatabase.MoveAsset(oldPath: currentAssetPath, newPath: targetFolderPath + "/" + this.PresetName + " Preset.asset");
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = this;

#endif

		}
		#endregion

		#region ODIN HELPERS - COPYING (MODE 1)
		/// <summary>
		/// Copies a collection of assets and returns the copies.
		/// Uses the name of the source objects themselves.
		/// </summary>
		/// <param name="sourceAssets">The assets to copy.</param>
		/// <param name="parentFolder">The folder containing the subfolder.</param>
		/// <param name="subFolder">The sub folder to combine with the target folder.</param>
		/// <param name="suffixString">The string to prepend to the file name.</param>
		/// <typeparam name="T">The type of asset to clone.</typeparam>
		/// <returns>The actual cloned assets.</returns>
		private List<T> CopyAssets<T>(IEnumerable<T> sourceAssets, string parentFolder, string subFolder, string suffixString) where T : UnityEngine.Object {
			return sourceAssets
				.Select(a =>
					this.CopyAsset<T>(
						sourceAsset: a, 
						parentFolder: parentFolder, 
						subFolder: subFolder,
						fileName: a.name + "(" + suffixString + ").asset"))
				.ToList();
		}
		/// <summary>
		/// Copies an asset and returns its copy.
		/// </summary>
		/// <param name="sourceAsset">The asset to copy.</param>
		/// <param name="targetFolder">The folder containing the subfolder.</param>
		/// <param name="fileName">The name to give the file.</param>
		/// <typeparam name="T">The type of asset to clone.</typeparam>
		/// <returns>The actual cloned asset.</returns>
		private T CopyAsset<T>(T sourceAsset, string targetFolder, string fileName) where T : UnityEngine.Object {
#if UNITY_EDITOR
			string sourcePath = AssetDatabase.GetAssetPath(sourceAsset);
			return this.CopyAsset<T>(sourcePath: sourcePath, targetFolder: targetFolder, fileName: fileName);
#endif
			throw new Exception("This should never be reached!");
		}
		/// <summary>
		/// Copies an asset at the given source path.
		/// </summary>
		/// <param name="sourcePath">The source of the object to copy.</param>
		/// <param name="targetFolder">The folder to add the asset to.</param>
		/// <param name="fileName">The name to give the file.</param>
		/// <typeparam name="T">The type of asset to clone.</typeparam>
		/// <returns>The actual cloned asset.</returns>
		private T CopyAsset<T>(string sourcePath, string targetFolder, string fileName) where T : UnityEngine.Object {
#if UNITY_EDITOR
			// I'm not doing error handling here because of how AssetDatabase.CreateFolder is set up. I don't care.
			string targetPath = Path.Combine(targetFolder, fileName);
			bool copyOK = AssetDatabase.CopyAsset(path: sourcePath, newPath: targetPath);
			if (copyOK == true) {
				return AssetDatabase.LoadAssetAtPath<T>(assetPath: targetPath);
			} else {
				throw new Exception("The copy operation failed!");
			}
#endif
			throw new Exception("This should never be reached!");
		}
		#endregion
		
		#region ODIN HELPERS - COPYING (MODE 2)
		/// <summary>
		/// Copies an asset and returns its copy.
		/// </summary>
		/// <param name="sourceAsset">The asset to copy.</param>
		/// <param name="parentFolder">The folder containing the subfolder.</param>
		/// <param name="subFolder">The sub folder to combine with the target folder.</param>
		/// <param name="fileName">The name to give the file.</param>
		/// <typeparam name="T">The type of asset to clone.</typeparam>
		/// <returns>The actual cloned asset.</returns>
		private T CopyAsset<T>(T sourceAsset, string parentFolder, string subFolder, string fileName) where T : UnityEngine.Object {
#if UNITY_EDITOR
			string sourcePath = AssetDatabase.GetAssetPath(sourceAsset);
			return this.CopyAsset<T>(sourcePath: sourcePath, parentFolder: parentFolder, subFolder: subFolder, fileName: fileName);
#endif
			throw new Exception("This should never be reached!");
		}
		/// <summary>
		/// Copies an asset at the given source path.
		/// </summary>
		/// <param name="sourcePath">The source of the object to copy.</param>
		/// <param name="parentFolder">The folder containing the subfolder.</param>
		/// <param name="subFolder">The sub folder to combine with the target folder.</param>
		/// <param name="fileName">The name to give the file.</param>
		/// <typeparam name="T">The type of asset to clone.</typeparam>
		/// <returns>The actual cloned asset.</returns>
		private T CopyAsset<T>(string sourcePath, string parentFolder, string subFolder, string fileName) where T : UnityEngine.Object {
#if UNITY_EDITOR
			
			// Generate the paths for the target and its folder.
			string targetFolder = Path.Combine(parentFolder, subFolder);
			string targetPath = Path.Combine(targetFolder, fileName);
			
			// Verify that the folder exists, and create it if needed.
			bool folderExists = AssetDatabase.IsValidFolder(path: targetFolder);
			if (folderExists == false) {
				AssetDatabase.CreateFolder(parentFolder: parentFolder, newFolderName: subFolder);
			}
			
			// Copy the asset, then load it back and return it.
			bool copyOK = AssetDatabase.CopyAsset(path: sourcePath, newPath: targetPath);
			if (copyOK == true) {
				return AssetDatabase.LoadAssetAtPath<T>(assetPath: targetPath);
			} else {
				throw new Exception("The copy operation failed!");
			}
			
			
#endif
			throw new Exception("This should never be reached!");
		}
		/// <summary>
		/// Copies a collection of assets and returns the copies.
		/// This version will also index them.
		/// </summary>
		/// <param name="sourceAssets">The assets to copy.</param>
		/// <param name="parentFolder">The folder containing the subfolder.</param>
		/// <param name="subFolder">The sub folder to combine with the target folder.</param>
		/// <param name="fileName">The name of the file to use.</param>
		/// <typeparam name="T">The type of asset to clone.</typeparam>
		/// <returns>The actual cloned assets.</returns>
		private List<T> CopyAssetsWithIndices<T>(IEnumerable<T> sourceAssets, string parentFolder, string subFolder, string fileName) where T : UnityEngine.Object {
			// Create a list so I can actually index these things.
			List<T> sourceAssetList = sourceAssets.ToList();
			return Enumerable.Range(0, sourceAssetList.Count)
				.Select(i => (sourceAssetList[i], i))
				.Select(t => 
					this.CopyAsset<T>(
						sourceAsset: t.Item1, 
						parentFolder: parentFolder, 
						subFolder: subFolder, 
						fileName: fileName + " " + t.i.ToString() + ".asset"))
				.ToList();
		}
		#endregion
		
		#region ODIN HELPERS - CONDITIONS
		/// <summary>
		/// A general check to see if this preset uses the first story beat in a calendar.
		/// </summary>
		/// <returns></returns>
		private bool UseFirstStoryBeat() {
			return this.PresetSceneType == GamePresetSceneType.FirstStoryBeat;
		}
		#endregion
		
		#region ENUM DEFINITIONS
		/// <summary>
		/// The way the scene should be loaded for this game preset.
		/// </summary>
		public enum GamePresetSceneType {
			SceneName		= 0,		// Use a custom scene name.
			FirstStoryBeat	= 1,		// Pick out the name of the scene in the first story beat.
		}
		#endregion

		
	}
	
}