using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Chat;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Utilities;

#if UNITY_EDITOR
using Grawly.Calendar;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace Grawly.Editors {

	/// <summary>
	/// A simple way for me to edit the serialized chat scripts in my posession.
	/// </summary>
	public class ChatScriptEditorWindow : OdinMenuEditorWindow {


		#region MAIN CALLS
		[MenuItem("Grawly/Chat Script Editor")]
		private static void OpenWindow() {
			var window = GetWindow<ChatScriptEditorWindow>();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
			window.titleContent = new GUIContent("Chat Script Editor");
		}
		#endregion

		#region OVERRIDES
		protected override OdinMenuTree BuildMenuTree() {

			// Create the tree.
			OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true) {
				{   "Home",                 this,            EditorIcons.House },
				{   "Serialized",           null,            EditorIcons.MultiUser },
			};

			// Find every preset template to be used and iterate through each.
			List<GamePresetTemplate> allPresetTemplates = GrawlyAssetHelper.GetAllAssets<GamePresetTemplate>();
			foreach (GamePresetTemplate presetTemplate in allPresetTemplates) {
				
				// I only want to build out trees for preset templates that have dedicated folders for their chat scripts.
				if (presetTemplate.HasChatScriptsFolder == false) {
					continue;
				}
				// Grab the name of the preset itself and the number of days in it.
				string presetName = presetTemplate.PresetName;
				int numberOfDays = presetTemplate.CalendarData.CalendarDayTemplateCount;
				GrawlyMenuItemReference presetPath = new GrawlyMenuItemReference(presetTemplate);
				for (int i = 0; i < numberOfDays; i++) {
					// Add a path for this day.
					string targetMenuPath = presetName + "/Day " + i;
					tree.Add(path: targetMenuPath, instance: presetPath);
					// Get all the scripts for this day and add them.
					List<SerializedChatScriptDX> daySerializedChatScripts = presetTemplate.GetAllSerializedChatScripts(dayNumber: i);
					List<SimpleChatScriptDX> daySimpleChatScripts = presetTemplate.GetAllSimpleChatScripts(dayNumber: i);
					foreach (SerializedChatScriptDX daySerializedChatScript in daySerializedChatScripts) {
						string chatScriptPath = targetMenuPath + "/" + daySerializedChatScript.name;
						tree.Add(path: chatScriptPath, instance: daySerializedChatScript);
					}
					foreach (SimpleChatScriptDX daySimpleChatScript in daySimpleChatScripts) {
						string chatScriptPath = targetMenuPath + "/" + daySimpleChatScript.name;
						tree.Add(path: chatScriptPath, instance: daySimpleChatScript);
					}
				}
			}
			
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
				
				if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Serialized Script"))) {

					string saveTargetPath = "";
					if (selected.Value is GrawlyMenuItemReference) {
						saveTargetPath = GrawlyAssetHelper.GetAssetFolderPath(obj: (selected.Value as GrawlyMenuItemReference).PresetTemplate) + "/Chat Scripts";
					} else {
						saveTargetPath = "Assets/" + GrawlyFilePaths.GamePresetTemplatePath;
					}
					ScriptableObjectCreatorDX.ShowDialog<SerializedChatScriptDX>(saveTargetPath, obj => {
						obj.chatScriptName = obj.name;
						base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
					});
					
				}
				
				if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Simple Script"))) {

					string saveTargetPath = "";
					if (selected.Value is GrawlyMenuItemReference) {
						saveTargetPath = GrawlyAssetHelper.GetAssetFolderPath(obj: (selected.Value as GrawlyMenuItemReference).PresetTemplate) + "/Chat Scripts";
					} else {
						saveTargetPath = "Assets/" + GrawlyFilePaths.GamePresetTemplatePath;
					}
					ScriptableObjectCreatorDX.ShowDialog<SimpleChatScriptDX>(saveTargetPath, obj => {
						// obj.chatScriptName = obj.name;
						base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
					});
					
				}

			}
			SirenixEditorGUI.EndHorizontalToolbar();
		}
		#endregion

	}

}

#endif