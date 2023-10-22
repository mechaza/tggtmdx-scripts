using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Grawly {

	/// <summary>
	/// A simple function that enables a gameobject when any key is hit twice. 
	/// </summary>
	public class DoublePressSkip : MonoBehaviour {

		private bool alreadyChecking = false;
		private bool triggered = false;

		[SerializeField]
		private GameObject gameObjectToEnable;
		[SerializeField]
		private SuperTextMesh confirmationToSkipLabel;

		private void Update() {
			if (Input.anyKeyDown == true && alreadyChecking == false && triggered == false) {
				alreadyChecking = true;
				StartCoroutine(this.CheckForDoubleDown());
			}
		}
		private IEnumerator CheckForDoubleDown() {
			confirmationToSkipLabel.gameObject.SetActive(true);
			float timer = 5f;
			yield return new WaitForSeconds(0.05f);
			while (timer > 0f) {

				timer -= 1f * Time.deltaTime;
				if (Input.anyKeyDown == true) {
					triggered = true;
					gameObjectToEnable.SetActive(true);
					break;
				}

				yield return null;
			}
			confirmationToSkipLabel.gameObject.SetActive(false);
			alreadyChecking = false;
		}

	}


}