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
	/// When rebuilding the analysis screen, there might be multiple ways to proceed.
	/// This identifies how that should happen.
	/// </summary>
	public enum AnalysisScreenRebuildType {
		
		None				= 0,		// Probably unused.
		
		FocusBanner			= 1,		// The banner is selected on rebuild.
		FocusBehaviorList	= 2,		// The behavior list is selected on rebuild.
		
	}

}
