using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.Serialization;
using Grawly.Gauntlet.Modifiers;
using System.Linq;
using System;
using System.Reflection;

namespace Grawly.Gauntlet.Nodes {

	/// <summary>
	/// Nodes can inherit from this class to do shit on the world map.
	/// </summary>
	[RequireComponent(typeof(Selectable))]
	public class GauntletNode : SerializedMonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IMoveHandler {
		
		#region FIELDS - VARIABLES
		/// <summary>
		/// The variables that govern the state for this gauntlet node.
		/// </summary>
		[SerializeField, FoldoutGroup("Variables", expanded: false), HideLabel]
		private GauntletNodeVariables variables = new GauntletNodeVariables();
		/// <summary>
		/// The variables that govern the state for this gauntlet node.
		/// </summary>
		public GauntletNodeVariables Variables {
			get {
				return this.variables;
			}
		}
		/// <summary>
		/// The neighbors that surround this node.
		/// </summary>
		[SerializeField, FoldoutGroup("Neighbors", expanded: false), HideLabel]
		private GauntletNodeNeighbors neighbors = new GauntletNodeNeighbors();
		#endregion

		#region FIELDS - BEHAVIORS AND LOGIC
		/// <summary>
		/// A list of behaviors that control most of the logic for this gauntlet node.
		/// </summary>
		[SerializeField, FoldoutGroup("Primary Behavior", expanded: false)]
		private List<GauntletNodeBehavior> gauntletNodeBehaviors = new List<GauntletNodeBehavior>();
		/// <summary>
		/// The "primary" behavior. This takes precience because it does shit like bring up the node title if it has to.
		/// </summary>
		private GauntletNodeBehavior PrimaryBehavior {
			get {
				return this.gauntletNodeBehaviors.First();
			}
		}
		/// <summary>
		/// A list of the behaviors that control most of the animations for this gauntlet node.
		/// Should be kept constant. If dynamic behavior is needed, use modifiers first.
		/// </summary>
		[SerializeField, FoldoutGroup("Animation Behavior", expanded: false)]
		private List<GauntletNodeAnimator> gauntletNodeAnimators = new List<GauntletNodeAnimator>();
		/// <summary>
		/// A list of gauntlet node modifiers that respond to different events.
		/// </summary>
		[SerializeField, FoldoutGroup("Serialized Modifiers", expanded: false)]
		private List<GauntletNodeModifier> gauntletNodeModifiers = new List<GauntletNodeModifier>();
		/// <summary>
		/// Just an extra list to keep things that are in the scene I can make prefabs out of.
		/// </summary>
		[SerializeField, FoldoutGroup("Scene Modifiers", expanded: false)]
		private List<GauntletNodeGameObject> gauntletNodeGameObjects = new List<GauntletNodeGameObject>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// Go through each node behavior and assign it a reference to this node.
			this.gauntletNodeBehaviors.ForEach(b => b.AssignGauntletNode(gauntletNode: this));
		}
		private void Start() {
			// Pass a reference to this node over to the gauntlet controller so it knows to send it events if needed.
			GauntletController.instance.AddGauntletNode(gauntletNode: this);
			// Set up the selectable based on the neighbors.
			this.RefreshNavigation();
		}
		#endregion

