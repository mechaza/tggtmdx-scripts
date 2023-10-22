using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grawly.Friends.Behaviors {
	
	[InfoBox("Sample behavior for the purpose of testing.")]
	[System.Serializable]
	public class SampleBehavior : FriendBehavior {
		public override string BehaviorName => "Sample Behavior";
		public override string BehaviorDescription => "This is a placeholder behavior.";
	}

	
}