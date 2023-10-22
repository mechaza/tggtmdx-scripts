using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Grawly.Dungeon {
	public class DungeonNodeLabel : MonoBehaviour {

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
		/// Tweens the node label into view.
		/// </summary>
		/// <param name="promptString">The string to display on the prompt.</param>
		public void ShowLabel(string promptString) {
			Debug.Log("Showing: " + promptString);
			nameLabelSprite.transform.DOComplete();
			nameLabelSprite.transform.DOLocalMoveX(1f, 0.2f);
			nameLabelSprite.DOColor(Color.black, 0.2f);
			nameLabelText.Text = promptString;
		}
		/// <summary>
		/// Tweens the node label into view for a given ICursorInteractable.
		/// </summary>
		/// <param name="interactable"></param>
		public void ShowLabel(IPlayerInteractable interactable) {
			this.ShowLabel(promptString: interactable.GetInteractableName());
			/*nameLabelSprite.transform.DOComplete();
			nameLabelSprite.transform.DOLocalMoveX(1f, 0.2f);
			nameLabelSprite.DOColor(Color.black, 0.2f);
			nameLabelText.Text = interactable.GetInteractableName();*/
		}
		/// <summary>
		/// Tweens thei node label out of view.
		/// </summary>
		public void HideLabel() {
			nameLabelSprite.transform.DOComplete();
			nameLabelSprite.transform.DOLocalMoveX(0.1f, 0.2f);
			nameLabelSprite.DOColor(Color.clear, 0.2f);
			nameLabelText.Text = "";
		}
		/// <summary>
		/// Tweens the node label out of view instantly.
		/// </summary>
		private void ClearLabel() {
			nameLabelSprite.transform.DOLocalMoveX(0.1f, 0.01f);
			nameLabelSprite.DOColor(Color.clear, 0.01f);
			nameLabelText.Text = "";
		}
		#endregion

	}
}