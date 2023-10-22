using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Grawly.Chat;
using DG.Tweening;
using Grawly.DungeonCrawler;
using Sirenix.OdinInspector;
using Grawly.Story;
using Grawly.Toggles;
using Grawly.Toggles.Audio;
using UnityEngine.AI;
using Grawly.UI;

namespace Grawly.Dungeon {

	public enum DungeonObjectPromptType {
		None			= 0,
		InfoBar			= 1, // Displays text on the bottom of the screen.
		ActionPrompt	= 2, // Displays the prompt with a button to accompany it.
		NodeLabel		= 3, // Displays the prompt using the (outdated) node label.
	}
	
	/// <summary>
	/// Something that has this attached can be used to display info on the bottom of the screen when approached.
	/// </summary>
	public class DungeonObjectPrompt : MonoBehaviour, IDungeonPlayerApproachHandler, IDungeonPlayerInteractionHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The kind of 
		/// </summary>
		[SerializeField]
		private DungeonObjectPromptType promptType = DungeonObjectPromptType.InfoBar;
		/// <summary>
		/// The text to use on the info box when the player approaches this dungeon object.
		/// </summary>
		[SerializeField]
		private string objectInfoText = "";
		#endregion

		#region INTERFACE - IDUNGEONPLAYERAPPROACHHANDLER
		/// <summary>
		/// Called when the DungeonPlayer approaches this DungeonObject.
		/// </summary>
		public void OnDungeonPlayerApproach() {
			
			// There are different ways to display the prompt. Figure out which one to use.
			switch (this.promptType) {
				case DungeonObjectPromptType.InfoBar:
					InfoBarController.Instance?.Display(bodyText: this.objectInfoText);
					break;
				case DungeonObjectPromptType.NodeLabel:
					DungeonPlayer.Instance.nodeLabel.ShowLabel(promptString: this.objectInfoText);
					break;
				case DungeonObjectPromptType.ActionPrompt:
					CrawlerActionPrompt.Instance?.Display(promptText: this.objectInfoText);
					break;
				default:
					throw new System.Exception("This kind of info display type has not been assessed!");
			}
		}
		/// <summary>
		/// Called when the DungeonPlayer leaves this DungeonObject.
		/// </summary>
		public void OnDungeonPlayerLeave() {
			
			// There are different ways to display the prompt. Figure out which one to use.
			switch (this.promptType) {
				case DungeonObjectPromptType.InfoBar:
					InfoBarController.Instance?.Dismiss();
					break;
				case DungeonObjectPromptType.NodeLabel:
					DungeonPlayer.Instance.nodeLabel.HideLabel();
					break;
				case DungeonObjectPromptType.ActionPrompt:
					CrawlerActionPrompt.Instance?.Dismiss();
					break;
				default:
					throw new System.Exception("This kind of info display type has not been assessed!");
			}
			
		}
		#endregion

		#region INTERFACE - IDUNGEONPLAYERINTERACTIONHANDLER
		/// <summary>
		/// When this object is interacted with, just dismiss the associated prompt.
		/// I can't imagine there will be situations where I don't want to do this,
		/// but I am also pressed for time.
		/// </summary>
		public void OnPlayerInteract() {
			// There are different ways to display the prompt. Figure out which one to use.
			switch (this.promptType) {
				case DungeonObjectPromptType.InfoBar:
					InfoBarController.Instance?.Dismiss();
					break;
				case DungeonObjectPromptType.NodeLabel:
					DungeonPlayer.Instance.nodeLabel.HideLabel();
					break;
				case DungeonObjectPromptType.ActionPrompt:
					CrawlerActionPrompt.Instance?.Dismiss();
					break;
				default:
					throw new System.Exception("This kind of info display type has not been assessed!");
			}
		}
		#endregion

		
	}
}