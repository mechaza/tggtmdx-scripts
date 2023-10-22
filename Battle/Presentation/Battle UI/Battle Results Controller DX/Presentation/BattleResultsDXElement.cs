using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using System.Linq;
using Grawly.Battle.Functions;
using Grawly.UI;
using UnityEngine.EventSystems;
using Grawly.Battle.Analysis;
using Grawly.UI.Legacy;

namespace Grawly.Battle.Results {
	/// <summary>
	/// An individual piece of information that can be presented on the results screen.
	/// </summary>
	public class BattleResultsDXElement : MonoBehaviour {
		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The name to use on this element.
		/// </summary>
		[SerializeField, TabGroup("Element", "Toggles")]
		private string elementName = "";
		/// <summary>
		/// The type of element this is.
		/// </summary>
		[SerializeField, TabGroup("Element", "Toggles")]
		private BattleResultsDXElementType elementType = BattleResultsDXElementType.None;
		/// <summary>
		/// The type of color associated with this element.
		/// </summary>
		[SerializeField, TabGroup("Element", "Toggles")]
		private GrawlyColorTypes elementColorType = GrawlyColorTypes.Black;
		/// <summary>
		/// The sprite to use for this element's icon.
		/// </summary>
		[SerializeField, TabGroup("Element", "Toggles")]
		private Sprite elementIconSprite;
		#endregion

		#region FIELDS - TOGGLES : TWEENING
		/// <summary>
		/// The position this element should be when hiding.
		/// </summary>
		[SerializeField, TabGroup("Element", "Tweening")]
		private Vector2Int hidingPos = new Vector2Int();
		/// <summary>
		/// The position this element should be when displayed.
		/// </summary>
		[SerializeField, TabGroup("Element", "Tweening")]
		private Vector2Int displayPos = new Vector2Int();
		/// <summary>
		/// The amount of time to take when tweening this element in.
		/// </summary>
		[SerializeField, TabGroup("Element", "Tweening")]
		private float tweenInTime = 1f;
		/// <summary>
		/// The easing to use when tweening this element in.
		/// </summary>
		[SerializeField, TabGroup("Element", "Tweening")]
		private Ease tweenInEaseType = Ease.InOutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the element's objects as children.
		/// </summary>
		[SerializeField, TabGroup("Element", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The RectTransform to use when moving this element on the canvas.
		/// </summary>
		[SerializeField, TabGroup("Element", "Scene References")]
		private RectTransform elementRectTransform;
		/// <summary>
		/// The image to use for this element's icon.
		/// </summary>
		[SerializeField, TabGroup("Element", "Scene References")]
		private Image elementIconImage;
		/// <summary>
		/// The label which shows the header text for this element.
		/// </summary>
		[SerializeField, TabGroup("Element", "Scene References")]
		private SuperTextMesh headerTextLabel;
		/// <summary>
		/// The label which shows the primary text for this element.
		/// </summary>
		[SerializeField, TabGroup("Element", "Scene References")]
		private SuperTextMesh primaryTextLabel;
		/// <summary>
		/// The label which shows the secondary text for this element.
		/// </summary>
		[SerializeField, TabGroup("Element", "Scene References")]
		private SuperTextMesh secondaryTextLabel;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The type of Results Element this is.
		/// </summary>
		public BattleResultsDXElementType ResultsElementType => this.elementType;
		#endregion

		#region UNITY CALLS
		private void Awake() {
		}
		private void Start() {
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this element.
		/// </summary>
		public void ResetState() {
			// Kill any tweens on the element's RectTransform and snap its position back.
			this.elementRectTransform.DOKill(complete: true);
			this.elementRectTransform.anchoredPosition = this.hidingPos;

			// Change the icon and its color.
			this.elementIconImage.overrideSprite = this.elementIconSprite;
			this.elementIconImage.color = GrawlyColors.colorDict[this.elementColorType];

			// Set the header text and clear out the primary/secondary labels.
			this.headerTextLabel.text = this.elementName;
			this.primaryTextLabel.text = "";
			this.secondaryTextLabel.text = "";

			// Deactivate all the objects.
			this.allObjects.SetActive(false);
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Presents this element onto the screen.
		/// </summary>
		/// <param name="resultsSet">The results of the battle that was just completed.</param>
		public void Present(BattleResultsSet resultsSet) {
			// If this is an item element, assert the results set has items.
			// This shouldn't be called in cases where there are none.
			if (this.elementType == BattleResultsDXElementType.Item) {
				Debug.Assert(resultsSet.ContainsItems);
			}

			// Turn on the objects.
			this.allObjects.SetActive(true);

			// Set the labels accordingly.
			this.primaryTextLabel.text = this.GeneratePrimaryLabelText(resultsSet: resultsSet);
			this.secondaryTextLabel.text = this.GenerateSecondaryLabelText(resultsSet: resultsSet);

			// Tween the element's RectTransform in.
			this.elementRectTransform.DOAnchorPos(endValue: this.displayPos, duration: this.tweenInTime, snapping: true).SetEase(ease: this.tweenInEaseType);
		}
		#endregion

		#region HELPERS : STRINGS
		/// <summary>
		/// Generates the text that should be displayed on the primary label on this element.
		/// </summary>
		/// <param name="resultsSet">The results set from the battle.</param>
		/// <returns>The string to display on the primary label.</returns>
		private string GeneratePrimaryLabelText(BattleResultsSet resultsSet) {
			switch (this.elementType) {
				case BattleResultsDXElementType.Experience:
					return resultsSet.TotalEXP + " EXP";
				case BattleResultsDXElementType.Money:
					return "$" + resultsSet.TotalMoney;
				case BattleResultsDXElementType.Item:
					string str = "";
					foreach (BattleBehavior item in resultsSet.ItemDict.Keys) {
						int count = resultsSet.ItemDict[item];
						str += item.behaviorName + "\n";
					}

					return str;
				default:
					throw new System.Exception("This should never be reached!");
			}
		}
		/// <summary>
		/// Generates the text that should be displayed on the secondary label on this element.
		/// </summary>
		/// <param name="resultsSet">The results set from the battle.</param>
		/// <returns>The string to display on the secondary label.</returns>
		private string GenerateSecondaryLabelText(BattleResultsSet resultsSet) {
			switch (this.elementType) {
				case BattleResultsDXElementType.Experience:
					return "";
				case BattleResultsDXElementType.Money:
					return "";
				case BattleResultsDXElementType.Item:
					string str = "";
					foreach (BattleBehavior item in resultsSet.ItemDict.Keys) {
						int count = resultsSet.ItemDict[item];
						str += "x" + count.ToString();
					}

					return str;
				default:
					throw new System.Exception("This should never be reached!");
			}
		}
		#endregion
	}
}