using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Grawly.DungeonCrawler.Generation;
using Grawly.UI.Legacy;
using Sirenix.Serialization;
using System.Linq;
using Grawly.Chat;

namespace Grawly.DungeonCrawler.Events {
	
	/// <summary>
	/// The different kinds of events that can be associated with an event in a Crawler.
	/// </summary>
	public enum CrawlerEventType {
		None		= 0,
		Chat		= 1,		// Invokes a chat script
	}
}