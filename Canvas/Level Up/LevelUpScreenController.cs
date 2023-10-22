using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Grawly.Gauntlet;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using Grawly.UI.Legacy;
using Grawly.Chat;

namespace Grawly.UI {

	/// <summary>
	/// A more nicer display for the level up screen.
	/// </summary>
	public class LevelUpScreenController : MonoBehaviour {

		public static LevelUpScreenController instance;

		#region FIELDS - STATE
		/// <summary>
		/// The GameVariables that are currently being parsed.
		/// </summary>
		private static GameVariables currentGameVariables;
		/// <summary>
		/// The callback that should get run when everything is done.
		/// </summary>
		private static Action currentFinishCallback;
		/// <summary>
		/// A queue of personas that leveled up. Important for making sure all leveled up personas are displayed.
		/// </summary>
		private Queue<CombatantLevelUpResults> leveledUpPersonaQueue = new Queue<CombatantLevelUpResults>();
		#endregion

		#region FIELDS - EASING
		/// <summary>
		/// The time it should take for the flasher to do its thing.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private float blurFadeTime = 0.5f;
		/// <summary>
		/// The ease type to use when tweening the top/bottom bars
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private Ease barEaseType = Ease.Linear;
		/// <summary>
		/// The time it should take for the top bars to swooce right in.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private float barMoveTime = 0.5f;
		/// <summary>
		/// The ease to use when tweening the thumbnails in.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private Ease thumbnailEaseInType = Ease.Linear;
		/// <summary>
		/// The amount of time to take when tweening the thumbnail images in.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private float thumbnailTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to wait in between each thumbnail tweening in.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private float thumbnailTweenInDelayTime = 0.5f;
		/// <summary>
		/// The ease to use when tweening the thumbnails out.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private Ease thumbnailEaseOutType = Ease.Linear;
		/// <summary>
		/// The amount to space the thumbnails vertically when building them.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private float thumbnailVerticalSpacingAmount = 0f;
		/// <summary>
		/// The amount to offset the thumbnails horizontally before tweening them in.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private float thumbnailHoritontalOffsetAmount = -1920f;
		#endregion

		#region FIELDS - INITIAL POSITIONS
		/// <summary>
		/// The initial positions of the thumbnails. 
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Toggles")]
		private List<Vector2> initialThumbnailPositions = new List<Vector2>();
		#endregion

		#region FIELDS - SCENE REFERENCES : MAIN VISUALS
		/// <summary>
		/// Contains all other scene references as children so they can easily be turned on and off.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Scene References")]
		private GameObject allGameObjects;
		/// <summary>
		/// The thumbnails for building the different combatant level up thinbygh
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Scene References")]
		private List<CombatantLevelUpThumbnail> combatantLevelUpThumbnails = new List<CombatantLevelUpThumbnail>();
		#endregion

		#region FIELDS - SCENE REFERENCES : DECORATION
		/// <summary>
		/// The bar that shows up at the top of the screen.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Scene References")]
		private Image topBarImage;
		/// <summary>
		/// The bar that shows up at the bottom of the screen.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Scene References")]
		private Image bottomBarImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : INVISIBLE BUTTON
		/// <summary>
		/// An invisible button to be hit when leveling up persona's.
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Scene References")]
		private Selectable invisibleButton;
		#endregion

		#region FIELDS - DEBUG
		/// <summary>
		/// Is debug mode on?
		/// </summary>
		[SerializeField, TabGroup("Level Up Screen", "Debug")]
		private bool debugMode = false;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				ResetController.AddToDontDestroy(this.gameObject);
				// DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}
		#endregion

