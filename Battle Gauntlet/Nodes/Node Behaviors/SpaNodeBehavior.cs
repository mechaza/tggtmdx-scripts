using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Battle;
// using UnityEngine.EventSystems;
using System.Linq;
using Sirenix.Serialization;
using Grawly.Battle.BattleMenu;

namespace Grawly.Gauntlet.Nodes {

	/// <summary>
	/// Defines how Battle Nodes should behave in response to certain events.
	/// </summary>
	public class SpaNodeBehavior : GauntletNodeBehavior, IGauntletNodeTitleUser, IOnEnterNode, IOnSubmitNode, IOptionPickerUser, IOnCompleteNode {

		#region INTERFACE IMPLEMENTATION - IGAUNTLETNODETITLEUSER
		/// <summary>
		/// Should the metadata visuals be visible on the node title?
		/// </summary>
		public bool UseMetadataVisuals {
			get {
				// Return the inverse of whether or not this node was completed.
				return false;
			}
		}
		/// <summary>
		/// The status of this node. Mostly for the node title graphics.
		/// </summary>
		public GauntletNodeStatusType NodeStatusType {
			get {

				if (this.gauntletNodeOwner.Variables.Visited == false) {
					// If the owner node has not been visited, return New.
					return GauntletNodeStatusType.New;


				} else if (this.gauntletNodeOwner.Variables.completed == true) {
					// If the node has been completed, return Completed.
					return GauntletNodeStatusType.Completed;


				} else {
					// For all other cases, just return Normal.
					return GauntletNodeStatusType.Normal;
				}
			}
		}
		/// <summary>
		/// The string to use on the node title graphics.
		/// </summary>
		public string NodeTitleString {
			get {
				// Return the title as it is stored in the variables.
				return this.gauntletNodeOwner.Variables.nodeTitle;
			}
		}
		/// <summary>
		/// The string for the primary label on the metadata.
		/// </summary>
		public string PrimaryMetadataString {
			get {
				// Return the level of the first enemy template.
				// return "<size=42>Lv</size>" + this.battleTemplate.EnemyTemplates.First().Level.ToString();
				// return "<size=32>risk</size><size=50>" + GauntletController.Instance.GetBattleRisk(gameVariables: GameController.Instance.Variables, battleTemplate: this.battleTemplate);
				return (this.gauntletNodeOwner.Variables.Visited == false) ? "<size=32><c=yellowy>new</c></size>" : "";
			}
		}
		/// <summary>
		/// The string for the secondary label on the metadata.
		/// </summary>
		public string SecondaryMetadataString {
			get {
				// Return the enemy count.
				return "";
			}
		}
		/// <summary>
		/// Should give the risk depending on if the node is complete or not.
		/// </summary>
		public string TertiaryMetadataString {
			get {
				return this.gauntletNodeOwner.Variables.completed == true ? "completed" : "";
			}
		}
		/// <summary>
		/// The sprite to use on the node title.
		/// </summary>
		public Sprite NodeTitleSprite {
			get {
				// Return the icon of the first enemy template.
				return null;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONENTERNODE
		/// <summary>
		/// When entering the node, just increment the move count.
		/// </summary>
		/// <param name="gauntletNode">The node this behavior belongs to.</param>
		/// <param name="gauntletMarker">The marker that moved to this node.</param>
		/// <returns></returns>
		public GauntletReaction OnEnterNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				// Tell the gauntlet menu controller to populate itself with the info from the primary behavior.
				GauntletMenuController.instance.NodeTitle.Prepare(nodeTitleUser: this);
				// Increment the step gount.
				eventParams.GauntletNode.Variables.enterCount += 1;
				reactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONSUBMITNODE
		/// <summary>
		/// Upon submission, the prompt for beginning the battle should appear.
		/// </summary>
		/// <param name="gauntletNode"></param>
		/// <returns></returns>
		public GauntletReaction OnSubmitNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				if (this.gauntletNodeOwner.Variables.completed == true) {
					AudioController.instance.PlaySFX(SFXType.Invalid);
					reactionSequence.ExecuteNextReaction();
				} else {
					GauntletController.instance.SetFSMState(GauntletStateType.Wait);
					OptionPicker.instance.Display(prompt: "Heal party?", sourceOfCall: this);
					reactionSequence.ExecuteNextReaction();
				}
			};
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IOPTIONPICKERUSER
		/// <summary>
		/// If the player wants to begin the battle, do that.
		/// </summary>
		/// <param name="buttonId"></param>
		/// <param name="lastSelectedGameObject"></param>
		public void OptionPicked(int buttonId, GameObject lastSelectedGameObject) {
			if (buttonId == 0) {
				GameController.RestoreParty();
				PlayerStatusDXController.instance.QuickRebuild();
				this.gauntletNodeOwner.CallModifiers<IOnCompleteNode>();
				// GauntletController.Instance.PrepareBattle(battleTemplate: this.battleTemplate);
			} else {
				GauntletController.instance.SetFSMState(GauntletStateType.Free);
				// EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONCOMPLETENODE
		/// <summary>
		/// Upon completion, the node should be marked as... complete.
		/// </summary>
		/// <param name="gauntletNode"></param>
		/// <returns></returns>
		public GauntletReaction OnCompleteNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				eventParams.GauntletNode.Variables.completed = true;
				// Upon completion, also re-open the pathways.
				this.gauntletNodeOwner.Variables.OpenNeighborPathway(GauntletNodeNeighborType.Omega);
				GauntletController.instance.SetFSMState(GauntletStateType.Free);
				this.gauntletNodeOwner.RefreshNavigation();
				// Redraw the title.
				GauntletMenuController.instance.NodeTitle.Prepare(this);
				GauntletMenuController.instance.NodeTitle.SetVisualsActive(true);
				reactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region INSPECTOR JUNK
		private string inspectorDescription = "Heals the party upon use.";
		protected override string InspectorDescription {
			get {
				return this.inspectorDescription;
			}
		}
		#endregion


	}


}