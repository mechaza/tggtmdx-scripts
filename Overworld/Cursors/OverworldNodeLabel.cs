using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Grawly.Overworld {
	public class OverworldNodeLabel : MonoBehaviour {

		#region SCENE REFERENCES
		[SerializeField]
		private SpriteRenderer nameLabelSprite;
		[SerializeField]
		private SuperTextMesh nameLabelText;
		#endregion

		private void Start() {
			// Hide the label immediately upon start.
			ClearLabel();
		}

		#region SHOW / HIDE
		/// <summary>
		/// Tweens the node label into view for a given ICursorInteractable.
		/// </summary>
		/// <param name="interactable"></param>
		public void ShowLabel(ICursorInteractable interactable) {
			nameLabelSprite.transform.DOComplete();
			nameLabelSprite.transform.DOLocalMoveX(2.5f, 0.2f);
			nameLabelSprite.DOColor(Color.black, 0.2f);
			nameLabelText.Text = interactable.GetInteractableName();
		}
		/// <summary>
		/// Tweens thei node label out of view.
		/// </summary>
		public void HideLabel() {
			nameLabelSprite.transform.DOComplete();
			nameLabelSprite.transform.DOLocalMoveX(0.5f, 0.2f);
			nameLabelSprite.DOColor(Color.clear, 0.2f);
			nameLabelText.Text = "";
		}
		/// <summary>
		/// Tweens the node label out of view instantly.
		/// </summary>
		private void ClearLabel() {
			nameLabelSprite.transform.DOLocalMoveX(0.5f, 0.01f);
			nameLabelSprite.DOColor(Color.clear, 0.01f);
			nameLabelText.Text = "";
		}
		#endregion

	}
}