		#region CALLING THE PAUSE MENU
		/// <summary>
		/// Opens up the Level Up Screen and parses the exp that is avaialable and gives it to the players. 
		/// </summary>
		/// <param name="gameVariables">The game variables that contain the shit that needs to be parsed out.</param>
		/// <param name="finishCallback">The callback to run when the screen is all done with.</param>
		public static void Open(GameVariables gameVariables, Action finishCallback) {
			Debug.Log("Opening the level up screen.");

			if (instance == null) {
				Debug.Log("Instance is null. Loading the Level Up scene.");

				// Save the game variables and finish callback. I need these to be static because of weird shit with unity loading the scene if it isn't avaialble already.
				LevelUpScreenController.currentGameVariables = gameVariables;
				LevelUpScreenController.currentFinishCallback = finishCallback;
				SceneManager.LoadSceneAsync(sceneName: "Level Up Screen", mode: LoadSceneMode.Additive).completed += LevelUpScreen_completed;
			} else {
				instance.BeginLevelUpProceedure(gameVariables: gameVariables, finishCallback: finishCallback);
			}
		}
		/// <summary>
		/// Gets called when the operation is complete.
		/// </summary>
		/// <param name="obj"></param>
		private static void LevelUpScreen_completed(AsyncOperation obj) {
			// Tell the Instance to open up.
			instance.BeginLevelUpProceedure(gameVariables: LevelUpScreenController.currentGameVariables, finishCallback: LevelUpScreenController.currentFinishCallback);
		}
		/// <summary>
		/// Doesn't need to be static because I assume the Instance is valid.
		/// This just closes the controller.
		/// </summary>
		public void Close() {
			// Tween the screen out.
			this.TweenLevelUpScreenOut();
			throw new System.Exception("iidk");
		}
		#endregion

		#region MAIN FUNCTION
		/// <summary>
		/// Goes through each combatant, levels them up, and returns the results.
		/// </summary>
		/// <param name="gameVariables">The GameVariables to parse and level up.</param>
		/// <param name="finishCallback">The callback to run when everything is finished.</param>
		private void BeginLevelUpProceedure(GameVariables gameVariables, Action finishCallback) {

			// Reset the screen.
			this.ResetLevelUpScreen();

			// Level up the players/personas and remember their results.
			List<CombatantLevelUpResults> playerLevelUpResults = GameController.Instance.LevelUpCombatants(
				combatants: gameVariables.Players
					.Where(p => p.IsDead == false)
					.Cast<Combatant>()
					.ToList(),
				experience: gameVariables.ExpBank);
			List<CombatantLevelUpResults> personaLevelUpresults = GameController.Instance.LevelUpCombatants(
				combatants: gameVariables.Players
					.Where(p => p.IsDead == false)
					.Select(p => p.ActivePersona)
					.Cast<Combatant>().ToList(),
				experience: gameVariables.ExpBank);

			// Save the personas that leveled up into the queue.
			this.leveledUpPersonaQueue = new Queue<CombatantLevelUpResults>(personaLevelUpresults.Where(p => p.LeveledUp));

			
			// Turn off the thumbnails.
			this.combatantLevelUpThumbnails.ForEach(c => c.SetVisualsActive(false));
			// Turn on the thumbnails for the same amount of players that leveled up.
			this.combatantLevelUpThumbnails
				.Take(playerLevelUpResults.Count(p => p.LeveledUp == true))
				.ToList()
				.ForEach(c => c.SetVisualsActive(true));

			// If there are any players that leveled up, build their icons.
			if (playerLevelUpResults.Count(p => p.LeveledUp == true) > 0) {
				// Go through each player that leveled up and build their icon.
				for (int i = 0; i < playerLevelUpResults.Count(p => p.LeveledUp == true); i++) {
					// Turn on thumbnails that are actually used.
					this.combatantLevelUpThumbnails[i].SetVisualsActive(true);
					this.combatantLevelUpThumbnails[i].Build(levelUpResults: playerLevelUpResults
						.Where(p => p.LeveledUp)
						.ElementAt(i));
				}

				// Tween that shit in.
				this.TweenLevelUpScreenIn();
			} else {
				// If no players leveled up, skip immediately to the event where the button was hit.
				this.DequeueNextLeveledUpPersona();
			}
			
		

		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Just resets the position of all the tutorial controller junk so that it can prepare for animatinos owhafgh
		/// </summary>
		private void ResetLevelUpScreen() {

			// Set the positions of the top/bottom bars.
			this.topBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: this.topBarImage.GetComponent<RectTransform>().anchoredPosition.x, y: 175f);
			this.bottomBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: this.bottomBarImage.GetComponent<RectTransform>().anchoredPosition.x, y: -175f);

