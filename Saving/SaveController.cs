using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using Grawly.Toggles;
using System;
using Sirenix.OdinInspector;

namespace Grawly {
	public class SaveController {
		#region FIELDS - STATE : GAMESAVE
		/// <summary>
		/// The save that is going to be loaded once the player selects it.
		/// I need this as a static field because the delegate being passed to the UnityEngine scene manager
		/// needs access to it somehow. Don't worry much about it. This save will be null 99% of the time.
		/// </summary>
		private static GameSave saveToLoad;
		/// <summary>
		/// The slot number of the save to load.
		/// </summary>
		private static int slotToLoad;
		/// <summary>
		/// The template that is going to be loaded for new saves. I just need a place to store it.
		/// </summary>
		private static GameVariablesTemplate templateToLoad;
		/// <summary>
		/// A callback to run when the save is loaded.
		/// </summary>
		private static Action onSaveLoadedCallback;
		#endregion

		#region FIELDS - STATE : TOGGLE CONFIGURATION
		/// <summary>
		/// The currently cached GameToggleSet.
		/// </summary>
		private static GameToggleSetDX currentGameToggleSet;
		/// <summary>
		/// The current GameToggleSet that is cached.
		/// SHOULD NOT BE USED WITH THE SETTINGS MENU!
		/// THIS IS ONLY FOR THE ABILITY TO GRAB VALUES FROM THE TOGGLES!
		/// </summary>
		public static GameToggleSetDX CurrentGameToggleSet {
			get {
				// If the current set is null, load the one from the save.
				if (currentGameToggleSet == null) {
					currentGameToggleSet = LoadCollection().gameToggleSet;
					// Add any toggles that I may have newly defined but aren't part of the save.
					// Note that this is a bit risky and I'm mostly doing it for debug reasons.
					currentGameToggleSet.AddToggles(DataController.GetDefaultGameTogglesTemplate().GenerateSet().GameToggles);
				}

				// Again, I assign this reference a new set every time the config is saved in SaveConfig.
				return currentGameToggleSet;
			}
		}
		/// <summary>
		/// Whether or not the GameToggleSet has been initialized.
		/// </summary>
		public static bool GameToggleSetHasInitialized {
			get {
				// Just check if the file exists at all.
				return File.Exists(Application.persistentDataPath + "/savedGames.gd");
			}
		}
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// Is there save data to load?
		/// </summary>
		public static bool HasSaveFile {
			get { return LoadCollection().HasAnySaveData; }
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Initializes the save controller so it's ready to provide acess to save data as needed.'
		/// </summary>
		public static void PrepareSaveController() {
			// This is so funny but basically I'm just calling CurrentToggleSet which handles this anyway.
			GameToggleSetDX set = CurrentGameToggleSet;
		}
		public static void DeleteSaveCollection() {
			File.Delete(Application.persistentDataPath + "/savedGames.gd");
		}
		/// <summary>
		/// Creates a database of 20 blank save files.
		/// </summary>
		/// <returns></returns>
		public static GameSaveCollection CreateSaveCollection() {
			Debug.Log("CLEARING OUT THE GAME SAVE COLLECTION!");
			// Create a new GameSaveCollection.
			GameSaveCollection gameSaveCollection = new GameSaveCollection();
			// Create a file and write it out.
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
			bf.Serialize(file, gameSaveCollection);
			file.Close();
			// Return the newly created saves that just had their contents written out.
			return gameSaveCollection;
		}
		#endregion

		#region SAVING
		/// <summary>
		/// Saves the configuration of the game.
		/// </summary>
		/// <param name="gameToggleSet">The toggles to save.</param>
		/// <returns></returns>
		public static FileStream WriteConfig(GameToggleSetDX gameToggleSet) {
			Debug.Log("Saving game configuration.");

			// Load up the collection.
			GameSaveCollection gameSaveCollection = LoadCollection();

			// Save the config there.
			gameSaveCollection.SaveConfig(gameToggleSet: gameToggleSet);

			// Also save it here for cache purposes.
			SaveController.currentGameToggleSet = gameToggleSet;

			// Call the function that actually saves things out.
			return WriteCollection(gameSaveCollection: gameSaveCollection);
		}
		/// <summary>
		/// Saves the current game variables to a file.
		/// </summary>
		/// <param name="variables">The variables to save out.</param>
		/// <param name="incrementSaveCount">Should the save count be incremented?</param>
		/// <param name="slot">The slot to save the variables to.</param>
		public static FileStream SaveGameVariables(GameVariables variables, bool incrementSaveCount, int slot) {
			Debug.Log("Saving variables to slot " + slot);
			
			// Load up all the game saves that are currently stored in the file.
			GameSaveCollection gameSaveCollection = LoadCollection();

			// If flaged, make sure to increment the save count.
			if (incrementSaveCount == true) {
				variables.SaveCount += 1;
			}
			
			// Store the desired game save into the designated slot.
			gameSaveCollection.Save(gameVariables: variables, slot: slot);
			
			// Now just save the collection. Returns the file stream to know when saving is done.
			return WriteCollection(gameSaveCollection: gameSaveCollection);
		}
		/// <summary>
		/// Saves the GameSaveCollection, which is basically all of the data the user needs to remember.
		/// </summary>
		/// <param name="gameSaveCollection"></param>
		public static FileStream WriteCollection(GameSaveCollection gameSaveCollection) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
			bf.Serialize(file, gameSaveCollection);
			file.Close();
			// Return the file stream so I know when its done saving.
			return file;
		}
		#endregion

