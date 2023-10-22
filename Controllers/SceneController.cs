using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Grawly.UI.Legacy;
using Grawly.Mission;
using Grawly.Story;
using Grawly.CodeBits;
using Grawly.Calendar;
using DG.Tweening;
using Grawly.Dungeon;
using System.Linq;
using UnityEngine.Events;
using System;

namespace Grawly {

	public class SceneController : MonoBehaviour {

		public static SceneController instance;

		#region FIELDS - STATE
		/// <summary>
		/// A very, very basic holder for a callback that should be run when not using scene load params.
		/// </summary>
		private UnityAction basicSceneLoadedCallback;
		/// <summary>
		/// A reference to the SceneLoadParams that are currently in use. The same parameters may be required for a while so it's important to save them.
		/// </summary>
		private SceneLoadParams currentSceneLoadParams;
		/// <summary>
		/// The callback to run when the add scene routine is finished.
		/// </summary>
		private TweenCallback currentAddSceneCallback;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region SCENE LOADING - BASIC
		/// <summary>
		/// Loads the scene associated with the given location and also just fades by default.
		/// </summary>
		/// <param name="locationType"></param>
		/// <param name="onComplete"></param>
		public void BasicLoadSceneWithFade(LocationType locationType, System.Action onComplete = null) {
			Flasher.FadeOut();
			GameController.Instance.WaitThenRun(1f, () => {
				this.BasicLoadScene(locationType: locationType, onComplete: delegate {
					Flasher.FadeIn();
					onComplete?.Invoke();
				});
			});
		}
		/// <summary>
		/// Loads the scene with the given name and also just fades by default.
		/// </summary>
		/// <param name="sceneName"></param>
		public void BasicLoadSceneWithFade(string sceneName, float fadeTime = 0.5f, System.Action onComplete = null) {
			Flasher.FadeOut(fadeTime: fadeTime);
			GameController.Instance.WaitThenRun(fadeTime + 0.05f, () => {
				this.BasicLoadScene(sceneName: sceneName, onComplete: delegate {
					Flasher.FadeIn();
					onComplete?.Invoke();
				});
			});
		}
		/// <summary>
		/// Loads the scene with the given name and also just fades by default.
		/// </summary>
		/// <param name="sceneIndex">The index of the scene to load.</param>
		public void BasicLoadSceneWithFade(int sceneIndex) {
			Flasher.FadeOut();
			GameController.Instance.WaitThenRun(1f, () => {
				this.BasicLoadScene(sceneIndex: sceneIndex, onComplete: delegate {
					Flasher.FadeIn();
				});
			});
			
		}
		/// <summary>
		/// Loads the scene with the specified location type.
		/// </summary>
		/// <param name="locationType">The location type to load.</param>
		/// <param name="onComplete">A callback to run when the scene is loaded.</param>
		public void BasicLoadScene(LocationType locationType, System.Action onComplete = null) {
			
			// Grab the current day number and time of day from the GameController.
			int currentDay = GameController.Instance.Variables.CurrentDayNumber;
			TimeOfDayType currentTime = GameController.Instance.Variables.CurrentTimeOfDay;
			
			// Use these to find the scene associated with said day/time combination.
			string sceneToLoad = CalendarController.Instance.CalendarData.GetScene(
				dayNumber: currentDay,
				locationType: locationType,
				timeOfDay: currentTime);
			
			// Pass this down to the BasicLoadScene function.
			this.BasicLoadScene(sceneName: sceneToLoad, onComplete: onComplete);
			
		}
		/// <summary>
		/// Loads the scene with the specified name.
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="onComplete"></param>
		public void BasicLoadScene(string sceneName, System.Action onComplete = null) {
			
			// Make an empty delegate if its null.
			if (onComplete == null) {
				onComplete = delegate {	};
			}
			
			// Set the basic scene loaded callback to be a new unity action.
			basicSceneLoadedCallback = new UnityAction(onComplete);

			// Add the callback for when the basic scene is loaded.
			SceneManager.sceneLoaded += this.BasicOnSceneLoaded;
			
			// Load!
			SceneManager.LoadScene(sceneName: sceneName);
			
		}
		/// <summary>
		/// Loads the scene at the given build index, then runs the action upon completion.
		/// </summary>
		/// <param name="sceneIndex"></param>
		/// <param name="onComplete"></param>
		public void BasicLoadScene(int sceneIndex, System.Action onComplete = null) {
			
			// Make an empty delegate if its null.
			if (onComplete == null) {
				onComplete = delegate {	};
			}
			
			// Set the basic scene loaded callback to be a new unity action.
			basicSceneLoadedCallback = new UnityAction(onComplete);

			// Add the callback for when the basic scene is loaded.
			SceneManager.sceneLoaded += this.BasicOnSceneLoaded;
			
			// Load!
			SceneManager.LoadScene(sceneBuildIndex: sceneIndex);

		}
		#endregion

