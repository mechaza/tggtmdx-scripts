using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Provides a certain amount of metadata/behavior that characterizes this modifier as an affliction. 
	/// A combatant can only have one affliction at a time.
	/// Also provides certain data needed in order to convey this affliction.
	/// </summary>
	public interface ICombatantAffliction {
		/// <summary>
		/// The type of affliction this is. (None, Poison, Burn, Paralyze, etc.)
		/// </summary>
		AfflictionType Type { get; }
	}

	/// <summary>
	/// Changes attributes on this combatant to be stronger/weaker. Similar in concept to the status modifier.
	/// </summary>
	public interface IPowerBooster {
		/// <summary>
		/// Gets the amount to boost the specified power boost type by. Uh. Yeah.
		/// This is in addition to the status modifiers.
		/// </summary>
		/// <param name="boostType"></param>
		/// <returns></returns>
		float GetPowerBoost(PowerBoostType boostType);
	}

	/// <summary>
	/// Modifies the cost of a battle behavior.
	/// </summary>
	public interface IInterceptBattleBehaviorCost {
		/// <summary>
		/// Modifies the cost of a battle behavior.
		/// </summary>
		/// <param name="runningCost">The *current* value of the behavior cost. This is important when I need to chain modifiers.</param>
		/// <param name="self">The combatant who owns this behavior.</param>
		/// <param name="behavior">The behavior being probed for its modified cost.</param>
		/// <returns>The new cost for the behavior.</returns>
		int InterceptBattleBehaviorCost(int runningCost, Combatant self, BattleBehavior behavior);
	}

	/// <summary>
	/// Restricts the set of behaviors a combatant can use. E.x., silence and forget don't allow for skills.
	/// </summary>
	public interface IBehaviorRestrictor {
		/// <summary>
		/// Restricts the behaviors available for use to a combatant.
		/// THIS SHOULD COME *AFTER* GETADDEDBATTLEBEHAVIORS.
		/// </summary>
		/// <param name="specialBehaviors">The combatant's special behaviors.</param>
		/// <returns>The special behaviors with things taken out.</returns>
		List<BattleBehavior> RestrictUsableBehaviors(List<BattleBehavior> specialBehaviors);
	}

	/// <summary>
	/// Only allows for the specified top level selections to be picked out.
	/// </summary>
	public interface ITopLevelSelectionRestrictor {
		/// <summary>
		/// Restricts the usable top level selections to the given list.
		/// If there are multiple modifiers of this type, the intersect will be used.
		/// </summary>
		/// <returns></returns>
		List<BattleMenuDXTopLevelSelectionType> RestrictTopLevelSelections();
	}

	/// <summary>
	/// Specifically overrides a combatant's normal Turn Behavior.
	/// </summary>
	public interface ITurnBehaviorOverride {
		/// <summary>
		/// Whether this should take priority or not.
		/// </summary>
		bool TakesPriority { get; }
		/// <summary>
		/// The function that actually picks out a move to run.
		/// </summary>
		void ExecuteTurn();
	}

	/// <summary>
	/// Gets called when the battle starts.
	/// </summary>
	public interface IOnBattleStart {
		BattleReaction OnBattleStart();
	}

	/// <summary>
	/// Gets called before the next combatant is dequeued.
	/// </summary>
	public interface IOnPreTurn {
		BattleReaction OnPreTurn();
	}

	/// <summary>
	/// An interface that runs an action on the turn's start.
	/// Gets called for *all* combatants; not just the one who is ready.
	/// </summary>
	public interface IOnTurnStart {
		/// <summary>
		/// Gets called at the start of a turn. Is invoked on *all* combatants at the start of a turn.
		/// </summary>
		/// <returns>A sequence to be appended to a larger sequence that is played at the beginning of the turn.</returns>
		BattleReaction OnTurnStart();
	}

	/// <summary>
	/// An interface that runs an action on this combatant's turn, when they're up.
	/// Will only run for the combatant who is ready.
	/// </summary>
	public interface IOnTurnReady {
		/// <summary>
		/// Gets called at the start of this combatant's turn specifically. 
		/// Will only be invoked on the combatant who is ready.
		/// </summary>
		/// <returns>A sequence to be appended to a larger sequence that is played before the combatant runs their turn behavior.</returns>
		BattleReaction OnTurnReady();
	}

	/// <summary>
	/// Intercepts an incoming DamageCalculationSet and performs some last minute modifications.
	/// </summary>
	public interface IInterceptIncomingDCS {
		/// <summary>
		/// Intercepts an incoming DamageCalculationSet and performs some last minute modifications.
		/// </summary>
		/// <param name="dcs">The DamageCalculationSet being intercepted.</param>
		/// <param name="self">The combatant that owns the behavior this passive function is attached to.</param>
		/// <returns></returns>
		DamageCalculationSet InterceptIncomingDCS(DamageCalculationSet dcs, Combatant self);
	}

	/// <summary>
	/// Intercepts an incoming DamageCalculationSet and performs some last minute modifications.
	/// This is a version that gets called AFTER IInterceptIncomindDCS, and should only ever be the last pass made in a DamageCalculationSet.
	/// </summary>
	public interface IInterceptIncomingDCSLate {
		/// <summary>
		/// Intercepts an incoming DamageCalculationSet and performs some last minute modifications.
		/// </summary>
		/// <param name="dcs">The DamageCalculationSet being intercepted.</param>
		/// <param name="self">The combatant that owns the behavior this passive function is attached to.</param>
		/// <returns></returns>
		DamageCalculationSet InterceptIncomingDCSLate(DamageCalculationSet dcs, Combatant self);
	}

	/// <summary>
	/// An interface that runs an action when this combatant is attacked.
	/// </summary>
	public interface IOnAttacked {
		/// <summary>
		/// Is this ready to trigger?
		/// Mostly needed to make sure it doesn't get activated the same turn it was set.
		/// </summary>
		bool ReadyToTrigger { get; }
		/// <summary>
		/// Gets called when the combatant is attacked
		/// This specifically returns void because its not called from any event in particular in the BattleController.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was the source for this attack.</param>
		BattleReaction OnAttacked(DamageCalculationSet damageCalculationSet);
	}

	/// <summary>
	/// Gets called when the behavior of the turn has been evaluaated.
	/// </summary>
	public interface IOnBehaviorEvaluated {
		/// <summary>
		/// Gets called when the behavior of the turn has been evaluaated.
		/// </summary>
		BattleReaction OnBehaviorEvaluated();
	}

	/// <summary>
	/// An interface that runs an action on the combatant's turn end.
	/// </summary>
	public interface IOnTurnEnd {
		/// <summary>
		/// Gets called when the combatant's turn ends.
		/// </summary>
		/// <returns>A sequence to be appended to a larger sequence that is played at the end of the turn.</returns>
		BattleReaction OnTurnEnd();
	}
	/// <summary>
	/// An interface that defines functionality for when the player returns to the dungeon.
	/// </summary>
	public interface IOnBackToDungeon {
		void OnBackToDungeon();
	}
	/// <summary>
	/// Basically just trying to make it clearer if an affliction was set or not.
	/// </summary>
	public enum AfflictionSetResultsType {
		Failure = 0,
		Success = 1,
	}

}

namespace Grawly.Battle {

	public enum AfflictionType {
		None = 0,
		Burn = 1,        // Take damage after turn
		Freeze = 2,      // Cannot move
		Poison = 3,      // Take damage after turn
		Paralyze = 4,    // Cannot move, causes electric shock to those who touch afflicted character
		Dizzy = 5,       // Decrease in accuracy
		Silence = 6,     // Cannot use skills
		Sleep = 7,       // Cannot move, recover HP/MP
		Confuse = 8,   // One of 3 actions each turn: give money to enemy, use recovery item, throw away item
		Fear = 9,        // High chance to not move
		Despair = 10,    // Cannot move, drain MP
		Rage = 11,       // Automatically attack with phys. Attack greatly increased, defense greatly decreased
		Brainwash = 12,      // One of 3 actions each turn: heal enemy, attack teammate, power up enemy
		Hunger = 13,     // Damage decreased
		Forget = 14,    // Cannot use skills.
		Horny = 15,		// Attack does double damage, but costs triple the normal amount
	}

}