		#region MODIFIERS
		/// <summary>
		/// Adds a GauntletNodeModifier to the list of gauntlet node modifiers.
		/// </summary>
		/// <param name="modifier">The modifier to add.</param>
		public void AddModifier(GauntletNodeModifier modifier) {
			this.gauntletNodeModifiers.Add(modifier);
		}
		/// <summary>
		/// Removes a GauntletNodeModifier to the list of gauntlet node modifiers.
		/// </summary>
		/// <param name="modifier">The modifier to remove.</param>
		/// <returns>Whether the modifier was removed successfully or not.</returns>
		public bool RemoveModifier(GauntletNodeModifier modifier) {
			return this.gauntletNodeModifiers.Remove(modifier);
		}
		/// <summary>
		/// Returns a list of modifiers of the given type.
		/// Also includes things like the animators/behaviors.
		/// </summary>
		/// <typeparam name="T">The type of interface that is being requested.</typeparam>
		/// <returns>A list of all objects which implement the given interface.</returns>
		public List<T> GetModifiers<T>() {

			return this.gauntletNodeBehaviors.Where(b => b is T).Cast<T>()          // Grab the node behaviors first. They take priority.
				.Concat(this.gauntletNodeAnimators.Where(a => a is T).Cast<T>())    // Grab the animators second.
				.Concat(this.gauntletNodeModifiers.Where(m => m is T).Cast<T>())	// Grab the modifiers last.
				.Concat(this.gauntletNodeGameObjects.Where(g => g is T).Cast<T>())	// Also get the scene game objects.
				.ToList();

			/*return this.gauntletNodeBehaviors.Cast<T>()				
				.Concat(this.gauntletNodeAnimators.Cast<T>())		
				.Concat(this.gauntletNodeModifiers.Cast<T>())		
				.ToList();*/
		}
		/// <summary>
		/// Creates a sequence which builds reactions out of all the modifiers of the specified type.
		/// </summary>
		public void CallModifiers<T>() {
			// Just call the normal version and pass it a blank delegate.
			this.CallModifiers<T>(finishCallback: delegate { });
		}
		/// <summary>
		/// Creates a sequence which builds reactions out of all the modifiers of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of event to call.</typeparam>
		/// <param name="finishCallback">The callback to run when the reaction sequence is finished.</param>
		public void CallModifiers<T>(Action finishCallback) {

			Debug.Log("Calling modifiers of type " + typeof(T).ToString() + " on node " + this.Variables.nodeTitle);

			// Create a new reaction sequence.
			GauntletReactionSequence reactionSequence = new GauntletReactionSequence();

			// Prep that sequence with the callback.
			reactionSequence.Prepare(defaultFinishCallback: finishCallback);

			// Get all the modifiers that implement the specified type and run their first method.
			reactionSequence.AddToSequence(gauntletReactions:
				this.GetModifiers<T>()																	// Get all the modifiers of the specified type.
				.Select(i => (GauntletReaction)typeof(T).GetMethods().First().Invoke(					// Find the very first function in that type.
					obj: i, parameters: new object[] {													// Invoke that first method, and pass it...
						new GauntletNodeEventParams(													// ... a new event params...
							gauntletNode: this,															// ... with the gauntlet node param being this node...
							gauntletMarker: GauntletMarker.instance) }))								// ... and the marker being the marker Instance.
				.ToList());

			// Begin the chain.
			reactionSequence.ExecuteNextReaction();
		}
		#endregion

