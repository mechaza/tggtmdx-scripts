using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Chat;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;
using Sirenix.Utilities;

#if UNITY_EDITOR

namespace Grawly.Editors {
	
	/// <summary>
	/// A weird workaround to help me design editors.
	/// </summary>
	[System.Serializable]
	public class GrawlyMenuItemReference {

		#region FIELDS - STATE : UH
		/// <summary>
		/// The preset template that should be pointed to.
		/// </summary>
		[OdinSerialize]
		public GamePresetTemplate PresetTemplate { get; private set; }
		#endregion

		#region CONSTRUCTORS
		public GrawlyMenuItemReference(GamePresetTemplate gamePresetTemplate) {
			this.PresetTemplate = gamePresetTemplate;
		}
		#endregion
		
	}
	
}

#endif 