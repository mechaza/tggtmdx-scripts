using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grawly {
	public class BillboardSprite : MonoBehaviour {

		/*#region FIELDS - DEBUG
		/// <summary>
		/// Should debug mode be turned on?
		/// </summary>
		[SerializeField]
		private bool debugMode = false;
		/// <summary>
		/// The camera to billboard towards when in debug mode.
		/// </summary>
		[SerializeField, ShowIf("debugMode")]
		private Camera debugTargetCamera;
		#endregion*/
		
		private void Update() {
			/*if (this.debugMode == true) {
				transform.forward = this.debugTargetCamera.transform.forward;
				transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
			} else if (Camera.main != null) {
				transform.forward = Camera.main.transform.forward;
				transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
			}*/
			if (Camera.main != null) {
				transform.forward = Camera.main.transform.forward;
				transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
			}
			
		}

	}
}