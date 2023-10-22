using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Effects {
	public class RotateEffect : MonoBehaviour {

		public Vector3 amount;

		private void Update() {
			transform.Rotate(amount * Time.deltaTime);
		}

	}
}