		#region NODE EVENTS
		/*/// <summary>
		/// The function that should get executed when the marker enters this node.
		/// </summary>
		public void EnterNode() {

			// Tell the gauntlet menu controller to populate itself with the info from the primary behavior.
			GauntletMenuController.Instance.NodeTitle.Prepare(nodeTitleUser: this.NodeTitleUser);

			// Clear out the enter node reaction sequence.
			this.enterNodeReactionSequence.ClearSequence();

			// Prep it with a delegate to run when finished.
			this.enterNodeReactionSequence.Prepare(defaultFinishCallback: delegate { });

			// Go through all the modifiers/behaviors/animators that implement IOnEnterNode and grab their sequences.
			this.enterNodeReactionSequence.AddToSequence(gauntletReactions: 
				this.GetModifiers<IOnEnterNode>()
				.Select(i => i.OnEnterNode(gauntletNode: this, gauntletMarker: GauntletMarker.Instance))
				.ToList());

			// Begin execution of the sequence.
			this.enterNodeReactionSequence.ExecuteNextReaction();

		}
		/// <summary>
		/// The function that should get executed when the marker exits this node.
		/// </summary>
		public void ExitNode() {

			// Clear out the enter node reaction sequence.
			this.exitNodeReactionSequence.ClearSequence();

			// Prep it with a delegate to run when finished.
			this.exitNodeReactionSequence.Prepare(defaultFinishCallback: delegate { });

			// Go through all the modifiers/behaviors/animators that implement IOnEnterNode and grab their sequences.
			this.exitNodeReactionSequence.AddToSequence(gauntletReactions:
				this.GetModifiers<IOnExitNode>()
				.Select(i => i.OnExitNode(gauntletNode: this, gauntletMarker: GauntletMarker.Instance))
				.ToList());

			// Begin execution of the sequence.
			this.exitNodeReactionSequence.ExecuteNextReaction();

		}
		/// <summary>
		/// The function that should get executed when this node receives a submit event.
		/// </summary>
		public void SubmitNode() {
			// Clear out the enter node reaction sequence.
			this.submitNodeReactionSequence.ClearSequence();

			// Prep it with a delegate to run when finished.
			this.submitNodeReactionSequence.Prepare(defaultFinishCallback: delegate { });

			// Go through all the modifiers/behaviors/animators that implement IOnEnterNode and grab their sequences.
			this.submitNodeReactionSequence.AddToSequence(gauntletReactions:
				this.GetModifiers<IOnSubmitNode>()
				.Select(i => i.OnSubmitNode(gauntletNode: this))
				.ToList());

			// Begin execution of the sequence.
			this.submitNodeReactionSequence.ExecuteNextReaction();
		}
		/// <summary>
		/// The function that should get executed when this node receives a Complete event.
		/// </summary>
		/// <param name="finishCallback">The callback that should be run when the sequence is complete.</param>
		public void CompleteNode(Action finishCallback) {

			// Turn off the node title for now since it will be redrawn soon.
			GauntletMenuController.Instance.NodeTitle.SetVisualsActive(false);

			// Create a reaction sequence.
			GauntletReactionSequence reactionSequence = new GauntletReactionSequence();

			// Make its default callback to be the reselection of the current node.
			reactionSequence.Prepare(defaultFinishCallback: delegate {
				// Turn the node title back on.
				GauntletMenuController.Instance.NodeTitle.SetVisualsActive(true);
				// Make sure to redraw the title after all this.
				GauntletMenuController.Instance.NodeTitle.Prepare(nodeTitleUser: this.NodeTitleUser);
			});

			// Find all IOnCompleteNode implementations that belong to this node and add them to the sequence.
			reactionSequence.AddToSequence(gauntletReactions: this.GetModifiers<IOnCompleteNode>().Select(m => m.OnCompleteNode(gauntletNode: this)).ToList());

			// Run that shit baybee.
			reactionSequence.ExecuteNextReaction();
		}*/
		#endregion

		#region OTHER
		/// <summary>
		/// Updates the navigation on the selectable component to reflect the current state of the node.
		/// </summary>
		public void RefreshNavigation() {
			// Just assign a new navigation object.
			this.GetComponent<Selectable>().navigation = this.neighbors.GetNavigation(gauntletNode: this);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - EVENT SYSTEMS
		public void OnSelect(BaseEventData eventData) {
			// Call the function that handles selection implementation.
			// this.SelectNode();
		}
		public void OnDeselect(BaseEventData eventData) {
			// Call the function that handles deselection implementation.
			// this.DeselectNode();
		}
		public void OnSubmit(BaseEventData eventData) {
			// Call the submit node event.
			this.CallModifiers<IOnSubmitNode>();
		}
		public void OnMove(AxisEventData eventData) {
			// Check if the target object is a gauntlet node and also is not this node.
			if (eventData.selectedObject.GetComponent<GauntletNode>() != null && eventData.selectedObject.GetComponent<GauntletNode>() != this) {
				// If it is a gauntlet node, tell the controller to select it.
				GauntletController.instance.MarkerMoveEvent(currentNode: this, nextNode: eventData.selectedObject.GetComponent<GauntletNode>());
			}
			
		}
		#endregion

		#region EDITOR FUNCTIONS
		/// <summary>
		/// Just renames the node to the ID in the variables.
		/// </summary>
		[ShowInInspector, TabGroup("Editor","Setup")]
		private void RenameNodeToID() {
			this.gameObject.name = "Node " + this.Variables.nodeId.ToString() + "     (" + this.PrimaryBehavior.GetType().Name.ToString() + ")";
		}
		#endregion

	}


}