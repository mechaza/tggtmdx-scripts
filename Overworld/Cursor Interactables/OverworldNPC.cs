using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Chat;
using System;
using DG.Tweening;

namespace Grawly.Overworld.Interactable {

	public class OverworldNPC : MonoBehaviour, ICursorInteractable {


		#region FIELDS - NPC VARIABLES
		/// <summary>
		/// The name of this NPC.
		/// </summary>
		public string npcName;
		#endregion

		#region FIELDS - SCENE REFERENCES
		[Header("Scene References")]
		[SerializeField]
		private GameObject colliderVisualization;
		[SerializeField]
		private SpriteRenderer worldSprite;
		[SerializeField]
		private OverworldNodeLabel nodeLabel;
		#endregion

		private void Start() {
			colliderVisualization.transform.localScale = worldSprite.GetComponent<BoxCollider>().size;
		}

		/*#region CHAT INTERFACE IMPLEMENTATION
		public void ChatClosed() {
			OverworldCursor2D.Instance.gameObject.SetActive(true);
		}
		public void ChatOpened() {
			OverworldCursor2D.Instance.gameObject.SetActive(false);
		}
		#endregion*/

		#region CURSOR INTERACTABLE IMPLMEENTATION
		public string GetInteractableName() {
			return npcName;
		}
		public void OnCursorEnter() {
			worldSprite.color = Color.red;
			// Animate the label showing up (though, complete any tweens if they are in motion.
			nodeLabel.ShowLabel(this);
		}
		public void OnCursorExit() {
			worldSprite.color = Color.white;
			nodeLabel.HideLabel();
		}
		public void OnCursorSubmit() {
			/*Grawly.Chat.Legacy.LegacyChatController.Open(
				script: chatScript,
				chatOpenedCallback: delegate {
					OverworldCursor2D.Instance.gameObject.SetActive(false);
				},
				chatClosedCallback: delegate {
					OverworldCursor2D.Instance.gameObject.SetActive(true);
				});*/
		}
		public Vector2 GetGravitationPoint() {
			return transform.position;
		}
		#endregion

	}


}