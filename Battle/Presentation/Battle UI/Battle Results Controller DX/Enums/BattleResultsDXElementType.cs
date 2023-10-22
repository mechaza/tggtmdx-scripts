using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using System.Linq;
using Grawly.Battle.Functions;
using Grawly.UI;
using UnityEngine.EventSystems;
using Grawly.Battle.Analysis;
using Grawly.UI.Legacy;

namespace Grawly.Battle.Results {
	
	/// <summary>
	/// The kinds of elements that can be presented on the new battle results screen.
	/// </summary>
	public enum BattleResultsDXElementType {
		None		= 0,
		Experience	= 1,
		Money		= 2,
		Item		= 3,
	}
	
}