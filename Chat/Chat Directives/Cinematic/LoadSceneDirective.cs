using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Calendar;
using Grawly.Dungeon;
using Grawly.DungeonCrawler;
using Grawly.UI.Legacy;

namespace Grawly.Chat {

	/// <summary>
	/// Loads a scene and keeps the chat going.
	/// </summary>
	[Title("Load Scene")]
	[GUIColor(r: 0.8f, g: 0.8f, b: 1f, a: 1f)]
	public class LoadSceneDirective : ChatDirective {

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should a custom scene be used?
		/// </summary>
		private bool useCustomScene = false;
		/// <summary>
		/// The kind of location to load.
		/// </summary>
		[HideIf("useCustomScene")]
		public LocationType LocationType;
		/// <summary>
		/// The scene to load if no location type is defined.
		/// </summary>
		[ShowIf("useCustomScene")]
		private string customSceneName = "";
		#endregion

		#region CONSTRUCTORS
		public LoadSceneDirective() {
			
		}
		public LoadSceneDirective(ChatDirectiveParams directiveParams) {
			this.useCustomScene = directiveParams.GetBool("customScene");
			if (this.useCustomScene == true) {
				this.customSceneName = directiveParams.GetValue("load");
			} else {
				this.LocationType = directiveParams.GetEnum<LocationType>("load");
			}
		}
		#endregion
		
		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {
						
			if (this.useCustomScene == true) {

				SceneController.instance.BasicLoadScene(sceneName: this.customSceneName, onComplete: () => {
					
					// Make sure to have the player wait when the scene loads.
					GameController.Instance.RunEndOfFrame(action: delegate {
						Debug.LogWarning("Please add a way to make this SAFE");
						DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Wait);
						CrawlerPlayer.Instance?.SetState(CrawlerPlayerState.Wait);
					});
				
					GameController.Instance.WaitThenRun(timeToWait: 1f, action: delegate {
						chatController.EvaluateNextDirective();
					});
				});
				
			} else {
				SceneController.instance.LoadScene(
					locationType: this.LocationType, 
					dayNumber: CalendarController.Instance.CurrentDay.EpochDay,
					timeOfDay: CalendarController.Instance.CurrentTimeOfDay, 
					onSceneLoaded: delegate {
				
						// Make sure to have the player wait when the scene loads.
						GameController.Instance.RunEndOfFrame(action: delegate {
							Debug.LogWarning("Please add a way to make this SAFE");
							DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Wait);
							CrawlerPlayer.Instance?.SetState(CrawlerPlayerState.Wait);
						});
				
						GameController.Instance.WaitThenRun(timeToWait: 1f, action: delegate {
							chatController.EvaluateNextDirective();
						});
				
					});
			}
			
			
			
			
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