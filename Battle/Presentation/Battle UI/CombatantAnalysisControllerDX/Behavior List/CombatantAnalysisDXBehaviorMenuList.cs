using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// The MenuList to use inside the analysis screen for showing BattleBehaviors.
	/// </summary>
	public class CombatantAnalysisDXBehaviorMenuList : MenuList {
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this element.
		/// </summary>
		public void ResetState() {
			
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the element into view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Present(CombatantAnalysisParams analysisParams) {
			this.Rebuild(analysisParams: analysisParams);
		}
		/// <summary>
		/// Dismisses the element from view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Dismiss(CombatantAnalysisParams analysisParams) {
			
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Rebuild the element to reflect any changes in the parameters passed in.
		/// </summary>
		/// <param name="analysisParams">The parameters for this analysis screen. May contain changes to be shown.</param>
		public void Rebuild(CombatantAnalysisParams analysisParams) {
			// TODO: Confirm if this is like, actually good.
			List<IMenuable> menuables = analysisParams.CurrentCombatant.AllBehaviors[BehaviorType.Special].Cast<IMenuable>().ToList();
			this.PrepareMenuList(
				allMenuables: menuables,
				startIndex: 0);
		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Is the battle behavior provided being displayed on the menu list?
		/// </summary>
		/// <param name="battleBehavior">The heavior to look for.</param>
		/// <returns>Whether or not this particular behavior is being displayed.</returns>
		public bool IsBehaviorDisplayed(BattleBehavior battleBehavior) {

			var behaviorListItems = this.menuListItems.Cast<CombatantAnalysisDXBehaviorListItem>().ToList();
			foreach (CombatantAnalysisDXBehaviorListItem menuItem in behaviorListItems) {
				if (menuItem.CurrentBattleBehavior == battleBehavior) {
					return true;
				}
			}

			return false;
			
		}
		/// <summary>
		/// Gets the MenuItem that contains the specified BattleBehavior.
		/// </summary>
		/// <param name="battleBehavior"></param>
		public CombatantAnalysisDXBehaviorListItem GetFocusedMenuItem(BattleBehavior battleBehavior) {
			
			// Make sure the list items ACTUALLY contain the behavior passed in.
			Debug.Assert(this.menuListItems
				.Cast<CombatantAnalysisDXBehaviorListItem>()
				.Select(li => li.CurrentBattleBehavior)
				.Contains(battleBehavior));

			// After rebuilding, find the menuitem that contains the battle behavior. 
			CombatantAnalysisDXBehaviorListItem focusItem = this.menuListItems
				.Cast<CombatantAnalysisDXBehaviorListItem>()
				.First(li => li.CurrentBattleBehavior == battleBehavior);
			
			// Return it.
			return focusItem;

		}
		#endregion
		
	}
}