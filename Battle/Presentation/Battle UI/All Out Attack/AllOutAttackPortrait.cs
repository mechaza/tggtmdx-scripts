using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Grawly.UI;

namespace Grawly.Battle.BattleMenu {
	
	/// <summary>
	/// Represents a player portrait in the all out attack.
	/// </summary>
	public class AllOutAttackPortrait : MonoBehaviour {

		#region FIELDS - TWEENING
		/// <summary>
		/// The position that the portrait should be in when starting out.
		/// </summary>
		[SerializeField, Title("Tweening")]
		private Vector2Int portraitStartPos = new Vector2Int();
		/// <summary>
		/// The position that the portrait should be aiming for.
		/// </summary>
		[SerializeField]
		private Vector2Int portraitTargetPos = new Vector2Int();
		/// <summary>
		/// The amount of time to take to tween the portrait in.
		/// </summary>
		[SerializeField]
		private float portraitTweenInTime = 1.3f;
		/// <summary>
		/// The easing to use when tweening the portrait in.
		/// </summary>
		[SerializeField]
		private Ease portraitEaseInType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the other objects associated with this portrait.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private GameObject allVisuals;
		/// <summary>
		/// The RectTransform that should be moved around and junk.
		/// </summary>
		[SerializeField]
		private RectTransform mainPivotRectTransform;
		/// <summary>
		/// The image that displays dorothy on this portrait.
		/// </summary>
		[SerializeField]
		private Image dorothyPortraitImage;
		/// <summary>
		/// The image that displays rose on this portrait.
		/// </summary>
		[SerializeField]
		private Image rosePortraitImage;
		/// <summary>
		/// The image that displays sophia on this portrait.
		/// </summary>
		[SerializeField]
		private Image sophiaPortraitImage;
		/// <summary>
		/// The image that displays blanche on this portrait.
		/// </summary>
		[SerializeField]
		private Image blanchePortraitImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the portrait.
		/// </summary>
		public void ResetState() {
			
			// Kill any tweens operating on the pivot and snap it back to its starting position.
			this.mainPivotRectTransform.DOKill(complete: true);
			this.mainPivotRectTransform.anchoredPosition = this.portraitStartPos;
			
			// Turn all of the portrait images off.
			this.dorothyPortraitImage.gameObject.SetActive(false);
			this.rosePortraitImage.gameObject.SetActive(false);
			this.sophiaPortraitImage.gameObject.SetActive(false);
			this.blanchePortraitImage.gameObject.SetActive(false);
			
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Animate the portrait to tween in.
		/// </summary>
		/// <param name="characterIDType">The ID of the character whos portrait should be displayed.</param>
		public void AnimatePortrait(CharacterIDType characterIDType) {
			
			// Reset the state to start off.
			this.ResetState();
			
			// Get the image of the portrait to enable and turn it on.
			Image targetPortraitImage = this.GetPortraitImage(characterIDType: characterIDType);
			targetPortraitImage.gameObject.SetActive(true);
			
			// Tween the portrait in.
			this.mainPivotRectTransform.DOAnchorPos(
				endValue: this.portraitTargetPos,portraitTweenInTime, 
				snapping: true)
				.SetEase(this.portraitEaseInType);

		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Gets the portrait image associated with the specified ID type.
		/// </summary>
		/// <param name="characterIDType"></param>
		/// <returns></returns>
		private Image GetPortraitImage(CharacterIDType characterIDType) {
			switch (characterIDType) {
				case CharacterIDType.Dorothy:
					return this.dorothyPortraitImage;
				case CharacterIDType.Rose:
					return this.rosePortraitImage;
				case CharacterIDType.Sophia:
					return this.sophiaPortraitImage;
				case CharacterIDType.Blanche:
					return this.blanchePortraitImage;
				default:
					throw new System.Exception("This should never be reached!");
			}
		}
		#endregion
		
	}
}