			// Reset the positions of the thumbnails.
			for (int i = 0; i < this.combatantLevelUpThumbnails.Count; i++) {
				this.combatantLevelUpThumbnails[i].transform.localPosition = this.initialThumbnailPositions[i] + new Vector2(x: this.thumbnailHoritontalOffsetAmount, y: 0f);
			}

			// Set the hidden button to be off.
			this.SetHiddenButtonActive(false);

		}
		/// <summary>
		/// Tweens the level up screen in.
		/// </summary>
		private void TweenLevelUpScreenIn() {

			// Blur the camera.
			throw new System.NotImplementedException("The blur needs to be re-added.");
			/*Camera.main.GetComponent<BlurOptimized>().enabled = true;
			DOTween.To(
				getter: () => Camera.main.GetComponent<BlurOptimized>().blurSize,
				setter: x => Camera.main.GetComponent<BlurOptimized>().blurSize = x,
				endValue: 3f,
				duration: this.blurFadeTime);*/

			// TWEEN THE BARS IN
			this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 42f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -60f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);

			// Create a new sequence.
			Sequence seq = DOTween.Sequence();
			// Go through each thumbnail and tween the position.
			for (int i = 0; i < this.combatantLevelUpThumbnails.Count; i++) {
				seq.Append(this.combatantLevelUpThumbnails[i].transform.DOLocalMove(
					endValue: this.initialThumbnailPositions[i], 
					duration: this.thumbnailTweenInTime, 
					snapping: true)
					.SetEase(ease: this.thumbnailEaseInType));
				seq.AppendInterval(this.thumbnailTweenInDelayTime);
			}
			seq.AppendInterval(interval: 3f);
			seq.AppendCallback(new TweenCallback(delegate {
				this.SetHiddenButtonActive(true);
			}));
			seq.Play();
		}
		/// <summary>
		/// Tweens the level up screen out.
		/// </summary>
		private void TweenLevelUpScreenOut() {
			// TWEEN THE BARS OUT
			this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			// Deblur the camera.
			throw new System.NotImplementedException("The blur needs to be re-added.");
			/*DOTween.To(
				getter: () => Camera.main.GetComponent<BlurOptimized>().blurSize,
				setter: x => Camera.main.GetComponent<BlurOptimized>().blurSize = x,
				endValue: 0f,
				duration: this.blurFadeTime)
					.onComplete = new TweenCallback(delegate {
						Camera.main.GetComponent<BlurOptimized>().enabled = false;
						});*/

			// Create a new sequence.
			Sequence seq = DOTween.Sequence();
			// Go through each thumbnail and tween the position.
			for (int i = 0; i < this.combatantLevelUpThumbnails.Count; i++) {
				seq.Append(this.combatantLevelUpThumbnails[i].transform.DOLocalMove(
					endValue: this.initialThumbnailPositions[i] + new Vector2(x: this.thumbnailHoritontalOffsetAmount, y: 0f),
					duration: this.thumbnailTweenInTime,
					snapping: true)
					.SetEase(ease: this.thumbnailEaseInType));
				seq.AppendInterval(this.thumbnailTweenInDelayTime);
			}
			
			seq.Play();

		}
		#endregion

		#region BULLSHIT METHODS WITH PERSONA LEVELING UP
		/// <summary>
		/// Builds a canvas showing the persona that leveled up.
		/// </summary>
		/// <param name="persona"></param>
		private void PersonaLevelUpEvent(Persona persona) {
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(new TweenCallback(delegate {
				// Build populates the combatant analysis canvas and makes it so the thing flashes and shit.
				CombatantAnalysisCanvas.instance.Build(persona, CombatantAnalysisCanvas.ContextType.PersonaLevelUp);
			}));
			seq.AppendInterval(1f);
			seq.AppendCallback(new TweenCallback(delegate {
				// After a second, open up a chat and say that this persona has leveled up.
				/*Chat.Legacy.LegacyChatController.Open(
						script: new PlainChatScript(persona.metaData.name + " has leveled up!"),
						chatOpenedCallback: delegate {
							
						}, chatClosedCallback: delegate {
							this.PersonaLevelUpMoveCheck(persona: persona);
						});*/
				ChatControllerDX.GlobalOpen(
					chatScript: new PlainChatScript(persona.metaData.name + " has leveled up!"),
					chatClosedCallback: delegate {
						this.PersonaLevelUpMoveCheck(persona: persona);
					});
			}));
			//
			//	Sequence has been assembled. Play it.
			//
			seq.Play();
		}
		/// <summary>
		/// A FUNCTION TO BE USED IN CONNECTION WITH PERSONALEVELUPEVENT!
		/// This will handle the functions of checking if a persona can learn a level up move,
		/// and making it happen if it can.
		/// </summary>
		/// <param name="persona"></param>
		private void PersonaLevelUpMoveCheck(Persona persona) {
			//
			//	Check for level up move.
			//
			if (persona.levelUpMoves.Count > 0) {
				//
				//	Persona has level up move. Check if it can learn it.
				//
				if (persona.levelUpMoves.Peek().level <= persona.Level) {
					BattleBehavior behavior = persona.levelUpMoves.Peek().behavior;
					CombatantAnalysisCanvas.instance.AddBehavior(
						persona: persona,
						behavior: behavior,
						startCallback: new TweenCallback(delegate { }),
						finishCallback: new TweenCallback(delegate {
							this.PersonaLevelUpMoveCheck(persona: persona);
						}),
						context: CombatantAnalysisCanvas.ContextType.PersonaLevelUp);

				} else {
					//
					//	Persona has moves, but cannot learn them.
					//
					this.SetHiddenButtonActive(true);
				}
			} else {
				//
				//	Persona does not have any level up moves.
				//
				this.SetHiddenButtonActive(true);
			}
		}
		#endregion

		#region HIDDEN BUTTON
		/// <summary>
		/// Gets called when the hidden button is hit.
		/// </summary>
		public void DequeueNextLeveledUpPersona() {
			// Turn off the hidden button to prevent further events.
			this.SetHiddenButtonActive(false);

			// If there are more personas in the queue, dequeue the next and level it up.
			if (this.leveledUpPersonaQueue.Count > 0) {
				this.PersonaLevelUpEvent(persona: this.leveledUpPersonaQueue.Dequeue().combatant as Persona);
			} else {
				// Tween Out if there is nothing left.
				this.TweenLevelUpScreenOut();
				// If there are no more personas that leveled up, close out the combatant analysis canvas.
				CombatantAnalysisCanvas.instance.ResetAndClose();
				// Run the finish callback when done.
				LevelUpScreenController.currentFinishCallback();
			}

		}
		/// <summary>
		/// Sets whether the hidden button should be activated or not.
		/// </summary>
		/// <param name="status"></param>
		private void SetHiddenButtonActive(bool status) {
			this.invisibleButton.gameObject.SetActive(status);
			if (status == true) {
				EventSystem.current.SetSelectedGameObject(this.invisibleButton.gameObject);
			}
		}
		#endregion

		#region DEBUG
		/// <summary>
		/// Just sets the thumbnails to the correct offset amount.
		/// </summary>
		[ShowInInspector, ShowIf("debugMode"), TabGroup("Level Up Screen", "Debug")]
		private void UpdateThumbnailPositions() {
			// Go through each thumbnail and remember the position.
			for(int i = 0; i < this.combatantLevelUpThumbnails.Count; i++) {
				this.initialThumbnailPositions[i] = new Vector2(
					x: this.combatantLevelUpThumbnails[i].transform.localPosition.x,
					y: 0 + (this.thumbnailVerticalSpacingAmount * i));
				this.combatantLevelUpThumbnails[i].transform.localPosition = this.initialThumbnailPositions[i];
			}
		}
		/// <summary>
		/// Just a simple way of tweening in.
		/// </summary>
		[ShowInInspector, ShowIf("debugMode"), TabGroup("Level Up Screen", "Debug")]
		private void TestTweenIn() {
			this.TweenLevelUpScreenIn();
		}
		#endregion

	}


}