using System.Collections;
using System.Collections.Generic;
using Grawly;
using HutongGames.PlayMaker;

namespace Grawly.PlayMakerActions {

	/// <summary>
	/// An action to be integrated with PlayMaker to open up the save menu.
	/// </summary>
	[ActionCategory("Grawly - Menus"), Tooltip("Opens up the save menu.")]
	public class OpenSaveMenu : FsmStateAction {

		#region FIELDS
		/// <summary>
		/// The type of save screen to open up.
		/// </summary>
		[Tooltip("The type of save screen to load up."), ObjectType(typeof(SaveScreenType))]
		public FsmEnum saveScreenType;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			UnityEngine.Debug.Log("HERES THE OWNER!! " + this.Owner.name);
			// Upon entering the state, open the canvas.
			// Grawly.UI.Legacy.LegacySaveCanvas.Instance.SetCanvas(status: true, screenType: ((SaveScreenType)this.saveScreenType.Value));
			UnityEngine.AsyncOperation loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName: "Save Menu DX", mode: UnityEngine.SceneManagement.LoadSceneMode.Additive);
			StartCoroutine(this.WaitForLoad(loadOperation));
		}
		#endregion

		#region WAITERS
		private IEnumerator WaitForLoad(UnityEngine.AsyncOperation loadOperation) {
			while (loadOperation.isDone == false) {
				yield return null;
			}
			yield return null;
			// When the save screen's scene has loaded up, call it and also pass it a reference to this FSM's owner so it knows to alert it when its done.
			Grawly.UI.SaveScreenController.instance.Show(saveScreenType: ((SaveScreenType)this.saveScreenType.Value), sourceOfCall: base.Owner);
			base.Finish();
		}
		#endregion

	}


}