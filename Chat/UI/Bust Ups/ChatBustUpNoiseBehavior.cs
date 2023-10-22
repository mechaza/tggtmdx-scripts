using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

namespace Grawly.Chat {

	/// <summary>
	/// This is what handles the noise for the chat bust up.
	/// </summary>
	public class ChatBustUpNoiseBehavior : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The RectTransform attached to this GameObject
		/// </summary>
		private RectTransform rectTransform;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The noise settings to use for the wiggle.
		/// </summary>
		[SerializeField]
		private NoiseSettings noiseSettings;
		/// <summary>
		/// The amout of time to offset the noise by.
		/// Good for when I use the same noise but want to have them all wiggly independently.
		/// </summary>
		[SerializeField]
		private float timeOffset = 0f;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			this.rectTransform = GetComponent<RectTransform>();
		}
		private void Update() {
			if (this.gameObject.activeInHierarchy == true) {
				NoiseSettings.TransformNoiseParams p = this.noiseSettings.PositionNoise.First();
				this.rectTransform.anchoredPosition = new Vector2(
					x: p.X.GetValueAt(time: Time.time, timeOffset: this.timeOffset),
					y: p.Y.GetValueAt(time: Time.time, timeOffset: this.timeOffset));
			}
			
		}
		#endregion

		#region SETTERS
		/// <summary>
		/// Overrides the noise settings to a specific kind of noise.
		/// </summary>
		/// <param name="noiseSettings"></param>
		public void SetNoise(NoiseSettings noiseSettings) {
			this.noiseSettings = noiseSettings;
		}
		#endregion

	}


}