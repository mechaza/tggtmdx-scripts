using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using System.Linq;
using DG.Tweening.Core;
using Grawly.Battle.Functions;
using Grawly.UI;
using UnityEngine.EventSystems;
using Grawly.Battle.Analysis;
using Grawly.Toggles.Proto;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;

namespace Grawly.Battle.Results {
	
	/// <summary>
	/// The different states that a Learn Skill screen can be in.
	/// This includes both level up moves and skill cards.
	/// </summary>
	public enum LearnSkillStateType {
		None				= 0,		// Theoretically, not displayed.
		PreConfirmation		= 1,		// Before a skill has been learned.
		PostConfirmation	= 2,		// After it has been learned.
	}
	
}


