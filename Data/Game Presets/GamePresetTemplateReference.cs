using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grawly.Calendar;
using Grawly.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Object = UnityEngine.Object;
using Grawly.Battle;
using Grawly.Friends;
using Grawly.MiniGames.ShuffleTime;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace Grawly {
	
	/// <summary>
	/// An object to put in the Resources folder to dynamically load in GamePresetTemplates.
	/// This gets used in the preset scene.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Presets/Preset Reference")]
	public class GamePresetTemplateReference : SerializedScriptableObject {

		#region FIELDS - PRESETS
		/// <summary>
		/// A raw list containing all the preset templates.
		/// </summary>
		[SerializeField]
		private List<GamePresetTemplate> allPresetTemplates = new List<GamePresetTemplate>();
		#endregion

		#region PROPERTIES
		/// <summary>
		/// A list of preset templates that are valid for use.
		/// (I.e., no null entries)
		/// </summary>
		public List<GamePresetTemplate> ValidPresetTemplates {
			get {
				// this.CleanTemplateList();
				return this.allPresetTemplates
					.Where(t => t != null)
					.ToList();
			}
		}
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Add a game preset to the list.
		/// </summary>
		/// <param name="presetTemplate">The preset to add to this reference.</param>
		/// <returns>Whether the operation was successful or not.</returns>
		public void AddPresetTemplate(GamePresetTemplate presetTemplate) {
			
			// this.CleanTemplateList();
			
			// Don't actually add this preset if it already exists in the reference.
			if (this.allPresetTemplates.Contains(presetTemplate)) {
				Debug.LogWarning("Preset template " + presetTemplate.PresetName + " already exists in template reference.");
				// return false;
			} else {
				Debug.Log("Adding " + presetTemplate.PresetName + " to preset reference asset.");
				this.allPresetTemplates.Add(presetTemplate);
				// return true;
			}
			
		}
		/// <summary>
		/// Add a game preset to the list and also add it to the build settings.
		/// </summary>
		/// <param name="presetTemplate">The preset to add to this reference.</param>
		/// <param name="scenePath">The path of the scene, so it can be added to the build settings.</param>
		/// <returns>Whether the operation was successful or not.</returns>
		public void AddPresetTemplate(GamePresetTemplate presetTemplate, string scenePath) {
			
			// Add this preset. Note that nothing will happen if it already existed.
			this.AddPresetTemplate(presetTemplate: presetTemplate);

#if UNITY_EDITOR
			// Add the scene to the build settings, but only if its not already in there.
			if (EditorBuildSettings.scenes.Select(bss => bss.path).Contains(scenePath)) {
				Debug.LogWarning("Build settings contains the path " + scenePath + ". Leaving as is.");
			} else {
				EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(path: scenePath, enabled: true);
				EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(newScene).ToArray();
			}
#endif

		}
		/// <summary>
		/// Just quickly truncates any missing templates from the list.
		/// </summary>
		private void CleanTemplateList() {
			this.allPresetTemplates = this.allPresetTemplates
				.Where(t => t != null)
				.Distinct()
				.ToList();
		}
		#endregion
		
	}
}