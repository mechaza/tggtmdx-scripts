using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace Grawly {

	/// <summary>
	/// Basically a very shoehorned in class for using SuperTextMeshes that have gradients for their dropshadows.
	/// </summary>
	public class FunTextMesh : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The string to use or whatever.
		/// </summary>
		[SerializeField, TabGroup("Fields", "Toggles")]
		private string labelString = "";
		/// <summary>
		/// The string to use or whatever.
		/// </summary>
		public string LabelString {
			get {
				return this.labelString;
			}
		}
		/// <summary>
		/// How much to offset the text by when this thing is selected.
		/// </summary>
		[SerializeField, TabGroup("Fields", "Toggles")]
		private Vector2 selectedOffset = new Vector2(x: 0f, y: 80f);
		/// <summary>
		/// The location of the dropshadow if this text gets big.
		/// </summary>
		[SerializeField, TabGroup("Fields", "Toggles")]
		private Vector2 bigSizeDropshadowPos = new Vector2(x: 7.5f, y: -4.5f);
		/// <summary>
		/// The location of the dropshadow if this text gets small.
		/// </summary>
		[SerializeField, TabGroup("Fields", "Toggles")]
		private Vector2 smallSizeDropshadowPos = new Vector2(x: 3f, y: -2f);
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The STM that appears on top.
		/// </summary>
		[SerializeField, TabGroup("Fields", "Scene References")]
		private SuperTextMesh label;
		/// <summary>
		/// The STM that appears on bottom.
		/// </summary>
		[SerializeField, TabGroup("Fields", "Scene References")]
		private SuperTextMesh dropshadow;
		#endregion

		#region FIELDS - STATE
		/// <summary>
		/// The position where this button is when it is first on screen.
		/// </summary>
		private Vector2 initialPosition = new Vector2();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// Remember what the initial position is.
			this.initialPosition = this.GetComponent<RectTransform>().anchoredPosition;
		}
		#endregion

		#region GRAPHICS
		public void Highlight() {
			this.GetComponent<RectTransform>().anchoredPosition = this.initialPosition + this.selectedOffset;
			this.dropshadow.GetComponent<RectTransform>().anchoredPosition = this.bigSizeDropshadowPos;
			this.label.Text = "<size=134>" + this.labelString;
			this.dropshadow.Text = "<size=134>" + "<c=rainbow2>" + this.labelString;
			// Debug.Log("TEXT: " + dropshadow.Text);
		}
		public void Dehighlight() {
			this.GetComponent<RectTransform>().anchoredPosition = this.initialPosition;
			this.dropshadow.GetComponent<RectTransform>().anchoredPosition = this.smallSizeDropshadowPos;
			this.label.Text = "<size=60>" + this.labelString;
			this.dropshadow.Text = "<size=60>" + "<c=weekday2>" + this.labelString;
		}
		#endregion

		#region UI EVENTS
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Hover);
			this.Dehighlight();
		}
		public void OnSelect(BaseEventData eventData) {
			this.Highlight();
		}
		public void OnSubmit(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Select);
		}
		#endregion

	}


}