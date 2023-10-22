using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Grawly.Calendar;
using System.Linq;
using Grawly.Battle.Equipment.Badges;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.IO;


namespace Grawly.Editors {
	
	/// <summary>
	/// A way to rapidly edit badge templates that can be used in game.
	/// </summary>
	public class BadgeTemplateEditorWindow : OdinMenuEditorWindow {
	
		#region MAIN FUNCTION
		/// <summary>
		/// Slips the option to open this editor window into the menu bar in the editor.
		/// </summary>
		[MenuItem("Grawly/Badges")]
		private static void OpenWindow() {
			var window = GetWindow<BadgeTemplateEditorWindow>();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
			window.titleContent = new GUIContent("Badge Database");
		}
		/// <summary>
		/// Odin uses this to build out the window.
		/// </summary>
		/// <returns></returns>
		protected override OdinMenuTree BuildMenuTree() {
			
			// Create the tree.
			OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true) {
				{   "Home",                 this,            EditorIcons.House },
				{   "Badges",                 null,            EditorIcons.MultiUser },
			};
		
		
			List<BadgeTemplate> allBadgeTemplates = GrawlyAssetHelper.GetAllAssets<BadgeTemplate>();
			
			// Add each badge template to the tree.
			foreach (BadgeTemplate badgeTemplate in allBadgeTemplates) {
				
				// Start by grabing the name of the file itself.
				string badgeName = badgeTemplate.BadgeName;
				
				// Add the data itself to the tree.
				tree.Add(
					path: "Badges/" + badgeName, 
					instance: badgeTemplate, 
					sprite: DataController.GetDefaultElementalIcon(badgeTemplate.ElementType));
				
			}

			return tree;
			
		}
		#endregion

		#region BUTTONS
		// [PropertyOrder(-10), Button(ButtonSizes.Large), HorizontalGroup]
		public void RefreshBadgeTemplates() {
			
			// Grab all the badge templates. Don't even worry about where they're coming from.
			List<BadgeTemplate> allBadgeTemplates = GrawlyAssetHelper.GetAllAssets<BadgeTemplate>();

			// Go through each badge template and reassign its raw id.
			for (int i = 0; i < allBadgeTemplates.Count; i++) {
				// Grab the template at the current index.
				BadgeTemplate badgeTemplate = allBadgeTemplates[i];
				// Update the raw ID number.
				badgeTemplate.SetRawID(rawIDNumber: i);
				// Set the name.
				badgeTemplate.BadgeName = "Badge " + i;
				// Set it Dirty.
				EditorUtility.SetDirty(badgeTemplate);
			}
			
			// Save the assets that were set dirty and refresh.
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			
		}
		#endregion
		
	}
}

#endif