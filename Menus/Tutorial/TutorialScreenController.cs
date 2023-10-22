using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using UnityStandardAssets.ImageEffects;

namespace Grawly.UI {

	/// <summary>
	/// The class that provides access to the tutorial screen and the  
	/// </summary>
	public class TutorialScreenController : MonoBehaviour {

		public static TutorialScreenController instance;

		#region FIELDS - STATE
		/// <summary>
		/// The index of the screen that is currently being shown.
		/// Should always get reset to zero when leaving.
		/// </summary>
		private int currentScreenIndex = 0;
		/// <summary>
		/// The template that is currently being used for the tutorial.
		/// </summary>
		private TutorialTemplate currentTemplate;
		/// <summary>
		/// The GameObject that should be selected when the tutorial closes out.
		/// </summary>
		private GameObject currentGameObjectToReselectOnClose;
		/// <summary>
		/// The action that should be run when the tutorial is closed out.
		/// </summary>
		private Action currentActionOnClose;
		#endregion

		#region FIELDS - EASING
		/// <summary>
		/// The time it should take for the flasher to do its thing.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Toggles")]
		private float blurFadeTime = 0.5f;
		/// <summary>
		/// The ease type to use when tweening the top/bottom bars
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Toggles")]
		private Ease barEaseType = Ease.Linear;
		/// <summary>
		/// The time it should take for the top bars to swooce right in.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Toggles")]
		private float barMoveTime = 0.5f;
		/// <summary>
		/// The ease to use when transitioning between two different thumbnail sprites.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Toggles")]
		private Ease thumbnailTurnEaseType = Ease.Linear;
		/// <summary>
		/// The amount of time to take when turning the thumbnail image.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Toggles")]
		private float thumbnailTurnTime = 0.5f;
		/// <summary>
		/// The amount that the thumbnails should move when tweening.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Toggles")]
		private float thumbnailTurnMagnitude = 200f;
		/// <summary>
		/// The ease typing to use when tweening the tutorial backing in.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Toggles")]
		private Ease tutorialBackingEaseInType = Ease.OutBack;
		/// <summary>
		/// The ease typing to use when tweening the tutorial backing out.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Toggles")]
		private Ease tutorialBackingEaseOutType = Ease.InBack;
		/// <summary>
		/// The amount of time it should take to tween the tutorial backing in/out.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Toggles")]
		private float tutorialBackingTweenTime = 0.3f;
		#endregion

		#region FIELDS - INITIAL POSITIONS
		/// <summary>
		/// The initial position for where the thumbnail image is placed.
		/// </summary>
		private Vector2 initialThumbnailImagePosition;
		#endregion

		#region FIELDS - HIDDEN SELECTABLES
		/// <summary>
		/// The selectable that should be used for receiving events from the UI system.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private Selectable hiddenCenterSelectable;
		/// <summary>
		/// The selectable that should be used for receiving events from the UI system.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private Selectable hiddenLeftSelectable;
		/// <summary>
		/// The selectable that should be used for receiving events from the UI system.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private Selectable hiddenRightSelectable;
		/// <summary>
		/// The GameObject that works as an Okay button. There's actually no Selectable on this. It's entirely just for Effects.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private GameObject okayButtonGameObject;
		#endregion

