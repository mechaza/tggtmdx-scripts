using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly {
	public class AutoDeactivate : MonoBehaviour {

		[SerializeField]
		private float time;


		private IEnumerator Start() {
			yield return new WaitForSeconds(time);
			this.gameObject.SetActive(false);
		}
	}

}