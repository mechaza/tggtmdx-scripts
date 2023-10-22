using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Grawly.UI.SubItem {

	/// <summary>
	/// A subitem that allows for selection of on/off.
	/// </summary>
	public class SimpleToggleSubItem : SubItem {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The kind of sub item this is.
		/// </summary>
		public override SubItemType SubItemType {
			get {
				return SubItemType.SimpleToggle;
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
		private Image leftArrowDropshadowImage;
		[SerializeField]
		private Image rightArrowDropshadowImage;
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
			this.leftArrowImage.CrossFadeAlpha(alpha: 1f, duration: 0f, ignoreTimeScale: true);
			this.rightArrowImage.CrossFadeAlpha(alpha: 1f, duration: 0f, ignoreTimeScale: true);
			this.leftArrowDropshadowImage.CrossFadeAlpha(alpha: 1f, duration: 0f, ignoreTimeScale: true);
			this.rightArrowDropshadowImage.CrossFadeAlpha(alpha: 1f, duration: 0f, ignoreTimeScale: true);
			// this.toggleValueLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			this.toggleValueLabel.Text = "<c=white>" + subItemParams.CurrentOption;
		}
		protected internal override void Dehighlight(SubItemParams subItemParams) {
			this.leftArrowImage.CrossFadeAlpha(alpha: 0f, duration: 0f, ignoreTimeScale: true);
			this.rightArrowImage.CrossFadeAlpha(alpha: 0f, duration: 0f, ignoreTimeScale: true);
			this.leftArrowDropshadowImage.CrossFadeAlpha(alpha: 0f, duration: 0f, ignoreTimeScale: true);
			this.rightArrowDropshadowImage.CrossFadeAlpha(alpha: 0f, duration: 0f, ignoreTimeScale: true);
			// this.toggleValueLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Default");
			this.toggleValueLabel.Text = "<c=black>" + subItemParams.CurrentOption;
		}
		#endregion

	}


}