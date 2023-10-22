using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon {
	
	
	public enum DungeonObjectType {
		StandardEnemy,
		Treasure,
	}

	/// <summary>
	/// This is a base interface for which I can identify other kinds of events a DungeonPlayer may be engaging in.
	/// </summary>
	public interface IDungeonPlayerEvent {
		
	}
	
	/// <summary>
	/// Similar to IPlayerInteractable, but whereas there can be only one IPlayerInteractable
	/// at a time on any given GameObject, this can allow for multiple components.
	/// </summary>
	public interface IDungeonPlayerApproachHandler : IDungeonPlayerEvent {
		/// <summary>
		/// Called when the DungeonPlayer approaches this DungeonObject.
		/// </summary>
		void OnDungeonPlayerApproach();
		/// <summary>
		/// Called when the DungeonPlayer leaves this DungeonObject.
		/// </summary>
		void OnDungeonPlayerLeave();
	}

	/// <summary>
	/// A new way to define components that a DungeonPlayer can theoretically interact with.
	/// </summary>
	public interface IDungeonPlayerInteractionHandler : IDungeonPlayerEvent {
		/// <summary>
		/// Gets called when the player does try to interact with this object.
		/// </summary>
		void OnPlayerInteract();
	}
	
	/// <summary>
	/// Something that needs to respond to being attacked should implement this.
	/// </summary>
	public interface IDungeonPlayerAttackHandler : IDungeonPlayerEvent {

		/// <summary>
		/// Should the DungeonPlayer be locked upon invokation of this handler?
		/// Good for situations where an enemy is attacked and I need to make sure the player isn't freed.
		/// </summary>
		bool LockDungeonPlayer { get; }
		
		/// <summary>
		/// Gets called when the dungeon player attacks whatever implements this.
		/// </summary>
		/// <param name="dungeonPlayer"></param>
		void OnDungeonPlayerAttack(DungeonPlayer dungeonPlayer);

	}
	
}