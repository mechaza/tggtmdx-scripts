using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Calendar.Behavior {
	
	/// <summary>
	/// Story behaviors can get run when scenes are loaded.
	/// It makes it easier for me to design, I suppose.
	/// </summary>
	// [GUIColor(r: 1.0f, g: 1.0f, b: 1.0f, a: 1f)]
	public abstract class StoryBehavior {

		#region REACTION GENERATION
		/// <summary>
		/// StoryBehaviors need to generate a reaction when they are loaded.
		/// Pretty much every behavior will end up doing this.
		/// </summary>
		/// <returns></returns>
		public abstract StoryBeatReaction OnStoryBeatLoad();
		#endregion
		
		#region CLONING
		/// <summary>
		/// Creates a clone of this story behavior.
		/// </summary>
		/// <returns>A clone of this story behavior.</returns>
		public StoryBehavior Clone() {
			return (StoryBehavior) this.MemberwiseClone();
		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// The string to use for the foldout groups in the inspector.
		/// </summary>
		protected abstract string FoldoutGroupTitle { get; }
		#endregion
		
	}

}