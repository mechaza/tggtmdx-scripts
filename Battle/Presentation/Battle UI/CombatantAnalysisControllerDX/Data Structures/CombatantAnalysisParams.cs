using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// A class to encapsulate the parameters that get processed
	/// when building the combatant analysis screen.
	/// </summary>
	public class CombatantAnalysisParams {

		#region FIELDS - STATE
		/// <summary>
		/// The index identifying the combatant currently in focus.
		/// </summary>
		public int CurrentCombatantIndex { get; set; } = 0;
		#endregion
		
		#region FIELDS - TOGGLES : GAME VALUES
		/// <summary>
		/// The kind of analysis screen to present.
		/// </summary>
		public AnalysisScreenCategoryType AnalysisType { get; set; } = AnalysisScreenCategoryType.None;
		/// <summary>
		/// The combatant to analyze.
		/// </summary>
		public List<Combatant> Combatants { get; set; } = new List<Combatant>();
		/// <summary>
		/// The GameVariables that contain things like known resistances, etc.
		/// </summary>
		public GameVariables Variables { get; set; }
		#endregion

		#region FIELDS - TOGGLES : CATEGORY SPECIFIC (SKILL CARD)
		/// <summary>
		/// The inventory item being that has contextual use within the analysis screen being built. 
		/// </summary>
		public InventoryItem CurrentInventoryItem { get; set; }
		#endregion
		
		#region PROPERTIES - COMBATANT
		/// <summary>
		/// The combatant currently in focus.
		/// </summary>
		public Combatant CurrentCombatant => this.Combatants[this.CurrentCombatantIndex];
		#endregion

		#region PROPERTIES - ELEMENTS
		/// <summary>
		/// The params that are used to build the auxiliary item, if one is available.
		/// </summary>
		public AuxiliaryItemParams CurrentAuxiliaryItemParams {
			get {
				
				// Assert that there IS an auxiliary item.
				Debug.Assert(this.HasLevelUpMove);
				// Cast the combatant as a Persona.
				Persona currentPersona = this.CurrentCombatant as Persona;
				// Peek at the next move.
				LevelUpMove levelUpMove = currentPersona.levelUpMoves.Peek();
				
				// Assemble the new item params and return them.
				return new AuxiliaryItemParams(itemObject: levelUpMove) {
					ItemName = levelUpMove.behavior.behaviorName,
					ItemQuantity = levelUpMove.behavior.QuantityString,
					ItemDescription = levelUpMove.behavior.description,
					ItemTargetLevel = levelUpMove.level.ToString(),
					ItemIconSprite = levelUpMove.behavior.Icon
				};

			}
		}
		#endregion

		#region PROPERTIES - FLAGS
		/// <summary>
		/// Should the auxiliary item be built in the analysis screen?
		/// </summary>
		public bool HasAuxiliaryItem {
			get {
				// If there are multiple kinds of auxiliary items,
				// this can be a check if any of the flags are true.
				return this.HasLevelUpMove;
			}
		}
		/// <summary>
		/// Are there any level up moves as part of these parameters?
		/// </summary>
		public bool HasLevelUpMove {
			get {
				
				// Return false if the combatant is anything other than a Persona.
				if ((this.CurrentCombatant is Persona) == false) {
					return false;
				}
				
				// Cast the combatant as a Persona.
				Persona currentPersona = this.CurrentCombatant as Persona;
				
				// If this Persona has ANY level up moves, its true.
				return (currentPersona.levelUpMoves.Count > 0);
				
			}
		}
		/// <summary>
		/// Are there multiple combatants being analyzed?
		/// </summary>
		public bool HasMultipleCombatants => this.Combatants.Count > 1;
		/// <summary>
		/// Are the banner arrows visible for these params?
		/// I probably only want to do this for analyzing enemies.
		/// </summary>
		public bool HasVisibleBannerArrows => this.AnalysisType == AnalysisScreenCategoryType.BattleEnemies;
		/// <summary>
		/// Should the level label be displayed?
		/// </summary>
		public bool CanDisplayCurrentLevel => true;
		/// <summary>
		/// Should the amount of EXP required to reach the next level be displayed?
		/// </summary>
		public bool CanDisplayNextLevelEXP {
			get {
				// Return false in the event the cobatant is an enemy.
				if (this.CurrentCombatant is Enemy) {
					return false;
				} else {
					return true;
				}
			}
		}
		#endregion
		
		#region GETTERS - CONTEXT SENSITIVE
		/// <summary>
		/// Gets the resistance type of the current combatant, if it is known.
		/// </summary>
		/// <param name="elementType">The element type to check resistance against.</param>
		/// <returns>The resistance type associated with the specified element, if it is known.</returns>
		public ResistanceType? GetDynamicResistance(ElementType elementType) {

			// If this combatant is an enemy, get their KNOWN resistance.
			if (this.CurrentCombatant is Enemy) {
				return this.Variables.GetKnownEnemyResistance(
					enemy: (this.CurrentCombatant as Enemy), 
					elementType: elementType);
			} else {
				// For all other combatant types, just check the resistance as normal.
				return this.CurrentCombatant.CheckResistance(elementType: elementType);
			}
			
			/*if (analysisParams.CurrentCombatant is Enemy) {
				Enemy enemy = (analysisParams.CurrentCombatant as Enemy);
				// Check if there is a known resistance type.
				var resistanceType = analysisParams.Variables.GetKnownEnemyResistance(enemy: enemy, elementType: this.elementType);
				if (resistanceType.HasValue == false) {
					// If there is no value (resistance unknown to player,) communicate as such. 
					this.unknownResistanceLabel.gameObject.SetActive(true);
					this.knownResistanceLabel.gameObject.SetActive(false);
					this.backingFrontImage.color = Color.black;
					this.backingDropshadowImage.color = Color.white;
				} else if (resistanceType.Value == ResistanceType.Nm) {
					// If there IS a value but its normal, just show the icon normally.
					this.unknownResistanceLabel.gameObject.SetActive(false);
					this.knownResistanceLabel.gameObject.SetActive(false);
					this.backingFrontImage.color = Color.black;
					this.backingDropshadowImage.color = Color.white;
				} else {
					// If the value is known and not normal, set it up as much.
					// If there IS a value but its normal, just show the icon normally.
					this.unknownResistanceLabel.gameObject.SetActive(false);
					this.knownResistanceLabel.gameObject.SetActive(true);
					this.backingFrontImage.color = Color.black;
					this.backingDropshadowImage.color = Color.white;
					this.knownResistanceLabel.Text = resistanceType.Value.ToString();
				}
				
			} else if (analysisParams.CurrentCombatant is Persona) {
				this.unknownResistanceLabel.gameObject.SetActive(false);
				this.knownResistanceLabel.gameObject.SetActive(true);
				this.backingFrontImage.color = Color.black;
				this.backingDropshadowImage.color = Color.white;
				ResistanceType resistanceType = analysisParams.CurrentCombatant.CheckResistance(elementType: this.elementType);
				if (resistanceType != ResistanceType.Nm) {
					this.knownResistanceLabel.Text = resistanceType.ToString();
				}
				
			} else {
				throw new System.Exception("This needs to be rethought!");
			}*/
		}
		#endregion
		
	}
	
}