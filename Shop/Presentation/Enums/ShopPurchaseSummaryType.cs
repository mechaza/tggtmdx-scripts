using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Grawly.UI.MenuLists;
using HutongGames.PlayMaker;

namespace Grawly.Shop {
	
	/// <summary>
	/// Just defines what kind of summaries should be built on the purchase confirmation screen.
	/// </summary>
	public enum ShopPurchaseSummaryType {
		None		= 0,
		Before		= 1,
		After		= 2,
	}
}