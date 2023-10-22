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

namespace Grawly.Menus.BattleTerminal {
	
	/// <summary>
	/// The list that should be used for displaying optinos for the battle terminal.
	/// </summary>
	public class BattleTerminalMenuList : MenuList {
		
		#region PREPARATION
		/// <summary>
		/// Kills all tweens operating on the selection menu list.
		/// </summary>
		private void KillAllTweens() {
			// this.backingFrontImage.DOKill(complete: true);
			// this.backingDropshadowImage.DOKill(complete: true);
			throw new NotImplementedException("CHECK THIS");
		}
		/// <summary>
		/// Resets the state of this menu list.
		/// </summary>
		public void ResetState() {
			
			// Kill all tweens that might be ongoing.
			this.KillAllTweens();
			
			// Snap the colors for both images to be clear.
			// this.backingFrontImage.color = Color.clear;
			// this.backingDropshadowImage.color = Color.clear;
			
			// Clear the menu list.
			this.ClearMenuList();
			
			throw new NotImplementedException("CHECK THIS");
			
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the element using the data contained in the parameters specified.
		/// </summary>
		public void Present() {
			
			// Kill any outstanding tweens.
			this.KillAllTweens();
			
			throw new NotImplementedException("CHECK THIS");
			
		}
		/// <summary>
		/// Dismisses this element from the screen.
		/// </summary>
		public void Dismiss() {
			// Kill any outstanding tweens.
			this.KillAllTweens();
			
			throw new NotImplementedException("CHECK THIS");
			
		}
		#endregion
		
	}
}