		#region SCENE LOADING - TIME TRANSITION
		/// <summary>
		/// Loads a scene of the given name while also showing the time transition screen.
		/// </summary>
		/// <param name="sceneName">The name of the scene to load.</param>
		/// <param name="startTime">The time the transition is happening from.</param>
		/// <param name="endTime">The time the transition is moving towards.</param>
		/// <param name="onSceneLoaded">The callback to run when the scene is loaded.</param>
		/// <param name="spawnPositionType">The spawn position to put the player at when the scene is loaded.</param>
		/// <param name="dontStopMusic">Sets whether music should continue playing or not.</param>
		public void LoadSceneWithTimeTransition(string sceneName, TimeOfDayType startTime, TimeOfDayType endTime, Action onSceneLoaded = null, DungeonSpawnType spawnPositionType = DungeonSpawnType.Default, bool dontStopMusic = false) {
			
			// Make a new set of params with the scene name but also pass in the start/end times.
			// If those times are different, it will automatically use the TimeSceneController instead of the flasher.
			this.LoadScene(new SceneLoadParams() {
				SceneName = sceneName,
				startTime = startTime,
				endTime = endTime,
				OnSceneLoaded = onSceneLoaded,
				spawnPositionType = spawnPositionType,
				DontStopMusic = dontStopMusic
			});
			
		}
		#endregion
		
		#region SCENE LOADING - STANDARD
		/// <summary>
		/// Loads a scene with the associated overworld type.
		/// </summary>
		/// <param name="dayNumber">The number of the day to check against..</param>
		/// <param name="locationType">The overworld type to obtain the scene for.</param>
		/// <param name="timeOfDay">The time of day to load up.</param>
		/// <param name="onSceneLoaded">The callback to run when the scene is loaded.</param>
		/// <param name="spawnPositionType">The spawn position to put the player at when the scene is loaded.</param>
		/// <param name="dontStopMusic">Sets whether music should continue playing or not.</param>
		public void LoadScene(int dayNumber, LocationType locationType, TimeOfDayType timeOfDay, Action onSceneLoaded = null, DungeonSpawnType spawnPositionType = DungeonSpawnType.Default, bool dontStopMusic = false) {
			
			// Determine the scene to load from the calendar data.
			string sceneToLoad = CalendarController.Instance.CalendarData.GetScene(
				dayNumber: dayNumber,
				locationType: locationType,
				timeOfDay: timeOfDay);
			
			// Load it.
			this.LoadScene(
				sceneName: sceneToLoad, 
				onSceneLoaded: onSceneLoaded,
				spawnPositionType: spawnPositionType,
				dontStopMusic: dontStopMusic);
			
		}
		/// <summary>
		/// Loads a scene of the given name.
		/// </summary>
		/// <param name="sceneName">The name of the scene to load.</param>
		/// <param name="onSceneLoaded">The callback to run when the scene is loaded.</param>
		/// <param name="spawnPositionType">The spawn position to put the player at when the scene is loaded.</param>
		/// <param name="dontStopMusic">Sets whether music should continue playing or not.</param>
		public void LoadScene(string sceneName, Action onSceneLoaded = null, DungeonSpawnType spawnPositionType = DungeonSpawnType.Default, bool dontStopMusic = false) {
			// Load up a new scene by creating a new set of parameters.
			this.LoadScene(sceneLoadParams: new SceneLoadParams() {
				SceneName = sceneName, 
				OnSceneLoaded = onSceneLoaded,
				spawnPositionType = spawnPositionType, 
				DontStopMusic = dontStopMusic
			});
		}
		/// <summary>
		/// Loads a scene with the given parameters.
		/// </summary>
		/// <param name="sceneLoadParams">The parameters on how to load up the specified scene.</param>
		public void LoadScene(SceneLoadParams sceneLoadParams) {
			Debug.Log("LOADING SCENE: " + sceneLoadParams.SceneName);
			// Save a reference to the load params. Might need em.
			this.currentSceneLoadParams = sceneLoadParams;

			// Check whether or not the time transition should be used.
			if (this.currentSceneLoadParams.UseTimeTransition == true) {
				this.StartCoroutine(this.TimeTransitionLoadSceneRoutine(sceneLoadParams: sceneLoadParams));
			} else {
				// Start up the coroutune that handles scene loading.
				this.StartCoroutine(this.StandardLoadSceneRoutine(sceneLoadParams: sceneLoadParams));
			}
			
		}
		#endregion
		
