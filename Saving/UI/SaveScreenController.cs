using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using HutongGames.PlayMaker;
using Grawly.Calendar;
using UnityEngine.SceneManagement;
using System;

namespace Grawly.UI {
	/// <summary>
	/// A class to present a GOOD menu for saving and shit.
	/// </summary>
	[RequireComponent(typeof(PlayMakerFSM))]
	public class SaveScreenController : MonoBehaviour {
		public static SaveScreenController instance;

		#region FIELDS - MUTABLE STATE
		/// <summary>
		/// The screen type currently displayed.
		/// </summary>
		public SaveScreenType ScreenType { get; private set; }
		/// <summary>
		/// The GameSaveCollection that should be used when opening this screen.
		/// </summary>
		private GameSaveCollection gameSaveCollection;
		/// <summary>
		/// Contains a list of positions that store where each of the save slots were positioned on the screen on startup.
		/// Handy for animations.
		/// </summary>
		private List<Vector2> initialSlotPositions = new List<Vector2>();
		/*/// <summary>
		/// The callback to run when a save is selected.
		/// </summary>
		private Action onSelectionMade;
		/// <summary>
		/// The callback to run when the operation is cancelled.
		/// </summary>
		private Action onSelectionCancelled;*/
		/// <summary>
		/// A reference to the GameObject that called the save screen controller.
		/// </summary>
		private GameObject sourceOfCall;
		/// <summary>
		/// The callback to run if the selection was cancelled.
		/// </summary>
		private System.Action onCancelCallback;
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// The index of the currently highlighted slot.
		/// </summary>
		public int CurrentHighlightedSlot {
			get { return this.fsm.FsmVariables.GetFsmInt("currentSlot").Value; }
		}
		/// <summary>
		/// Does the game save at the current slot have actual data?
		/// </summary>
		public bool CurrentSlotHasSave {
			get { return this.gameSaveCollection.HasSaveData(slot: this.CurrentHighlightedSlot); }
		}
		#endregion

