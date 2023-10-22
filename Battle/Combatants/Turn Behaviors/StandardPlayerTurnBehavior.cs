using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;
using Grawly.Battle.Modifiers;
using Sirenix.OdinInspector;

namespace Grawly.Battle.TurnBehaviors {

	/// <summary>
	/// The behavior that should be run when it's the player's turn. 
	/// This is technically just a wrapper for interfacing with the battle menu controller but it helps not have to use if/else statements in the battle controller.
	/// </summary>
	[System.Serializable]
	[InfoBox("Brings up the battle menu for the player to make their selection.")]
	public class StandardPlayerTurnBehavior : CombatantTurnBehavior {

		#region TOGGLES
		/// <summary>
		/// The number of actions per turn this player can make.
		/// </summary>
		[SerializeField]
		private int actionsPerTurn = 1;
		#endregion
		
		#region GETTERS - STATE
		/// <summary>
		/// Does this combatant have more actions on this turn?
		/// </summary>
		/// <param name="turnActionCount">The number of actions the combatant has taken so far.</param>
		/// <returns>Whether or not this combatant has any more actions this turn.</returns>
		public override bool HasMoreTurnActions(int turnActionCount) {
			
			// Theoretically this should always be more than zero.
			Debug.Assert(this.actionsPerTurn > 0);
			
			// If the combatant has taken any actions, they're done.
			if (turnActionCount >= this.actionsPerTurn) {
				return false;
			} else {
				return true;
			}
		}
		#endregion
		
		#region TURN EXECUTION
		public override void ExecuteTurn() {

			Debug.Log("BATTLE: Executing turn for " + this.combatant.metaData.name);

			// Set the sprite for the bust up. Note that this doesn't necessarily turn the sprite ON. Just resets the image that is used.
			BattleMenuControllerDX.instance.SetPlayerBustUpSprite(player: this.combatant as Player);

			// Update the status real quick.
			(this.combatant as Player).PlayerStatusDX.QuickRebuild();

			// If the affliction overrides the turn behavior, run that.
			if (this.combatant.Affliction is ITurnBehaviorOverride == true && (this.combatant.Affliction as ITurnBehaviorOverride).TakesPriority == true) {
				Debug.Break();
				(this.combatant.Affliction as ITurnBehaviorOverride).ExecuteTurn();
			} else {
				// If I can run the turn as normal, do that.
				AudioController.instance.PlaySFX(SFXType.PlayerBattleMenu, 1.5f);
				BattleMenuControllerDX.instance.TriggerEvent(eventName: "Player Turn");
			}

			

			/*// If the affliction allows for the player to move, do so.
			if (this.combatant.Affliction.CanMoveOnReady == true) {
				// If the player can make their move, tell the battle menu controller to do its thing.
				AudioController.Instance.PlaySFX(SFXType.PlayerBattleMenu, 1.5f);
				BattleMenuControllerDX.Instance.TriggerEvent(eventName: "Player Turn");
			} else {
				// If not, call whatever code needs to be run from the affliction.
				// this.combatant.Affliction.OverrideTurnEvent();
				Debug.LogError("Should I continue to use OverrideTurnEvent(), or find something else?");
			}*/

		}
		#endregion

	}


}