		#region FIELDS - SCENE REFERENCES : MAIN VISUALS
		/// <summary>
		/// Contains all other scene references as children so they can easily be turned on and off.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private GameObject allGameObjects;
		/// <summary>
		/// The RectTransform that contains all the visuals for the primary thumbnail image.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private RectTransform tutorialScreenPrimaryThumbnailRectTransform;
		/// <summary>
		/// The image that shows the thumbnail of whatever screen of the tutorial is currently displayed.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private Image tutorialScreenPrimaryThumbnailImage;
		/// <summary>
		/// The RectTransform that contains all the visuals for the secondary thumbnail image.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private RectTransform tutorialScreenSecondaryThumbnailRectTransform;
		/// <summary>
		/// The image that shows the thumbnail of the previous screen. Gets used every time an animation plays.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private Image tutorialScreenSecondaryThumbnailImage;
		/// <summary>
		/// The SuperTextMesh used for the paragraph inside the tutorial screen..
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private SuperTextMesh tutorialScreenParagraphLabel;
		/// <summary>
		/// The SuperTextMesh used for the title above the paragraph.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private SuperTextMesh tutorialScreenTitleLabel;
		/// <summary>
		/// The image that darkens the background when in use.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private Image backgroundDarkenImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : NAVIGATION BAR
		/// <summary>
		/// The GameObjcect that contains all the visuals for the navigation bar.
		/// Important if there is more than one screen.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private GameObject navigationBarObjects;
		/// <summary>
		/// The image for the left navigation arrow.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private GameObject leftNavigationArrowGameObject;
		/// <summary>
		/// The image for the right navigation arrow image.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private GameObject rightNavigationArrowGameObject;
		/// <summary>
		/// The dots that get shown to indicate what the current page is.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private List<Image> navigationDotImages = new List<Image>();
		#endregion

		#region FIELDS - SCENE REFERENCES : DECORATION
		/// <summary>
		/// The bar that shows up at the top of the screen.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private Image topBarImage;
		/// <summary>
		/// The bar that shows up at the bottom of the screen.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private Image bottomBarImage;
		/// <summary>
		/// The RectTransform of the Image that shows the backing for the thumbnail/text.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private RectTransform tutorialBackingRectTransform;
		#endregion

