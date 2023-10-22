using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Chat;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
namespace Grawly.Editors {
    
	/// <summary>
	/// A window to help me create new GamePresets.
	/// </summary>
	public class GamePresetCreatorEditorWindow : OdinMenuEditorWindow {


		#region FIELDS - EDITOR : PARAMS
		/// <summary>
		/// A place to hold the variables that can be manipulated when creating new presets.
		/// </summary>
		public static GamePresetCreationParams PresetCreationParams = new GamePresetCreationParams();
		#endregion
		
		#region MAIN CALLS
		// [MenuItem("Grawly/Game Preset Creator")]
		private static void OpenWindow() {
			var window = GetWindow<GamePresetCreatorEditorWindow>();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
			window.titleContent = new GUIContent("Game Preset Creator");
		}
		#endregion

		#region OVERRIDES
		protected override OdinMenuTree BuildMenuTree() {

			// Create the tree.
			OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true) {
				{   "Home",                 this,            EditorIcons.House },
			};

			tree.AddObjectAtPath(menuPath: "Home", instance: PresetCreationParams);

			// Get all of the GamePresets.
			List<GamePresetTemplate> allGamePresets = GrawlyAssetHelper.GetAllAssets<GamePresetTemplate>();
			
			return tree;
		}

		

		protected override void OnBeginDrawEditors() {
			var selected = this.MenuTree.Selection.FirstOrDefault();
			var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

			// Draws a toolbar with the name of the currently selected menu item.
			SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
			{
				if (selected != null) {
					GUILayout.Label(selected.Name);
				}

				
				
				/*if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Script"))) {
					
					ScriptableObjectCreatorDX.ShowDialog<SerializedChatScriptDX>("Assets/" + GrawlyFilePaths.SerializedChatScriptsPath, obj => {
						obj.chatScriptName = obj.name;
						base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
					});
				}*/

		
			}
			SirenixEditorGUI.EndHorizontalToolbar();
		}
		#endregion
		
	}

    
}
#endif