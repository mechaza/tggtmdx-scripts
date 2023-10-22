using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using Grawly.Dungeon;
using Grawly.Toggles;
using CameraTransitions;
using Grawly.Battle;
using Cinemachine;
using UnityEngine.Playables;

namespace Grawly.Cutscenes {
	
	/// <summary>
	/// The different kinds of operations that can be performed upon completing a cutscene.
	/// </summary>
	public enum CutsceneCompleteEventType {
		None			= 0,
		NextStoryBeat	= 10,
	}
}