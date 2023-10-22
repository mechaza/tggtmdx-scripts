using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;
using Grawly.Battle.Equipment.Badges;
using Sirenix.Serialization;

namespace Grawly {

	/// <summary>
	/// A template for how to prepare a set of toggles to be used for debugging.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Misc/Toggles Template")]
	public class DifficultyVariablesTemplate : SerializedScriptableObject {

		#region FIELDS - GENERAL TOGGLES
		/// <summary>
		/// Should the remote settings be used instead of the local settings?
		/// </summary>
		[TitleGroup("Settings")]
		[VerticalGroup("Settings/General")]
		[SerializeField, BoxGroup("Settings/General/Toggles"), PropertyTooltip("Should the remote settings be used instead of the local settings?")]
		private bool useRemoteSettings = false;
		/// <summary>
		/// The game's difficulty.
		/// </summary>
		[SerializeField, BoxGroup("Settings/General/Toggles"), PropertyTooltip("The game's difficulty.")]
		public DifficultyType difficultyType = DifficultyType.Normal;
		#endregion

		#region FIELDS - BATTLE DROP TOGGLES
		/// <summary>
		/// The amount to multiply experience by.
		/// </summary>
		[SerializeField, BoxGroup("Settings/General/Drops"), PropertyTooltip("The amount to multiply experience by.")]
		public float experienceMultiplier = 1f;
		/// <summary>
		/// The amount to multiply money by.
		/// </summary>
		[SerializeField, BoxGroup("Settings/General/Drops"), PropertyTooltip("The amount to multiply money by.")]
		public float moneyMultiplier = 1f;
		#endregion

		#region FIELDS - BADGE EXPERIENCE
		/// <summary>
		/// The amount of experience needed to master a badge of very easy mastery.
		/// </summary>
		[SerializeField, BoxGroup("Settings/General/Badges"), PropertyTooltip("The amount of experience needed to master a badge of very easy mastery.")]
		public int BadgeMasteryVeryEasyCeiling { get; private set; } = 500;
		/// <summary>
		/// The amount of experience needed to master a badge of easy mastery.
		/// </summary>
		[SerializeField, BoxGroup("Settings/General/Badges"), PropertyTooltip("The amount of experience needed to master a badge of easy mastery.")]
		public int BadgeMasteryEasyCeiling { get; private set; } = 1000;
		/// <summary>
		/// The amount of experience needed to master a badge of medium mastery.
		/// </summary>
		[SerializeField, BoxGroup("Settings/General/Badges"), PropertyTooltip("The amount of experience needed to master a badge of medium mastery.")]
		public int BadgeMasteryMediumCeiling { get; private set; } = 5000;
		/// <summary>
		/// The amount of experience needed to master a badge of hard mastery.
		/// </summary>
		[SerializeField, BoxGroup("Settings/General/Badges"), PropertyTooltip("The amount of experience needed to master a badge of hard mastery.")]
		public int BadgeMasteryHardCeiling { get; private set; } = 10000;
		/// <summary>
		/// The amount of experience needed to master a badge of easy mastery.
		/// </summary>
		[SerializeField, BoxGroup("Settings/General/Badges"), PropertyTooltip("The amount of experience needed to master a badge of very hard mastery.")]
		public int BadgeMasteryVeryHardCeiling { get; private set; } = 20000;
		/// <summary>
		/// The amount of experience needed to master a badge of maximum mastery.
		/// </summary>
		[SerializeField, BoxGroup("Settings/General/Badges"), PropertyTooltip("The amount of experience needed to master a badge of maximum mastery.")]
		public int BadgeMasteryMasterCeiling { get; private set; } = 50000;
		#endregion
		
		#region FIELDS - BATTLE CALCULATION TOGGLES
		/// <summary>
		/// How much should every single calculation be multiplied by?
		/// </summary>
		[TitleGroup("Battle", subtitle: "For use in all battle calculations, regardless of source.")]
		[VerticalGroup("Battle/General")]
		[OdinSerialize, BoxGroup("Battle/General/Multipliers"), PropertyTooltip("How much should every single calculation be multiplied by? E.x., if a move would land 20 damage, setting this to 2f would land 40.")]
		public float FinalValueMultiplier { get; private set; } = 1f;
		/// <summary>
		/// How much should the power of a battle behavior be multiplied by when doing calculations?
		/// </summary>
		[OdinSerialize, BoxGroup("Battle/General/Multipliers"), PropertyTooltip("How much should the power of a battle behavior be multiplied by when doing calculations?")]
		public float BattleBehaviorPowerMultiplier { get; private set; } = 1f;
		/// <summary>
		/// How much should the power of a battle behavior be powered by when doing calculations?
		/// </summary>
		[OdinSerialize, BoxGroup("Battle/General/Multipliers"), PropertyTooltip("How much should the power of a battle behavior be powered by when doing calculations?")]
		public float BattleBehaviorPowerExponent { get; private set; } = 1f;
		#endregion

