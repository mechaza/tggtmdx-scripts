using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;

namespace Grawly.Data {

	/// <summary>
	/// Stores objects I need for any kind of BFX.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Data/Scriptable BFX")]
	public class ScriptableBFX : ScriptableObject {
		
		#region FIELDS - QUERIES
		public ElementType elementType = ElementType.None;
		#endregion

		#region FIELDS - RESOURCES
		/// <summary>
		/// The prefab to be instansiated into the scene.
		/// </summary>
		[Title("Resources")]
		public GameObject bfxPrefab;
		/// <summary>
		/// The audio clip that should get played when this bfx is instansiated.
		/// </summary>
		public AudioClip bfxAudioClip;
		#endregion


		
	}


}