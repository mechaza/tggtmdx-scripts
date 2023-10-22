using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly {

	/// <summary>
	/// A simple shorthand for when I want to think of a switch as being "not ready/ready/complete."
	/// E.x., good for firing off an event that I want to play a cutscene and then mark that same cutscene flag as complete.
	/// </summary>
	public enum FlagStatusType {
		NotReady = 0,
		Ready = 1,
		Complete = 2,
		Skipped = 3,
		Failed = 4,
	}


}