		#region FIELDS - DEBUG
		/// <summary>
		/// Is debug mode on?
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Debug")]
		private bool debugMode = false;
		/// <summary>
		/// The template to use in debug mode.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Debug")]
		private TutorialTemplate debugTemplate;
		#endregion


		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				ResetController.AddToDontDestroy(this.gameObject);
				// DontDestroyOnLoad(this.gameObject);
				this.initialThumbnailImagePosition = this.tutorialScreenPrimaryThumbnailRectTransform.anchoredPosition;
				// Reset the visuals.
				this.ResetTutorialController();
			} 
		}
		#endregion

		#region TUTORIAL DISPLAY
		/// <summary>
		/// Just resets the position of all the tutorial controller junk so that it can prepare for animatinos owhafgh
		/// </summary>
		private void ResetTutorialController() {
			// Set the positions of the top/bottom bars.
			this.topBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: this.topBarImage.GetComponent<RectTransform>().anchoredPosition.x, y: 175f);
			this.bottomBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: this.bottomBarImage.GetComponent<RectTransform>().anchoredPosition.x, y: -175f);
			// Reset the scale of the visuals.
			this.allGameObjects.transform.localScale = Vector3.zero;
			// Kill any tweens on the darkening image and also make it clear.
			this.backgroundDarkenImage.DOKill(complete: true);
			this.backgroundDarkenImage.color = Color.clear;
		}
		/// <summary>
		/// Opens the tutorial window.
		/// </summary>
		/// <param name="tutorialTemplate">The tutorial template that should be used.</param>
		/// <param name="objectToReselectOnClose">The object to reselect on close. Can be null.</param>
		/// <param name="actionOnClose">The action to run on close. Can be null.</param>
		public static void OpenTutorial(TutorialTemplate tutorialTemplate, GameObject objectToReselectOnClose = null, Action actionOnClose = null) {
			// If the Instance does not exist, load up the scene.
			if (instance == null) {
				Debug.LogWarning("TUTORIAL: I'm doing something very weird where I wait a hard 0.5 seconds before calling the tutorial and I don't check if it's actually loaded or not. This may be a problem?");
				SceneManager.LoadScene(sceneName: "Tutorial Scene", mode: LoadSceneMode.Additive);
				// Wait half a second and then open it up.
				GameController.Instance.WaitThenRun(timeToWait: 0.5f, action: delegate {
					instance.DisplayTutorial(tutorialTemplate: tutorialTemplate, objectToReselectOnClose: objectToReselectOnClose, actionOnClose: actionOnClose);
				});
			} else {
				// If the Instance does exist, just open it.
				instance.DisplayTutorial(tutorialTemplate: tutorialTemplate, objectToReselectOnClose: objectToReselectOnClose, actionOnClose: actionOnClose);
			}
		}
		/// <summary>
		/// Shows the tutorial screen.
		/// </summary>
		/// <param name="tutorialTemplate">The template containing the tutorial to show to the player.</param>
		/// <param name="objectToReselectOnClose">The GameObject that should be selected when the tutorial closes. Very important for shit like menus.</param>
		private void DisplayTutorial(TutorialTemplate tutorialTemplate, GameObject objectToReselectOnClose = null, Action actionOnClose = null) {

			// Enable all the selectables. Adding this 4/9/19
			// this.EnableTutorialSelectables(true);

			// Play a funny sound effect.
			AudioController.instance?.PlaySFX(SFXType.Pause, scale: 0.7f);

			// Reset the screen index and save a reference to the tutorial template.
			this.currentScreenIndex = 0;
			this.currentTemplate = tutorialTemplate;
			// Save references to the shit I will need to know for when I close out.
			this.currentGameObjectToReselectOnClose = objectToReselectOnClose;
			this.currentActionOnClose = actionOnClose;

			// Completely null out the currently selected gameobject.
			EventSystem.current.SetSelectedGameObject(null);

			// Reset the tutorial controller visuals.
			this.ResetTutorialController();

			// If set to darken the background, do so.
			if (tutorialTemplate.DarkenBackground == true) {
				this.backgroundDarkenImage.DOFade(endValue: .95f, duration: 0.5f);
				// this.backgroundDarkenImage.DOColor(endValue: new Color(0f, 0f, 0f, 230f),duration: 0.5f);
			}
			
			// Blur the camera.
			if (Battle.BattleController.Instance != null) {
				Battle.BattleCameraController.Instance.MainCamera.GetComponent<BlurOptimized>().enabled = true;
				DOTween.To(
					getter: () => Battle.BattleCameraController.Instance.MainCamera.GetComponent<BlurOptimized>().blurSize,
					setter: x => Battle.BattleCameraController.Instance.MainCamera.GetComponent<BlurOptimized>().blurSize = x,
					endValue: 3f,
					duration: this.blurFadeTime);
			}
			/*Battle.BattleCameraController.instance.MainCamera.GetComponent<BlurOptimized>().enabled = true;
			DOTween.To(
				getter: () => Battle.BattleCameraController.instance.MainCamera.GetComponent<BlurOptimized>().blurSize,
				setter: x => Battle.BattleCameraController.instance.MainCamera.GetComponent<BlurOptimized>().blurSize = x,
				endValue: 3f,
				duration: this.blurFadeTime);*/
			


			// TWEEN THE BARS IN
			this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 42f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -60f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);

			// TWEEN THE BACKING IN
			// this.tutorialBackingRectTransform.DOScale(endValue: 1f, duration: this.tutorialBackingTweenTime).SetEase(ease: this.tutorialBackingEaseType);
			this.allGameObjects.transform.DOScale(endValue: 1f, duration: this.tutorialBackingTweenTime).SetEase(ease: this.tutorialBackingEaseInType);

			// Set the title text.
			this.SetTitleText(titleString: this.currentTemplate.TutorialName);

			// Set the paragraph text to the first string.
			this.SetParagraphText(paragraphString: this.currentTemplate.TutorialExplainationScreens.First().paragraphText);

			// Set the sprites on the primary/secondary thumbnails.
			this.SetPrimaryImageSprite(sprite: this.currentTemplate.TutorialExplainationScreens.First().sprite);
			this.SetSecondaryImageSprite(sprite: this.currentTemplate.TutorialExplainationScreens.First().sprite);

			// If there is only one screen, turn off the nav bar.
			if (this.currentTemplate.TutorialExplainationScreens.Count == 1) {
				this.navigationBarObjects.SetActive(false);
			} else {
				// If there are multiple screens, turn the navigation bar on and call refresh.
				this.navigationBarObjects.SetActive(true);
				this.RefreshNavigationBar(
					currentIndex: this.currentScreenIndex,
					totalScreens: this.currentTemplate.TutorialExplainationScreens.Count);
			}


			// Enable the Okay button for now.
			// this.okayButtonGameObject.SetActive(true);

			// Select the hidden button.
			this.ReselectCenterSelectable();
		}
		/// <summary>
		/// Tweens the tutorial screen out.
		/// </summary>
		private void DismissTutorial() {

			// Disable all the selectables. Adding this 4/9/19
			// this.EnableTutorialSelectables(false);

			// TWEEN THE BARS OUT
			this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);

			// Kill any tweens on the darkening background and make it clear again.
			this.backgroundDarkenImage.DOKill(complete: true);
			this.backgroundDarkenImage.DOFade(endValue: 0f, duration: 0.5f);
			// this.backgroundDarkenImage.DOColor(endValue: Color.clear, duration: 0.5f);
			
			// Deblur the battle camera.
			if (Battle.BattleCameraController.Instance != null) {
				DOTween.To(
						getter: () => Battle.BattleCameraController.Instance.MainCamera.GetComponent<BlurOptimized>().blurSize,
						setter: x => Battle.BattleCameraController.Instance.MainCamera.GetComponent<BlurOptimized>().blurSize = x,
						endValue: 0f,
						duration: this.blurFadeTime)
					.onComplete = new TweenCallback(delegate {
					Battle.BattleCameraController.Instance.MainCamera.GetComponent<BlurOptimized>().enabled = false;
				});
			}
			/*DOTween.To(
								getter: () => Battle.BattleCameraController.instance.MainCamera.GetComponent<BlurOptimized>().blurSize,
								setter: x => Battle.BattleCameraController.instance.MainCamera.GetComponent<BlurOptimized>().blurSize = x,
								endValue: 0f,
								duration: this.blurFadeTime)
								.onComplete = new TweenCallback(delegate {
									Battle.BattleCameraController.instance.MainCamera.GetComponent<BlurOptimized>().enabled = false;
								});*/


			// SET THE NAVIGATION BAR OFF
			this.navigationBarObjects.SetActive(false);

			// TWEEN THE BACKING OUT
			this.allGameObjects.transform.DOScale(endValue: 0f, duration: this.tutorialBackingTweenTime).SetEase(ease: this.tutorialBackingEaseOutType);

			// Reselect the gameobject that needs to be reselected. This is important for shit like menus or whatever.
			if (this.currentGameObjectToReselectOnClose != null) {	// Only do this if it is not null, however.
				GameController.Instance?.WaitThenRun(timeToWait: 0.2f, action: delegate {
					EventSystem.current.SetSelectedGameObject(this.currentGameObjectToReselectOnClose);
				});
			}

			// Run the action that should be used on close.
			this.currentActionOnClose?.Invoke();

		}
		#endregion

		#region STATE
		/// <summary>
		/// Transitions the tutorial screen to the next screen in the template.
		/// </summary>
		/// <param name="newScreenIndex">The index to transition to.</param>
		private void TransitionScreen(int newScreenIndex) {

			// Play the hover sound effect.
			AudioController.instance?.PlaySFX(SFXType.Hover);

			// Grab the next screen that needs to be shown on the screen.
			TutorialScreen nextScreen = this.currentTemplate.TutorialExplainationScreens[newScreenIndex];
			// Also grab the old screen for the sake of clarity.
			TutorialScreen currentScreen = this.currentTemplate.TutorialExplainationScreens[this.currentScreenIndex];

			// Refresh the navigation bar.
			this.RefreshNavigationBar(currentIndex: newScreenIndex, totalScreens: this.currentTemplate.TutorialExplainationScreens.Count);

			// Refresh the paragraph text.
			this.SetParagraphText(paragraphString: nextScreen.paragraphText);

			// Check to see if the thumbnail sprites are different at all.
			if (currentScreen.sprite != nextScreen.sprite) {
				// If they are, set the secondary screen/primary screen sprites and tween them.
				this.SetPrimaryImageSprite(sprite: nextScreen.sprite);
				this.SetSecondaryImageSprite(sprite: currentScreen.sprite);
				// Switch directions depending on if the new index is higher or lower than the current index.
				if (newScreenIndex > this.currentScreenIndex) {
					this.AnimateThumbnailTurn(directionType: ThumbnailTransitionDirectionType.Right, easeType: this.thumbnailTurnEaseType);
				} else {
					this.AnimateThumbnailTurn(directionType: ThumbnailTransitionDirectionType.Left, easeType: this.thumbnailTurnEaseType);
				}
			}

			// Overwrite the current index.
			this.currentScreenIndex = newScreenIndex;

		}
		#endregion

		#region VISUALS
		/// <summary>
		/// Sets the sprite on the primary image.
		/// </summary>
		/// <param name="sprite">The sprite to put on the primary thumbnail.</param>
		private void SetPrimaryImageSprite(Sprite sprite) {
			// this.tutorialScreenPrimaryThumbnailImage.sprite = sprite;
			this.tutorialScreenPrimaryThumbnailImage.overrideSprite = sprite;
		}
		/// <summary>
		/// Sets the sprite on the secondary image.
		/// </summary>
		/// <param name="sprite">The sprite to put on the secondary thumbnail.</param>
		private void SetSecondaryImageSprite(Sprite sprite) {
			// this.tutorialScreenSecondaryThumbnailImage.sprite = sprite;
			this.tutorialScreenSecondaryThumbnailImage.overrideSprite = sprite;
		}
		/// <summary>
		/// Sets the text on the title label.
		/// </summary>
		/// <param name="titleString">The string to use.</param>
		private void SetTitleText(string titleString) {
			this.tutorialScreenTitleLabel.Text = titleString;
		}
		/// <summary>
		/// Sets the text on the tutorial paragraph.
		/// </summary>
		/// <param name="paragraphString"></param>
		private void SetParagraphText(string paragraphString) {
			this.tutorialScreenParagraphLabel.Text = paragraphString;
		}
		/// <summary>
		/// Refreshes the navigation bar's visuals to be up 
		/// </summary>
		/// <param name="currentIndex"></param>
		/// <param name="totalScreens"></param>
		private void RefreshNavigationBar(int currentIndex, int totalScreens) {

			// Deactivate all the navigation images.
			this.navigationDotImages.ForEach(i => i.gameObject.SetActive(false));

			// Take only the number of ones that I need and set them active again and make them black.
			this.navigationDotImages.Take(count: totalScreens).ToList().ForEach(i => i.gameObject.SetActive(true));
			this.navigationDotImages.Take(count: totalScreens).ToList().ForEach(i => i.color = Color.black);

			// Make the currently highlighted index's dot burple.
			this.navigationDotImages[currentIndex].color = GrawlyColors.colorDict[GrawlyColorTypes.Purple];

			// Enable the left arrow if the current index is not the first.
			leftNavigationArrowGameObject.SetActive(value: (currentIndex == 0) == false);

			// Enable the right arrow if the current index is not the last.
			rightNavigationArrowGameObject.SetActive(value: (currentIndex == totalScreens - 1) == false);


			// Enable the Okay button if the current screen IS the last.
			this.okayButtonGameObject.SetActive((currentIndex == totalScreens - 1) == true);

		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the thumbnails transitioning.
		/// </summary>
		/// <param name="directionType">The direction to transition the thumbnails.</param>
		private void AnimateThumbnailTurn(ThumbnailTransitionDirectionType directionType, Ease easeType) {
			switch (directionType) {
				case ThumbnailTransitionDirectionType.Left:
					this.AnimateThumbnailTurn(endPosition: this.thumbnailTurnMagnitude, tweenTime: this.thumbnailTurnTime, easeType: this.thumbnailTurnEaseType);
					break;
				case ThumbnailTransitionDirectionType.Right:
					this.AnimateThumbnailTurn(endPosition: -this.thumbnailTurnMagnitude, tweenTime: this.thumbnailTurnTime, easeType: this.thumbnailTurnEaseType);
					break;
				default:
					throw new System.Exception("This shouldn't be possible.");
			}
		}
		/// <summary>
		/// Animates the thumbnails transitioning.
		/// </summary>
		/// <param name="endPosition">The magnitude of how they should transition.</param>
		private void AnimateThumbnailTurn(float endPosition, float tweenTime, Ease easeType) {

			// Reset the positions of both the primary and secondary thumbnails.
			this.tutorialScreenPrimaryThumbnailRectTransform.anchoredPosition = this.initialThumbnailImagePosition;
			this.tutorialScreenSecondaryThumbnailRectTransform.anchoredPosition = this.initialThumbnailImagePosition;

			// Set the secondary thumbnail's color to be white so it appears again.
			this.tutorialScreenSecondaryThumbnailImage.color = Color.white;

			// Tween the secondary thumbnail out.
			this.tutorialScreenSecondaryThumbnailRectTransform.DOAnchorPosX(endValue: endPosition, duration: tweenTime, snapping: true).SetEase(ease: easeType);
			this.tutorialScreenSecondaryThumbnailImage.DOColor(endValue: Color.clear, duration: tweenTime);

		}
		#endregion

		#region UI EVENT HANDLERS
		/// <summary>
		/// Gets called when a Select event is received from the left/right invisible buttons.
		/// </summary>
		/// <param name="indexIncrementValue"></param>
		public void SelectedHiddenSelectable(int indexIncrementValue) {
			// Run the routine to select the center selectable.
			this.ReselectCenterSelectable();
			// Transition the screen.
			this.TransitionScreen(newScreenIndex: Mathf.Clamp(
				value: (this.currentScreenIndex + indexIncrementValue), 
				min: 0, 
				max: this.currentTemplate.TutorialExplainationScreens.Count-1));
		}
		/// <summary>
		/// Gets called when the center selectable receives a Submit event.
		/// </summary>
		public void SubmitEventFromCenterSelectable() {
			if (this.okayButtonGameObject.activeInHierarchy == true) {
				AudioController.instance?.PlaySFX(SFXType.Select);
				// For right now, just dismiss the tutorial.
				this.DismissTutorial();
			}
		}
		#endregion

		#region UI EVENT SETUP
		/// <summary>
		/// Reselects the center button.
		/// </summary>
		private void ReselectCenterSelectable() {
			this.StartCoroutine(this.ReselectCenterSelectableRoutine());
		}
		/// <summary>
		/// This needs to be run as a routine bc of stupid unity event bullshit.
		/// </summary>
		private IEnumerator ReselectCenterSelectableRoutine() {
			yield return null;
			EventSystem.current.SetSelectedGameObject(this.hiddenCenterSelectable.gameObject);
		}
		/// <summary>
		/// Sets the selectability of the left/right invisible buttons.
		/// </summary>
		/// <param name="enableLeftSelectable">Is the left button selectable?</param>
		/// <param name="enableRightSelectable">Is the right button selectable?</param>
		private void EnableHiddenTurnSelectables(bool enableLeftSelectable, bool enableRightSelectable) {
			// I'm setting both. I'm not gonna fuck with that.
			this.hiddenLeftSelectable.enabled = enableLeftSelectable;
			this.hiddenLeftSelectable.gameObject.SetActive(value: enableLeftSelectable);

			this.hiddenRightSelectable.enabled = enableRightSelectable;
			this.hiddenRightSelectable.gameObject.SetActive(value: enableRightSelectable);
		}
		#endregion

		#region DEBUG 
		/// <summary>
		/// A debug method that just prepares the debug tutorial.
		/// </summary>
		[ShowInInspector, TabGroup("Tutorial Screen", "Debug"), HideInEditorMode]
		private void DebugPrepareTutorial() {
			// Just call the debug tutorial template.
			this.DisplayTutorial(tutorialTemplate: this.debugTemplate);
		}
		#endregion

		#region ENUM DEFINITIONS
		/// <summary>
		/// I probably don't need this but love too strict typeing lol.
		/// </summary>
		private enum ThumbnailTransitionDirectionType {
			Left = 0,
			Right = 1,
		}
		#endregion



	}


}