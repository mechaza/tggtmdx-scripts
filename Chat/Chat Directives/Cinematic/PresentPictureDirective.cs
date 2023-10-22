using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Chat {

	/// <summary>
	/// Shows a picture on the chat.
	/// </summary>
	[Title("Present Picture")]
	public class PresentPictureDirective : ChatDirective {

		#region FIELDS
		/// <summary>
		/// Should a picture asset be used?
		/// </summary>
		[SerializeField, LabelWidth(DEFAULT_LABEL_WIDTH)]
		private bool usePictureAsset = false;
		/// <summary>
		/// The sprite of the picture to show. Assumes an ID is not being used.
		/// </summary>
		[ShowIf("ShouldUsePictureAsset"), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public Sprite pictureToShow;
		/// <summary>
		/// The ID of the picture that should be shown, if not using an asset.
		/// </summary>
		[ShowIf("ShouldUsePictureID"), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public string targetPictureID = "";
		#endregion

		#region CONSTRUCTORS
		public PresentPictureDirective() {
			
		}
		public PresentPictureDirective(ChatDirectiveParams directiveParams) {
			// When making a PresentPicture directive from a text chat, an ID will always be used.
			this.targetPictureID = directiveParams.GetValue(key: directiveParams.FirstLabel);
			this.usePictureAsset = false;
		}
		#endregion
		
		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {

			// Figure out what sprite should be used for the picture.
			Sprite pictureSprite = (this.usePictureAsset == true) 
				? this.pictureToShow 
				: DataController.GetChatPicture(pictureID: this.targetPictureID);
				
			// Tell the first of the chat pictures to present the picture.
			chatController.ChatPictures.First().PresentPicture(
				chatPictureParams: new ChatPictureParams() { pictureSprite = pictureSprite }
			);
			
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
		/// <summary>
		/// Is a picture asset being used?
		/// </summary>
		/// <returns></returns>
		private bool ShouldUsePictureAsset() {
			return this.usePictureAsset == true;
		}
		/// <summary>
		/// Is a picture being used through its ID?
		/// </summary>
		/// <returns></returns>
		private bool ShouldUsePictureID() {
			return this.usePictureAsset == false;
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