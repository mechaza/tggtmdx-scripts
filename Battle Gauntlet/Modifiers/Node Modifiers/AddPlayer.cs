using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Gauntlet.Nodes;
using Grawly.Chat;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using Grawly.Battle;
using Grawly.UI;
using Grawly.Battle.BattleMenu;

namespace Grawly.Gauntlet.Modifiers.NodeModifiers {

	/// <summary>
	/// Adds a player to the party.
	/// </summary>
	[System.Serializable]
	public class AddPlayer : GauntletNodeEventModifier {

		#region FIELDS - RESOURCES
		/// <summary>
		/// The script to read when completing this node.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "Assets")]
		private TextAsset scriptToRead;
		/// <summary>
		/// The player to add.
		/// </summary>
		[SerializeField, TabGroup("Modifier", "Assets")]
		private PlayerTemplate playerTemplate;
		#endregion

		/// <summary>
		/// Adds a player to the party upon the given event.
		/// </summary>
		/// <param name="eventParams"></param>
		/// <returns></returns>
		protected override GauntletReaction ModifierEvent(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				GauntletController.instance.SetFSMState(GauntletStateType.Wait);
				// Upon completion of the node, open a chat script. When its done, add the player template, rebuild their statuses, and remove this modifier.
				ChatControllerDX.GlobalOpen(
					textAsset: this.scriptToRead,
					chatClosedCallback: delegate {
						GameController.Instance.Variables.AddPlayers(templates: new List<PlayerTemplate>() { this.playerTemplate });
						PlayerStatusDXController.instance.BuildPlayerStatuses(players: GameController.Instance.Variables.Players);
						PlayerStatusDXController.instance.TweenSize(big: false);
						eventParams.GauntletNode.RemoveModifier(modifier: this);
						GauntletController.instance.SetFSMState(GauntletStateType.Free);
						reactionSequence.ExecuteNextReaction();
					},
					chatOpenedCallback: delegate {

					});

			};
		}

		#region INSPECTOR JUNK
		private string inspectorDescription = "Adds a player to the party.";
		protected override string InspectorDescription {
			get {
				return this.inspectorDescription;
			}
		}
		#endregion


	}


}