using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using Grawly.Battle.Equipment;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.Chat;
using Grawly.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;
using UnityEngine.Events;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// A way for me to listen for buttons from the controller when operating the crane.
	/// </summary>
	public class BadgeBoardPieceCraneActionListener : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		[SerializeField]
		private BadgeBoardPieceCrane boardPieceCrane;
		#endregion

		#region UNITY CALLS
		private void Update() {

			bool rotateLeftPressed = InputController.Instance.GetButtonDown("Secondary Horizontal Left");
			bool rotateRightPressed = InputController.Instance.GetButtonDown("Secondary Horizontal Right");
			bool flipPressed = InputController.Instance.GetButtonDown(actionName: "Secondary Submit");
			
			if (rotateLeftPressed) {
				boardPieceCrane.RotateDummyPieceClockwise();
			} else if (rotateRightPressed) {
				boardPieceCrane.RotateDummyPieceCounterClockwise();
			} else if (flipPressed) {
				boardPieceCrane.FlipDummyPiece();
			}
			
		}
		#endregion
		
	}
}