using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle.WorldEnemies;
using Sirenix.Serialization;
using System.Linq;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Grawly.Battle {
	
	[CreateAssetMenu(menuName = "Grawly/Combatant/Enemy")]
	public class EnemyTemplate : CombatantTemplate {
	
		#region FIELDS - WORLD ENEMY
		/// <summary>
		/// The kind of WorldEnemy this template defines.
		/// </summary>
		[TabGroup("General", "Graphics"), SerializeField]
		private WorldEnemyType worldEnemyType = WorldEnemyType.Sprite;
		/// <summary>
		/// The prefab to instantiate for this enemy.
		/// </summary>
		[TabGroup("General", "Graphics"), InlineEditor(InlineEditorModes.LargePreview), SerializeField, ShowIf("UsesPrefab")]
		private GameObject enemyPrefab;
		/// <summary>
		/// The prefab to instantiate for this enemy.
		/// </summary>
		public GameObject EnemyPrefab {
			get {
				if (this.UsesPrefab == false) {
					throw new System.Exception("This EnemyTemplate isn't supposed to use a prefab! " 
					                           + "This should never be called under these conditions.");
				}
				return this.enemyPrefab;
			}
		}
		/// <summary>
		/// The sprite this enemy uses.
		/// </summary>
		[TabGroup("General", "Graphics"), InlineEditor(InlineEditorModes.LargePreview), SerializeField, HideIf("UsesPrefab")]
		private Sprite bodySprite;
		/// <summary>
		/// The sprite this enemy uses.
		/// </summary>
		public Sprite BodySprite {
			get {
				if (this.UsesPrefab == true || this.bodySprite == null) {
					Debug.Log("Enemy Template for " + this.metaData.name + " is set to use prefab or the body sprite was not set. Returning placeholder bust up.");
					return DataController.Instance.GetSprite("EnemyErrorSprite");
				} else {
					return this.bodySprite;
				}
				
			}
		}
		#endregion
		
		#region FIELDS - DROPS
		/// <summary>
		/// The drops this enemy gives.
		/// </summary>
		[TabGroup("General", "General"), InlineProperty, Space, HideLabel, Title("Enemy (Toggles)")]
		public Drops drops;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The kind of WorldEnemy this template defines.
		/// </summary>
		public WorldEnemyType WorldEnemyType => this.worldEnemyType;
		/// <summary>
		/// Does this template need to instantiate a prefab?
		/// </summary>
		public bool UsesPrefab => this.WorldEnemyType == WorldEnemyType.Prefab;
		#endregion

	}

	/// <summary>
	/// The different types of enemies that can be defined.
	/// </summary>
	public enum WorldEnemyType {
		Sprite = 0,
		Prefab = 1,
	}
	
}