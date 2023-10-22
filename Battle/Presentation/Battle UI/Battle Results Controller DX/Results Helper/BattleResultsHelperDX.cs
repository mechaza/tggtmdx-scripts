using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using System.Linq;
using DG.Tweening.Core;
using Grawly.Battle.Functions;
using Grawly.UI;
using UnityEngine.EventSystems;
using Grawly.Battle.Analysis;
using Grawly.Toggles.Proto;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;

namespace Grawly.Battle.Results {
	
	/// <summary>
	/// Literally just meant to help with setting up things in the results controller.
	/// I define things like callback sets for the analysis screen and junk.
	/// </summary>
	public partial class BattleResultsHelperDX : MonoBehaviour {
		
		public static BattleResultsHelperDX Instance { get; private set; }

		/*#region FIELDS - STATE
		/// <summary>
		/// The current state of the screen that lets you select a skill to learn.
		/// </summary>
		public LearnSkillDXStateType CurrentScreenState { get; set; } = LearnSkillDXStateType.PreConfirmation;
		#endregion*/
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The callback set to use when showing a persona learning a new skill from their level up moves.
		/// I define all of this here so I don't clutter the controller.
		/// </summary>
		public AnalysisCallbackSet PersonaLearnSkillCallbackSet { get; private set; } = new AnalysisCallbackSet();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				this.PersonaLearnSkillCallbackSet.OnBehaviorItemSubmit = tuple => {
					this.BehaviorListItemSubmit(
						currentCombatant: tuple.combatant, 
						submittedBehavior: tuple.listItem.CurrentBattleBehavior, 
						submittedMenuItem: tuple.listItem);
				};
				this.PersonaLearnSkillCallbackSet.OnBehaviorItemCancel = tuple => {
					this.BehaviorListItemCancel(
						currentCombatant: tuple.combatant, 
						menuItemBehavior: tuple.listItem.CurrentBattleBehavior,
						menuItem: tuple.listItem);
				};
				
			}
		}
		#endregion
		
	}
	
}