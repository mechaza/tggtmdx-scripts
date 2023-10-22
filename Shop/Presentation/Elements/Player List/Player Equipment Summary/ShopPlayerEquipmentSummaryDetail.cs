using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Grawly.UI.MenuLists;

namespace Grawly.Shop {
	
	/// <summary>
	/// Describes an item equipped on the currently selected player in the shop menu.
	/// Used in the ShopPlayerEquipmentSummary.
	/// </summary>
	public class ShopPlayerEquipmentSummaryDetail : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The equipment category this detail is representing.
		/// </summary>
		[SerializeField, TabGroup("Detail", "Toggles")]
		private ShopEquipmentDetailCategoryType equipmentCategoryType = ShopEquipmentDetailCategoryType.None;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the objects as children.
		/// </summary>
		[SerializeField, TabGroup("Detail", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The backing image for the detail's icon.
		/// </summary>
		[SerializeField, TabGroup("Detail", "Scene References")]
		private Image detailIconBackingImage;
		/// <summary>
		/// The actual image displaying the graphic representing the detail.
		/// </summary>
		[SerializeField, TabGroup("Detail", "Scene References")]
		private Image detailIconGraphicImage;
		/// <summary>
		/// The label displaying the category this detail is representing.
		/// </summary>
		[SerializeField, TabGroup("Detail", "Scene References")]
		private SuperTextMesh detailCategoryLabel;
		/// <summary>
		/// The label displaying the currently equipped item for this category on the detail.
		/// </summary>
		[SerializeField, TabGroup("Detail", "Scene References")]
		private SuperTextMesh detailActiveItemLabel;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The RectTransform used for scaling tweens.
		/// </summary>
		private RectTransform MainScalePivotRectTransform => this.allObjects.GetComponent<RectTransform>();
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Kills all tweens operating on this object.
		/// </summary>
		private void KillAllTweens() {
			
			// There should only be scaling tweens on the allObjects object. Kill that.
			this.MainScalePivotRectTransform.DOKill(complete: true);

		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {

			// Kill all outstanding tweens.
			this.KillAllTweens();
			
			// Set the scale on the scale pivot.
			this.MainScalePivotRectTransform.localScale = Vector3.zero;
			
			// Reset the text on the labels.
			this.detailCategoryLabel.Text = "";
			this.detailActiveItemLabel.Text = "";
			
			// Turn all the objects off.
			this.allObjects.SetActive(false);
			
		}
		/// <summary>
		/// Preps this summary detail with the parameters specified.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Prepare(ShopMenuParams shopMenuParams) {
			
			// Set the colors on the images.
			this.detailIconGraphicImage.color = shopMenuParams.ShopThemeTemplate.EquipmentSummaryIconShapeColor;
			this.detailIconBackingImage.color = shopMenuParams.ShopThemeTemplate.EquipmentSummaryIconBackingColor;
			
			// Set the material on the label. The string prefix is done when building later on.
			this.detailCategoryLabel.textMaterial = shopMenuParams.ShopThemeTemplate.EquipmentSummaryDetailLabelMaterial;
			this.detailActiveItemLabel.textMaterial = shopMenuParams.ShopThemeTemplate.EquipmentSummaryDetailLabelMaterial;

		}
		#endregion

		#region BUILDING
		
		#endregion
		
	}
}