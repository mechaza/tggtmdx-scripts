using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Grawly.Battle;
using Grawly.Gauntlet.Nodes;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Linq;
using Grawly.UI;
using System;

namespace Grawly.Gauntlet {

	/// <summary>
	/// Provides control over different things around the Gauntlet screen.
	/// </summary>
	[RequireComponent(typeof(PlayMakerFSM))]
	public class GauntletController : MonoBehaviour {

		public static GauntletController instance;

		#region FIELDS - TOGGLES

		#endregion

		#region FIELDS - SETUP
		/// <summary>
		/// Should the controller select a node on the scene start?
		/// </summary>
		[SerializeField, TabGroup("Controller", "Setup")]
		private bool selectNodeOnStart = false;
		/// <summary>
		/// The gauntlet node to select by default, if selection on scene start is active.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Setup"), ShowIf("selectNodeOnStart")]
		private GauntletNode defaultGauntletNode;
		#endregion

		#region FIELDS - STATE
		/// <summary>
		/// The node that is currently being selected.
		/// Good for when I need to reselect it coming from the pause menu.
		/// </summary>
		public GauntletNode CurrentSelectedNode { get; private set; }
		#endregion

		#region FIELDS - SCENE REFERENCES : EVENT RECEIVERS
		/// <summary>
		/// A list of all GauntletNodes in the scene. The nodes will add themselves to the list on startup.
		/// </summary>
		public List<GauntletNode> GauntletNodes { get; private set; } = new List<GauntletNode>();
		#endregion

		#region FIELDS - SCENE REFERENCES - OTHER
		/// <summary>
		/// The FSM that controls higher level state for this controller.
		/// </summary>
		public PlayMakerFSM FSM { get; private set; }
		/// <summary>
		/// The mmain camera. Basically just tracks the marker.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private Camera mainCam;
		/// <summary>
		/// The mmain camera. Basically just tracks the marker.
		/// </summary>
		public Camera MainCamera {
			get {
				return this.mainCam;
			}
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
			this.FSM = this.transform.GetComponent<PlayMakerFSM>();
			// Also make sure that the UI scene is loaded.
			SceneManager.LoadScene("Gauntlet UI", LoadSceneMode.Additive);
		}
		private void Start() {
			// If I want to select a node on start, do so.
			if (this.selectNodeOnStart == true) {
				this.CurrentSelectedNode = this.defaultGauntletNode;
				EventSystem.current.SetSelectedGameObject(this.defaultGauntletNode.gameObject);
				GauntletMarker.instance.MoveToNode(node: this.defaultGauntletNode);
				// this.defaultGauntletNode.EnterNode();
				this.defaultGauntletNode.CallModifiers<IOnEnterNode>(finishCallback: delegate { });
			} else {
				GauntletMenuController.instance.NodeTitle.SetVisualsActive(false);
			}
			this.GetAllModifiers<IOnGauntletStart>().ForEach(m => m.OnGauntletStart());
		}
		private void Update() {
			if (this.FSM.ActiveStateName == "Node Select" && InputController.Instance.GetButtonDown("Pause") == true) {
				this.SetFSMState(state: GauntletStateType.Pause);
			}
		}
		#endregion

		#region MODIFIERS
		/// <summary>
		/// Gets all the modifiers associated with the given type.
		/// </summary>
		/// <typeparam name="T">A list of objects which implement T.</typeparam>
		public List<T> GetAllModifiers<T>() {
			return this.GauntletNodes.SelectMany(n => n.GetModifiers<T>()).ToList();
			// return this.gauntletNodes.Where(n => n is T).Cast<T>().ToList();
		}
		#endregion

