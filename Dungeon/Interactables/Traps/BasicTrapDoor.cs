using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Grawly.Dungeon.Interactable {
	public class BasicTrapDoor : MonoBehaviour {

		[SerializeField]
		private GameObject leftPanel;
		[SerializeField]
		private GameObject rightPanel;

		private void OnTriggerEnter(Collider other) {
			if (other.CompareTag("Player") == true) {

			}
		}

	}

}