		#region SCENE ADDING
		/// <summary>
		/// Adds a scene to the game and then runs a callback when the scene has loaded.
		/// Mostly used for UI junk that I keep in separate scenes.
		/// </summary>
		/// <param name="sceneName">The name of the scene to load up.</param>
		/// <param name="callback">The callback that should be run when this scene is loaded.</param>
		public void AddScene(string sceneName, TweenCallback callback) {
			// Remember the callback. I will need it.
			this.currentAddSceneCallback = callback;
			// Load up the scene.
			SceneManager.LoadSceneAsync(sceneName: sceneName, mode: LoadSceneMode.Additive).completed += AddSceneLoadComplete;
		}
		/// <summary>
		/// The thing that gets run when the additive scene loading is complete.
		/// </summary>
		/// <param name="obj"></param>
		private void AddSceneLoadComplete(AsyncOperation obj) {
			// Run the callback.
			this.currentAddSceneCallback();
			// I will think about nulling it out later.
		}
		#endregion

		#region ROUTINES
		/// <summary>
		/// The routine for loading a scene most of the time.
		/// </summary>
		/// <param name="sceneLoadParams">The parameters involved in loading the scene.</param>
		/// <returns></returns>
		private IEnumerator StandardLoadSceneRoutine(SceneLoadParams sceneLoadParams) {

			// If the music needs to keep playing during a scene load, set the lock on the audio controller.
			if (sceneLoadParams.DontStopMusic == true) {
				AudioController.instance.SetMusicPlaybackLock(MusicPlaybackLockType.Locked);
			} else {
				// If the music should stop, fade it out.
				AudioController.instance.StopMusic(track: 0, fade: 0.5f);
			}

			// Fade out the screen and wait a second.
			Flasher.instance.FadeOut(color: Color.black, fadeTime: 0.5f);
			yield return new WaitForSeconds(0.5f);
			// Have the loading text appear.
			Flasher.instance.SetLoadingTextActive(true);
			yield return new WaitForSeconds(0.5f);

			// Load the scene. Use a callback for when the scene has been loaded.
			SceneManager.sceneLoaded += StandardOnSceneLoaded;

			try {
				SceneManager.LoadScene(sceneLoadParams.SceneName);
			} catch (System.Exception e) {
				Debug.LogError("Couldn't load scene! Scene name: " + sceneLoadParams.SceneName);
			}

		}
		/// <summary>
		/// The routine for loading up a given scene, but uses the TimeSceneController instead of the flasher.
		/// </summary>
		/// <param name="sceneLoadParams">The parameters that define how the scene should be loaded.</param>
		/// <returns></returns>
		private IEnumerator TimeTransitionLoadSceneRoutine(SceneLoadParams sceneLoadParams) {
			
			Debug.Log("Beginning time transition.");
			// Load the time change scene as an additive thing and save a reference to the operation.
			AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName: "Time Change Scene", mode: LoadSceneMode.Additive);

