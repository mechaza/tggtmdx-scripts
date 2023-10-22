using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace Grawly {

	/// <summary>
	/// This is what I use to configure filepaths. Neato. It's bad!
	/// </summary>
	public static class GrawlyFilePaths {

		#region FIELDS - COMBATANT PATHNAMES
		/// <summary>
		/// The path that directs to the folder where all the PlayerTemplates are stored.
		/// </summary>
		public static string PlayerTemplatesPath {
			get {
				// return "_TGGTMDX/Definitions/Players/";
				return "_TGGTMDX/Resources/Players/";
			}
		}
		/// <summary>
		/// The path that directs to the folder where all the PlayerTemplates are stored.
		/// </summary>
		public static string EnemyTemplatesPath {
			get {
				// return "_TGGTMDX/Definitions/Enemies/";
				return "_TGGTMDX/Resources/Enemies/";
			}
		}
		#endregion

		#region FIELDS - BATTLE PATHNAMES
		/// <summary>
		/// The path that directs to the folder where all the BattleTemplates are stored.
		/// </summary>
		public static string BattleTemplatesPath {
			get {
				return "_TGGTMDX/Definitions/Battles/";
			}
		}
		#endregion

		#region FIELDS - BEHAVIOR PATHNAMES
		/// <summary>
		/// The path that directs to the folder where all the BattleBehaviors are stored.
		/// They're sorted by folders here.
		/// </summary>
		public static string BattleBehaviorsPath {
			get {
				// return "_TGGTMDX/Definitions/Behaviors/";
				return "_TGGTMDX/Resources/Behaviors/";
			}
		}
		#endregion

		#region FIELDS - CHAT PATHNAMES
		public static string SerializedChatScriptsPath {
			get {
				return "_TGGTMDX/Definitions/Serialized Chat Scripts/";
			}
		}
		#endregion

		#region FIELDS - GLOBAL ASSET DATA PATHNAMES
		public static string GlobalAssetDataPath {
			get {
				// return "_TGGTMDX/Definitions/Global Asset Data/";
				return "_TGGTMDX/Resources/Global Asset Data/";
			}
		}
		#endregion

		#region FIELDS - CALENDAR DATA PATHNAMES
		public static string CalendarDataPath {
			get {
				return "_TGGTMDX/Resources/Calendar/";
			}
		}
		#endregion

		#region FIELDS - GAME PRESET PATHNAMES
		public static string GamePresetTemplatePathASSETDATABASEVER => "_TGGTMDX/Definitions/Game Presets";
		public static string GamePresetTemplatePath => GamePresetTemplatePathASSETDATABASEVER + "/";
		#endregion

	}


}