		#region NODES
		/// <summary>
		/// Adds a GauntletNode to the list of GauntletNodes that should receive events.
		/// </summary>
		/// <param name="gauntletNode">The GauntletNode to add.</param>
		public void AddGauntletNode(GauntletNode gauntletNode) {
			this.GauntletNodes.Add(gauntletNode);
		}
		/// <summary>
		/// Sets whether or not the gauntlet nodes are allowed to be selected.
		/// Helps with making sure I don't accidentally select something I shouldn't.
		/// </summary>
		/// <param name="status">Whether or not the nodes should be selecable or not.</param>
		private void SetGauntletNodeSelectables(bool status) {
			//
			// IDEA: Maybe make a variable whbich says whether a node is selecable or not, and make the enabled property set to status && [isselectable]
			//
			Debug.Log("GAUNTLET: Setting node selectables to " + status);
			this.GauntletNodes.ForEach(n => n.GetComponent<Selectable>().enabled = status);
		}
		/// <summary>
		/// The event that gets called when the marker moves to a new node.
		/// </summary>
		/// <param name="currentNode">The node that was previously selected.</param>
		/// <param name="nextNode">The node to select.</param>
		public void MarkerMoveEvent(GauntletNode currentNode, GauntletNode nextNode) {
			Debug.Log("GAUNTLET: Selecting node: " + nextNode.gameObject.name);

			// Remember that this node is being selected.
			this.CurrentSelectedNode = nextNode;

			// Tell the marker to move over to the node.
			GauntletMarker.instance.MoveToNode(node: nextNode);

			// Tell the current node an exit just occured.
			// currentNode.ExitNode();
			currentNode.CallModifiers<IOnExitNode>();

			// Tell the next node that it has just been entered.
			// nextNode.EnterNode();
			nextNode.CallModifiers<IOnEnterNode>();

		}
		#endregion

		#region GAUNTLET EVENTS - STATE MACHINE
		/// <summary>
		/// Sets the state of the FSM.
		/// </summary>
		/// <param name="state"></param>
		public void SetFSMState(GauntletStateType state) {

			Debug.Log("GAUNTLET: Setting state to " + state.ToString());

			switch (state) {
				case GauntletStateType.Battle:
					this.FSM.SendEvent("Enemy Encounter");
					break;
				case GauntletStateType.Free:
					this.FSM.SendEvent("Free");
					break;
				case GauntletStateType.Wait:
					this.FSM.SendEvent("Wait");
					break;
				case GauntletStateType.Pause:
					this.FSM.SendEvent("Pause");
					break;
				case GauntletStateType.BattleResults:
					this.FSM.SendEvent("Battle Complete");
					break;
				default:
					Debug.LogError("Couldn't determine GauntletController FSM state!");
					break;
			}
		}
		/// <summary>
		/// Just a simple method to get called in PlayMaker to open up the pause menu controller.
		/// </summary>
		public void OpenPauseMenu() {
			this.Wait();
			PauseMenuController.Open();
		}
		/// <summary>
		/// Gets called from the FSM.
		/// Disables things like the pause menu and doesn't allow selection of nodes.
		/// </summary>
		private void Wait() {
			// Set the selectables on the nodes off.
			this.SetGauntletNodeSelectables(false);
			// Null the currently selected game object.
			EventSystem.current.SetSelectedGameObject(null);
		}
		/// <summary>
		/// Gets called from the FSm.
		/// Allows for use of the pause menu and whatever.
		/// </summary>
		public void Free() {
			// Set the selectables on.
			this.SetGauntletNodeSelectables(true);
			// Reselect the current node. It is assumed I WILL be reselecting the current node variable. That's... what I should be on. Right?
			EventSystem.current.SetSelectedGameObject(this.CurrentSelectedNode.gameObject);
		}
		#endregion

		#region GETTERS - HELPERS AND LOGIC
		/// <summary>
		/// Gets the risk associated with the given battle when done by the players in the specifid game variables.
		/// </summary>
		/// <param name="gameVariables">The GameVariables containing the state of the party.</param>
		/// <param name="battleTemplate">The BattleTemplate that is being evaluated.</param>
		/// <returns></returns>
		public BattleRiskType GetBattleRisk(GameVariables gameVariables, BattleTemplate battleTemplate) {
			return BattleRiskType.High;
		}
		/// <summary>
		/// Gets the string associated with the given battle risk.
		/// </summary>
		/// <param name="gameVariables"></param>
		/// <param name="battleTemplate"></param>
		/// <returns></returns>
		public string GetBattleRiskString(GameVariables gameVariables, BattleTemplate battleTemplate) {
			switch (this.GetBattleRisk(gameVariables, battleTemplate)) {
				case BattleRiskType.Low:
					return "<c=greeny>safe</c>";
				case BattleRiskType.Normal:
					return "";
				case BattleRiskType.High:
					return "<c=yellowy>danger</c>";
				case BattleRiskType.VeryHigh:
					return "<c=reddy>fatal</c>";
				default:
					return "";
			}
		}
		#endregion

	}

	/// <summary>
	/// The possible types of state for the gauntlet. Mostly to just make things easier for me.
	/// </summary>
	public enum GauntletStateType {
		Initialization = 0,
		Free = 10,
		Wait = 15,
		Battle = 20,
		BattleResults = 25,
		Pause = 50,
		ERROR = -1,
	}

}