using System.Collections;
using System.Collections.Generic;
using Grawly.PlayMakerActions;
using Grawly.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Grawly.Dungeon.Interactable {
	
	/// <summary>
	/// This is what can be interacted with to open the save/load menu.
	/// </summary>
	public class SaveArcadeCabinet : MonoBehaviour, IPlayerInteractable {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The kind of screen to present on approaching this cabinet.
		/// </summary>
		[SerializeField]
		private SaveScreenType saveScreenType = SaveScreenType.Load;
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IPLAYERINTERACTABLE
		public string GetInteractableName() {
			return this.saveScreenType == SaveScreenType.Load ? "Load" : "Save";
		}
		public void PlayerEnter() {
			// DungeonPlayer.Instance.nodeLabel.ShowLabel(this);
		}
		public void PlayerExit() {
			// DungeonPlayer.Instance.nodeLabel.HideLabel();
		}
		public void PlayerInteract() {
			
			// If saving is disabled, display a prompt and back out.
			if (GameController.Instance.DebugSavingEnabled == false) {
				DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);
				OptionPicker.instance.Display(
					prompt: "Saving/Loading is disabled for this preset.", 
					option1: "Ok.", 
					option2: "Ok.", 
					callback1: () => {
						DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
					},
					callback2: () => {
						DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
					});
				return;
			}
			
			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);
			GameController.Instance.WaitThenRun(0.1f, () => {
				SaveScreenController.GlobalShow(saveScreenType: this.saveScreenType, sourceOfCall: this.gameObject, onCancelCallback: () => {
					GameController.Instance.WaitThenRun(1f, () => {
						
						// If the save screen controller's instance is not null, unload the save scene.
						if (SaveScreenController.instance != null) {
							SceneManager.UnloadSceneAsync(sceneName: "Save Menu DX");
						}
						
						// SceneManager.UnloadSceneAsync(sceneName: "Save Menu DX");
						GameController.Instance.WaitThenRun(1f, () => {
							DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
						});
					});
				});
			});
		}
		#endregion
		
		
	}
}