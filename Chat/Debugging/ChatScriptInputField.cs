using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Grawly.Chat {
	
	/// <summary>
	/// Just using this script to accept events to tell when its highlighted or not.
	/// </summary>
	public class ChatScriptInputField : MonoBehaviour, ISelectHandler, IDeselectHandler {

		[SerializeField]
		private Image image;
		
		public void OnSelect(BaseEventData eventData) {
			this.image.color = new Color(r: 0.2f, g: 0f, b: 0f, a: image.color.a);
		}

		public void OnDeselect(BaseEventData eventData) {
			this.image.color = new Color(r: 0f, g: 0f, b: 0f, a: image.color.a);
		}
	}

	
}