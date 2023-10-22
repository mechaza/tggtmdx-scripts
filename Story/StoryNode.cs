using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Chat;
using Grawly.Calendar;

namespace Grawly.Story {
	public class StoryNode : MonoBehaviour {

		#region FIELDS - METADATA
		/// <summary>
		/// The ID for this story node. 
		/// </summary>
		[SerializeField]
		private string storyNodeId = "BLANK";
		/// <summary>
		/// The days this node should be active on.
		/// </summary>
		[SerializeField]
		private List<int> activeDays = new List<int>();
		#endregion

		#region FIELDS - COMPUTED
		private bool IsAvailable {
			get {
				return this.activeDays.Contains(CalendarController.Instance.CurrentDay.EpochDay);
				/*if (LegacyStoryController.Instance.CheckStoryNodeId(storyNodeId) && activeDays.Contains(LegacyStoryController.Instance.DayNumber)) {
					return true;
				} else {
					return false;
				}*/
			}
		}
		#endregion

		private void Start() {
			// Destroy this node if it's not supposed to be active for this day.
			if (IsAvailable == false) {
				Destroy(this.gameObject);
			}
			/*if (activeDays.Contains(StoryController.Instance.DayNumber) == false) {
				Destroy(this.gameObject);
			}*/
		}

	}
}