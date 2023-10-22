using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Grawly;
using Grawly.Console;
using UnityEngine;
using SRDebugger;

public partial class SROptions {

	#region SAVE DATA
	/// <summary>
	/// Delete the save data entirely.
	/// </summary>
	[Category("Debug"), DisplayName("Delete Save Data")]
	public void DeleteSaveData() {
		Debug.Log("Deleting save data...");
		SaveController.DeleteSaveCollection();
		Debug.Log("Save data deleted.");
	}
	#endregion
	
}