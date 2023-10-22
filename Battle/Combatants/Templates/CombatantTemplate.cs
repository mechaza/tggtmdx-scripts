using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle.TurnBehaviors;
using Sirenix.Serialization;
using Grawly.Battle.Modifiers;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Grawly.Battle {

	public class CombatantTemplate : SerializedScriptableObject {

		#region FIELDS - GENERAL : METADATA
		/// <summary>
		/// The metadata that defines this combatant.
		/// </summary>
		[TabGroup("General", "General"), InlineProperty, HideLabel, Title("Metadata")]
		public Metadata metaData;
		/// <summary>
		/// The icon that should be used inside menus.
		/// </summary>
		[TabGroup("General", "General")]
		public Sprite iconSprite;
		#endregion

		#region FIELDS - CONFIGURATION - LEVEL
		/// <summary>
		/// The amount of EXP this combatant should be instantiated with.
		/// </summary>
		[TabGroup("General", "Stats"), Title("Level")]
		public int totalExp;
		/// <summary>
		/// The preferred level to set for this combatant.
		/// Overrides the setting in total exp.
		/// </summary>
		[TabGroup("General", "Stats")]
		public int preferedLevel = 0;
		#endregion

		#region FIELDS - CONFIGURATION - ATTRIBUTES
		/// <summary>
		/// The "ceiling" for the HP stat.
		/// </summary>
		[TabGroup("General", "Stats"), Range(1f, 999f)]
		public float HPCeiling;
		/// <summary>
		/// The "ceiling" for the MP stat.
		/// </summary>
		[TabGroup("General", "Stats")]
		public float MPCeiling;
		/// <summary>
		/// The "rank" to set the ST value to. This is a preset for the ceiling.
		/// </summary>
		[TabGroup("General", "Stats"), SerializeField, Title("Attributes")]
		private AttributeRankType STRank = AttributeRankType.Medium;
		/// <summary>
		/// The "rank" to set the MA value to. This is a preset for the ceiling.
		/// </summary>
		[TabGroup("General", "Stats"), SerializeField]
		private AttributeRankType MARank = AttributeRankType.Medium;
		/// <summary>
		/// The "rank" to set the EN value to. This is a preset for the ceiling.
		/// </summary>
		[TabGroup("General", "Stats"), SerializeField]
		private AttributeRankType ENRank = AttributeRankType.Medium;
		/// <summary>
		/// The "rank" to set the AG value to. This is a preset for the ceiling.
		/// </summary>
		[TabGroup("General", "Stats"), SerializeField]
		private AttributeRankType AGRank = AttributeRankType.Medium;
		/// <summary>
		/// The "rank" to set the LU value to. This is a preset for the ceiling.
		/// </summary>
		[TabGroup("General", "Stats"), SerializeField]
		private AttributeRankType LURank = AttributeRankType.Medium;
		/// <summary>
		/// A list of the things this combatant is weak/strong/etc to.
		/// Anything not specified in the list is regarded as Normal affiliation.
		/// </summary>
		[TabGroup("General", "Stats"), Title("Resistances"), ListDrawerSettings(Expanded = true)]
		public List<ResistanceTuple> resistances = new List<ResistanceTuple>();
		#endregion

		#region PROPERTIES - CONFIGURATION
		/// <summary>
		/// The level for this combatant. I'm basically just copy/pasting it from the Combatant class. This is only for reference purposes, like building the menus of the enemy spawn waves.
		/// </summary>
		public int Level {
			get {
				float numerator = Mathf.Sqrt(totalExp);
				float denominator = Mathf.Sqrt(Mathf.Sqrt(totalExp)) * 0.5f;
				return Mathf.CeilToInt(numerator / denominator);
			}
		}
		/// <summary>
		/// The "ceiling" for the ST stat.
		/// </summary>
		public float STCeiling {
			get {
				return (float) this.STRank;
			}
		}
		/// <summary>
		/// The "ceiling" for the MA stat.
		/// </summary>
		public float MACeiling {
			get {
				return (float) this.MARank;
			}
		}
		/// <summary>
		/// The "ceiling" for the EN stat.
		/// </summary>
		public float ENCeiling {
			get {
				return (float) this.ENRank;
			}
		}
		/// <summary>
		/// The "ceiling" for the AG stat.
		/// </summary>
		public float AGCeiling {
			get {
				return (float) this.AGRank;
			}
		}
		/// <summary>
		/// The "ceiling" for the LU stat.
		/// </summary>
		public float LUCeiling {
			get {
				return (float) this.LURank;
			}
		}
		#endregion

		#region FIELDS - BATTLE BEHAVIORS
		/// <summary>
		/// The BattleBehaviors at this combatant's disposal.
		/// </summary>
		[TabGroup("Moves", "Battle Behaviors"), ListDrawerSettings(Expanded = true)]
		public List<BattleBehavior> battleBehaviors = new List<BattleBehavior>();
		#endregion

		#region FIELDS - TURN BEHAVIOR
		/// <summary>
		/// The behavior that implements how this combatant should make their move.
		/// </summary>
		[TabGroup("Modifiers", "Turn Behavior"), OdinSerialize]
		private CombatantTurnBehavior turnBehavior;
		/// <summary>
		/// The behavior that implements how this combatant should make their move.
		/// </summary>
		public CombatantTurnBehavior TurnBehavior {
			get {
				return this.turnBehavior;
			}
		}
		#endregion

		#region FIELDS - MODIFIERS
		/// <summary>
		/// Modifiers that should be applied when this combatant is initialized.
		/// Note that as of writing this, it only really makes sense for enemies to have this. 6/9/18.
		/// </summary>
		[TabGroup("Modifiers", "Modifiers"), ListDrawerSettings(Expanded = true)]
		public List<CombatantModifier> defaultCombatantModifiers = new List<CombatantModifier>();
		#endregion

		#region FIELDS - OTHER
		/// <summary>
		/// Literally just a note that I can have in the inspector for when I'm editing shit.
		/// </summary>
		[TabGroup("Notes", "Notes"), MultiLineProperty(lines: 10), HideLabel]
		private string combatantTemplateNote = "";
		#endregion

		#region HELPERS
		[SerializeField]
		private void BuildBehaviorsFromList() {
			List<string> strs = new List<string>(combatantTemplateNote.Split('\r', '\n'));
			this.battleBehaviors.Clear();
			// 2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/texture.jpg", typeof(Texture2D));
			foreach (string str in strs) {
#if UNITY_EDITOR
				BattleBehavior bb = (BattleBehavior)AssetDatabase.LoadAssetAtPath("Assets/_TGGTMDX/Definitions/Behaviors/All Moves/" + str + ".asset", typeof(BattleBehavior));
				this.battleBehaviors.Add(bb);
#endif
			}
		}
		#endregion

	}

	/// <summary>
	/// A really primative way of making it easier to assign resistance types in the combatant templates.
	/// Upon initialization of a combatant, assume anything not explicitly stated has a resistance of "normal"
	/// (This should be set in the combatant constructor.)
	/// </summary>
	[System.Serializable]
	public struct ResistanceTuple {
		public ElementType element;
		public ResistanceType resistance;
	}


}
