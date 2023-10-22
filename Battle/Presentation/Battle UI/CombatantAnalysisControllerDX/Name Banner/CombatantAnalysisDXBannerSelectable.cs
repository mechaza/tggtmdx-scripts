using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// Captures events sent by the EventSystem when the banner is what should be focused on.
	/// </summary>
	[RequireComponent(typeof(Selectable))]
	public class CombatantAnalysisDXBannerSelectable : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, ICancelHandler, IMoveHandler {
		
		#region INTERFACE IMPLEMENTATION - EVENT SYSTEM
		public void OnSubmit(BaseEventData eventData) {
			CombatantAnalysisControllerDX.Instance.TriggerEvent(eventName: "Banner Submit");
		}
		public void OnCancel(BaseEventData eventData) {
			
		}
		public void OnSelect(BaseEventData eventData) {
			Debug.Log("Banner Selected.");
		}
		public void OnDeselect(BaseEventData eventData) {
			
		}
		public void OnMove(AxisEventData eventData) {
			Debug.Log("Banner Move: " + eventData.moveDir.ToString());
			switch (eventData.moveDir) {
				
				case MoveDirection.Left:
					CombatantAnalysisHelperDX.Instance.CurrentHorizontalMovement = HorizontalMoveDirType.Left;
					CombatantAnalysisControllerDX.Instance.TriggerEvent(eventName: "Banner Horizontal Movement");
					break;
				
				case MoveDirection.Right:
					CombatantAnalysisHelperDX.Instance.CurrentHorizontalMovement = HorizontalMoveDirType.Right;
					CombatantAnalysisControllerDX.Instance.TriggerEvent(eventName: "Banner Horizontal Movement");
					break;
				
				case MoveDirection.Down:
					CombatantAnalysisHelperDX.Instance.CurrentVerticalMovement = VerticalMoveDirType.Down;
					CombatantAnalysisControllerDX.Instance.TriggerEvent(eventName: "Banner Vertical Movement");
					break;
				default:
					break;
			}
			
		}
		#endregion
		
	}

}
