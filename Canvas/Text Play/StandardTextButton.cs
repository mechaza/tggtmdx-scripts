using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Grawly.UI {

	/// <summary>
	/// Just a very simple button that does stuff.
	/// Mostly using this on the initialization controller.
	/// Maybe elsewhere?
	/// </summary>
	public class StandardTextButton : MonoBehaviour, IDeselectHandler, ISelectHandler {

		[SerializeField]
		private SuperTextMesh textMesh;
		[SerializeField]
		private Material dehighlightMaterial;
		[SerializeField]
		private Material highlightMaterial;
		[SerializeField]
		private string buttonText = "";
		[SerializeField]
		private string dehighlightString = "<c=black>";
		[SerializeField]
		private string highlightString = "<c=white>";

		private void Start() {
			this.Dehighlight();
		}
		public void Highlight() {
			this.textMesh.textMaterial = this.highlightMaterial;
			this.textMesh.Text = this.highlightString + this.buttonText;
		}
		public void Dehighlight() {
			this.textMesh.textMaterial = this.dehighlightMaterial;
			this.textMesh.Text = this.dehighlightString + this.buttonText;
		}

		public void OnDeselect(BaseEventData eventData) {
			this.Dehighlight();
		}

		public void OnSelect(BaseEventData eventData) {
			this.Highlight();
		}
	}


}