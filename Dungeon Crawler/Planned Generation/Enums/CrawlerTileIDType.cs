using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler.Generation {
	
	/// <summary>
	/// The different kinds of "IDs" associated with tiles inside map templates.
	/// Note that this only really applies when generating tiles.
	/// </summary>
	public enum CrawlerTileIDType {
		None			= -1,
		Blocked			= 0,
		Empty			= 1,
		EmptyDoor		= 2,
		Chest			= 3,
		ShortcutDown	= 4,
		ShortcutLeft	= 5,
		ShortcutRight	= 6,
		ShortcutUp		= 7,
		HorizontalDoor	= 8,
		VerticalDoor	= 9,
		Event			= 10,
		Start			= 11,
		UpStairs		= 12,
		DownStairs		= 13,
		DownStairsUP	= 14,
		DownStairsRT	= 15,
		DownStairsDN	= 16,
		DownStairsLF	= 17,
		UpStairsUP		= 18,
		UpStairsRT		= 19,
		UpStairsDN		= 20,
		UpStairsLF		= 21,
		
		StartUP			= 22,
		StartRT			= 23,
		StartDN			= 24,
		StartLF			= 25,
		
		Chest1			= 358,
		Chest2			= 359,
		Chest3			= 360,
		Chest4			= 361,
		Chest5			= 362,
		Chest6			= 363,
		Chest7			= 364,
		Chest8			= 365,
		Chest9			= 366,
		Chest10			= 367,
		Chest11			= 368,
		Chest12			= 369,
		Chest13			= 370,
		Chest14			= 371,
		Chest15			= 372,
		Chest16			= 373,
		Chest17			= 374,
		Chest18			= 375,
		Chest19			= 376,
		Chest20			= 377,
		
		Teleporter1		= 378,
		Teleporter2		= 379,
		Teleporter3		= 380,
		Teleporter4		= 381,
		Teleporter5		= 382,
		Teleporter6		= 383,
		Teleporter7		= 384,
		Teleporter8		= 385,
		Teleporter9		= 386,
		Teleporter0		= 387,
		
		Event1			= 388,
		Event2			= 389,
		Event3			= 390,
		Event4			= 391,
		Event5			= 392,
		Event6			= 393,
		Event7			= 394,
		Event8			= 395,
		Event9			= 396,
		Event10			= 397,
		Event11			= 398,
		Event12			= 399,
		Event13			= 400,
		Event14			= 401,
		Event15			= 402,
		Event16			= 403,
		Event17			= 404,
		Event18			= 405,
		Event19			= 406,
		Event20			= 407,
		
		Finish			= 408,
		FinishUP		= 409,
		FinishRT		= 410,
		FinishDN		= 411,
		FinishLF		= 412,
		
		Exit			= 413,
		ExitUP			= 414,
		ExitRT			= 415,
		ExitDN			= 416,
		ExitLF			= 417,

		
	}
	
}