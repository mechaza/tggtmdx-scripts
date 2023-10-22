using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

namespace Grawly.UI.SubItem {

	/// <summary>
	/// A subitem that allows selection of a value from a specific range.
	/// </summary>
	public class ValueRangeSubItem : SubItem {


		#region FIELDS - TOGGLES
		/// <summary>
		/// The kind of sub item this is.
		/// </summary>
		public override SubItemType SubItemType {
			get {
				return SubItemType.ValueRange;
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES
		[Title("Scene References")]
		[SerializeField]
		private Image leftArrowImage;
		[SerializeField]
		private Image rightArrowImage;
		[SerializeField]
		private Image valueDotImage;
		[SerializeField]
		private Image valueBarImage;
		[SerializeField]
		private RectTransform leftDotEndPos;
		[SerializeField]
		private RectTransform rightDotEndPos;
		[SerializeField]
		private SuperTextMesh toggleValueLabel;
		#endregion

		#region BUILDING
		protected internal override void Build(SubItemParams subItemParams) {
			this.Dehighlight(subItemParams: subItemParams);
		}
		#endregion

		#region VISUALS
		protected internal override void Highlight(SubItemParams subItemParams) {
			// Set the colors/alpha.
			this.leftArrowImage.CrossFadeAlpha(alpha: 1f, duration: 0f, ignoreTimeScale: true);
			this.rightArrowImage.CrossFadeAlpha(alpha: 1f, duration: 0f, ignoreTimeScale: true);
			this.valueBarImage.DOColor(endValue: Color.white, duration: 0f);
			this.valueDotImage.DOColor(endValue: Color.white, duration: 0f);
			// Also remember to put the dot in the right position.
			throw new System.NotImplementedException("Need to calculate distance between values.");
			/*this.valueDotImage.rectTransform.anchoredPosition = this.PositionBetweenValue(
				currentIndex: currentIndex,
				maxIndex: subItemParams.Length - 1,
				leftEndPos: this.leftDotEndPos.anchoredPosition,
				rightEndPos: this.rightDotEndPos.anchoredPosition);*/
			// Set the value label.
			// this.toggleValueLabel.Text = "<c=white>" + (subItemParams as SettingsSubItemParams).basicValues[currentIndex];
		}
		protected internal override void Dehighlight(SubItemParams subItemParams) {
			// Set the colors/alpha.
			this.leftArrowImage.CrossFadeAlpha(alpha: 0f, duration: 0f, ignoreTimeScale: true);
			this.rightArrowImage.CrossFadeAlpha(alpha: 0f, duration: 0f, ignoreTimeScale: true);
			this.valueBarImage.DOColor(endValue: Color.black, duration: 0f);
			this.valueDotImage.DOColor(endValue: Color.black, duration: 0f);
			// Also remember to put the dot in the right position.
			throw new System.NotImplementedException("Need to calculate distance between values.");
			/*this.valueDotImage.rectTransform.anchoredPosition = this.PositionBetweenValue(
				currentIndex: currentIndex,
				maxIndex: subItemParams.Length - 1,
				leftEndPos: this.leftDotEndPos.anchoredPosition,
				rightEndPos: this.rightDotEndPos.anchoredPosition);*/
			// Set the value label.
			// this.toggleValueLabel.Text = "<c=black>" + (subItemParams as SettingsSubItemParams).basicValues[currentIndex];
		}
		#endregion

		#region UTILS
		/// <summary>
		/// Calculates the position the dot should be on given the current index, the possible values, and the positions it should be stuck between.
		/// </summary>
		/// <param name="currentIndex"></param>
		/// <param name="maxIndex"></param>
		/// <param name="leftEndPos"></param>
		/// <param name="rightEndPos"></param>
		/// <returns></returns>
		private Vector2 PositionBetweenValue(int currentIndex, int maxIndex, Vector2 leftEndPos, Vector2 rightEndPos) {
			// Calculate the interpolant.
			float interpolant = (float)currentIndex / (float)maxIndex;
			// Get the x/y positions inbetween.
			int xPos = (int)Mathf.Lerp(a: leftEndPos.x, b: rightEndPos.x, t: interpolant);
			int yPos = (int)Mathf.Lerp(a: leftEndPos.y, b: rightEndPos.y, t: interpolant);
			// return them as a new vector2int
			return new Vector2(x: xPos, y: yPos);
		}
		#endregion

	}


}