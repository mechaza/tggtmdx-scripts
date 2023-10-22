using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.UI.SubItem;

namespace Grawly.Toggles {

	/// <summary>
	/// This mostly exists so I have an easier way of making changes to the way most standard toggles are designed.
	/// </summary>
	[System.Serializable]
	public abstract class StandardToggleDX : GameToggleDX, GTSubItem {

		#region INTERFACE IMPLEMENTATION - GTSUBITEM
		/// <summary>
		/// The SubItemParams that represent the state this toggle is currently in.
		/// </summary>
		public abstract SubItemParams CurrentSubItemParams { get; }
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		/// <summary>
		/// The primary string is just the name of this toggle.
		/// </summary>
		public override string PrimaryString => this.ToggleName;
		/// <summary>
		/// The description string is just the description.
		/// </summary>
		public override string DescriptionString => this.ToggleDescription;
		public override Sprite Icon => throw new System.NotImplementedException();
		#endregion
	}


}