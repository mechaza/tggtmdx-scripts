using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Grawly.UI.Legacy;
using Grawly.Chat;
using System;
using Sirenix.OdinInspector;

namespace Grawly.Dungeon.Legacy {
	/*
		/// <summary>
		/// Contains the data that refers to a specific set of enemies that are to be spawned for a particular battle.
		/// Can also contain other information about the battle, such as how boss fights should play out.
		/// </summary>
		[Serializable]
		public class EnemySpawnTemplate : ILegacyMenuable {

			#region FIELDS - METADATA
			/// <summary>
			/// The name for this particular spawn.
			/// </summary>
			[TabGroup("Metadata", "Metadata")]
			public string spawnName;
			/// <summary>
			/// The description for this spawn.
			/// </summary>
			[TabGroup("Metadata", "Metadata")]
			public string spawnDescription;
			#endregion

			#region FIELDS - ENEMIES
			/// <summary>
			/// The enemies for this particular spawn.
			/// </summary>
			[TabGroup("Metadata", "Enemies")]
			public List<EnemyTemplate> enemies = new List<EnemyTemplate>();
			#endregion

			#region FIELDS - CUTSCENES
			/// <summary>
			/// Is this spawn associated with a boss?
			/// </summary>
			[TabGroup("Metadata", "Settings")]
			public bool isBoss = false;
			/// <summary>
			/// The chat script for this boss.
			/// </summary>
			[TabGroup("Metadata", "Settings"), ShowIf("IsBoss")]
			public SerializedChatScript bossScript;
			#endregion

			#region MENUABLE IMPLEMENTATION
			public Sprite GetMenuSprite() {
				return enemies[0].worldSprite;
			}
			public string GetPrimaryMenuString() {
				return spawnName;
			}
			public string GetSecondaryMenuString() {
				return "Lv: " + enemies[0].Level.ToString();
			}
			public string GetTertiaryMenuString() {
				return spawnDescription;
			}
			#endregion

			#region ODIN FUNCTIONS
			private bool IsBoss() {
				return isBoss;
			}
			#endregion
		}*/

}