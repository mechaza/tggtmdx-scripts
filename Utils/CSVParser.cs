using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Grawly.Calendar;

namespace Grawly.Data {

	/// <summary>
	/// Just a basic static class to parse out the different kinds of CSV files I'll be using.
	/// </summary>
	public static class CSVParser  {

		/// <summary>
		/// Parses out a list of comma separated values that include the scene of a given day number and time of day combination.
		/// </summary>
		/// <param name="file">The file to parse.</param>
		/// <returns></returns>
		public static List<Tuple<int, TimeOfDayType, string>> ParseStoryBeatCSV(string file) {
			return file
				.Trim('\r')													// Remove carriage returns
				.Split('\n')												// Split by newline.
				.Skip(count: 1)												// Skip the header.
				.Select(s => s.Split(','))									// Split by commas.
				.Select(arr => new Tuple<int, TimeOfDayType, string>(		// Create new tuples, which contain...
					item1: int.Parse(arr[0]),								// 1) DAY NUMBER
					item2: (TimeOfDayType)int.Parse(arr[1]),				// 2) TIME OF DAY
					item3: arr[2].Trim('\n', '\r')))						// 3) SCENE TO LOAD (need to trim an extra newline for some fucking reason tho)
				.ToList();
	
		}
		/// <summary>
		/// Parses out the CSV that contains a list of time/place locations 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static List<Tuple<int, TimeOfDayType, LocationType, string>> ParseTimeAndPlaceCSV(string file) {
			return file
				.Trim('\r')																	// Remove carriage returns
				.Split('\n')																// Split by newline.
				.Skip(count: 1)																// Skip the header.
				.Select(s => s.Split(','))												    // Split by commas.
				.Select(arr => new Tuple<int, TimeOfDayType, LocationType, string>(       // Create new tuples, which contain...
					item1: int.Parse(arr[0]),												// 1) DAY NUMBER
					item2: (TimeOfDayType)int.Parse(arr[1]),                                // 2) TIME OF DAY
					item3: (LocationType)int.Parse(arr[2]),								// 3) STORY SCENE TYPE
					item4: arr[3].Trim('\n', '\r')))										// 4) SCENE TO LOAD (need to trim an extra newline for some fucking reason tho)
				.ToList();
		}

	}


}