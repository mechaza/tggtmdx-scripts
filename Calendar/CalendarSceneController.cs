using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Grawly.Calendar {

	/// <summary>
	/// The script that controls what should happen when the CalendarScene is loaded.
	/// </summary>
	public class CalendarSceneController : MonoBehaviour {

		public static CalendarSceneController instance;

		#region FIELDS - STATE
		/// <summary>
		/// The scene to load when this transition is finished.
		/// </summary>
		private static string sceneToLoad = "";
		/// <summary>
		/// The callback to run when the scene is loaded.
		/// </summary>
		private static System.Action onSceneLoadCallback;
		/// <summary>
		/// The scene to load when this transition is finished.
		/// </summary>
		public static string SceneToLoad {
			get {
				return sceneToLoad;
			} set {
				Debug.Log("Setting scene to load: " + value);
				sceneToLoad = value;
			}
		}
		/// <summary>
		/// The callback to run when the scene is loaded.
		/// </summary>
		public static System.Action OnSceneLoadCallback {
			get {
				return onSceneLoadCallback;
			}
			set {
				Debug.Log("Assigning an OnSceneLoadCallback.");
				onSceneLoadCallback = value;
			}
		}
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should this run in debug mode? If it does, it will populate the scene with a start index date of 0.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Debug")]
		private bool debugMode = false;
		/// <summary>
		/// If in debug mode, what day should be used for focusing when building out the icons?
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Debug"), ShowIf("debugMode")]
		private int dayOfFocus = 0;
		/// <summary>
		/// The "day of focus" that should start the animation.
		/// </summary>
		private int DayOfFocus {
			get {
				if (debugMode == true) {
					return this.dayOfFocus;
				} else {
					return GameController.Instance.Variables.CurrentDayNumber - 1;
				}
			}
		}
		/// <summary>
		/// The ease type to use for the calendar transition when it slides.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Debug"), ShowIf("debugMode")]
		private Ease slideEaseType = Ease.Linear;
		/// <summary>
		/// The ease type to use for the calendar transition when it scales.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Debug"), ShowIf("debugMode")]
		private Ease scaleEaseType = Ease.Linear;
		/// <summary>
		/// How much should the values move vertically? Also sets the initial angle of rotation of the "line."
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Debug"), ShowIf("debugMode")]
		private float verticalOffset = 150f;
		/// <summary>
		/// The data that should be read from get getting this to work.
		/// Only relevant if I don't have other CalendarData to pull from.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Debug"), ShowIf("debugMode")]
		private CalendarData debugCalendarData;
		/// <summary>
		/// The calendar data that is being read from. May need overriding if running in debug mode.
		/// </summary>
		private CalendarData CalendarData {
			get {
				if (debugMode == true) {
					return this.debugCalendarData;
				} else {
					return CalendarController.Instance.CalendarData;
				}
			}
		}
		/// <summary>
		/// A collection of locations where the icons are by default. Helpful if I need to reset shit while debugging animations in playmode.
		/// </summary>
		private List<Vector2> defaultPositions = new List<Vector2>();
		/// <summary>
		/// A collection of scales where the icons are by default. Helpful if I need to reset shit while debugging animations in playmode.
		/// </summary>
		private List<Vector3> defaultScales = new List<Vector3>();
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The CalendarDayIcons that are used to display information about the different calendar days.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private List<CalendarDayIcon> calendarDayIcons = new List<CalendarDayIcon>();
		/// <summary>
		/// The text that displays the month. Is on by default.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private SuperTextMesh monthText1;
		/// <summary>
		/// The text that displays the month. If there is a transition, this one fades in.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private SuperTextMesh monthText2;
		#endregion

		#region FIELDS - ASSETS
		/// <summary>
		/// External sprites to help make the calendar's weekday labels look nice.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Assets")]
		private List<Sprite> weekdaySprites = new List<Sprite>();
		#endregion

		#region UNITY FUNCTIONS
		private void Awake() {
			instance = this;

			this.defaultPositions = this.calendarDayIcons.Select(c => c.GetComponent<RectTransform>().anchoredPosition).ToList();
			this.defaultScales = this.calendarDayIcons.Select(c => c.transform.localScale).ToList();
		}
		private void Start() {
			// If debug mode is active, don't bother playing the transition animation.
			if (debugMode == true) { return; }
			this.monthText1.Text = this.CalendarData.GetDay(dayOfFocus: this.DayOfFocus).MonthNumber.ToString();
		
			// Reset the positions. This actually does use the debug data to "tilt" the icons but it's fine putting into production.
			this.ResetPositions();
			// Prep the icons with the day stored in the GameController. It SHOULD have been set to the new day prior to loading this scene.
			// (It's one less because the day was advanced in code before loading this scene as well. So, this is the day the player is coming from.
			this.PrepareCalendarDayIcons(dayOfFocus: this.DayOfFocus);


			// Create a sequence that plays the transition animation and then tells the CalendarController to load up the next scene.
			Sequence seq = DOTween.Sequence();
			seq.AppendInterval(interval: 3f);
			seq.AppendCallback(new TweenCallback(delegate {
				this.PlayTransitionAnimation();
			}));
			seq.AppendInterval(interval: 4f);
			seq.AppendCallback(new TweenCallback(delegate {
	
				// Tell the scene controller to load the specified scene.
				SceneController.instance?.LoadScene(
					sceneName: CalendarSceneController.SceneToLoad, 
					onSceneLoaded: OnSceneLoadCallback);
				
				// Reset the scene to load and the callback.
				CalendarSceneController.SceneToLoad = "";
				CalendarSceneController.OnSceneLoadCallback = null;
				
			}));
			seq.Play();

		}
		#endregion

		#region COLORING HELP
		/// <summary>
		/// Gets the string to be pre-pended to the calendar day icon's text to make the gradient look nice.
		/// </summary>
		/// <param name="calendarDay"></param>
		/// <returns></returns>
		public string GetDateColor(CalendarDay calendarDay) {

			if (calendarDay.MonthNumber == 4 && calendarDay.DayNumber == 20) {
				return "<c=greeny>";
			}

			// TODO: I'm repeating myself here kinda oops.
			// Theres something VERY similar to this in CalendarDay, except it also returns the day itself along with the color.
			switch (calendarDay.WeekdayType) {
				case CalendarWeekdayType.Sat:
					return "<c=saturday>";
				case CalendarWeekdayType.Sun:
					return "<c=sunday>";
				default:
					return "<c=weekday>";
			}
		}
		/// <summary>
		/// Gets the string to be pre-pended to the calendar day icon's text to make the gradient look nice.
		/// </summary>
		/// <param name="calendarDay"></param>
		/// <returns></returns>
		public string GetWeekDayColor(CalendarDay calendarDay) {

			if (calendarDay.MonthNumber == 4 && calendarDay.DayNumber == 20) {
				return "<c=greeny>";
			}

			switch (calendarDay.WeekdayType) {
				case CalendarWeekdayType.Sat:
					return "<c=saturday>";
				case CalendarWeekdayType.Sun:
					return "<c=sunday>";
				default:
					return "";
			}
		}
		/// <summary>
		/// Gets the color to be associated with a given CalendarDay (e.x., saturdays are blue, sundays are red.)
		/// </summary>
		/// <param name="calendarDay"></param>
		/// <returns></returns>
		public Color GetColor(CalendarDay calendarDay) {
			switch (calendarDay.WeekdayType) {
				case CalendarWeekdayType.Sat:
					return Color.blue;
				case CalendarWeekdayType.Sun:
					return Color.red;
				default:
					return Color.white;
			}
		}
		/// <summary>
		/// Gets the weekday sprite associated with a given CalendarDay.
		/// </summary>
		/// <param name="calendarDay"></param>
		/// <returns></returns>
		public Sprite GetWeekdaySprite(CalendarDay calendarDay) {
			return this.weekdaySprites[(int)calendarDay.WeekdayType];
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Plays the animation that moves the calendar to the left.
		/// </summary>
		private void PlayTransitionAnimation() {

			// Set the month text i fucking guess
			this.monthText1.Text = this.CalendarData.GetDay(dayOfFocus: this.DayOfFocus+1).MonthNumber.ToString();

			this.calendarDayIcons
				.Select(c => c.GetComponent<RectTransform>())
				.ToList()
				.ForEach(rt => {
					// The animations involved with moving the icons around may depend on which icon it is.
					// Appologies to my future self for whatever the hell this is. I love you.
					
					if (rt.GetComponent<CalendarDayIcon>() == this.calendarDayIcons[3]) {
						rt.DOScale(endValue: new Vector3(x: 1.0f, y: 1.0f), duration: 1f).SetEase(ease: this.scaleEaseType);
						rt.DOAnchorPosX(endValue: rt.anchoredPosition.x - 490f, duration: 1f, snapping: true).SetEase(this.slideEaseType);
						
					} else if (rt.GetComponent<CalendarDayIcon>() == this.calendarDayIcons[4]) {
						rt.DOScale(endValue: new Vector3(x: 1.7f, y: 1.7f), duration: 1f).SetEase(ease: this.scaleEaseType);
						rt.DOAnchorPosX(endValue: rt.anchoredPosition.x - 610f, duration: 1f, snapping: true).SetEase(this.slideEaseType);

					} else {
						rt.DOAnchorPosX(endValue: rt.anchoredPosition.x - 400f, duration: 1f, snapping: true).SetEase(this.slideEaseType);
					}

					// All of them should scale vertically the same way. Or should they?
					rt.DOAnchorPosY(endValue: rt.anchoredPosition.y - this.verticalOffset, duration: 1f, snapping: true).SetEase(this.slideEaseType);

				});

		}
		/// <summary>
		/// Preps the CalendarDayIcons with the things they need to be displayed.
		/// </summary>
		/// <param name="dayOfFocus"></param>
		private void PrepareCalendarDayIcons(int dayOfFocus) {
			// Grab the required days.

			List<CalendarDay> days = this.CalendarData.GetSurroundingDays(dayOfFocus: this.DayOfFocus, surroundingDays: 4);
			// List<CalendarDay> days = CalendarController.Instance.CalendarData.GetSurroundingDays(dayOfFocus: dayOfFocus, surroundingDays: 4);

			// Use these days to prep the calendar icons.
			int i = 0;
			this.calendarDayIcons.ForEach(c => {
				c.Prepare(calendarDay: days[i]);
				i += 1;
			});
		}
		#endregion

		#region DEBUG METHODS
		/// <summary>
		/// Builds up the calendar day icons with information from the calendar data.
		/// </summary>
		[HideInEditorMode, TabGroup("Calendar", "Debug"), ShowIf("debugMode")]
		private void DebugCalendarData() {

			this.ResetPositions();

			this.PrepareCalendarDayIcons(dayOfFocus: this.DayOfFocus);

			/*Sequence seq = DOTween.Sequence();
			seq.AppendInterval(2f);
			seq.AppendCallback(new TweenCallback(delegate {
				this.PlayTransitionAnimation();
			}));*/
			Invoke("PlayTransitionAnimation", time: 2f);

		}
		/// <summary>
		/// Just resets the popsitions of the icons.
		/// </summary>
		private void ResetPositions() {

			// Reset the locations of the icons with the data that I saved earlier in Awake().
			Enumerable.Range(start: 0, count: this.calendarDayIcons.Count).ToList().ForEach(ind => {
				this.calendarDayIcons[ind].GetComponent<RectTransform>().anchoredPosition = this.defaultPositions[ind];
				this.calendarDayIcons[ind].transform.localScale = this.defaultScales[ind];
			});

			// Also g through each of the icons and adjust the vertical offset so I can prototype angles.
			Enumerable.Range(start: 0, count: this.calendarDayIcons.Count).ToList().ForEach(i => {
				RectTransform rt = this.calendarDayIcons[i].GetComponent<RectTransform>();
				rt.anchoredPosition = new Vector2(x: rt.anchoredPosition.x, y: (i - 3) * this.verticalOffset);
			});

		}
		#endregion

	}


}