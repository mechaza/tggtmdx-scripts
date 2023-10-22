using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Grawly.Dungeon;
using Grawly.UI;
using Grawly.Overworld;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;
using Grawly.Calendar;
using Grawly.Toggles;
using Grawly.Toggles.Audio;
using Grawly.Toggles.Gameplay;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;

namespace Grawly {
	public class GameController : SerializedMonoBehaviour {
		public static GameController Instance { get; private set; }

		#region FIELDS - DEBUG STATE
		/// <summary>
		/// The amount to multiply calculations by at the very end.
		/// </summary>
		public float DebugFinalAmountMultiplier { get; set; } = 1f;
		/// <summary>
		/// The amount to multiply battle behavior power by before using them in calculations.
		/// </summary>
		public float DebugBasePowerMultiplier { get; set; } = 1f;
		/// <summary>
		/// The amount player attacks should be multiplied by.
		/// </summary>
		public float DebugPlayerAttackMultiplier { get; set; } = 1f;
		/// <summary>
		/// Is saving enabled? This should be turned off when using a preset.
		/// It should also be removed in the final game.
		/// </summary>
		public bool DebugSavingEnabled { get; set; } = true;
		#endregion

		#region FIELDS - PREFABS
		/// <summary>
		/// The prefab that should be instansiated if there is no InputController in the scene.
		/// This is mostly relevant for shit like, debug mode. The input controller usually stays put.
		/// </summary>
		[SerializeField]
		private GameObject inputControllerPrefab;
		#endregion

		#region FIELDS - LONG TERM VARIABLES
		/// <summary>
		/// The variables that contain the things that need to be remembered long term.
		/// </summary>
		public GameVariables Variables { get; private set; }
		/// <summary>
		/// The preset template to use in debug mode.
		/// </summary>
		[SerializeField]
		private GamePresetTemplate debugPresetTemplate;
		/// <summary>
		/// The toggles to use for balancing and junk.
		/// </summary>
		public DifficultyVariablesTemplate DifficultyToggles { get; private set; }
		#endregion

