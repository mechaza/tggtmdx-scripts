using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;


namespace Grawly.Editors {

	/// <summary>
	/// Edits the GlobalAssetData that is used for misc assets at runtime. I can only get the first for right now.
	/// </summary>
	public class GlobalAssetDataEditorWindow : OdinMenuEditorWindow {

		/// <summary>
		/// Slips the option to open this editor window into the menu bar in the editor.
		/// </summary>
		// [MenuItem("Grawly/Global Asset Data")]
		private static void OpenWindow() {
			var window = GetWindow<GlobalAssetDataEditorWindow>();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
			window.titleContent = new GUIContent("Global Asset Data");
		}

		protected override OdinMenuTree BuildMenuTree() {

			GlobalAssetData globalAssetData = AssetDatabase.LoadAssetAtPath<GlobalAssetData>(assetPath: "Assets/_TGGTMDX/Definitions/Global Asset Data/Default Global Asset Data.asset");

			// Create the tree.
			OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true) {
				{   "Home",                 this,            EditorIcons.House },
				{   "Audio",				null,            EditorIcons.MultiUser },
				{   "VFX",					null,            EditorIcons.Crosshair },
			};

			tree.AddObjectAtPath("Audio/Music", globalAssetData.musicDict, true);

			return tree;
		}
	}


}
#endif