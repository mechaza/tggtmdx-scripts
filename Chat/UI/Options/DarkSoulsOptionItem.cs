using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Grawly.Chat {

	public class DarkSoulsOptionItem : ChatOptionItem {

		#region FIELDS - SCENE REFERENCES : SELECTABLES
		/// <summary>
		/// The Selectable for this item. I need this exposed so I can. Well, select it.
		/// </summary>
		[Title("Selectables")]
		[SerializeField, TabGroup("Option Item", "Scene References")]
		public Selectable Selectable { get; private set; }
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image for the front of the option.
		/// </summary>
		[Title("Images")]
		[SerializeField, TabGroup("Option Item", "Scene References")]
		private Image optionBackingFrontImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : TEXT
		/// <summary>
		/// The SuperTextMesh that shows this option.
		/// </summary>
		[Title("Text")]
		[SerializeField, TabGroup("Option Item", "Scene References")]
		private SuperTextMesh optionTextLabel;
		#endregion

		#region UNITY CALLS
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Totally resets the state of the option item.
		/// </summary>
		protected override void ResetState() {
			// Set the option label text back to the default.
			this.optionTextLabel.textMaterial = DataController.GetDefaultSTMMaterial("UIDefault");
			// Also reset the text itself.
			this.optionTextLabel.Text = "";
		}
		/// <summary>
		/// Builds the option item with the parameters passed in.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		public override void Build(ChatOptionItemParams optionItemParams) {
			Debug.Log(optionItemParams.optionLabelText);
			// Save a reference to the item parameters.
			this.optionItemParams = optionItemParams;

			// Set the graphics to be the dehighlighted version.
			this.Dehighlight(optionItemParams: optionItemParams);

		}
		/// <summary>
		/// Dismisses this specific option.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		protected override void Dismiss(ChatOptionItemParams optionItemParams) {

		}
		/// <summary>
		/// Gets called when this is the option that was picked.
		/// </summary>
		/// <param name="optionItemParams">The parameters that were used when constructing this option item.</param>
		protected override void PickedOption(ChatOptionItemParams optionItemParams) {
			// Make sure the event system knows to null out this option.
			EventSystem.current.SetSelectedGameObject(null);
			// Upon picking the item, just tell the picker to do something about it.
			optionItemParams.optionPicker.PickedOption(
				optionItemParams: optionItemParams,
				optionItem: this);
		}
		#endregion

		#region GRAPHICS
		/// <summary>
		/// Sets the graphics on the item to look like it has been highlighted.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		protected override void Highlight(ChatOptionItemParams optionItemParams) {
			this.optionTextLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			this.optionTextLabel.Text = "<c=white>" + optionItemParams.optionLabelText;
			this.optionBackingFrontImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Purple];

			// Also play a sound effect.
			AudioController.instance?.PlaySFX(SFXType.Hover, scale: 1f);

		}
		/// <summary>
		/// Sets the graphics on the item to look like it has been dehighlighted.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		protected override void Dehighlight(ChatOptionItemParams optionItemParams) {
			this.optionTextLabel.textMaterial = DataController.GetDefaultSTMMaterial("UIDefault");
			this.optionTextLabel.Text = optionItemParams.optionLabelText;
			this.optionBackingFrontImage.color = Color.white;
		}
		#endregion

	}


}