using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;
using Cinemachine;
using Grawly.Dungeon;

namespace Grawly {
	
	/// <summary>
	/// This class can be put into a scene when I need to reference objects/assets by ID,
	/// but said references are specific to whatever scene is loaded.
	/// </summary>
	public class LocalVariablesController : SerializedMonoBehaviour {
		
		public static LocalVariablesController Instance { get; private set; }

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// A dictionary of GameObjects identified by string keys.
		/// </summary>
		[SerializeField, TabGroup("Local", "Scene References")]
		private Dictionary<string, GameObject> gameObjectDict = new Dictionary<string, GameObject>();
		#endregion
		
		#region FIELDS - RESOURCES : BATTLE TEMPLATES
		/// <summary>
		/// A dictionary of battle templates to store in the local scene.
		/// </summary>
		[SerializeField, TabGroup("Local", "Assets")]
		private Dictionary<string, BattleTemplate> localBattleTemplateDict = new Dictionary<string, BattleTemplate>();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			// Theoretically, this object should be destroyed when loading a new scene.
			// If there is already one in a scene and this code is running, that's... not good.
			Debug.Assert(Instance == null);
			Instance = this;
		}
		#endregion

		#region GETTERS - GAMEOBJECTS
		/// <summary>
		/// Gets a GameObject in the scene with the specified Key.
		/// </summary>
		/// <param name="key">The key which identifies the desired GameObject.</param>
		/// <returns>The GameObject associated with the specified key.</returns>
		public GameObject GetLocalObject(string key) {
			return this.gameObjectDict[key];
		}
		/// <summary>
		/// Gets an object of the specified type that is associated with the specified key.
		/// Will fail if there is no object for the key or if it isn't the provided type.
		/// </summary>
		/// <param name="key">The key which identifies the object to return.</param>
		/// <typeparam name="T">The type of the desired object.</typeparam>
		/// <returns>The object associated with the specified key.</returns>
		public T GetLocalObject<T>(string key) {
			// Get the GameObject normally then find its component of the given type.
			return this.GetLocalObject(key: key).GetComponent<T>();
		}
		#endregion
		
		#region GETTERS - BATTLE TEMPLATES
		/// <summary>
		/// Returns a BattleTemplate associated with the specified key.
		/// </summary>
		/// <param name="key">The key that identifies this battle.</param>
		/// <returns>The battle template associated with this key.</returns>
		public BattleTemplate GetBattleTemplate(string key) {
			// Obviously, situations that call this function should be damn sure it exists.
			Debug.Assert(this.localBattleTemplateDict.ContainsKey(key));
			return this.localBattleTemplateDict[key];
		}
		#endregion
		
	}
}