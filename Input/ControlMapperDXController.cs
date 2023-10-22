using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Grawly {
	
	/// <summary>
	/// Provides access to functions that will be needed in the ControlMapperDX scene.
	/// </summary>
	public class ControlMapperDXController : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The name of the scene to return to when going back.
		/// </summary>
		[SerializeField]
		private string sceneToReturnTo = "";
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Returns back to the title screen.
		/// </summary>
		public void ReturnToTitle() {
			
			// Null out the current object just in case.
			EventSystem.current.SetSelectedGameObject(null);
			
			// Load whatever scene is designated as the return point.
			SceneController.instance.BasicLoadSceneWithFade(sceneName: this.sceneToReturnTo);
			
		}
		#endregion
		
	}
	
}