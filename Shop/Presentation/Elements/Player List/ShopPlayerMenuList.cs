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
	/// The menu to select a player from when adjusting their equipment in the shop.
	/// </summary>
	public class ShopPlayerMenuList : MenuList {

		#region FIELDS - TWEENING : POSITIONS
		/// <summary>
		/// The position the player list should be at when hiding.
		/// </summary>
		[SerializeField, TabGroup("Player List", "Tweening"), Title("Positions")]
		private Vector2Int playerListHidingPos = new Vector2Int();
		/// <summary>
		/// The position the player list should be at when on display.
		/// </summary>
		[SerializeField, TabGroup("Player List", "Tweening")]
		private Vector2Int playerListDisplayPos = new Vector2Int();
		/// <summary>
		/// The position the player list should be at when on standby
		/// (i.e., later in the chain of menus).
		/// </summary>
		[SerializeField, TabGroup("Player List", "Tweening")]
		private Vector2Int playerListStandbyPos = new Vector2Int();
		#endregion

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when tweening the player list in.
		/// </summary>
		[SerializeField, TabGroup("Player List", "Tweening"), Title("Timing")]
		private float playerListTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the player list out.
		/// </summary>
		[SerializeField, TabGroup("Player List", "Tweening")]
		private float playerListTweenOutTime = 0.2f;
		#endregion

		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the player list in.
		/// </summary>
		[SerializeField, TabGroup("Player List", "Tweening"), Title("Easing")]
		private Ease playerListEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the player list out.
		/// </summary>
		[SerializeField, TabGroup("Player List", "Tweening")]
		private Ease playerListEaseOutType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects. As children. Fucked up.
		/// </summary>
		[SerializeField, TabGroup("Player List", "Scene References")]
		private GameObject allObjects;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The RectTransform to be used for positional tweens.
		/// </summary>
		private RectTransform MainPositionPivotRectTransform => this.allObjects.GetComponent<RectTransform>();
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Kills all tweens operating on this object.
		/// </summary>
		private void KillAllTweens() {
			
			// Stop any tweens on the position pivot.
			this.MainPositionPivotRectTransform.DOKill(complete: true);

		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			
			// Kill all tweens.
			this.KillAllTweens();
			
			// Snap the position pivot to its hiding position.
			this.MainPositionPivotRectTransform.anchoredPosition = this.playerListHidingPos;
			
			// Clear the list itself.
			this.ClearMenuList();
			
			// Turn all of the objects off.
			this.allObjects.SetActive(false);
			
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Presents the player list onto the screen.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Present(ShopMenuParams shopMenuParams) {
			throw new NotImplementedException();
		}
		#endregion
		
	}
}