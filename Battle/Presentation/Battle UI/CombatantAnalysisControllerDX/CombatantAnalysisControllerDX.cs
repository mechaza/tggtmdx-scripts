using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Chat;
using System.Linq;
using Grawly.Battle.Functions;
using Grawly.UI;
using Sirenix.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// The new way in which combatant's should be analyzed.
	/// </summary>
	public class CombatantAnalysisControllerDX : SerializedMonoBehaviour {

		public static CombatantAnalysisControllerDX Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The current parameters that are being used to build this screen.
		/// </summary>
		public CombatantAnalysisParams CurrentAnalysisParams { get; private set; }
		/// <summary>
		/// The current callback set in use for the Analysis screen.
		/// If control needs to be dictated in code on how to handle elements on screen, this is what is used.
		/// </summary>
		public AnalysisCallbackSet CurrentCallbackSet { get; private set; }
		/// <summary>
		/// The callback to run when this screen is closed and finished with.
		/// </summary>
		private Action CurrentDismissedCallback { get; set; }
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// A shortcut to reference the currently focused combatant.
		/// </summary>
		public Combatant CurrentCombatant => this.CurrentAnalysisParams.CurrentCombatant;
		#endregion
		
		#region FIELDS - PROTOTYPING
		/// <summary>
		/// A variables template to use when prototyping if the analysis controller works or not.
		/// </summary>
		[Title("General")]
		[SerializeField, TabGroup("Analysis","Prototyping")]
		private GameVariablesTemplate prototypeVariablesTemplate;
		/// <summary>
		/// A battle template to use when building an analysis screen that looks at enemies.
		/// </summary>
		[SerializeField, TabGroup("Analysis","Prototyping")]
		private BattleTemplate prototypeBattleTemplate;
		/// <summary>
		/// The kind of screen to build when prototyping.
		/// </summary>
		[SerializeField, TabGroup("Analysis", "Prototyping")]
		private AnalysisScreenCategoryType prototypeAnalysisType = AnalysisScreenCategoryType.None;
		/// <summary>
		/// A list of behaviors to add to the combatant to make sure this function works.
		/// </summary>
		[Title("Sample Behaviors")]
		[SerializeField, TabGroup("Analysis", "Prototyping")]
		private List<BattleBehavior> prototypeBattleBehaviors = new List<BattleBehavior>();
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects for this analysis canvas.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis", "Scene References")]
		public GameObject AllObjects { get; private set; }
		/// <summary>
		/// The state machine that manages the analysis controller.
		/// </summary>
		[SerializeField, TabGroup("Analysis", "Scene References")]
		private PlayMakerFSM fsm;
		/// <summary>
		/// The borders that can be tweened in/out for the analysis screen.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis","Scene References")]
		public ChatBorders ChatBorders{ get; private set; }
		/// <summary>
		/// Manages the visuals/effects in the background of the analysis screen.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis","Scene References")]
		public CombatantAnalysisDXBackground AnalysisBackground{ get; private set; }
		/// <summary>
		/// Manages the visuals for the combatant's body graphic specifically.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis","Scene References")]
		public CombatantAnalysisDXBodyGraphic CombatantBodyGraphic { get; private set; }
		/// <summary>
		/// Manages and displays animations for the combatant's name.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis","Scene References")]
		public CombatantAnalysisDXNameBanner CombatantNameBanner { get; private set; }
		/// <summary>
		/// The class that should render the combatant's level.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis","Scene References")]
		public CombatantAnalysisDXLevelLabel CombatantLevelLabel { get; private set; }
		/// <summary>
		/// The class that encapsulates and manages the various attribute bars.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis","Scene References")]
		public CombatantAnalysisDXAttributeBarSet AttributeBarSet { get; private set; }
		/// <summary>
		/// The class that encapsulates and manages the different elemental icons.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis","Scene References")]
		public CombatantAnalysisDXElementIconSet ElementIconSet { get; private set; }
		/// <summary>
		/// The menu list displaying any and all behaviors that should be displayed.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis","Scene References")]
		public CombatantAnalysisDXBehaviorMenuList BehaviorMenuList { get; private set; }
		/// <summary>
		/// An "extra" item to add on the top of the behavior menu list
		/// that shows things like the next move learned on level up.
		/// </summary>
		[OdinSerialize, TabGroup("Analysis","Scene References")]
		public CombatantAnalysisDXAuxiliaryItem BehaviorAuxiliaryItem { get; private set; }
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				ResetController.AddToDontDestroy(this.gameObject);	
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			this.ResetVisualState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of the analysis screen.
		/// </summary>
		private void ResetVisualState() {
			
			// Turn off all the other objects.
			this.AllObjects.SetActive(false);
			
			// Just go through everything and reset manually.
			this.ChatBorders.ResetState();
			this.AnalysisBackground.ResetState();
			this.CombatantBodyGraphic.ResetState();
			this.CombatantNameBanner.ResetState();
			this.CombatantLevelLabel.ResetState();
			this.AttributeBarSet.ResetState();
			this.ElementIconSet.ResetState();
			this.BehaviorMenuList.ResetState();
			this.BehaviorAuxiliaryItem.ResetState();
			
		}
		#endregion

		#region OPENING
		/// <summary>
		/// Opens the combatant analysis screen with the parameters provided.
		/// This is primarily for situations that only focus on one combatant.
		/// </summary>
		/// <param name="analysisType">The kind of screen to open.</param>
		/// <param name="focusCombatant">The combatant to focus on.</param>
		/// <param name="callbackSet">A set of callbacks to run in response to certain events. May be empty.</param>
		/// <param name="presentCompleteCallback">The callback to run when the screen has been presented.</param>
		/// <param name="analysisDismissedCallback">The callback to run when the screen is closed.</param>
		public void Open(AnalysisScreenCategoryType analysisType, Combatant focusCombatant, AnalysisCallbackSet callbackSet = null, Action presentCompleteCallback = null, Action analysisDismissedCallback = null) {
			// Call the version of this function that takes a list.
			this.Open(
				analysisType: analysisType, 
				combatants: new List<Combatant>(){ focusCombatant },
				callbackSet: callbackSet,
				presentCompleteCallback: presentCompleteCallback,
				analysisDismissedCallback: analysisDismissedCallback);
		}
		/// <summary>
		/// Opens the combatant analysis screen with the parameters provided.
		/// Will focus on the first combatant and use the GameController's variables.
		/// </summary>
		/// <param name="analysisType">The kind of screen to open.</param>
		/// <param name="combatants">The combatants that should be listed for analysis.</param>
		/// <param name="callbackSet">A set of callbacks to run in response to certain events. May be empty.</param>
		/// <param name="presentCompleteCallback">The callback to run when the screen has been presented.</param>
		/// <param name="analysisDismissedCallback">The callback to run when the screen is closed.</param>
		public void Open(AnalysisScreenCategoryType analysisType, List<Combatant> combatants, AnalysisCallbackSet callbackSet = null, Action presentCompleteCallback = null, Action analysisDismissedCallback = null) {
			// For this version of the function, use the GameController's variables and start focus at 0.
			this.Open(
				analysisType: analysisType, 
				combatants: combatants, 
				focusCombatantIndex: 0, 
				gameVariables: GameController.Instance.Variables,
				callbackSet: callbackSet, 
				presentCompleteCallback: presentCompleteCallback,
				analysisDismissedCallback: analysisDismissedCallback);
		}
		/// <summary>
		/// Opens the combatant analysis screen with the parameters provided.
		/// </summary>
		/// <param name="analysisType">The kind of screen to open.</param>
		/// <param name="combatants">The combatants that should be listed for analysis.</param>
		/// <param name="focusCombatant">The combatant to focus on when opening the screen.</param>
		/// <param name="gameVariables">The game variables currently in use.</param>
		/// <param name="callbackSet">A set of callbacks to run in response to certain events. May be empty.</param>
		/// <param name="presentCompleteCallback">The callback to run when the screen has been presented.</param>
		/// <param name="analysisDismissedCallback">The callback to run when the screen is closed.</param>
		public void Open(AnalysisScreenCategoryType analysisType, List<Combatant> combatants, Combatant focusCombatant, GameVariables gameVariables, AnalysisCallbackSet callbackSet = null, Action presentCompleteCallback = null, Action analysisDismissedCallback = null) {
			
			// Figure out what the index of the focus combatant is.
			Debug.Assert(combatants.Contains(focusCombatant));
			int focusCombatantIndex = combatants.IndexOf(focusCombatant);
			
			// Use this index on the version of this function that takes an index.
			this.Open(
				analysisType: analysisType, 
				combatants: combatants, 
				focusCombatantIndex: focusCombatantIndex, 
				callbackSet: callbackSet,
				gameVariables: gameVariables, 
				presentCompleteCallback: presentCompleteCallback,
				analysisDismissedCallback: analysisDismissedCallback);
			
		}
		/// <summary>
		/// Opens the combatant analysis screen with the parameters provided.
		/// </summary>
		/// <param name="analysisType">The kind of screen to open.</param>
		/// <param name="combatants">The combatants that should be listed for analysis.</param>
		/// <param name="focusCombatantIndex">The index of the combatant to focus on when opening the screen.</param>
		/// <param name="gameVariables">The game variables currently in use.</param>
		/// <param name="callbackSet">A set of callbacks to run in response to certain events. May be empty.</param>
		/// <param name="presentCompleteCallback">The callback to run when the screen has been presented.</param>
		/// <param name="analysisDismissedCallback">The callback to run when the screen is closed.</param>
		public void Open(AnalysisScreenCategoryType analysisType, List<Combatant> combatants, int focusCombatantIndex, GameVariables gameVariables, AnalysisCallbackSet callbackSet = null, Action presentCompleteCallback = null, Action analysisDismissedCallback = null) {
			
			// Create some new analysis params.
			CombatantAnalysisParams analysisParams = new CombatantAnalysisParams() {
				AnalysisType = analysisType,
				Combatants = combatants,
				Variables = gameVariables,
				CurrentCombatantIndex = focusCombatantIndex
			};
	
			// Save references to the params and the callback.
			this.CurrentAnalysisParams = analysisParams;
			this.CurrentDismissedCallback = analysisDismissedCallback;
			this.CurrentCallbackSet = callbackSet;
			
			// Present the screen.
			this.Present(
				analysisParams: analysisParams, 
				presentCompleteCallback: presentCompleteCallback);
			
		}
		#endregion

		#region CLOSING
		/// <summary>
		/// Close out the analysis screen and run the finish callback that was saved.
		/// </summary>
		public void Close() {
			// Call Dismiss.
			this.Dismiss(analysisParams: this.CurrentAnalysisParams);
		}
		#endregion

		#region PRESENTATION - GENERAL
		/// <summary>
		/// Rebuilds the analysis screen with the provided parameters.
		/// </summary>
		/// <param name="analysisParams">The parameters to use in building the screen.</param>
		private void Rebuild(CombatantAnalysisParams analysisParams) {
			
			this.CombatantBodyGraphic.Present(analysisParams);
			this.CombatantNameBanner.Rebuild(analysisParams);
			this.CombatantLevelLabel.Rebuild(analysisParams);
			this.AttributeBarSet.Rebuild(analysisParams);
			this.ElementIconSet.Rebuild(analysisParams);
			this.BehaviorMenuList.Rebuild(analysisParams);
			this.BehaviorAuxiliaryItem.Rebuild(analysisParams); // Note this will turn off the item if no auxiliary item is available.
			
			// If requested, also null out the currently selected GameObject.
			EventSystem.current.SetSelectedGameObject(null);
			
			GameController.Instance.RunEndOfFrame(action: () => {
				this.TriggerEvent(eventName: "Rebuild Complete");
			});
		}
		#endregion
		
		#region PRESENTATION - ANIMATIONS : WHOLE SCREEN
		/// <summary>
		/// Presents the combatant analysis screen with the provided parameters.
		/// </summary>
		/// <param name="analysisParams">The parameters to use in building the analysis screen.</param>
		/// <param name="presentCompleteCallback">The callback to run when the screen has been presented.</param>
		private void Present(CombatantAnalysisParams analysisParams, Action presentCompleteCallback) {
			
			Debug.Log("Presenting the analysis screen. This has no animations at the moment.");
			
			// Reset the state. Just in case.
			this.ResetVisualState();
			
			// Turn all the objects on.
			this.AllObjects.SetActive(true);
			
			// Present the chat borders.
			this.ChatBorders.PresentBorders();
		
			// I guess just... present everything else.
			this.AnalysisBackground.Present(analysisParams);
			this.CombatantBodyGraphic.Present(analysisParams);
			this.CombatantNameBanner.Present(analysisParams);
			this.CombatantLevelLabel.Present(analysisParams);
			this.AttributeBarSet.Present(analysisParams);
			this.ElementIconSet.Present(analysisParams);
			this.BehaviorMenuList.Present(analysisParams);
			this.BehaviorAuxiliaryItem.Present(analysisParams); // Note this will turn off the item if no auxiliary item is available.
			
			// this.TriggerEvent(eventName: "Present Complete");
			
			// Invoke the present complete callback to continue execution.
			presentCompleteCallback?.Invoke();
			
		}
		/// <summary>
		/// Dismisses the analysis screen and passes control to whoever needs it next.
		/// </summary>
		private void Dismiss(CombatantAnalysisParams analysisParams) {
			// Start up a routine for this one.
			GameController.Instance.StartCoroutine(this.DismissRoutine(analysisParams: analysisParams));
		}
		/// <summary>
		/// The routine that handles dismissing of the analysis screen.
		/// </summary>
		/// <returns></returns>
		private IEnumerator DismissRoutine(CombatantAnalysisParams analysisParams) {
			
			this.ChatBorders.DismissBorders();
			this.AnalysisBackground.Dismiss(analysisParams: analysisParams);
			this.CombatantBodyGraphic.Dismiss(analysisParams: analysisParams);
			this.CombatantNameBanner.Dismiss(analysisParams: analysisParams);
			this.CombatantLevelLabel.Dismiss(analysisParams: analysisParams);
			this.AttributeBarSet.Dismiss(analysisParams: analysisParams);
			this.ElementIconSet.Dismiss(analysisParams: analysisParams);
			this.BehaviorMenuList.Dismiss(analysisParams: analysisParams);
			this.BehaviorAuxiliaryItem.Dismiss(analysisParams);
			
			yield return new WaitForSeconds(0.5f);
			this.AllObjects.SetActive(false);
			this.CurrentAnalysisParams = null;
			this.CurrentCallbackSet = null;
			this.CurrentDismissedCallback?.Invoke();
			
			// Trigger the Close Complete event so that the state machine knows to continue.
			this.TriggerEvent(eventName: "Close Complete");
			
		}
		#endregion
		
		#region STATE MANIPULATION - GENERAL
		/// <summary>
		/// Triggers an event on the analysis screen.
		/// </summary>
		/// <param name="eventName">The name of the event to trigger.</param>
		public void TriggerEvent(string eventName) {
			string str = "Triggering event with name " + eventName + " on the analysis controller.";
			Debug.Log(str);
			this.fsm.SendEvent(eventName: eventName);
		}
		#endregion
		
		#region STATE MANIPULATION - COMBATANT
		/// <summary>
		/// Shifts the combatant currently being focused on based on horizontal input and rebuilds.
		/// </summary>
		/// <param name="moveDir">Whether input was made left/right.</param>
		public void ShiftFocusCombatant(HorizontalMoveDirType moveDir) {
			// Determine the new index by adding the movement direction and wrapping around if needed.
			int newIndex = ((int) moveDir + this.CurrentAnalysisParams.CurrentCombatantIndex)
			               % this.CurrentAnalysisParams.Combatants.Count;
			// If the index equals -1, that means it shifted too far left. Wrap back around.
			if (newIndex == -1 ) {
				newIndex = this.CurrentAnalysisParams.Combatants.Count - 1;
			}
			// Pass it to the function that uses this index specifically.
			this.SetFocusCombatant(combatantIndex: newIndex);		
		}
		/// <summary>
		/// Sets the combatant being focused on to the one passed in.
		/// </summary>
		/// <param name="combatant">The combatant to focus on.</param>
		public void SetFocusCombatant(Combatant combatant) {
			
			// Assert this combatant is ACTUALLY in the parameters.
			Debug.Assert(this.CurrentAnalysisParams.Combatants.Contains(combatant));
			
			// Figure out what the index to focus on next should be.
			int newIndex = this.CurrentAnalysisParams.Combatants.IndexOf(combatant);
			
			// Use this in the new method.
			this.SetFocusCombatant(combatantIndex: newIndex);

		}
		/// <summary>
		/// Directly sets the combatant currently being focused on and rebuilds.
		/// </summary>
		/// <param name="combatantIndex">The index of the combatant to focus on.</param>
		private void SetFocusCombatant(int combatantIndex) {
			// Set it on the params.
			this.CurrentAnalysisParams.CurrentCombatantIndex = combatantIndex;
			// Rebuild.
			this.Rebuild(analysisParams: this.CurrentAnalysisParams);
		}
		#endregion
		
		#region PROTOTYPING
		/// <summary>
		/// Invoked in the inspector to test building the screen.
		/// </summary>
		[Button, HideInEditorMode, TabGroup("Analysis","Prototyping")]
		private void PrototypeOpenEnemies() {
			
			// Copy the analysis type. This is just for the sake of consistency.
			AnalysisScreenCategoryType analysisType = this.prototypeAnalysisType;
			
			// Create a new GameVariables.
			GameVariables gameVariables = new GameVariables(template: this.prototypeVariablesTemplate);
			
			// Also get the enemies to make.
			List<Combatant> combatants = this.prototypeBattleTemplate.EnemyTemplates
				.Select(et => new Enemy(template: et, gameVariables: gameVariables))
				.Cast<Combatant>()
				.ToList();
			
			// Use these to create an object containing all relevant parameters.
			CombatantAnalysisParams prototypeParams = new CombatantAnalysisParams() {
				AnalysisType = analysisType,
				Variables = gameVariables,
				Combatants = combatants
			};
			
			this.Open(
				analysisType: analysisType, 
				combatants: combatants, 
				focusCombatantIndex: 0, 
				gameVariables: gameVariables,
				analysisDismissedCallback: () => {
					Debug.Log("Finish callback executed.");
				});
			
		}
		/// <summary>
		/// Invoked in the inspector to test building the screen.
		/// </summary>
		[Button, HideInEditorMode, TabGroup("Analysis","Prototyping")]
		private void PrototypeOpenPersonas() {
			
			// Copy the analysis type. This is just for the sake of consistency.
			AnalysisScreenCategoryType analysisType = this.prototypeAnalysisType;
			
			// Create a new GameVariables.
			GameVariables gameVariables = new GameVariables(template: this.prototypeVariablesTemplate);
			
			// Also get the personas to make.
			List<Combatant> combatants = gameVariables.Players
				.Select(p => p.ActivePersona)
				.Cast<Combatant>()
				.ToList();
			
			// Use these to create an object containing all relevant parameters.
			CombatantAnalysisParams prototypeParams = new CombatantAnalysisParams() {
				AnalysisType = analysisType,
				Variables = gameVariables,
				Combatants = combatants
			};
			
			
			this.Open(
				analysisType: analysisType, 
				combatants: combatants, 
				focusCombatantIndex: 0, 
				gameVariables: gameVariables,
				analysisDismissedCallback: () => {
					Debug.Log("Finish callback executed.");
					
				});
					
		}
		/// <summary>
		/// Invoked in the inspector to test the adding of a behavior to the combatant.
		/// </summary>
		/// <param name="behaviorIndex">The index of the BattleBehavior inside the prototype list.</param>
		[Button(parameterBtnStyle: ButtonStyle.Box), HideInEditorMode, TabGroup("Analysis", "Prototyping")]
		private void PrototypeAddBehavior(int behaviorIndex) {

			// Get the behavior and combatant to use.
			BattleBehavior prototypeBehavior = this.prototypeBattleBehaviors[behaviorIndex];
			Combatant combatant = this.CurrentAnalysisParams.CurrentCombatant;
			
			// Add the behavior to the combatant, then to the list.
			combatant.AddBehavior(behavior: prototypeBehavior);
			this.BehaviorMenuList.Append(menuable: prototypeBehavior, focusOnAdd: true);
			
			// Get the list item associated with the behavior, now that its being shown.
			var listItem = this.BehaviorMenuList.GetFocusedMenuItem(battleBehavior: prototypeBehavior);
			
			// If the current selected object is NOT the list item, select it.
			if (EventSystem.current.currentSelectedGameObject != listItem.gameObject) {
				EventSystem.current.SetSelectedGameObject(listItem.gameObject);
			} else {
				// If this list item was selected when rebuilt, it will need to be manually highlighted again.
				listItem.Highlight(prototypeBehavior);
			}
			
		}
		#endregion
		
	}
	
}