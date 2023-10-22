using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UI;

namespace Grawly.Calendar {

	/// <summary>
	/// Just a place to store the icons that are used for the calendar transition scene.
	/// </summary>
	public class CalendarDayIcon : MonoBehaviour {

		#region FIELDS
		/// <summary>
		/// The day that is going to be used for this icon. Just saving a reference in the event I might need to use it later.
		/// </summary>
		private CalendarDay calendarDay;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The object that holds all the rest of the scene GameObjects.
		/// For reference, the GameObject with this script as a component should be the parent of it.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private GameObject calendarDayIconObject;
		/// <summary>
		/// The STM that has the actual date on display.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private SuperTextMesh dateNumberText;
		/// <summary>
		/// The text that shows off what day of the week it is.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private SuperTextMesh weekDayText;
		/// <summary>
		/// The image that shows what day of the week it is.
		/// </summary>
		[SerializeField, TabGroup("Calendar", "Scene References")]
		private Image weekDayImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Prep the icon for use to display information.
		/// </summary>
		/// <param name="calendarDay">The day to prep this icon with.</param>
		public void Prepare(CalendarDay calendarDay) {
			// Just in case, save a reference to the day.
			this.calendarDay = calendarDay;

			// Set the date number/weekday texts.
			this.dateNumberText.Text = CalendarSceneController.instance.GetDateColor(calendarDay: calendarDay) + calendarDay.DayNumber.ToString();
			this.weekDayText.Text = CalendarSceneController.instance.GetWeekDayColor(calendarDay: calendarDay) + calendarDay.WeekdayType.ToString();

		}
		#endregion


	}


}