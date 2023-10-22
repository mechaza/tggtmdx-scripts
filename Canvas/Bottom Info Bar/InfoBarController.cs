using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

namespace Grawly.UI {
	
	/// <summary>
	/// A version of the info bar used in the BattleMenuControllerDX, but applicable to other scenarios.
	/// Yes I did just copy/paste the GameObjects in the scene.
	/// </summary>
	public class InfoBarController : MonoBehaviour {
		
		public static InfoBarController Instance { get; private set; }

		#region FIELDS - TOGGLES
		/// <summary>
		/// The default color type to use on the diamond graphic image.
		/// </summary>
		[Title("Toggles"), SerializeField]
		private GrawlyColorTypes diamondGraphicDefaultColorType;
		/// <summary>
		/// The default color to be used by the info box's backing.
		/// </summary>
		[SerializeField]
		private Color infoBoxBackingDefaultColor;
		/// <summary>
		/// The text to place on the diamond graphic's label by default.
		/// </summary>
		[SerializeField]
		private string diamondGraphicDefaultText = "!";
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The default color to use on the diamond graphic image.
		/// </summary>
		private Color DiamondGraphicDefaultColor => GrawlyColors.colorDict[this.diamondGraphicDefaultColorType];
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects containing the bar itself.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The "diamond" that appears on the bottom left.
		/// </summary>
		[SerializeField]
		private Image diamondGraphicImage;
		/// <summary>
		/// The label that rests on top of the diamond graphic.
		/// </summary>
		[SerializeField]
		private SuperTextMesh diamondGraphicLabel;
		/// <summary>
		/// The image that serves as the backing for the info box.
		/// </summary>
		[SerializeField]
		private Image infoBoxBackingImage;
		/// <summary>
		/// The label used to display the body text of the info box.
		/// </summary>
		[SerializeField]
		private SuperTextMesh infoBoxBodyTextLabel;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (Instance != null) {
				return;
			}
			Instance = this;
			ResetController.AddToDontDestroy(this.gameObject);
		}
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the infobar controller.
		/// </summary>
		private void ResetState() {
			this.diamondGraphicImage.color = this.DiamondGraphicDefaultColor;
			this.infoBoxBackingImage.color = this.infoBoxBackingDefaultColor;
			this.allObjects.SetActive(false);
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Displays the InfoBar with the provided body text and default styling.
		/// </summary>
		/// <param name="bodyText">The text to display on the info bar.</param>
		public void Display(string bodyText) {
			this.Display(
				bodyText: bodyText, 
				diamondText: this.diamondGraphicDefaultText, 
				barColor: this.infoBoxBackingDefaultColor,
				diamondColor: this.DiamondGraphicDefaultColor);
		}
		/// <summary>
		/// Displays the info bar with the provided body text and decoration settings.
		/// </summary>
		/// <param name="bodyText">The text to display on the body of the info bar.</param>
		/// <param name="diamondText">The text to use on the diamond graphic.</param>
		/// <param name="barColor">The color the bar backing should be.</param>
		/// <param name="diamondColor">The color the diamond should be.</param>
		private void Display(string bodyText, string diamondText, Color barColor, Color diamondColor) {
			this.allObjects.SetActive(true);
			this.infoBoxBodyTextLabel.Text = bodyText;
			this.diamondGraphicLabel.Text = diamondText;
			this.infoBoxBackingImage.color = barColor;
			this.diamondGraphicImage.color = diamondColor;
		}
		/// <summary>
		/// Dismisses the infobar from view.
		/// </summary>
		public void Dismiss() {
			this.allObjects.SetActive(false);
		}
		#endregion
		
	}
}