using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Chat {
	
	/// <summary>
	/// Dismisses a picture from the chat.
	/// </summary>
	[Title("Dismiss Picture")]
	public class DismissPictureDirective : ChatDirective {
		
		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {

			// Dismiss the picture.
			chatController.ChatPictures.First().DismissPicture();
			
			// Wait a few seconds, then evaluate the next directive.
			GameController.Instance.WaitThenRun(timeToWait: 1f, action: delegate {
				// Nothing to do here.
				chatController.EvaluateNextDirective();
			});
			
			/*// Figure out if I want to show or hide the picture.
			if (this.showPicture == true) {
				
				// Figure out what sprite should be used for the picture.
				// This depends if I'm using an ID or if I'm refrencing an asset in the inspector.
				Sprite pictureSprite = (this.usePictureAsset == true) 
					? this.pictureToShow 
					: DataController.GetChatPicture(pictureID: this.targetPictureID);
				
				// Tell the first of the chat pictures to present the picture.
				chatController.ChatPictures.First().PresentPicture(
					chatPictureParams: new ChatPictureParams() { pictureSprite = pictureSprite }
				);
				
			} else {
				// If the picture should not be shown, dismiss it.
				chatController.ChatPictures.First().DismissPicture();
			}
			
			// Wait a few seconds, then evaluate the next directive.
			GameController.Instance.WaitThenRun(timeToWait: 1f, action: delegate {
				// Nothing to do here.
				chatController.EvaluateNextDirective();
			});*/
			
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