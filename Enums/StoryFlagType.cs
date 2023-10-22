using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly {

	/// <summary>
	/// A very basic flag type which can be used as a key in the flags checker to see whether or not certain conditions have been met.
	/// </summary>
	public enum StoryFlagType {

		None				= 0,
		
		// BATTLE TOGGLES
		CanUseAttackInBattle = 1001,
		CanUseSpecialInBattle = 1002,
		CanUseTacticsInBattle = 1003,
		CanUseItemInBattle = 1004,
		CanUseMiscInBattle = 1005,

		// CLOWN COLLEGE DX
		CCDX_CanAccessFrontDeskToAtrium		= 5501,
		CCDX_TalkedToFrontDesk				= 5502,
		CCDX_InitialWestHallEntered			= 5503,
		
		CCDX_InitialTestingRoomEntered		= 5550,
		CCDX_TestTaken						= 5580,
		
		// DEBUG
		DebugFlag1 = 10001,
		DebugFlag2 = 10002,
		DebugFlag3 = 10003,
		DebugFlag4 = 10004,
		DebugFlag5 = 10005,
		DebugFlag6 = 10006,

		
		
		// STATE
		/*WentToDungeonToday = 15001,

		// STORY
		CompletedDungeon1 = 20001,

		// MIAMI MALL
		MM_DidShopping			= 21001,
		MM_DiscoveredDungeon	= 21002,
		
		// CLOWN COLLEGE
		CC_TalkedToReceptionist1 = 25001,
		CC_TalkedToReceptionist2 = 25002,
		CC_TalkedToReceptionist3 = 25003,

		CC_FoughtWarai =			25004,
		CC_SawDetentionScene1 =		25005,
		CC_RealizedWaterParkIsBad =		25006,
		CC_ApologizedToCybergothClown = 25007,
		CC_RetrievedMangaForCGClown =	25008,

		CC_STATUS_WaterSlideCutscene = 25201,		// Status on the water slide cutscene.

		CC_ParkingLotAccessible =	25501,
		CC_EastHallAccessible =		25502,
		CC_WestHallAccessible =		25503,
		CC_NorthHallAccessible =	25504,*/

		// TUTORIAL
		SawAllOutAttackTutorial = 30001,

	}


}