		#region COLLECTION LOADING
		/// <summary>
		/// Loads all saved game files.
		/// </summary>
		public static GameSaveCollection LoadCollection() {
			
			// If the file exists, load it up.
			if (SaveController.GameToggleSetHasInitialized == true) {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
				GameSaveCollection gameSaveCollection;
				try {
					// Note that if the file is corrupt, loading will fail. Make sure it can be deserialized properly.
					gameSaveCollection = (GameSaveCollection) bf.Deserialize(file);
					file.Close();
				} catch (System.Exception e) {
					// If something went wrong, I just need to create a new collection.
					Debug.LogError("Couldn't read file! Remaking it now.");
					file.Close();
					gameSaveCollection = CreateSaveCollection();
				}

				return gameSaveCollection;
			} else {
				Debug.LogError("Couldn't load game save collection! Making one now.");
				return CreateSaveCollection();
				// throw new System.Exception("Couldn't load saved games!");
			}
			
			/*if (File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
				GameSaveCollection gameSaveCollection;
				try {
					// Note that if the file is corrupt, loading will fail. Make sure it can be deserialized properly.
					gameSaveCollection = (GameSaveCollection) bf.Deserialize(file);
					file.Close();
				} catch (System.Exception e) {
					// If something went wrong, I just need to create a new collection.
					Debug.LogError("Couldn't read file! Remaking it now.");
					file.Close();
					gameSaveCollection = CreateSaveCollection();
				}

				return gameSaveCollection;
			} else {
				Debug.LogError("Couldn't load game save collection! Making one now.");
				return CreateSaveCollection();
				// throw new System.Exception("Couldn't load saved games!");
			}*/
		}
		#endregion

