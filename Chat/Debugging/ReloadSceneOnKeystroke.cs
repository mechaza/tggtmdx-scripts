using System.Collections;
using System.Collections.Generic;
using Grawly.Chat;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneOnKeystroke : MonoBehaviour {

	public float timer = 3f;
	public string keybind = "r";

	private float timeElapsed = 0f;
	
	private void Update() {
		if (Input.GetKey(this.keybind)) {
			this.timeElapsed += Time.deltaTime;
			if (this.timeElapsed >= this.timer) {
				this.timeElapsed = 0f;
				// GameObject.Destroy(ChatControllerDX.StandardInstance.gameObject);
				ChatControllerDX.StandardInstance.Close(runCloseCallback: false);
				this.timeElapsed = -10000f;
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				
			}
		}
		else {
			this.timeElapsed = 0f;
		}
	}
}