		#region FIELDS - EASING
		/// <summary>
		/// The ease type to use when tweening the top/bottom bars
		/// </summary>
		[SerializeField, TabGroup("Save Screen", "Settings")]
		private Ease barEaseType = Ease.Linear;
		/// <summary>
		/// The time it should take for the flasher to do its thing.
		/// </summary>
		[SerializeField, TabGroup("Save Screen", "Settings")]
		private float flasherFadeTime = 0.5f;
		/// <summary>
		/// The time it should take for the top bars to swooce right in.
		/// </summary>
		[SerializeField, TabGroup("Save Screen", "Settings")]
		private float barMoveTime = 0.5f;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The PlayMaker FSM to send events to.
		/// </summary>
		private PlayMakerFSM fsm;
		/// <summary>
		/// The image that should flash when transitioning.
		/// </summary>
		[SerializeField, TabGroup("Save Screen", "Scene References")]
		private Image flasherImage;
		/// <summary>
		/// The bar that shows up at the top of the screen.
		/// </summary>
		[SerializeField, TabGroup("Save Screen", "Scene References")]
		private Image topBarImage;
		/// <summary>
		/// The bar that shows up at the bottom of the screen.
		/// </summary>
		[SerializeField, TabGroup("Save Screen", "Scene References")]
		private Image bottomBarImage;
		/// <summary>
		/// The game object that has all of the save file buttons. Needs to be disabled when not being shown.
		/// </summary>
		[SerializeField, TabGroup("Save Screen", "Scene References")]
		public GameObject saveFileButtonsObject;
		/// <summary>
		/// Contains references to the save file buttons on screen. There aren't that many.
		/// </summary>
		[SerializeField, TabGroup("Save Screen", "Scene References")]
		private List<SaveFileButton> saveFileButtons = new List<SaveFileButton>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				this.fsm = this.GetComponent<PlayMakerFSM>();
				// Upon start up, remember the positions of all the save file buttons.
				this.initialSlotPositions = this.saveFileButtons.Select(b => b.GetComponent<RectTransform>()).Select(rt => rt.anchoredPosition).ToList();
			}
		}
		private void Start() {
			this.ResetState();
			/*// Reset the bar positions.
			this.topBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0, y: 175f);
			this.bottomBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0, y: -175f);
			// Fade the flasher out.
			// this.flasherImage.CrossFadeAlpha(alpha: 0f, duration: 0f, ignoreTimeScale: true);
			this.flasherImage.DOFade(endValue: 0f, duration: 0f);
			// Make sure the save file buttons are not shown.
			this.saveFileButtonsObject.SetActive(false);*/
		}
		#endregion

		#region PREPARATION
		private void ResetState() {
			// Reset the bar positions.
			this.topBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0, y: 175f);
			this.bottomBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0, y: -175f);
			this.flasherImage.color = new Color(1f, 1f, 1f, 0f);
			// Make sure the save file buttons are not shown.
			this.saveFileButtonsObject.SetActive(false);
		}
		/// <summary>
		/// Plays the animation that happens when the save screen is being displayed.
		/// </summary>
		private void FadeIn() {
			// Tween the bar positions.
			this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 42f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -60f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			// Fade the flasher in.
			// this.flasherImage.CrossFadeAlpha(alpha: 1f, duration: 0.5f, ignoreTimeScale: true);
			this.flasherImage.DOColor(endValue: Color.white, duration: 0.5f);
		}
		private void FadeOut() {
			// Tween the bar positions.
			this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			// Fade the flasher out.
			// this.flasherImage.CrossFadeAlpha(alpha: 0f, duration: 0.5f, ignoreTimeScale: true);
			this.flasherImage.DOFade(endValue: 0f, duration: 0.5f);
		}
		#endregion

		#region SHOW SAVE/LOAD SCREEN
		/// <summary>
		/// Shows the save screen controller from anywhere.
		/// I mostly do this so I can access it from places like the initialization scene.
		/// </summary>
		/// <param name="saveScreenType">Whether the screen should save or load.</param>
		public static void GlobalShow(SaveScreenType saveScreenType, GameObject sourceOfCall) {
			GlobalShow(saveScreenType: saveScreenType, sourceOfCall: sourceOfCall, onCancelCallback: delegate {
			});
		}
		/// <summary>
		/// Shows the save screen controller from anywhere.
		/// I mostly do this so I can access it from places like the initialization scene.
		/// </summary>
		/// <param name="saveScreenType">Whether the screen should save or load.</param>
		public static void GlobalShow(SaveScreenType saveScreenType, GameObject sourceOfCall, System.Action onCancelCallback) {
			// If the Instance does not exist, load the scene.
			if (instance == null) {
				
				UnityEngine.AsyncOperation loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
					sceneName: "Save Menu DX", 
					mode: UnityEngine.SceneManagement.LoadSceneMode.Additive);
				
				loadOperation.completed += delegate {
					instance.Show(
						saveScreenType: saveScreenType, 
						sourceOfCall: sourceOfCall, 
						onCancelCallback: onCancelCallback);
					
				};
			} else {
				
				// If it does exist, just call show as normal.
				instance.Show(
					saveScreenType: saveScreenType,
					sourceOfCall: sourceOfCall, 
					onCancelCallback: onCancelCallback);
				
			}
		}
		/// <summary>
		/// Shows the save screen with a bunch of shit.
		/// </summary>
		/// <param name="saveScreenType">The type of screen to show.</param>
		/// <param name="sourceOfCall">The GameObject who called the controller. Needed for sending an alert back to when it's closed out.</param>
		public void Show(SaveScreenType saveScreenType, GameObject sourceOfCall) {
			this.Show(saveScreenType: saveScreenType, sourceOfCall: sourceOfCall, onCancelCallback: delegate {
			});
		}
		/// <summary>
		/// Shows the save screen with a bunch of shit.
		/// </summary>
		/// <param name="saveScreenType">The type of screen to show.</param>
		/// <param name="sourceOfCall">The GameObject who called the controller. Needed for sending an alert back to when it's closed out.</param>
		public void Show(SaveScreenType saveScreenType, GameObject sourceOfCall, System.Action onCancelCallback) {
			
			// Remember the type of screen being shown.
			this.ScreenType = saveScreenType;
			this.onCancelCallback = onCancelCallback;

			// Reset the state.
			this.ResetState();

			// Play the prep animation.
			this.FadeIn();
			// Load the game save collection, since that will be needed.
			this.ReloadGameSaveCollection();

			// Save a reference to the game object that called this thing. It will be needed later.
			this.sourceOfCall = sourceOfCall;
			this.fsm.SendEvent("START");

			// Begin by setting the current highlight to be the last save slot.
			this.fsm.FsmVariables.GetFsmInt("currentSlot").Value = gameSaveCollection.LastSaveSlot;

			// Send it the event with the way to proceed. The FSM branches here. (Save/Load)
			this.fsm.SendEvent(saveScreenType.ToString());
			
		}
		/// <summary>
		/// Exits the save screen.
		/// </summary>
		public void Exit(bool savePicked) {
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(new TweenCallback(delegate {
				// Turn off the save file buttons object.
				this.saveFileButtonsObject.SetActive(false);
				// Faaaade out.
				this.FadeOut();
			}));
			seq.AppendInterval(this.flasherFadeTime + 0.1f);
			seq.OnComplete(new TweenCallback(delegate {
				// Tell the source of the call that it's done. This may not remain the way it is but. Whatever.
				this.sourceOfCall?.GetComponent<PlayMakerFSM>()?.SendEvent("SAVE MENU CLOSED");

				// If no save was picked, run the callback.
				if (savePicked == false) {
					this.onCancelCallback?.Invoke();
				}

				// If the save screen controller's instance is not null, unload the save scene.
				if (SaveScreenController.instance != null) {
					SceneManager.UnloadSceneAsync(sceneName: "Save Menu DX");
				}
				
			}));
			seq.Play();
		}
		#endregion

		#region DATA RETRIEVAL
		/// <summary>
		/// Reloads the gameSaveCollection. I'm writing it as a method like this so I can just call it from PlayMaker as an event.
		/// </summary>
		public void ReloadGameSaveCollection() {
			this.gameSaveCollection = SaveController.LoadCollection();
		}
		/// <summary>
		/// Loads the save from the specified slot.
		/// </summary>
		/// <param name="slotToFocus"></param>
		public void LoadSave(int slotToFocus) {
			
			// Don't forget to fade out bitch!!! I don't want to exit because that will tell the source I exited and thats No.
			this.FadeOut();
			
			// Tell the save controller to load the save. This will handle all the scene shit from there.
			SaveController.LoadGameSave(
				gameSave: this.gameSaveCollection.GetAllSaves()[slotToFocus], 
				slot: slotToFocus,
				incrementLoadCount: true);
			
		}
		#endregion

		#region BUILDING THE LIST
		/// <summary>
		/// Builds the list of save slots while bringing focus to the specified index.
		/// This is meant to be called from PlayMaker
		/// </summary>
		/// <param name="slotToFocus"></param>
		public void BuildSaveSlots(int slotToFocus) {
			this.BuildSaveSlots(
				gameSaves: this.gameSaveCollection.GetAllSaves(), 
				calendarData: CalendarController.Instance.CalendarData,
				slotToFocus: slotToFocus);
		}
		/// <summary>
		/// Builds the list of save slots while bringing focus to the specified index.
		/// </summary>
		/// <param name="gameSaves"></param>
		/// <param name="slotToFocus"></param>
		private void BuildSaveSlots(List<GameSave> gameSaves, CalendarData calendarData, int slotToFocus) {
			// Determine how many save slots are on the top/bottom of the slot being focused on.
			// For example, right now it's 5, and 5/2 is 2.5, but the integer drops that .5 and it's just 2. Which is what I need.
			int borderingSlotCount = this.saveFileButtons.Count / 2;
			// Pick out the index that shows the save file at the top of the screen.
			int logicalIndex = slotToFocus - borderingSlotCount;

			// Loop through each of the save file buttons and build them, while passing that logical index.
			for (int i = 0; i < this.saveFileButtons.Count; i++) {
				// Note that if the number is less than zero or exceeds the game saves count, the button turns off. As it should.
				if (logicalIndex >= 0 && logicalIndex < gameSaves.Count) {
					// This also passes it the CalendarDay for this save. It only really needs it for the graphics of the button.
					CalendarDay calendarDay = CalendarController.Instance.CalendarData.GetDay(dayOfFocus: gameSaves[logicalIndex].currentDayNumber);
					// CalendarDay calendarDay = DataController.GetDefaultCalendarData().GetDay(dayOfFocus: gameSaves[logicalIndex].currentDayNumber);
					this.saveFileButtons[i].Prepare(gameSaves: gameSaves, slotNumber: logicalIndex, calendarDay: calendarDay);

					// I will probably remove this... in a bit.
					if (logicalIndex == slotToFocus) {
						this.saveFileButtons[i].Highlight(gameSave: gameSaves[logicalIndex], slotNumber: logicalIndex, calendarDay: calendarDay);
					} else {
						this.saveFileButtons[i].Dehighlight(gameSave: gameSaves[logicalIndex], slotNumber: logicalIndex, calendarDay: calendarDay);
					}
				} else {
					this.saveFileButtons[i].TurnOff();
				}

				// Increment the logical index as well.
				logicalIndex += 1;
			}
		}
		/// <summary>
		/// Gets called from SaveScreenEventHandler when the list needs to move up.
		/// </summary>
		public void MoveListUp() {
			Debug.Log("Attepting to move the list up.");
			// If the list can't be moved up, don't.
			if (this.fsm.FsmVariables.GetFsmInt("currentSlot").Value == (this.gameSaveCollection.GetAllSaves().Count - 1)) {
				return;
			}

			// Increment the current slot by one.
			this.fsm.FsmVariables.GetFsmInt("currentSlot").Value += 1;
			// Build the slots with the new number.
			this.BuildSaveSlots(slotToFocus: this.fsm.FsmVariables.GetFsmInt("currentSlot").Value);
			// Play the animation that moves the slots up.
			this.MoveSlotsUpAnimation();
		}
		/// <summary>
		/// Gets called from SaveScreenEventHandler when the list needs to move down.
		/// </summary>
		public void MoveListDown() {
			Debug.Log("Attepting to move the list down.");
			// If the list can't be moved down, don't.
			if (this.fsm.FsmVariables.GetFsmInt("currentSlot").Value == 0) {
				return;
			}

			// Decrement the current slot by one.
			this.fsm.FsmVariables.GetFsmInt("currentSlot").Value -= 1;
			// Build the slots with the new number.
			this.BuildSaveSlots(slotToFocus: this.fsm.FsmVariables.GetFsmInt("currentSlot").Value);
			// Play the animation that moves the slots down.
			this.MoveSlotsDownAnimation();
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Makes the save slots look like they're moving up.
		/// </summary>
		private void MoveSlotsUpAnimation() {
			// This isn't well documented but tldr, I'm not moving ALL of them. Just the edge cases.

			// First off, play the sound.
			AudioController.instance?.PlaySFX(SFXType.Hover, 1f);

			// Go through each of the save buttons and tell DOTween to finish up any animations it may have previously been doing on them.
			// This is important if like, the user is tapping the button a lot.
			this.saveFileButtons.Select(b => b.GetComponent<RectTransform>()).ToList().ForEach(rt => {
				rt.DOComplete();
			});

			// Create an index variable to keep track of shit.
			int index = 1;

			// Push the buttons down a bit. This goes through every button except the last.
			this.saveFileButtons.Take(count: this.saveFileButtons.Count - 1).Select(b => b.GetComponent<RectTransform>()).ToList().ForEach(rt => {
				// Since the indexer starts at 1, it sets the position of the button to the one that is below it.
				rt.anchoredPosition = this.initialSlotPositions[index];
				index += 1;
			});

			// Reset that index number, because now I'm telling every one of the buttons to scroll back to their initial positions.
			index = 0;
			this.saveFileButtons.Select(b => b.GetComponent<RectTransform>()).ToList().ForEach(rt => {
				rt.DOAnchorPos(endValue: this.initialSlotPositions[index], duration: 0.5f, snapping: false);
				index += 1;
			});
		}
		/// <summary>
		/// Makes the save slots look like they're moving down.
		/// </summary>
		private void MoveSlotsDownAnimation() {
			// First off, play the sound.
			AudioController.instance?.PlaySFX(SFXType.Hover, 1f);

			// Complete all the other animations.
			this.saveFileButtons.Select(b => b.GetComponent<RectTransform>()).ToList().ForEach(rt => {
				rt.DOComplete();
			});

			// Create an index variable to keep track of shit.
			int index = 0;

			// Push the buttons up a bit. This skips the first button (the top one).
			this.saveFileButtons.Skip(count: 1).Select(b => b.GetComponent<RectTransform>()).ToList().ForEach(rt => {
				// This sets the position of the button to the position of the one thats above it.
				rt.anchoredPosition = this.initialSlotPositions[index];
				index += 1;
			});

			// Re-initialize the indexer.
			index = 0;
			// Reset that index number, because now I'm telling every one of the buttons to scroll back to their initial positions.
			this.saveFileButtons.Select(b => b.GetComponent<RectTransform>()).ToList().ForEach(rt => {
				// This sets each button to its "original" slot."
				rt.DOAnchorPos(endValue: this.initialSlotPositions[index], duration: 0.5f, snapping: false);
				index += 1;
			});
		}
		#endregion
	}
}