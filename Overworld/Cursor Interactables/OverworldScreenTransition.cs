using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Overworld {
	public class OverworldScreenTransition : MonoBehaviour, ICursorInteractable {

		[SerializeField]
		private string nodeName;

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The world sprite showing the visuals for this transtion. (Like, an arrow or something.)
		/// </summary>
		[SerializeField]
		private SpriteRenderer worldSprite;
		/// <summary>
		/// The node label showing the name of the destination.
		/// </summary>
		[SerializeField]
		private OverworldNodeLabel nodeLabel;
		/// <summary>
		/// The destination screen to move to if this node is selected.
		/// </summary>
		[SerializeField]
		private OverworldScreen destinationScreen;
		#endregion

		#region CURSOR INTERACTABLE IMPLEMENTATIONS
		public Vector2 GetGravitationPoint() {
			return transform.position;
		}
		public string GetInteractableName() {
			return nodeName;
		}
		public void OnCursorEnter() {
			worldSprite.color = Color.red;
			nodeLabel.ShowLabel(this);
		}
		public void OnCursorExit() {
			worldSprite.color = Color.white;
			nodeLabel.HideLabel();
		}
		public void OnCursorSubmit() {
			OverworldController.instance.MoveToScreen(destinationScreen);
		}
		#endregion


	}
}