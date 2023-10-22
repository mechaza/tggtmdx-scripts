using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using System.Linq;
using Grawly.Battle.Functions;
using Grawly.UI;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// Provides implementation of common use cases for the CombatantAnalysisScreen.
	/// This exists so I don't clutter the controller itself with functions that rely on context.
	/// </summary>
	public class CombatantAnalysisHelperDX : MonoBehaviour {
		
		public static CombatantAnalysisHelperDX Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// A place to cache the GameObject that was selected before opening the analysis screen.
		/// When the screen is closed, this should be re-selected.
		/// </summary>
		public GameObject LastSelectedGameObject { get; private set; }
		/// <summary>
		/// The vertical movement that was last captured in the menus.
		/// </summary>
		public VerticalMoveDirType CurrentVerticalMovement { get; set; }
		/// <summary>
		/// The horizontal movement that was last captured in the menus.
		/// </summary>
		public HorizontalMoveDirType CurrentHorizontalMovement { get; set; }
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
			}
		}
		#endregion

		#region CONTEXT SPECIFIC - BATTLE
		/// <summary>
		/// A routine that will build the analysis screen with the information on enemies currently in battle.
		/// </summary>
		public void DisplayEnemies() {
			
			// Get the enemies that are still alive.
			List<Combatant> enemies = BattleController.Instance.AliveEnemies.Cast<Combatant>().ToList();
			
			// Open the analysis screen.
			CombatantAnalysisControllerDX.Instance.Open(
				analysisType: AnalysisScreenCategoryType.BattleEnemies,
				combatants: enemies, 
				analysisDismissedCallback: () => {
					
				});
			
		}
		#endregion

		#region GENERAL
		/// <summary>
		/// Uses the cached horizontal direction to shift the focus of the combatant.
		/// </summary>
		public void ShiftFocusCombatant() {
			Debug.Log("Shifting the focused combatant with the direction saved in cache: " + this.CurrentHorizontalMovement);
			CombatantAnalysisControllerDX.Instance.ShiftFocusCombatant(moveDir: this.CurrentHorizontalMovement);
		}
		#endregion
		
		#region EVENTSYSTEM MANAGEMENT - SCREEN ELEMENTS
		/// <summary>
		/// Tells the EventSystem to select the first item in the BattleBehavior list.
		/// </summary>
		public void SelectBattleBehaviorList() {
			CombatantAnalysisControllerDX.Instance.BehaviorMenuList.SelectFirstMenuListItem();
		}
		/// <summary>
		/// Tells the EventSystem to select the name banner.
		/// </summary>
		public void SelectNameBanner() {
			Debug.Break();
			CombatantAnalysisControllerDX.Instance.CombatantNameBanner.SelectNameBanner();
		}
		#endregion
		
		#region EVENTSYSTEM MANAGEMENT - CACHE
		/// <summary>
		/// Saves the GameObject that is currently selected into cache so it can be used later.
		/// </summary>
		public void PushCurrentSelectedGameObject() {
			// For right now, I'm just saving one at a time.
			Debug.Log("Saving current selected GameObject: " + EventSystem.current.currentSelectedGameObject);
			this.LastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		}
		/// <summary>
		/// Reselects the GameObject that was saved 
		/// </summary>
		public void ReselectLastSelectedGameObject() {
			
			// Abort if the last object is null.
			if (this.LastSelectedGameObject == null) {
				Debug.LogError("LastSelectedGameObject is null! This cannot be called as is!");
				return;
			}
			// Grab the last selected object.
			Debug.Log("Reselecting last selected GameObject: " + this.LastSelectedGameObject);
			EventSystem.current.SetSelectedGameObject(this.LastSelectedGameObject);
			
			// Null it out, since I don't need it anymore.
			this.LastSelectedGameObject = null;
			
		}
		/// <summary>
		/// Deselects the GameObject that is currently selected.
		/// This is helpful when I need to transition between screens and don't want to accept events.
		/// </summary>
		public void DeselectCurrentSelectedGameObject() {
			
			// Just a little preemptive checking.
			if (this.LastSelectedGameObject == null) {
				Debug.LogWarning("DeselectCurrentSelectedGameObject was called, " 
				                 + "but there is no cached object to return to. Be careful.");
			}
			
			// Just set it to null.
			EventSystem.current.SetSelectedGameObject(null);
			
		}
		#endregion
		
	}
	
}