		#region FIELDS - MENUABLE VARIABLES
		/// <summary>
		/// A list of Personas not in use, which can be read by a MenuList.
		/// </summary>
		public List<IMenuable> MenuableUnusedPersonas {
			get {
				// Go through the Personas and return ones which are not in use.
				return this.Variables.Personas.Where(p => p.InUse == false).Cast<IMenuable>().ToList();
			}
		}
		/// <summary>
		/// Similar to the MenuableItems, except just the keys, and in a format a MenuList can interpret.
		/// </summary>
		public List<IMenuable> MenuableItemsList {
			get { return this.Variables.Items.Keys.OrderBy(b => b.PauseFunctions.Count).Cast<IMenuable>().Reverse().ToList(); }
		}
		/// <summary>
		/// A dictionary of items from the player's inventory that have pause menu functionality.
		/// </summary>
		public Dictionary<BattleBehavior, int> MenuableItems {
			get { return Variables.Items.Where(kvp => kvp.Key.PauseFunctions.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
		}
		/// <summary>
		/// The party members, but in a form that can be read by a MenuList.
		/// </summary>
		public List<IMenuable> MenuablePlayers {
			get { return this.Variables.Players.Cast<IMenuable>().Reverse().ToList(); }
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				DontDestroyOnLoad(this.gameObject);

				// DOTween.useSafeMode = true;

				// Check if the current loaded scene is one where I do/do not want to set the variables.
				bool canSetDebugVariables = Application.isEditor == true && SceneManager.GetActiveScene().name != "GameSaveLoader" && SceneManager.GetActiveScene().name != "GamePresetLoader" && SceneManager.GetActiveScene().name != "Initialization";

				// If debug mode is on, just use the debug values.
				if (canSetDebugVariables) {
					Debug.Log("Setting the debug variables on Awake...");
					this.OpenPreset(presetTemplate: this.debugPresetTemplate, loadFirstScene: false);
				}
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			// If the input controller does not exist, instansiate it. This is mostly relevant for debug mode.
			if (InputController.Instance == null) {
				GameObject.Instantiate(this.inputControllerPrefab);
			}

			// If the initialization controller is null...
			if (InitializationControllerVX.Instance == null) {
				// ...run the boot toggles
				ToggleController.ProcessBootToggles();

				// ...and load up the appropriate scenes.
				SceneManager.LoadSceneAsync(sceneName: "Chat Controller DX", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "SettingsMenuDX", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "SocialLinkUI", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "BottomInfoBar", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "Shuffle Time", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "CombatantAnalysisDX", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "BattleResultsControllerDX", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "Badge Grid Screen", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "AllOutAttackDX", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "PrototypeShop", mode: LoadSceneMode.Additive);
			}
		}
		private void Update() {
			if (this.Variables != null) {
				// Update the play time.
				this.Variables.PlayTimeSeconds += Time.deltaTime;
			}
		}
		private void OnDestroy() {
			this.StopAllCoroutines();
		}
		/// <summary>
		/// Gets run when uController has been added to the scene. Only in builds, and only for debug purposes.
		/// </summary>
		/// <param name="obj"></param>
		private void uConsoleSceneLoadComplete(AsyncOperation obj) {
			// Don't let the uConsole get destroyed.
			DontDestroyOnLoad(FindObjectOfType<uConsole.ConsoleCommand>().gameObject);
			obj.completed -= this.uConsoleSceneLoadComplete;
		}
		#endregion

		#region GAME SETUP - PRESETS AND VARIABLES
		[Button]
		private void DeleteSaveCollection() {
			SaveController.DeleteSaveCollection();
		}
		/// <summary>
		/// Sets the difficulty type.
		/// </summary>
		/// <param name="difficultyType"></param>
		public void SetDifficulty(DifficultyType difficultyType) {
			// Check if there is an override type in the settings.
			DifficultyOverrideType overrideType = ToggleController.GetToggle<DifficultyOverride>().GetToggleEnum();
			if (overrideType != DifficultyOverrideType.Off) {
				// Force a conversion of the override type if it is not off. They mostly match anyway.
				Debug.Log("Difficulty Override is set to " + overrideType.ToString() + ". Using override instead of " + difficultyType.ToString());
				this.DifficultyToggles = DataController.Instance.GetDifficultyTemplate((DifficultyType)overrideType);
			} else {
				this.DifficultyToggles = DataController.Instance.GetDifficultyTemplate(difficultyType);
			}

			// Also grab the multipliers.
			this.DebugFinalAmountMultiplier = ToggleController.GetToggle<FinalAmountMultiplier>().GetToggleFloat();
			this.DebugBasePowerMultiplier = ToggleController.GetToggle<BasePowerMultiplier>().GetToggleFloat();
			this.DebugPlayerAttackMultiplier = ToggleController.GetToggle<PlayerAttackScale>().GetToggleFloat();
		}
		/// <summary>
		/// Opens a game preset template and makes it run.
		/// </summary>
		/// <param name="presetTemplate">The preset to open and set the variables for.</param>
		/// <param name="loadFirstScene">Should the first scene be loaded? Useful for when launching from the preset menu.</param>
		public void OpenPreset(GamePresetTemplate presetTemplate, bool loadFirstScene) {
			// Some presets may enable/disable saving. Toggle that now.
			this.DebugSavingEnabled = presetTemplate.AllowSaving;

			// Set the CalendarData.
			CalendarController.Instance.SetCalendarData(calendarData: presetTemplate.CalendarData);

			// Set the variabless
			this.SetVariables(template: presetTemplate.VariablesTemplate);

			// If loading the first scene, do so.
			if (loadFirstScene == true) {
				this.LoadPresetInitialScene(presetTemplate: presetTemplate);
			}
		}
		/// <summary>
		/// Loads the "initial" scene within the game preset.
		/// </summary>
		/// <param name="presetTemplate">The GamePreset containing the scene to load.</param>
		private void LoadPresetInitialScene(GamePresetTemplate presetTemplate) {
			// If the preset is designed to load the first story beat, use that.
			if (presetTemplate.PresetSceneType == GamePresetTemplate.GamePresetSceneType.FirstStoryBeat) {
				CalendarController.Instance.GoToNextStoryBeat(gameVariables: this.Variables);
			} else {
				// If using a custom scene, use that instead.
				SceneController.instance.BasicLoadSceneWithFade(sceneName: presetTemplate.SceneName);
			}
		}
		#endregion

		#region GAME SETUP - GAMEVARIABLES
		/// <summary>
		/// Overrides the GameVariables with a specified game save.
		/// </summary>
		/// <param name="gameSave">The game save to use when loading this shit.</param>
		public void SetVariables(GameSave gameSave) {
			// Grab the template with the same difficulty type.
			this.SetDifficulty(gameSave.difficultyType);
			// Create a new variables from the game save and load that shit.
			this.SetVariables(variables: new GameVariables(gameSave: gameSave));
		}
		/// <summary>
		/// Overrides the GameVariables with a specified template.
		/// Usually will only get called once but I may need this for debugging.
		/// </summary>
		/// <param name="template"></param>
		public void SetVariables(GameVariablesTemplate template) {
			// Grab the template with the same difficulty type.
			this.SetDifficulty(template.DifficultyType);
			// Just call the version of this method that takes a set of variables.
			this.SetVariables(variables: new GameVariables(template: template));
		}
		/// <summary>
		/// Overrides the GameVariables with a new set of variables.
		/// Usually will only get called once but I may need this for debugging.
		/// </summary>
		/// <param name="variables"></param>
		public void SetVariables(GameVariables variables) {
			Debug.Log("OVERRIDING GAME VARIABLES");
			// Grab the template with the same difficulty type.
			// this.DifficultyToggles = DataController.Instance.GetDifficultyTemplate(variables.DifficultyType);
			this.SetDifficulty(variables.DifficultyType);
			// Set the variables.
			this.Variables = variables;
		}
		#endregion

		#region GAME SETUP - PLAYERS
		/// <summary>
		/// Adds player to the players list.
		/// </summary>
		public void AddPlayers(List<PlayerTemplate> templates) {
			Debug.LogError("PLEASE CALL THIS FROM VARIABLES FROM NOW ON THANK YOU");
			this.Variables.AddPlayers(templates: templates);
			// templates.ForEach(pt => this.players.Add(new Player(pt)));
			/*foreach (PlayerTemplate template in templates) {
				players.Add(new Player(template));
			}*/
			// Debug.Log("PLAYER COUNT: " + players.Count);
		}
		/// <summary>
		/// Adds players to the pool of personas.
		/// </summary>
		/// <param name="templates">The persona templates to add.</param>
		public void AddPersonas(List<PersonaTemplate> templates) {
			Debug.LogError("PLEASE CALL THIS FROM VARIABLES FROM NOW ON THANK YOU");
			this.Variables.AddPersonas(templates: templates);
			// templates.ForEach(pt => this.personas.Add(new Persona(pt)));
		}
		#endregion

		#region MODIFIERS
		/// <summary>
		/// Returns all modifiers of the specified type.
		/// This is primarily for modifiers that need to exist outside of the battle but are not part of a combatant.
		/// A prime example of this is shuffle time, which persists beyond battles.
		/// </summary>
		/// <typeparam name="T">The type of modifier to get.</typeparam>
		/// <returns></returns>
		public List<T> GetPersistentModifiers<T>() {
			return this.Variables.ShuffleCardDeck.ShuffleCards // Go through the shuffle cards...
				.SelectMany(c => c.Behaviors) // ... select the behaviors...
				.Where(b => b is T) // ... find the ones that implement the given interface...
				.Cast<T>() // ... cast...
				.ToList(); // ... and return.
		}
		#endregion

		#region LEVELING UP
		/// <summary>
		/// Levels up a list of comabtants with the amount of experience provided.
		/// </summary>
		/// <param name="combatants">The combatants to level up.</param>
		/// <param name="experience">The amount of experience to hand to the combatants.</param>
		/// <returns>The results of the level up operation.</returns>
		public List<CombatantLevelUpResults> LevelUpCombatants(List<Combatant> combatants, int experience) {
			return combatants.Select(c => c.ParseExperience(experience: experience)).ToList();
		}
		#endregion

		#region THINGS FOR MACROS I GUESS
		/// <summary>
		/// Restores the HP/MP of the entire party.
		/// </summary>
		public static void RestoreParty() {
			GameController.Instance.Variables.Players.ForEach(p => {
				p.HP = 999;
				p.MP = 999;
			});
		}
		#endregion

		#region HELPER FUNCTION BC I NEED TO GET THIS SHIT OVER WITH
		/// <summary>
		/// Waits a certain amount of time then runs the specified action.
		/// </summary>
		/// <param name="timeToWait">The amount of time to wait in seconds.</param>
		/// <param name="action">The action to run once the time is up.</param>
		public void WaitThenRun(float timeToWait, Action action) {
			this.StartCoroutine(this.WaitThenRunRoutine(timeToWait: timeToWait, action: action));
		}
		/// <summary>
		/// A quick and dirty function that waits until the end of the frame and then does an action.
		/// </summary>
		/// <param name="action">The action to run at the end of the frame.</param>
		public void RunEndOfFrame(Action action) {
			this.StartCoroutine(this.RunEndOfFrameRoutine(action: action));
		}
		/// <summary>
		/// Waits a certain amount of time then runs the specified action.
		/// </summary>
		/// <param name="timeToWait">The amount of time to wait in seconds.</param>
		/// <param name="action">The action to run once the time is up.</param>
		private IEnumerator WaitThenRunRoutine(float timeToWait, Action action) {
			yield return new WaitForSeconds(seconds: timeToWait);
			action();
		}
		private IEnumerator RunEndOfFrameRoutine(Action action) {
			yield return new WaitForEndOfFrame();
			action();
		}
		#endregion
	}
}