			// While waiting for the scene to load, wait it out.
			while (loadOperation.isDone == false) {
				yield return null;
			}

			// When it is done, start with the rest.

			if (sceneLoadParams.DontStopMusic == false) {
				// Fade out the music if it was told to do so.
				AudioController.instance.StopMusic(track: 0, fade: 1.5f);
			} 

			// Fade the time scene controller.
			TimeSceneController.instance.FadeOut(startTime: sceneLoadParams.startTime, endTime: sceneLoadParams.endTime);
			// Wait a few seconds.
			yield return new WaitForSeconds(1.5f);

			// Tell the scene manager the function to call when the scene has been loaded.
			// So uh. I guess save the callback real quick...?
			
			SceneManager.sceneLoaded += TimeSceneController.instance.TimeTransitionOnSceneLoaded;
			SceneManager.sceneLoaded += this.TimeTransitionOnSceneLoaded;
			// Attempt to load the level.
			try {
				SceneManager.LoadScene(sceneLoadParams.SceneName);
			} catch (System.Exception e) {
				Debug.LogError("Couldn't load scene! Scene name: " + sceneLoadParams.SceneName);
			}
			
		}
		#endregion

		#region ON SCENE LOADED
		/// <summary>
		/// Gets called when a scene is finished loading using the basic load.
		/// </summary>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		private void BasicOnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
			this.basicSceneLoadedCallback?.Invoke();
			this.basicSceneLoadedCallback = null;
			SceneManager.sceneLoaded -= BasicOnSceneLoaded;
		}
		/// <summary>
		/// Another function to run when a scene is loaded during a time transition.
		/// </summary>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		private void TimeTransitionOnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
			this.currentSceneLoadParams.OnSceneLoaded?.Invoke();
			SceneManager.sceneLoaded -= TimeTransitionOnSceneLoaded;
		}
		/// <summary>
		/// Gets called when a scene is finished loading.
		/// </summary>
		private void StandardOnSceneLoaded(Scene arg0, LoadSceneMode arg1) {

			if (arg0.name != this.currentSceneLoadParams.SceneName) {
				Debug.LogWarning("Scene " + arg0.name + " is not defined in the current scene load params. Returning.");
				return;
			}
			
			Flasher.instance.SetLoadingTextActive(false);
			Flasher.instance.FadeIn(fadeTime: 0.5f);

			// If the DontStopMusic flag was set, that means the AudioController was previously locked during the scene transition.
			// At this point, the lock should be released. But wait a moment before doing so so AutoMusic doesn't have a chance to activate.
			if (this.currentSceneLoadParams.DontStopMusic == true) {
				GameController.Instance.WaitThenRun(timeToWait: 0.5f, () => {
					AudioController.instance.SetMusicPlaybackLock(MusicPlaybackLockType.Unlocked);
				});
			}
			
			// If a spawn ID was specified, find the spawn position that shares that same ID and tell the player to go there.
			if (this.currentSceneLoadParams.spawnPositionType != DungeonSpawnType.Default ) {
				GameObject spawnPositionObject = GameObject
					.FindObjectsOfType<DungeonEntrancePosition>()
					.First(s => s.spawnPositionType == this.currentSceneLoadParams.spawnPositionType)
					.gameObject;
				DungeonPlayer.Instance?.Relocate(pos: spawnPositionObject.transform);
			}

			foreach (CodeBit codeBit in this.currentSceneLoadParams.OnSceneLoadCodeBits) {
				codeBit.Run();
			}
			
			// Invoke the OnSceneLoaded callback, if there is one.
			this.currentSceneLoadParams.OnSceneLoaded?.Invoke();
			
			SceneManager.sceneLoaded -= StandardOnSceneLoaded;
		}
		#endregion

	}
}