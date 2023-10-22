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
	/// Contains a set of callbacks to be used on the CombatantAnalysisDX in the event I want to control things from code.
	/// </summary>
	public class AnalysisCallbackSet {

		#region FIELDS - TOGGLES : BEHAVIOR LIST CALLBACKS
		/// <summary>
		/// A callback to invoke when submit is hit on the behavior item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXBehaviorMenuList menuList, CombatantAnalysisDXBehaviorListItem listItem)> OnBehaviorItemSubmit { get; set; } = tuple => {}; 
		/// <summary>
		/// A callback to invoke when cancel is hit on the behavior item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXBehaviorMenuList menuList, CombatantAnalysisDXBehaviorListItem listItem)> OnBehaviorItemCancel { get; set; } = tuple => {}; 
		/// <summary>
		/// A callback to invoke when select is fired on the behavior item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXBehaviorMenuList menuList, CombatantAnalysisDXBehaviorListItem listItem)> OnBehaviorItemSelect { get; set; } = tuple => {};
		/// <summary>
		/// A callback to invoke when deselect is fired on the behavior item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXBehaviorMenuList menuList, CombatantAnalysisDXBehaviorListItem listItem)>  OnBehaviorItemDeselect { get; set; } = tuple => {};
		/// <summary>
		/// A callback to invoke when horizontal movement is detected on the behavior item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXBehaviorMenuList menuList, CombatantAnalysisDXBehaviorListItem listItem, HorizontalMoveDirType horizontalDir)> OnBehaviorItemHorizontalMove { get; set; }  = tuple => {};
		/// <summary>
		/// A callback to invoke when vertical movement is detected on the behavior item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXBehaviorMenuList menuList, CombatantAnalysisDXBehaviorListItem listItem, VerticalMoveDirType verticalDir)> OnBehaviorItemVerticalMove { get; set; } = tuple => {};	
		#endregion

		#region FIELDS - TOGGLES : AUXILIARY ITEM CALLBACKS
		/// <summary>
		/// A callback to invoke when submit is hit on the auxiliary item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXAuxiliaryItem auxiliaryItem)> OnAuxiliaryItemSubmit { get; set; } = tuple => {};
		/// <summary>
		/// A callback to invoke when cancel is hit on the auxiliary item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXAuxiliaryItem auxiliaryItem)> OnAuxiliaryItemCancel { get; set; } = tuple => {};
		/// <summary>
		/// A callback to invoke when select is invoked on the auxiliary item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXAuxiliaryItem auxiliaryItem)> OnAuxiliaryItemSelect { get; set; } = tuple => {};
		/// <summary>
		/// A callback to invoke when deselect is invoked on the auxiliary item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXAuxiliaryItem auxiliaryItem)> OnAuxiliaryItemDeselect { get; set; } = tuple => {};
		/// <summary>
		/// A callback to invoke when horizontal movement is detected on the auxiliary item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXAuxiliaryItem auxiliaryItem, HorizontalMoveDirType horizontalDir)> OnAuxiliaryItemHorizontalMove { get; set; } = tuple => {};
		/// <summary>
		/// A callback to invoke when vertical movement is detected on the auxiliary item.
		/// </summary>
		public Action<(Combatant combatant, CombatantAnalysisDXAuxiliaryItem auxiliaryItem, VerticalMoveDirType verticalDir)> OnAuxiliaryItemVerticalMove { get; set; } = tuple => {};
		#endregion
		
	}

}