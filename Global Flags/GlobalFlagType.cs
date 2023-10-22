using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly {

	/// <summary>
	/// These can be used to identify types of "global" flags.
	/// These are not part of a Variables object,
	/// but still need to be made available in the GlobalFlagController.
	/// </summary>
	public enum GlobalFlagType {
		
		None							= 0,	// Used at design time to make shit easier.
		
		SuppressAutoPlayMusic			= 1001,	// Stops any music from auto playing upon loading a new scene.
		
		
		
	}

}