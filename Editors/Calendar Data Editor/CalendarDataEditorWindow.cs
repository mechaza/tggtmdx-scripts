using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Grawly.Calendar;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.IO;

namespace Grawly.Editors {

	/// <summary>
	/// Basically a window to help organize the days of the calendar.
	/// </summary>
	public class CalendarDataEditorWindow : OdinMenuEditorWindow {

		#region MAIN FUNCTION
		/// <summary>
		/// Slips the option to open this editor window into the menu bar in the editor.
		/// </summary>
		[MenuItem("Grawly/Calendar Data")]
		private static void OpenWindow() {
			var window = GetWindow<CalendarDataEditorWindow>();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
			window.titleContent = new GUIContent("Calendar Database");
		}

		/// <summary>
		/// Odin uses this to build out the window.
		/// </summary>
		/// <returns></returns>
		protected override OdinMenuTree BuildMenuTree() {
			
			// Create the tree.
			OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true) {
				{   "Home",                 this,            EditorIcons.House },
				{   "Calendars",                 null,            EditorIcons.MultiUser },
			};

			List<CalendarData> allCalendarData = GrawlyAssetHelper.GetAllAssets<CalendarData>();
			
			// Add each CalendarData to the tree.
			foreach (CalendarData calendarData in allCalendarData) {
				// Start by grabing the name of the file itself.
				string calendarName = calendarData.name;
				
				// Add the data itself to the tree.
				tree.Add(path: calendarName, instance: calendarData);
				
				// Grab the templates to use, then iterate through them and add them to the tree.
				List<CalendarDayTemplate> calendarDayTemplates = calendarData.CalendarDayTemplates;
				foreach (CalendarDayTemplate calendarDayTemplate in calendarDayTemplates) {
					string dayPath = calendarName + "/Day " + calendarDayTemplate.dayNumber;
					tree.Add(path: dayPath, instance: calendarDayTemplate);
				}
			}

			return tree;
			
			
		}
		#endregion

		#region HELPERS
		[ShowInInspector, TabGroup("Initialization","Initialization"), InfoBox("Reset the scenes that are loaded by default for each time of day.")]
		public void ResetDefaultScenes() {
			// Load up the calendar data real quick.
			CalendarData calendarData = (CalendarData)AssetDatabase.LoadAssetAtPath(
				assetPath: "Assets/" + GrawlyFilePaths.CalendarDataPath + "DefaultCalendarData.asset", 
				type: typeof(CalendarData));
			calendarData.GetAllDays().ForEach(c => {
				// c.SetDefaultScene(timeOfDayType: TimeOfDayType.Afternoon, sceneName: "Miami Day");
				// c.SetDefaultScene(timeOfDayType: TimeOfDayType.Evening, sceneName: "Blanche House Evening");
			});
		}

		#endregion

	}


}
#endif