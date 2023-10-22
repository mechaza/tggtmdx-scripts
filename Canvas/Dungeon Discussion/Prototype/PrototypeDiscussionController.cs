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
using Grawly.Calendar;
using Grawly.UI.Legacy;
using Grawly.Chat;
using Grawly.Dungeon;

namespace Grawly.UI {
	/// <summary>
	/// A prototype version of the menu that presents itself when discussing going into a dungeon.
	/// </summary>
	public class PrototypeDiscussionController : MonoBehaviour {
		
		public static PrototypeDiscussionController Instance { get; private set; }

		#region FIELDS - TOGGLES
		/// <summary>
		/// The location that should be loaded upon leaving the discussion.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private LocationType leaveLocation = LocationType.MiamiMallLobby;
		/// <summary>
		/// The spawn location to use when leaving.
		/// </summary>
		[SerializeField]
		private DungeonSpawnType leaveSpawnType = DungeonSpawnType.Default;
		/// <summary>
		/// The location that should be loaded upon teleporting to a dungeon.
		/// </summary>
		[SerializeField]
		private LocationType dungeonLocation = LocationType.DungeonLobby;
		/// <summary>
		/// The spawn type to use when going inside the dungeon.
		/// </summary>
		[SerializeField]
		private DungeonSpawnType dungeonSpawnType = DungeonSpawnType.Default;
		/// <summary>
		/// The text asset that should be loaded when the Talk option is picked.
		/// </summary>
		[SerializeField]
		private TextAsset talkDialogueChatAsset;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The menu buttons to help with navigating the discussion.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private List<PrototypeDiscussionMenuButton> discussionMenuButtons = new List<PrototypeDiscussionMenuButton>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			// Upon start, select the first menu button.
			// Wait one frame, though, just so Dehighlight() doesn't interfere with the order of operations.
			GameController.Instance.RunEndOfFrame(() => {
				EventSystem.current.SetSelectedGameObject(this.discussionMenuButtons.First().gameObject);
			});
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Enter the dungeon.
		/// </summary>
		public void EnterDungeon() {
			
			// Note that the event system has its selected object nulled when the button is submitted.
			
			// When going back, just load up the location that is designated as the leave location.
			// SceneController.instance.BasicLoadSceneWithFade(locationType: this.dungeonLocation);
			SceneController.instance.LoadScene(
				dayNumber: CalendarController.Instance.CurrentDay.EpochDay,
				locationType: this.dungeonLocation,
				timeOfDay: CalendarController.Instance.CurrentTimeOfDay,
				spawnPositionType: this.dungeonSpawnType, 
				dontStopMusic: false);
			
		}
		/// <summary>
		/// When selected, have the party talk amongst themselves.
		/// </summary>
		public void Talk() {
			
			// Note that the event system has its selected object nulled when the button is submitted.
			
			ChatControllerDX.GlobalOpen(
				textAsset: this.talkDialogueChatAsset,
				chatClosedCallback:((str, num, toggle) => {
					// Grab the GameObject for the talk button.
					GameObject talkButtonObject = this.discussionMenuButtons.First(mb => mb.DiscussionButtonType == DungeonDiscussionButtonType.Talk).gameObject;
					// Select it upon closing the chat.
					EventSystem.current.SetSelectedGameObject(talkButtonObject);
				}));
			
		}
		/// <summary>
		/// If backtracking from the menu, this will transition back to the dungeon appropriately.
		/// </summary>
		public void BackToOverworld() {
			
			// Note that the event system has its selected object nulled when the button is submitted.
			
			// When going back, just load up the location that is designated as the leave location.
			// SceneController.instance.BasicLoadSceneWithFade(locationType: this.leaveLocation);
			SceneController.instance.LoadScene(
				dayNumber: CalendarController.Instance.CurrentDay.EpochDay,
				locationType: this.leaveLocation,
				timeOfDay: CalendarController.Instance.CurrentTimeOfDay,
				spawnPositionType: this.leaveSpawnType, 
				dontStopMusic: false);
			
		}
		#endregion
		
	}
}