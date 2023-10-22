using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Calendar;
using UnityEngine.UI;
using DG.Tweening;

namespace Grawly.UI {

	/// <summary>
	/// Shows the date at the top right of the screen.
	/// </summary>
	public class CalendarDateUI : MonoBehaviour {

		public static CalendarDateUI instance;

		#region FIELDS - TOGGLES
		/// <summary>
		/// The ease type for which to tween the UI in or out.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Toggles")]
		private Ease tweenEaseType = Ease.InOutQuart;
		/// <summary>
		/// The time for which to tween the UI in or out.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Toggles")]
		private float tweenTime = 0.5f;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The object that can show/hide the date info at the top right.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private GameObject calendarDateInfoObject;
		/// <summary>
		/// The label that shows the month/day on the top right.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private SuperTextMesh dateLabel;
		/// <summary>
		/// The label that shows the day of the week.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private SuperTextMesh dayOfWeekLabel;
		/// <summary>
		/// The label that shows the time of day on the top right.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private SuperTextMesh timeOfDayLabel;
		/// <summary>
		/// The image that serves as a backing for the time of day label. Handy for changing the color. Whoa.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private Image timeOfDayLabelBack;
		/// <summary>
		/// Shows the icon's front for the current weather.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private Image weatherIconFrontImage;
		/// <summary>
		/// Shows the icon's dropshadow for the current weather.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private Image weatherIconDropshadowImage;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		private void Start() {
			this.Refresh(
				calendarDay: CalendarController.Instance.CurrentDay,
				timeOfDay: CalendarController.Instance.CurrentTimeOfDay, 
				weatherType: CalendarController.Instance.CurrentWeatherType);
		}
		private void OnEnable() {
			// Apparently OnEnable can get called before Start in some cases, so having this check will keep things save. Probably.
			if (CalendarController.Instance != null && GameController.Instance != null) {
				this.Refresh(
					calendarDay: CalendarController.Instance.CurrentDay, 
					timeOfDay: CalendarController.Instance.CurrentTimeOfDay, 
					weatherType: CalendarController.Instance.CurrentWeatherType);
			}
			
		}
		private void OnDestroy() {
			instance = null;
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Sets whether or not the info should appear at the top right.
		/// </summary>
		/// <param name="status">Whether the info should be on or off.</param>
		public void SetActive(bool status) {
			this.calendarDateInfoObject.SetActive(status);
		}
		/// <summary>
		/// Refreshes the calendar date info with information of the given calendar day and time of day.
		/// </summary>
		/// <param name="calendarDay">The calendar day to use for the date.</param>
		/// <param name="timeOfDay">The time of day to use on the label.</param>
		/// <param name="weatherType">The weather that should be on display..</param>
		public void Refresh(CalendarDay calendarDay, TimeOfDayType timeOfDay, WeatherType weatherType) {
			this.dateLabel.Text = calendarDay.MonthNumber + "/" + calendarDay.DayNumber;
			this.dayOfWeekLabel.Text = calendarDay.GetWeekdayColorString();
			this.timeOfDayLabel.Text = timeOfDay.ToString();
			this.weatherIconFrontImage.overrideSprite = DataController.GetDefaultWeatherIcon(weatherType: weatherType);
			this.weatherIconDropshadowImage.overrideSprite = DataController.GetDefaultWeatherIcon(weatherType: weatherType);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Tweens the calendar date UI in or out.
		/// </summary>
		/// <param name="status"></param>
		public void Tween(bool status) {
			if (status == true) {
				this.calendarDateInfoObject.GetComponent<RectTransform>().DOAnchorPosY(endValue: 0f, duration: this.tweenTime, snapping: true).SetEase(ease: this.tweenEaseType);
			} else {
				this.calendarDateInfoObject.GetComponent<RectTransform>().DOAnchorPosY(endValue: 220f, duration: this.tweenTime, snapping: true).SetEase(ease: this.tweenEaseType);
			}
		}
		#endregion

	}


}