		#region SAVE LOADING
		/// <summary>
		/// Load the save of the last played save file.
		/// </summary>
		/// <param name="incrementLoadCount">Should the load count on this game save be incremented?</param>
		/// <param name="onSaveLoaded">A callback to run when the load is finished.</param>
		public static void LoadLastSave(bool incrementLoadCount, Action onSaveLoaded) {

			// Load up the save collection.
			GameSaveCollection gameSaveCollection = LoadCollection();
			
			// Just grab the last save from the collection and use it to load accordingly.
			LoadGameSave(
				gameSave: gameSaveCollection.LastSave,
				slot: gameSaveCollection.LastSaveSlot,
				incrementLoadCount: incrementLoadCount,
				onSaveLoaded: onSaveLoaded);
			
		}
		/// <summary>
		/// Loads up a game save.
		/// </summary>
		/// <param name="gameSave">The game save to load up.</param>
		/// <param name="slot">The slot the GameSave belongs to. Important for updating load count.</param>
		/// <param name="incrementLoadCount">Should the load count on this game save be incremented?</param>
		public static void LoadGameSave(GameSave gameSave, int slot, bool incrementLoadCount) {
			// Call the callback version of this function with the blank callback.
			LoadGameSave(
				gameSave: gameSave, 
				slot: slot,
				incrementLoadCount: incrementLoadCount,
				onSaveLoaded: delegate { });
		}
		/// <summary>
		/// Loads up a GameSave.
		/// </summary>
		/// <param name="gameSave">The GameSave to load.</param>
		/// <param name="slot">The slot the GameSave belongs to. Important for updating load count.</param>
		/// <param name="incrementLoadCount">Should the load count on this game save be incremented?</param>
		/// <param name="onSaveLoaded">A callback to run when the load is finished.</param>
		public static void LoadGameSave(GameSave gameSave, int slot, bool incrementLoadCount, Action onSaveLoaded) {

			// If flagged to increment the load count, do so.
			if (incrementLoadCount == true) {
				Debug.Log("Incrementing load count on slot " + slot + "(" + gameSave.saveCount + " Saves, " + gameSave.loadCount + " Loads)");
				// Load the collection, increment the associated slot, then save it back out.
				GameSaveCollection saveCollection = SaveController.LoadCollection();
				saveCollection.IncrementGameSaveLoadCount(slot: slot);
				WriteCollection(gameSaveCollection: saveCollection);
				// This is a little redundant but I need to do it
				// because the GameSave here may get out of sync with the collection.
				gameSave.loadCount += 1;
				Debug.Log("Finished incrementing load count on slot " + slot + "(" + gameSave.saveCount + " Saves, " + gameSave.loadCount + " Loads)");
			}
			
			// Keep track of the save that needs to be loaded.
			SaveController.saveToLoad = gameSave;
			SaveController.slotToLoad = slot;
			// Also save the callback.
			SaveController.onSaveLoadedCallback = onSaveLoaded;

			// Add a callback to the scene manager that will then call the GameController.
			SceneManager.sceneLoaded += OnGameSaveLoaderLoadSave;

			// Load up the GameSaveLoader. If I can use the scene controller, use that, but if not, use the regular Unity method for doing so.
			if (SceneController.instance != null) {
				SceneController.instance.LoadScene("GameSaveLoader");
			} else {
				SceneManager.LoadScene("GameSaveLoader");
			}
		}
		/// <summary>
		/// This is one hell of a name for a method but tldr this is what runs when the GameSaveLoader scene... loads up.
		/// </summary>
		private static void OnGameSaveLoaderLoadSave(Scene arg0, LoadSceneMode arg1) {
			
			// Tell the game controller to make variables from the game save.
			GameController.Instance.SetVariables(gameSave: SaveController.saveToLoad);

			// Tell the game save loader controller the location to load.
			GameSaveLoaderController.sceneToLoad = SaveController.saveToLoad.DefaultSceneToLoad;
			GameSaveLoaderController.onSceneLoaded = onSaveLoadedCallback;

			// Null out the save, because I never want it to hold a value when it doesn't need to.
			SaveController.saveToLoad = null;
			SaveController.onSaveLoadedCallback = null;

			// Tell the FSM of the loader that we did it.
			GameSaveLoaderController.instance.GetComponent<PlayMakerFSM>().SendEvent("LOADED");

			// Remove this event from the scene loaded event.
			SceneManager.sceneLoaded -= OnGameSaveLoaderLoadSave;
		}
		#endregion

		#region NEW GAME
		/// <summary>
		/// Starts up a new game with the specified variables template.
		/// Theoretically this doesn't actually need to be here but I'm keeping it here for the sake of convinience + easy to remember where it is.
		/// </summary>
		/// <param name="template"></param>
		public static void NewGame(GameVariablesTemplate template) {
			// Save the template since I'll be using it in a few seconds.
			SaveController.templateToLoad = template;
			// Add the callback.
			SceneManager.sceneLoaded += OnGameSaveLoaderNewSave;

			// Load up the GameSaveLoader. If I can use the scene controller, use that, but if not, use the regular Unity method for doing so.
			if (SceneController.instance != null) {
				SceneController.instance.LoadScene("GameSaveLoader");
			} else {
				SceneManager.LoadScene("GameSaveLoader");
			}
		}
		/// <summary>
		/// Similar to OnGameSaveLoaderLoadSave but specifically for uh. Loading up a new save. I don't know what I want to do.
		/// </summary>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		private static void OnGameSaveLoaderNewSave(Scene arg0, LoadSceneMode arg1) {
			throw new System.Exception("I want to remove the defaultSceneName from the variables template.");
			/*
			// Assign the new variables to the game controller.
			GameController.Instance.SetVariables(template: SaveController.templateToLoad);
			// Grab the default scene from the template.
			GameSaveLoaderController.sceneToLoad = SaveController.templateToLoad.defaultSceneName;
			// Null the reference.
			SaveController.templateToLoad = null;
			// Tell the FSM of the loader that we did it.
			GameSaveLoaderController.Instance.GetComponent<PlayMakerFSM>().SendEvent("LOADED");
			// Remove this event from the scene loaded event.
			SceneManager.sceneLoaded -= OnGameSaveLoaderNewSave;*/
		}
		#endregion
	}
}