using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Grawly.DungeonCrawler {
	
	#region ENUM DEFINITIONS
	/// <summary>
	/// The different kinds of action prompts available.
	/// </summary>
	public enum ActionPromptType {
		None		= 0,
	}
	#endregion
	
	#region GENERAL INTERFACES
	/// <summary>
	/// An interface for any components inside the crawler scenes.
	/// </summary>
	public interface ICrawlerComponent {
		
	}
	#endregion

	#region PLAYER EVENTS
	/// <summary>
	/// Gets invoked when a player steps.
	/// </summary>
	public interface IPlayerStepHandler : ICrawlerComponent {
		void OnPlayerStep();
	}
	/// <summary>
	/// When a crawler steps on this, well, thats dandy.
	/// </summary>
	public interface IPlayerLandHandler : ICrawlerComponent {
		void OnLand(CrawlerProgressionSet crawlerProgressionSet, int floorNumber);
	}
	/// <summary>
	/// For situations where a player approaches something one tile away in front of them.
	/// </summary>
	public interface IPlayerApproachHandler : ICrawlerComponent {
		void OnApproach(CrawlerProgressionSet crawlerProgressionSet, int floorNumber);
	}
	/// <summary>
	/// Similar to IPlayerApproachHandler, but when turning away.
	/// </summary>
	public interface IPlayerLookAwayHandler : ICrawlerComponent {
		void OnLookAway();
	}
	/// <summary>
	/// When a crawler needs to interact with a tile in front of it, this is what is implemented.
	/// </summary>
	public interface IPlayerInteractHandler : ICrawlerComponent {
		void OnInteract(CrawlerProgressionSet crawlerProgressionSet, int floorNumber);
	}
	#endregion

	#region DUNGEON EVENTS
	/// <summary>
	/// Should get invoked when a floor is generated in a crawler dungeon.
	/// </summary>
	public interface IFloorGeneratedHandler : ICrawlerComponent {
		void OnFloorGenerated(CrawlerProgressionSet crawlerProgressionSet, int floorNumber);
	}
	#endregion
	
}