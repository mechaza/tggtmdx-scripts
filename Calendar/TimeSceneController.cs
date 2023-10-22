using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Grawly.Calendar {

	/// <summary>
	/// Similar to the CalendarSceneController but mostly just for the time transition.
	/// </summary>
	public class TimeSceneController : MonoBehaviour {

		public static TimeSceneController instance;

		#region FIELDS - STATE
		/// <summary>
		/// When loading a scene, there may be a race condition between when the scene finally loads and when the animation tweening the text has finished.
		/// This bool is set to True by the callback when the scene manager finally loads up.
		/// </summary>
		private bool readyToProceed = false;
		#endregion

		#region FIELDS - EASING
		/// <summary>
		/// The ease type to use when tweening the stuff.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Settings")]
		private Ease easeType = Ease.Linear;
		/// <summary>
		/// The time it should take for the flasher to do its thing.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Settings")]
		private float flasherFadeTime = 0.5f;
		/// <summary>
		/// The time it should take for the top bars to swooce right in.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Settings")]
		private float barMoveTime = 0.5f;
		/// <summary>
		/// The amount of time that should be waited before allowing the time object to come in.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Settings")]
		private float timeOfDayWaitTime = 0.5f;
		/// <summary>
		/// The amount of time that it should take for the time of day object to move in.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Settings")]
		private float timeOfDayMoveTime = 1.5f;
		/// <summary>
		/// The amount of time that it should take for the time text to do their thing.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Settings")]
		private float textMoveTime = 0.5f;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The image that should flash when transitioning.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private Image flasherImage;
		/// <summary>
		/// The bar that shows up at the top of the screen.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private Image topBarImage;
		/// <summary>
		/// The bar that shows up at the bottom of the screen.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private Image bottomBarImage;
		/// <summary>
		/// The entire object that moves across the screen.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private RectTransform timeOfDayRectTransform;
		/// <summary>
		/// The object that contains the texts that get used for start/end times.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private RectTransform textLabelsRectTransform;
		/// <summary>
		/// The STM that shows the time of day that is being transitioned FROM.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private Text startTimeText;
		/// <summary>
		/// The STM that shows the time of day that is being transitioned TO.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private Text endTimeText;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				// Need to call dont destroy on load because even if I load this game object in an additive scene, it will be destroyed in any other non-additive load scene call.
				// I'll manually destroy it later.
				ResetController.AddToDontDestroy(this.gameObject);
				// DontDestroyOnLoad(this.gameObject);
			}
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Preps the flasher into its default locations.
		/// </summary>
		private void Prepare(TimeOfDayType startTime, TimeOfDayType endTime) {
			// Set the state to be Not Ready. This will become True when the scene is loaded up.
			this.readyToProceed = false;
			// Reset the bar positions.
			this.topBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0, y: 175f);
			this.bottomBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0, y: -175f);
			// Fade the flasher out.
			this.flasherImage.CrossFadeAlpha(alpha: 0f, duration: 0f, ignoreTimeScale: true);
			// Set the time of day rect transform to the far left of the screen.
			this.timeOfDayRectTransform.anchoredPosition = new Vector2(x: -1500f, y: 0f);
			// Make sure the text is reset in its positions.
			this.textLabelsRectTransform.anchoredPosition = Vector2.zero;
			// Set the labels on those texts.
			this.startTimeText.text = startTime.ToString();
			this.endTimeText.text = endTime.ToString();
		}
		#endregion

		#region FADING
		/// <summary>
		/// Fades into the screen and shows the time animation. This can be thought of analogous to the flasher.
		/// </summary>
		/// <param name="startTime">The time that should be shown as the time transitioning FROM.</param>
		/// <param name="endTime">The time that should be shown as transitioning TO.</param>
		public void FadeOut(TimeOfDayType startTime, TimeOfDayType endTime) {

			// Prep the locations/initial positions of the misc scene elements.
			this.Prepare(startTime: startTime, endTime: endTime);
			// Tween the top bar in.
			this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 42f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.easeType);
			// Tween the bottom bar in.
			this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -60f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.easeType);
			// Fade the flasher image in.
			this.flasherImage.CrossFadeAlpha(alpha: 1f, duration: this.flasherFadeTime, ignoreTimeScale: true);

			// Start up a new sequence.
			Sequence seq = DOTween.Sequence();
			// Wait a few moments before bringing in the time of day.
			seq.AppendInterval(interval: this.timeOfDayWaitTime);
			// Bring the time of day in.
			seq.AppendCallback(new TweenCallback(delegate {
				// Tween timeOfDayObject into the center of the screen.
				this.timeOfDayRectTransform.DOAnchorPosX(endValue: 0f, duration: this.timeOfDayMoveTime, snapping: true).SetEase(ease: this.easeType);
			}));
			// Pause for a moment, and scroll the startTimeText and endTimeText
			seq.AppendInterval(2f);
			seq.AppendCallback(new TweenCallback(delegate {
				this.textLabelsRectTransform.DOAnchorPosY(endValue: 220f, duration: this.textMoveTime, snapping: true).SetEase(ease: this.easeType);
			}));
			seq.AppendInterval(interval: this.textMoveTime);
			// Once all of that is done, start up the routine that checks for whether the scene has loaded or not.
			seq.AppendCallback(new TweenCallback(delegate {
				this.StartCoroutine(this.WaitForSceneToLoad());
			}));
			seq.Play();
		}
		/// <summary>
		/// Fades the transition out from the screen. This can be thought of as analogous to the flasher.
		/// </summary>
		public void FadeIn(bool unloadOnFinish = false) {

			// Complete the text tween if it hasn't done so already.
			Debug.LogWarning("Fix this so it seamlessly switches the label properly.");
			this.textLabelsRectTransform.DOComplete();

			// Start up a new Fade In Sequence.
			Sequence fadeInSeq = DOTween.Sequence();
			// fadeInSeq.AppendInterval(interval: 1.5f);

			fadeInSeq.AppendCallback(new TweenCallback(delegate {
				// Tween timeOfDayObject to the right of the screen.
				this.timeOfDayRectTransform.DOAnchorPosX(endValue: 1500f, duration: this.timeOfDayMoveTime, snapping: true).SetEase(ease: this.easeType);
			}));
			// Wait a hot moment before tweening the rest.
			fadeInSeq.AppendInterval(interval: this.timeOfDayMoveTime);
			fadeInSeq.AppendCallback(new TweenCallback(delegate {
				// Fade the flasher image out.
				this.flasherImage.CrossFadeAlpha(alpha: 0f, duration: this.flasherFadeTime, ignoreTimeScale: true);
				// Tween the top bar out.
				this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.easeType);
				// Tween the bottom bar out.
				this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.easeType);
			}));
			fadeInSeq.AppendInterval(1f);
			fadeInSeq.onComplete = new TweenCallback(delegate {
				// When the sequence is finished, if specified to unload, do so.
				if (unloadOnFinish == true) {
					Debug.Log("UNLOAD ON FINISH");
					Destroy(this.gameObject);
					// SceneManager.UnloadSceneAsync(sceneName: "Time Change Scene");
				}
			});
			fadeInSeq.Play();
		}
		#endregion

		#region WAITING ROUTINE
		/// <summary>
		/// Effectively the second half of FadeOut, but continuously checks for whether or not the scene was loaded properly. 
		/// When readyToProceed is set in TimeTransitionOnSceneLoaded, FadeIn will be called.
		/// </summary>
		/// <returns></returns>
		private IEnumerator WaitForSceneToLoad() {
			yield return null;
			// Keep checking for whether or not the scene has been loaded or not. 
			while (this.readyToProceed == false) {
				yield return null;
			}

			// If this has been reached, it means that TimeTransitionOnSceneLoaded was called. Fade In.
			this.FadeIn(unloadOnFinish: true);
		}
		#endregion

		#region SCENE LOADED EVENTS
		/// <summary>
		/// Gets cvalled when a scene is finished loading. Used in the TimeTransitionLoadSceneRoutine.
		/// Note that the SceneController I have is referencing this directly. I have it here instead of there
		/// because I want to set the readyToProceed variable, which is private.
		/// </summary>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		public void TimeTransitionOnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
			Debug.Log("Destination scene loaded. Unloading time transition scene.");
			
			// Set ready to proceed to true. This will make the WaitForSceneToLoad routine stop.
			this.readyToProceed = true;
			// Remove this callback from the scene manager.
			SceneManager.sceneLoaded -= this.TimeTransitionOnSceneLoaded;
		}
		#endregion

		#region DEBUGGING
		/// <summary>
		/// Just tests to make sure the animation works.
		/// </summary>
		[ShowInInspector, TabGroup("Debug","Debug"), HideInEditorMode]
		public void TestAnimation() {
			// Start up an animation.
			Sequence animationSeq = DOTween.Sequence();
			// Add a callback to "fade out."
			animationSeq.AppendCallback(new TweenCallback(delegate {
				this.FadeOut(startTime: TimeOfDayType.Afternoon, endTime: TimeOfDayType.Evening);
			}));
			// Fake a load time.
			animationSeq.AppendInterval(5f);
			// Fade back in.
			animationSeq.AppendCallback(new TweenCallback(delegate {
				this.FadeIn();
			}));
			animationSeq.Play();
		}
		#endregion

	}

}