		#region FIELDS - PLAYER TOGGLES
		/// <summary>
		/// The values to multiply a player's attributes by.
		/// </summary>
		[TitleGroup("Combatants", subtitle: "Specific to combatants of the given type.")]
		[VerticalGroup("Combatants/Player")]
		[SerializeField, ToggleGroup("Combatants/Player/usePlayerToggles"), BoxGroup("Combatants/Player/usePlayerToggles/Multipliers"), HideLabel]
		private CombatantAttributeCollection playerMultiplierAttributeCollection;
		/// <summary>
		/// The values to power the player's attributes by. Gets applied after multiplication.
		/// </summary>
		[SerializeField, ToggleGroup("Combatants/Player/usePlayerToggles"), BoxGroup("Combatants/Player/usePlayerToggles/Exponents"), HideLabel]
		private CombatantAttributeCollection playerExponentAttributeCollection;
		#endregion

		#region FIELDS - ENEMY TOGGLES
		/// <summary>
		/// The values to multiply an enemy's attributes by.
		/// </summary>
		[VerticalGroup("Combatants/Enemy")]
		[SerializeField, ToggleGroup("Combatants/Enemy/useEnemyToggles"), BoxGroup("Combatants/Enemy/useEnemyToggles/Multipliers"), HideLabel]
		private CombatantAttributeCollection enemyMultiplierAttributeCollection;
		/// <summary>
		/// The values to power an enemy's attributes by. Gets applied after multiplication.
		/// </summary>
		[SerializeField, ToggleGroup("Combatants/Enemy/useEnemyToggles"), BoxGroup("Combatants/Enemy/useEnemyToggles/Exponents"), HideLabel]
		private CombatantAttributeCollection enemyExponentAttributeCollection;
		#endregion

		#region FIELDS - PERSONA TOGGLES
		/// <summary>
		/// The values to multiply an persona's attributes by.
		/// </summary>
		[VerticalGroup("Combatants/Persona")]
		[SerializeField, ToggleGroup("Combatants/Persona/usePersonaToggles"), BoxGroup("Combatants/Persona/usePersonaToggles/Multipliers"), HideLabel]
		private CombatantAttributeCollection personaMultiplierAttributeCollection;
		/// <summary>
		/// The values to power an persona's attributes by. Gets applied after multiplication.
		/// </summary>
		[SerializeField, ToggleGroup("Combatants/Persona/usePersonaToggles"), BoxGroup("Combatants/Persona/usePersonaToggles/Exponents"), HideLabel]
		private CombatantAttributeCollection personaExponentAttributeCollection;
		#endregion

		#region FIELDS - USAGE TOGGLES
		/// <summary>
		/// Whether or not the player toggles should be used.
		/// Only used for Odin.
		/// </summary>
		[SerializeField, ToggleGroup("Combatants/Player/usePlayerToggles", ToggleGroupTitle = "Player Toggles")]
		private bool usePlayerToggles = true;
		/// <summary>
		/// Whether or not the enemy toggles should be used.
		/// Only used for Odin.
		/// </summary>
		[SerializeField, ToggleGroup("Combatants/Enemy/useEnemyToggles", ToggleGroupTitle = "Enemy Toggles")]
		private bool useEnemyToggles = true;
		/// <summary>
		/// Whether or not the persona toggles should be used.
		/// Only used for Odin.
		/// </summary>
		[SerializeField, ToggleGroup("Combatants/Persona/usePersonaToggles", ToggleGroupTitle = "Persona Toggles")]
		private bool usePersonaToggles = true;
		#endregion

		#region GETTERS - BADGE MASTERY
		/// <summary>
		/// Returns the amount of experience needed on a badge to fully master the given mastery type.
		/// </summary>
		/// <param name="badgeMasteryType">The mastery type associated with the experience needed.</param>
		/// <returns>The amount of badge experience required to master the given type.</returns>
		public int GetBadgeMasteryCeiling(BadgeMasteryType badgeMasteryType) {
			switch (badgeMasteryType) {
				case BadgeMasteryType.None:
					return 0;
				case BadgeMasteryType.VeryEasy:
					return this.BadgeMasteryVeryEasyCeiling;
				case BadgeMasteryType.Easy:
					return this.BadgeMasteryEasyCeiling;
				case BadgeMasteryType.Medium:
					return this.BadgeMasteryMediumCeiling;
				case BadgeMasteryType.Hard:
					return this.BadgeMasteryHardCeiling;
				case BadgeMasteryType.VeryHard:
					return this.BadgeMasteryVeryHardCeiling;
				case BadgeMasteryType.Master:
					return this.BadgeMasteryMasterCeiling;
				default:
					throw new System.Exception("This should never be reached!");
			}
		}
		#endregion
		
