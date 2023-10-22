using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;

namespace Grawly.Calendar {

	/// <summary>
	/// Contains data that can help with defining certain aspects of any given day.
	/// I don't actually know what I want yet. lmao.
	/// </summary>
	[System.Serializable]
	public class CalendarDay {

		#region FIELDS - DAY INFO
		/// <summary>
		/// The month number to use. Gets set in CalendarData.
		/// </summary>
		public int MonthNumber { get; set; } = -1;
		/// <summary>
		/// The day of the month to use. Gets set in CalendarData.
		/// </summary>
		public int DayNumber { get; set; } = -1;
		/// <summary>
		/// The day of the week. Gets set in CalendarData.
		/// </summary>
		public CalendarWeekdayType WeekdayType { get; set; } = CalendarWeekdayType.Sun;
		/// <summary>
		/// The number of days since epoch that this day occurs on. Handy for sorting. Gets set in CalendarData.
		/// </summary>
		public int EpochDay { get; set; } = -1;
		#endregion

		#region COLOR HELPERS
		/// <summary>
		/// Returns a string containing the weekday type as well as a color to associate with it.
		/// This is mostly just for convinience.
		/// </summary>
		/// <returns></returns>
		public string GetWeekdayColorString() {
			switch (this.WeekdayType) {
				case CalendarWeekdayType.Sat:
					return "<c=saturday>" + this.WeekdayType.ToString();
				case CalendarWeekdayType.Sun:
					return "<c=sunday>" + this.WeekdayType.ToString();
				default:
					return "<c=weekday>" + this.WeekdayType.ToString();
			}
		}
		#endregion

	}


}