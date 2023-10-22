using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.IO;
#endif
namespace Grawly {

#if UNITY_EDITOR
	/// <summary>
	/// Just a class to help me find assets that I'm tired of going everywhere else to find.
	/// </summary>
	public static class GrawlyAssetHelper {

		#region GETTERS - ASSETS
		/// <summary>
		/// Get all of the assets of the specified type located in the given folders.
		/// </summary>
		/// <typeparam name="T">The type of asset to retrieve.</typeparam>
		/// <returns>A list of assets of the given type.</returns>
		public static List<T> GetAllAssets<T>() where T : Object {
			return GrawlyAssetHelper.GetAllAssets<T>(searchInFolders: null);
		}
		/// <summary>
		/// Get all of the assets of the specified type located in the given folders.
		/// </summary>
		/// <param name="searchInFolders">The folders to search.</param>
		/// <typeparam name="T">The type of asset to retrieve.</typeparam>
		/// <returns>A list of assets of the given type.</returns>
		public static List<T> GetAllAssets<T>(string[] searchInFolders) where T : Object {
			
			// Grab the name of the type itself.
			string typeName = typeof(T).Name;
			
			// Find the GUIDs of all the assets that match the specified type and transform them into paths.
			string[] assetGUIDs = AssetDatabase.FindAssets(filter: "t:" + typeName, searchInFolders: searchInFolders);
			var assetPaths = assetGUIDs.Select(p => AssetDatabase.GUIDToAssetPath(p));
			
			// Create a list of those assets by then loading them.
			List<T> assets = assetPaths
				.Select(p => AssetDatabase.LoadAssetAtPath<T>(assetPath: p))
				.ToList();

			// Return that list.
			return assets;

		}
		#endregion

		#region GETTERS - PATHS
		/// <summary>
		/// Returns the path to the folder that the specified object belongs to.
		/// </summary>
		/// <param name="obj">The object to find the folder it belongs to.</param>
		/// <returns>A path to the folder the specified object resides in.</returns>
		public static string GetAssetFolderPath(Object obj) {
			// Get the full path of the asset itself.
			string fullAssetPath = AssetDatabase.GetAssetPath(obj);
			// Trim the end of the path so it leads to the folder instead.
			string assetFolder = fullAssetPath.Replace(Path.GetFileName(fullAssetPath), "").TrimEnd('/');
			// Return that.
			return assetFolder;
		}
		#endregion
		
	}
#endif

}