		#region GETTERS - COMBATANT ATTRIBUTES
		/// <summary>
		/// Returns the multiplier associated with the specified combatant and attribute type.
		/// </summary>
		/// <returns>The multiplier value</returns>
		/// <param name="combatant">The combatant who needs their toggle retrieved.</param>
		/// <param name="attributeType">The attribute type being retrieved.</param>
		public float GetMultiplier(Combatant combatant, AttributeType attributeType) {

			// If using remote settings, return that instead.
			if (this.useRemoteSettings == true) {
				return this.GetRemoteMultiplierValue(combatant: combatant, attributeType: attributeType);
			}

			// If the combatant is a player and I want to use their toggles, return it.
			if (combatant.GetType() == typeof(Player) && this.usePlayerToggles == true) {
				return this.playerMultiplierAttributeCollection[attributeType];

				// If the combatant is an enemy and i want to use their toggles, return it.
			} else if (combatant.GetType() == typeof(Enemy) && this.useEnemyToggles == true) {
				return this.enemyMultiplierAttributeCollection[attributeType];

				// If the combatant is a persona and i want to use their toggles, return it.
			} else if (combatant.GetType() == typeof(Persona) && this.usePersonaToggles == true) {
				return this.personaMultiplierAttributeCollection[attributeType];

				// Otherwise, just return 1.
			} else {
				return 1;
			}
		}
		/// <summary>
		/// Returns the exponent associated with the specified combatant and attribute type.
		/// </summary>
		/// <returns>The exponent value</returns>
		/// <param name="combatant">The combatant who needs their toggle retrieved.</param>
		/// <param name="attributeType">The attribute type being retrieved.</param>
		public float GetExponent(Combatant combatant, AttributeType attributeType) {

			// If using remote settings, return that instead.
			if (this.useRemoteSettings == true) {
				return this.GetRemoteExponentValue(combatant: combatant, attributeType: attributeType);
			}

			// If the combatant is a player and I want to use their toggles, return it.
			if (combatant.GetType() == typeof(Player) && this.usePlayerToggles == true) {
				return this.playerExponentAttributeCollection[attributeType];

				// If the combatant is an enemy and i want to use their toggles, return it.
			} else if (combatant.GetType() == typeof(Enemy) && this.useEnemyToggles == true) {
				return this.enemyExponentAttributeCollection[attributeType];

				// If the combatant is a persona and i want to use their toggles, return it.
			} else if (combatant.GetType() == typeof(Persona) && this.usePersonaToggles == true) {
				return this.personaExponentAttributeCollection[attributeType];

				// Otherwise, just return 1.
			} else {
				return 1;
			}
		}
		#endregion

		#region REMOTE SETTINGS
		/// <summary>
		/// Gets a multiplier from the remote settings.
		/// </summary>
		/// <param name="combatant">The combatant who needs the exponent.</param>
		/// <param name="attributeType">The attribute type to retrieve.</param>
		/// <returns></returns>
		private float GetRemoteMultiplierValue(Combatant combatant, AttributeType attributeType) {

			if (combatant.GetType() == typeof(Player) && this.usePlayerToggles == true) {
				return this.GetRemoteSettingsValue(keyName: "PLR_MUL_" + attributeType.ToString());

			} else if (combatant.GetType() == typeof(Enemy) && this.useEnemyToggles == true) {
				return this.GetRemoteSettingsValue(keyName: "EMY_MUL_" + attributeType.ToString());

			} else if (combatant.GetType() == typeof(Persona) && this.usePersonaToggles == true) {
				return this.GetRemoteSettingsValue(keyName: "PER_MUL_" + attributeType.ToString());

			} else {
				return 1f;
			}
		}
		/// <summary>
		/// Gets a multiplier from the remote settings.
		/// </summary>
		/// <param name="combatant">The combatant who needs the exponent.</param>
		/// <param name="attributeType">The attribute type to retrieve.</param>
		/// <returns></returns>
		private float GetRemoteExponentValue(Combatant combatant, AttributeType attributeType) {

			if (combatant.GetType() == typeof(Player) && this.usePlayerToggles == true) {
				return this.GetRemoteSettingsValue(keyName: "PLR_EXP_" + attributeType.ToString());

			} else if (combatant.GetType() == typeof(Enemy) && this.useEnemyToggles == true) {
				return this.GetRemoteSettingsValue(keyName: "EMY_EXP_" + attributeType.ToString());

			} else if (combatant.GetType() == typeof(Persona) && this.usePersonaToggles == true) {
				return this.GetRemoteSettingsValue(keyName: "PER_EXP_" + attributeType.ToString());

			} else {
				return 1f;
			}
		}
		/// <summary>
		/// Gets a value from the RemoteSettings.
		/// </summary>
		/// <param name="keyName">The name of the setting to retrieve.</param>
		/// <returns></returns>
		private float GetRemoteSettingsValue(string keyName) {
			return RemoteSettings.GetFloat(key: keyName, defaultValue: 1f);
		}
		#endregion

	}


}