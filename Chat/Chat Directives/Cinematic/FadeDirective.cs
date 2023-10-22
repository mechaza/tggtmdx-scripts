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
	/// Can be used to fade in/out.
	/// </summary>
	[Title("Fade")]
	public class FadeDirective : ChatDirective {

		#region FIELDS - TOGGLES
		/// <summary>
		/// Are you fading out?
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		public bool fadeOut = true;
		#endregion

		#region CONSTRUCTORS
		public FadeDirective() {
			
		}
		public FadeDirective(ChatDirectiveParams directiveParams) {
			// This sucks!
			this.fadeOut = directiveParams.GetValue(directiveParams.FirstLabel) == "out" ? true : false;
		}
		#endregion
		
		#region EXECUTION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			
			if (this.fadeOut == true) {
				Flasher.instance.FadeOut(Color.black);
			} else {
				Flasher.instance.FadeIn();
			}
			
			GameController.Instance.WaitThenRun(timeToWait: 0.6f, () => {
				
				chatController.EvaluateNextDirective();
			});
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