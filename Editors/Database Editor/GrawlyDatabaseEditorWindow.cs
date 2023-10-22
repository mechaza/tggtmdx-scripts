using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Battle;

using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace Grawly.Editors {

	/// <summary>
	/// This is what I primarily use for editing stuff in the database.
	/// </summary>
	public class GrawlyDatabaseEditorWindow : OdinMenuEditorWindow {

		/// <summary>
		/// Slips the option to open this editor window into the menu bar in the editor.
		/// </summary>
		[MenuItem("Grawly/Grawly Database")]
		private static void OpenWindow() {
			var window = GetWindow<GrawlyDatabaseEditorWindow>();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
			window.titleContent = new GUIContent("Grawly Database");
		}

		protected override OdinMenuTree BuildMenuTree() {
			// Create the tree.
			OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true) {
				{	"Home",					this,			 EditorIcons.House },
				{   "Combatants",           null,            EditorIcons.MultiUser },
				{	"Battles",				null,            EditorIcons.AlertTriangle },
				{   "Battle Behaviors",     null,            EditorIcons.Crosshair },
			};


			tree.DefaultMenuStyle.IconSize = 28.00f;
			
			// Add the PlayerTemplates.
			tree.AddAllAssetsAtPath(
				menuPath: "Combatants/Players",
				assetFolderPath: GrawlyFilePaths.PlayerTemplatesPath,
				includeSubDirectories: true);

			// Add the EnemyTemplates.
			tree.AddAllAssetsAtPath(
				menuPath: "Combatants/Enemies",
				assetFolderPath: GrawlyFilePaths.EnemyTemplatesPath,
				includeSubDirectories: true);

			// Add the battles
			tree.AddAllAssetsAtPath(
				menuPath: "Battles",
				assetFolderPath: GrawlyFilePaths.BattleTemplatesPath,
				includeSubDirectories: true);

			// Add the BattleBehaviors
			tree.AddAllAssetsAtPath(
				menuPath: "Battle Behaviors",
				assetFolderPath: GrawlyFilePaths.BattleBehaviorsPath,
				includeSubDirectories: true);

			tree.EnumerateTree().AddIcons<BattleTemplate>(x =>x.EnemyTemplates.FirstOrDefault()?.iconSprite?.texture ?? DataController.GetDefaultElementalIcon(ElementType.Phys).texture);
			tree.EnumerateTree().AddIcons<BattleBehavior>(x => x.Icon.texture ?? DataController.GetDefaultElementalIcon(ElementType.Phys).texture);

		

			return tree;
		}
	}


}
#endif