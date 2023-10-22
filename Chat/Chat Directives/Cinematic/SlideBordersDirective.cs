using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using Grawly.Battle.BattleMenu;
using Grawly.UI;
using Grawly.Calendar;
using Grawly.Dungeon;
using Grawly.DungeonCrawler;

namespace Grawly.Chat {

	/// <summary>
	/// Slides in the black bars at the top/bottom to give a more cinematic look.
	/// </summary>
	[Title("Slide Borders")]
	public class SlideBordersDirective : ChatDirective {

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should the borders be active or inactive?
		/// </summary>
		[SerializeField]
		private bool setBordersActive = true;
		#endregion

		#region CONSTRUCTORS
		public SlideBordersDirective() {

		}
		public SlideBordersDirective(ChatDirectiveParams directiveParams) {
			this.setBordersActive = directiveParams.GetBool("borders");
		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTATION
		/// <summary>
		/// Slides in the black bars at the top/bottom to give a more cinematic look.
		/// </summary>
		/// <param name="chatController">The ChatController controlling the current chat.</param>
		public override void EvaluateDirective(ChatControllerDX chatController) {

			// Based on the value, set the borders active or inactive.

			if (this.setBordersActive == true) {
				DungeonPlayer.Instance?.nodeLabel.HideLabel();
				CalendarDateUI.instance?.Tween(status: false);
				PlayerStatusDXController.instance?.TweenVisible(status: false);
				NotificationController.Instance?.Dismiss();
				CrawlerMiniMap.Instance?.DismissMap();
				EnemyRadar.Instance?.Dismiss();
				chatController.ChatBorders.PresentBorders(borderParams: new ChatBorderParams());
			} else {
				CalendarDateUI.instance?.Tween(status: true);
				PlayerStatusDXController.instance?.TweenVisible(status: true);
				NotificationController.Instance?.Present();
				CrawlerMiniMap.Instance?.DisplayMap();
				EnemyRadar.Instance?.Display();
				chatController.ChatBorders.DismissBorders(borderParams: new ChatBorderParams());
			}
			chatController.EvaluateNextDirective();
		}
		#endregion

		
		#region ODIN HELPERS
		protected override string FoldoutGroupTitle {
			get {
				return this.GetType().FullName;
			}
		}
		#endregion

	}


}