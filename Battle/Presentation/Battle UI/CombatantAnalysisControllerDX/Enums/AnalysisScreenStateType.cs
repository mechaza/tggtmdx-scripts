using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// The different states the analysis screen can be in.
	/// </summary>
	public enum AnalysisScreenStateType {
		None		= 0,
		Hidden		= 1,
		Displayed	= 2,
	}
}