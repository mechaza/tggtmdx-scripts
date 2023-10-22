using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Grawly.Battle;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Grawly.Battle.BattleMenu;
using Grawly.Gauntlet;
using Grawly.Dungeon.UI;
using Grawly.DungeonCrawler;
using Grawly.Menus.BadgeBoardScreen;
using Grawly.Menus.SocialLink;
using Grawly.Toggles.Proto;
using Grawly.UI.MenuLists;
using UnityStandardAssets.ImageEffects;

namespace Grawly.UI {

	/// <summary>
	/// The NEW pause menu controller to make up for me being a fucking idiot.
	/// </summary>
	public class PauseMenuController : MonoBehaviour {

		public static PauseMenuController instance;

		#region FIELDS - STATE : CALLBACKS
		/// <summary>
		/// The callback to run when the menu is exited.
		/// </summary>
		private static System.Action CurrentExitCallback = null;
		#endregion
		
		#region FIELDS - STATE : GAME VARIABLES
		/// <summary>
		/// A reference to the GameVariables used for this menu. Will use its own if DebugMode is on.
		/// </summary>
		public GameVariables Variables {
			get {
				return GameController.Instance.Variables;
			}
		}
		#endregion

		#region FIELDS - STATE : GAME OBJECTS
		/// <summary>
		/// PLAY MAKER HAS SOMETHING FUCKING WRONG WITH IT BECAUSE ITS NOT SAVING THESE CORRECTLY SO UH GET FUCKED I HAVE TO DO THIS MYSELF
		/// </summary>
		private GameObject lastSelectedTopLevelGameObject;
		/// <summary>
		/// PLAY MAKER HAS SOMETHING FUCKING WRONG WITH IT BECAUSE ITS NOT SAVING THESE CORRECTLY SO UH GET FUCKED I HAVE TO DO THIS MYSELF
		/// </summary>
		public GameObject CurrentSelectedTopLevelGameObject {
			get {
				return this.lastSelectedTopLevelGameObject;
			} set {
				Debug.Log("OVERRIDING THE LAST SELECTED TOP LEVEL GAME OBJECT WITH " + value.name);
				this.lastSelectedTopLevelGameObject = value;
			}
		}
		#endregion

		#region FIELDS - STATE : INDICIES
		/// <summary>
		/// The index of the menu list item that was last selected.
		/// </summary>
		private int currentItemIndex;
		/// <summary>
		/// The index of the player status that is acting as the source of a skill/bersona transition.
		/// </summary>
		private int currentPlayerSourceIndex;
		/// <summary>
		/// The index of the player status that is acting as the target of a skill/bersona/item.
		/// If it's less than zero, it's all of them. 
		/// I Don't care anymore.
		/// </summary>
		private int currentPlayerTargetIndex;
		/// <summary>
		/// An uh. Additional state that PauseMenuPlayerStatus sets and eventually collapses to either currentPlayerSourceIndex or currentPlayerTargetIndex.
		/// </summary>
		private int currentPlayerSelectionIndex;
		#endregion

		#region FIELDS - STATE : COMPUTATION AS A RESULT OF THE INDICIES ABOVE
		/// <summary>
		/// The source player to be used in computation by the PauseMenuEvaluator.
		/// </summary>
		public Player SourcePlayer {
			get {
				return this.pauseMenuPlayerStatuses[this.currentPlayerSourceIndex].player;
			}
		}
		/// <summary>
		/// The target players to be used in computation by the PauseMenuEvaluator.
		/// </summary>
		public List<Player> TargetPlayers {
			get {
				// Oh boy this is a doozy. So uh.
				// If the index is 4, it means I want to have all the players.
				// There's no real connection between the number 4 beyond its higher than 3; the highest possible index for a player status.
				if (this.currentPlayerTargetIndex == 4) {
					return this.Variables.Players;
				} else {
					// If the target player index is not four, return a list with only one player.
					return new List<Player>() { this.pauseMenuPlayerStatuses[this.currentPlayerTargetIndex].player };
				}
				
			}
		}
		/// <summary>
		/// The currently selected skill. 
		/// Note that it's entirely possible for the IMenuable being called forth is in fact an InventoryItem and not a BattleBehavior.
		/// I'm going to try and only compute SelectedSkill in contexts where that is not the case, though.
		/// </summary>
		public BattleBehavior SelectedSkill {
			get {
				return ((BattleBehavior)((InventoryMenuListItem)this.inventoryMenuList.MenuListItems[this.currentItemIndex]).item);
			}
		}
		/// <summary>
		/// The currently selected skill. 
		/// Note that it's entirely possible for the IMenuable being called forth is in fact an InventoryItem and not a BattleBehavior.
		/// I'm going to try and only compute SelectedSkill in contexts where that is not the case, though.
		/// </summary>
		public InventoryItem SelectedInventoryItem {
			get {
				return ((InventoryItem)((InventoryMenuListItem)this.inventoryMenuList.MenuListItems[this.currentItemIndex]).item);
			}
		}
		/// <summary>
		/// The currently selected Bersona. 
		/// Note that it's entirely possible for the IMenuable being called forth is in fact an InventoryItem and not a Bersona.
		/// I'm going to try and only compute PersonaToSwitchTo in contexts where that is not the case, though.
		/// </summary>
		public Persona PersonaToSwitchTo {
			get {
				return ((Persona)((InventoryMenuListItem)this.inventoryMenuList.MenuListItems[this.currentItemIndex]).item);
			}
		}
		#endregion

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The time it should take for the flasher to do its thing.
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Toggles")]
		private float blurFadeTime = 0.5f;
		/// <summary>
		/// The ease type to use when tweening the top/bottom bars
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Toggles")]
		private Ease barEaseType = Ease.Linear;
		/// <summary>
		/// The time it should take for the top bars to swooce right in.
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Toggles")]
		private float barMoveTime = 0.5f;
		/// <summary>
		/// The ease type to use when tweening the menu lists.
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Toggles")]
		private Ease menuListEaseType = Ease.Linear;
		/// <summary>
		/// The time it should take for the menu list to tween in/out.
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Toggles")]
		private float menuListMoveTime = 0.5f;
		#endregion

		#region FIELDS - TOGGLES : INITIAL POSITIONS
		/// <summary>
		/// The initial position of the inventory menu list object. Good for prototyping.
		/// </summary>
		private Vector2 inventoryMenuListInitialPos;
		/// <summary>
		/// The initial position of the game object with the player statuses. Good for prototyping.
		/// </summary>
		private Vector2 playerStatusesRectTransformInitialPos;
		/// <summary>
		/// The initial position of the rect transform containing the buttons used to navigate the menu.
		/// </summary>
		private Vector2 topLevelSelectionRectTransformInitialPos;
		/// <summary>
		/// The initial position of the rect transform that has the pause label.
		/// </summary>
		private Vector2 pauseLabelRectTransformInitialPos;
		#endregion

		#region FIELDS - TOGGLES : HIDING POSITIONS
		/// <summary>
		/// The position the top level selection should be when its hiding.
		/// Different from initial position, which is where it should end up.
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Hiding Positions")]
		private Vector2 topLevelSelectionRectTransformHidingPos;
		/// <summary>
		/// The position the inventory menu list should be when its hiding.
		/// Different from initial position, which is where it should end up.
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Hiding Positions")]
		private Vector2 inventoryMenuListHidingPos;
		/// <summary>
		/// The position the player statuses rect transform should be when its hiding.
		/// Different from initial position, which is where it should end up.
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Hiding Positions")]
		private Vector2 playerStatusesRectTransformHidingPos;
		/// <summary>
		/// The position the pause label rect transform should be in when its hiding.
		/// Different from initial position, which is where it should end up.
		/// </summary>
		[SerializeField, TabGroup("Toggles", "Hiding Positions")]
		private Vector2 pauseLabelRectTransformHidingPos;
		#endregion

		#region FIELDS - SCENE REFERENCES : GENERAL
		/// <summary>
		/// The FSM assocaited with the PauseMenuController. May be needed to send events or set variables.
		/// </summary>
		public PlayMakerFSM FSM { get; private set; }
		/// <summary>
		/// The bar that shows up at the top of the screen.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "General")]
		private Image topBarImage;
		/// <summary>
		/// The bar that shows up at the bottom of the screen.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "General")]
		private Image bottomBarImage;
		/// <summary>
		/// The rect transform that has the pause label. Only for decoration.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "General")]
		private RectTransform pauseLabelRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : TOP LEVEL
		/// <summary>
		/// The rect transform associated with the top level buttons.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Top Level")]
		private RectTransform topLevelSelectionRectTransform;
		/// <summary>
		/// The FunTextMeshes that I have up at the top level of the menu. These are the ones that have the fun highlights.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Top Level")]
		private List<FunTextMesh> topLevelSelectionLabels = new List<FunTextMesh>();
		#endregion

		#region FIELDS - SCENE REFERENCES : MENU LIST
		/// <summary>
		/// A reference to the InventoryMenuList, which is used to display... the inventory.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Menu List")]
		private InventoryMenuList inventoryMenuList;
		/*/// <summary>
		/// A reference to the InventoryMenuList, which is used to display... the inventory.
		/// Publically accessible in the event I want to refresh shit.
		/// </summary>
		public InventoryMenuList InventoryMenuList {
			get {
				return this.inventoryMenuList;
			}
		}*/
		/// <summary>
		/// The rect transform that has all of the other player statuses in them.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Menu List")]
		private RectTransform playerStatusesRectTransform;
		/// <summary>
		/// A list of player statuses used to represent the players eooohh.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Menu List")]
		private List<PauseMenuPlayerStatus> pauseMenuPlayerStatuses = new List<PauseMenuPlayerStatus>();
		/// <summary>
		/// The game object that serves as a punching bag for the EventSystem for when I want to have all players selected but also fake it.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Menu List")]
		private GameObject allPlayersTargetGameObject;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance != null) {
				return;
			}

			instance = this;
			ResetController.AddToDontDestroy(this.gameObject);
			// DontDestroyOnLoad(this.gameObject);

			// Remember the initial positions of the things that need those remembered.
			this.inventoryMenuListInitialPos = this.inventoryMenuList.GetComponent<RectTransform>().anchoredPosition;
			this.playerStatusesRectTransformInitialPos = this.playerStatusesRectTransform.anchoredPosition;
			this.topLevelSelectionRectTransformInitialPos = this.topLevelSelectionRectTransform.anchoredPosition;
			this.pauseLabelRectTransformInitialPos = this.pauseLabelRectTransform.anchoredPosition;

			// Reset their positions to where they should be hiding.
			this.ResetPositions();

			this.FSM = this.GetComponent<PlayMakerFSM>();
		}
		/*private void Start() {
			if (debugMode == true) {
				Debug.Log("PAUSE MENU: Debug mode is on. Running Debug Initialize.");
				this.debugGameVariables = new GameVariables(template: this.debugGameVariablesTemplate);
			}
		}*/
		private void OnEnable() {
			// Send the START event to the FSM on enable.
			// this.FSM.SendEvent("START");
		}
		#endregion

		#region CALLING THE PAUSE MENU
		/// <summary>
		/// A way to open up the pause menu.
		/// </summary>
		public static void Open() {
			// Call the version of this function that takes a callback and just make it emptuy.
			Open(onExitCallback: delegate { });
		}
		/// <summary>
		/// A way to open up the pause menu.
		/// </summary>
		public static void Open(System.Action onExitCallback) {
			
			Debug.Log("Opening the pause menu. Also nullifying the currently selected GameObject.");

			// Totally reset the current selected gameobject. I don't think this should cause an issue but.. if it does, heres where its happening.
			EventSystem.current.SetSelectedGameObject(null);
			
			// Dismiss the prompt. This should work regardless of it was shown or not.
			// TODO: Make it re-display the prompt upon dismissing, if one existed.
			CrawlerActionPrompt.Instance?.Dismiss();
			
			// Save the exit callback.
			CurrentExitCallback = onExitCallback;
			
			// If the gauntlet title is in the scene, hide it. BOY I am absolutely shoehorning shit in but. okay.
			GauntletMenuController.instance?.NodeTitle.SetVisualsActive(false);

			FullMapDX.instance?.DisplayMap();
			MiniMapDX.instance?.DismissMap();
			// For the three checks below, I used to use the null conditional but for some fucking reason
			// it doesn't work when using gohom.
			/*if (CrawlerFullMap.Instance != null) {
				CrawlerFullMap.Instance.DisplayMap();
			}
			if (CrawlerMiniMap.Instance != null) {
				CrawlerMiniMap.Instance.DismissMap();
			}
			if (EnemyRadar.Instance != null) {
				EnemyRadar.Instance.Dismiss();
			}*/
			CrawlerFullMap.Instance?.DisplayMap();
			CrawlerMiniMap.Instance?.DismissMap();
			EnemyRadar.Instance?.Dismiss();
			NotificationController.Instance?.Dismiss();

			if (instance == null) {
				Debug.Log("Instance is null. Loading the Pause Menu scene.");
				// SceneManager.LoadScene(sceneName: "Pause Menu", mode: LoadSceneMode.Additive);
				SceneManager.LoadSceneAsync(sceneName: "Pause Menu", mode: LoadSceneMode.Additive).completed += PauseMenuController_completed;
			} else {
				instance.FSM.SendEvent("OPEN PAUSE MENU");
				AudioController.instance?.PlaySFX(SFXType.Pause, scale: 0.5f);
			}
		}
		/// <summary>
		/// Gets called when the operation is complete.
		/// </summary>
		/// <param name="obj"></param>
		private static void PauseMenuController_completed(AsyncOperation obj) {
			instance.FSM.SendEvent("OPEN PAUSE MENU");
			AudioController.instance?.PlaySFX(SFXType.Pause, scale: 0.5f);
			// obj.completed -= PauseMenuController_completed;
		}
		/// <summary>
		/// Doesn't need to be static because I assume the Instance is valid.
		/// This just closes the controller.
		/// </summary>
		public void Close() {

			FullMapDX.instance?.DismissMap();
			MiniMapDX.instance?.DisplayMap();
			CrawlerFullMap.Instance?.DismissMap();
			CrawlerMiniMap.Instance?.DisplayMap();
			EnemyRadar.Instance?.Display();
			NotificationController.Instance?.Present();
			
			// If the gauntlet node title is on the scene, also make sure to make it visible again.
			GauntletMenuController.instance?.NodeTitle.SetVisualsActive(true);

			GauntletController.instance?.SetFSMState(GauntletStateType.Free);
			
			// Run the exit callback.
			CurrentExitCallback.Invoke();
			// Null it out.
			CurrentExitCallback = null;
		}
		#endregion

		#region CALLING THE SETTINGS MENU
		/// <summary>
		/// The settings menu is kind of separated from the pause menu.
		/// I'm just gonna slap a call to it here.
		/// </summary>
		public void OpenSettings() {
			SettingsMenuControllerDX.GlobalOpen();
			
			// Blur the camera.
			Camera.main.GetComponent<BlurOptimized>().enabled = true;
			DOTween.To(
				getter: () => Camera.main.GetComponent<BlurOptimized>().blurSize,
				setter: x => Camera.main.GetComponent<BlurOptimized>().blurSize = x,
				endValue: 3f,
				duration: this.blurFadeTime);
		}
		/// <summary>
		/// The settings menu is kind of separated from the pause menu.
		/// I'm just gonna slap a call to it here.
		/// </summary>
		public void CloseSettings() {
			// Deblur the camera.
			DOTween.To(
				getter: () => Camera.main.GetComponent<BlurOptimized>().blurSize,
				setter: x => Camera.main.GetComponent<BlurOptimized>().blurSize = x,
				endValue: 0f,
				duration: this.blurFadeTime)
				.onComplete = new TweenCallback(delegate {
					Camera.main.GetComponent<BlurOptimized>().enabled = false;
				});
			
		}
		#endregion

		#region INITIALIZATION
		/// <summary>
		/// ONLY call this function in debug contexts or if I know what I'm doing!
		/// </summary>
		public void AbortPauseMenu() {
			Debug.LogWarning("PAUSE MENU IS ABORTING. ONLY CALL THIS FUNCTION IF YOU KNOW WHAT YOURE DOING.");
			this.ResetIndicies();
			this.ResetPositions();
			this.pauseLabelRectTransform.anchoredPosition = this.pauseLabelRectTransformHidingPos;
			this.FadeOutBorders();
		
			EventSystem.current.SetSelectedGameObject(null);
		}
		/// <summary>
		/// Resets the positions of all the relevant rect transforms to their hiding positions. Usually needed when re-enabling the canvas.
		/// </summary>
		private void ResetPositions() {
			this.inventoryMenuList.GetComponent<RectTransform>().anchoredPosition = this.inventoryMenuListHidingPos;
			this.playerStatusesRectTransform.anchoredPosition = this.playerStatusesRectTransformHidingPos;
			this.topLevelSelectionRectTransform.anchoredPosition = this.topLevelSelectionRectTransformHidingPos;
			this.pauseLabelRectTransform.anchoredPosition = this.pauseLabelRectTransformInitialPos;
		}
		/// <summary>
		/// Resets the indicies of the current/last selected items in the menu.
		/// </summary>
		public void ResetIndicies() {
			Debug.Log("PAUSE MENU: RESETTING INDICIES");
			this.currentItemIndex = -1;
			this.currentPlayerSelectionIndex = -1;
			this.currentPlayerSourceIndex = -1;
			this.currentPlayerTargetIndex = -1;
		}
		#endregion

		#region BUILDING - GENERAL
		/// <summary>
		/// Prepares the inventory menu list to be used with the specified menuables. Starts at the specified index.
		/// </summary>
		/// <param name="menuables"></param>
		/// <param name="startIndex"></param>
		private void PrepareMenuList(List<IMenuable> menuables, int startIndex) {
			// Build out the buttons and shit.
			Debug.Log("PAUSE MENU: PREPARING THE INVENTORY MENU LIST");
			this.inventoryMenuList.PrepareMenuList(allMenuables: menuables, startIndex: startIndex);
		}
		/// <summary>
		/// A version of the RebuildInventoryMenuList that gets called from PlayMaker.
		/// Implicitly assumes the Variables items are being used, and that it should use the version of the Rebuild function that doesn't take any variables either.
		/// </summary>
		public void RebuildMenuList() {
			Debug.LogWarning("PAUSE MENU: Calling the version of RebuildInventoryMenuList that takes no parameters. This is a different function from the one that does. Be careful! If contents of the menu list change, it could be bad.");
			this.inventoryMenuList.RebuildMenuList();
		}
		#endregion

		#region BUILDING - ITEM MENU LIST
		/// <summary>
		/// A version of the RebuildInventoryMenuList that gets called from PlayMaker.
		/// Implicitly assumes the Variables items are being used.
		/// </summary>
		/// <param name="startIndex"></param>
		public void RebuildInventoryMenuList(int startIndex) {
			this.PrepareMenuList(menuables: this.Variables.MenuableItems, startIndex: startIndex);
		}
		#endregion

		#region BUILDING - SKILL MENU LIST
		/// <summary>
		/// Builds up the skill list.
		/// </summary>
		/// <param name="startIndex"></param>
		public void RebuildSkillMenuList(int startIndex) {
			// Check the source player that has currently been saved and query their behaviors that have pause functions.
			// Yes this is terrible.
			// this.RebuildMenuList(menuables: this.SourcePlayer.Behaviors[BehaviorType.Special].Where(b => b.PauseFunctions.Count > 0).Cast<IMenuable>().ToList(), startIndex: startIndex);


			// Instead of using the source player to build the list, try just using the one that is currently selected. It may not be the source player ultimately. 
			// this.PrepareMenuList(menuables: this.pauseMenuPlayerStatuses[this.currentPlayerSelectionIndex].player.Behaviors[BehaviorType.Special].Where(b => b.PauseFunctions.Count > 0).Cast<IMenuable>().ToList(), startIndex: startIndex);
			this.PrepareMenuList(menuables: this.pauseMenuPlayerStatuses[this.currentPlayerSelectionIndex].player.AllBehaviors[BehaviorType.Special].Cast<IMenuable>().ToList(), startIndex: startIndex);
		}
		#endregion

		#region BUILDING - BERSONA MENU LIST
		/// <summary>
		/// Rebuilds the bersona menu.
		/// </summary>
		/// <param name="startIndex"></param>
		public void RebuildBersonaMenuList(int startIndex) {
			// Only display the bersonas that are not in use.
			this.PrepareMenuList(menuables: this.Variables.Personas.Where(p => p.IsInUse(variables: this.Variables) == false).Cast<IMenuable>().ToList(), startIndex: startIndex);
		}
		#endregion

		#region BUILDING - PLAYER STATUSES
		/// <summary>
		/// Rebuilds the player statuses. Everyone is dehighlighted.
		/// </summary>
		public void RebuildPauseMenuPlayerStatuses() {
			// Just pass an empty list. No players means nobody gets highlighted.
			this.RebuildPauseMenuPlayerStatuses(toKeepHighlighted: new List<Player>() { });
		}
		/// <summary>
		/// Rebuilds the player statuses while also making sure the specified players are kept highlighted.
		/// </summary>
		/// <param name="toKeepHighlighted"></param>
		public void RebuildPauseMenuPlayerStatuses(List<Player> toKeepHighlighted) {
			Debug.Log("PAUSE MENU: Rebuilding pause menu player statuses.");
			// Build the status for those who are actually present.
			// Note that if a player is referenced in the "toKeepHighlighted" list, they are highlighted.
			// If not, they're just kept dehighlighted.
			for (int i = 0; i < this.Variables.Players.Count; i++) {
				this.pauseMenuPlayerStatuses[i].Build(
					player: this.Variables.Players[i], 
					highlighted: toKeepHighlighted.Contains(item: this.Variables.Players[i]));
			}
			// If anyone is not present, just deactivate their game object.
			for (int i = this.Variables.Players.Count; i < this.pauseMenuPlayerStatuses.Count; i++) {
				this.pauseMenuPlayerStatuses[i].gameObject.SetActive(false);
			}
		}
		#endregion

		#region BUILDING - ITEM BOX
		/// <summary>
		/// Builds the info box with the specified string.
		/// </summary>
		/// <param name="description">The string to have in the info box.</param>
		public void BuildInfoBox(string description) {
			this.inventoryMenuList.InfoBoxLabel.Text = description;
		}
		#endregion

		#region STATE - INDICIES
		/// <summary>
		/// Sets the index of the currently selected menu list item to the specified value.
		/// Handy for reselecting it later.
		/// Gets called from InventoryMenuListItem.
		/// </summary>
		/// <param name="index"></param>
		public void SetCurrentItemIndex(int index) {
			// Debug.Log("PAUSE MENU: Setting the current item index to " + index);
			this.currentItemIndex = index;
		}
		/// <summary>
		/// Sets the index of the currently selected source player to the specified value.
		/// Handy for reselecting it later.
		/// Gets called from this PauseMenuPlayerStatus, because I can't really do both from the status without excessive state.
		/// </summary>
		/// <param name="index"></param>
		public void SetCurrentPlayerSelectionIndex(int index) {
			// Debug.Log("PAUSE MENU: Setting the current player selection index to " + index);
			this.currentPlayerSelectionIndex = index;
		}
		/// <summary>
		/// Gets called when needing to figure out who to pick by default when picking a player.
		/// This may be a bit extra but its the only thing I can think of right now.
		/// </summary>
		/// <param name="stateBeingCalledFrom"></param>
		public void InferDefaultTargetPlayerIndex(string stateBeingCalledFrom) {
			// An index of four implies all allies.









			// FOR THE RECORD I MADE THE SETSUBMITTABLES THING
			// AND ALSO SORT OF INTERCEPT TARGET PALYERS IN A DDIFFERENT WAY IN THE EVALUATOR.







			int index = -1;
			switch (stateBeingCalledFrom) {
				case "Skill":
					// If the currently selected item is a skill (playmaker will specify this) reference the item as a skill.
					// Zero is being picked for the other option because that is the index of the first player status. Obviously. But good to jog my memory.
					this.pauseMenuPlayerStatuses.Where(ps => ps.gameObject.activeInHierarchy == true).ToList().ForEach(ps => ps.SetSubmittableForBehavior(behavior: this.SelectedSkill));
					index = this.SelectedSkill.targetType == TargetType.AllAliveAllies ? 4 : 0;
					break;
				case "Item":
					// If the currently selected item is an inventory item, reference it as an item.
					// Zero is being picked for the other option because that is the index of the first player status. Obviously. But good to jog my memory.
					this.pauseMenuPlayerStatuses.Where(ps => ps.gameObject.activeInHierarchy == true).ToList().ForEach(ps => ps.SetSubmittableForBehavior(behavior: this.SelectedInventoryItem.behavior));
					index = this.SelectedInventoryItem.behavior.targetType == TargetType.AllAliveAllies ? 4 : 0;
					break;
				case "Bersona":
					this.pauseMenuPlayerStatuses.ForEach(ps => ps.SetSubmittableForPersona());
					break;
				default:
					throw new System.Exception("Could not determine state type! Was the string passed in properly?");
			}

			// Set the selection. 
			this.currentPlayerSelectionIndex = index;
			this.RememberTargetPlayerIndex();
		}
		#endregion

		#region STATE - PLAYERS
		/// <summary>
		/// Sets the inventory item to be the inventory item that was last selected.
		/// Gets called from PlayMaker.
		/// </summary>
		public void RememberTargetPlayerIndex() {
			// Debug.Log("PAUSE MENU: Remembering target player with index " + this.currentPlayerSelectionIndex);
			this.currentPlayerTargetIndex = this.currentPlayerSelectionIndex;
		}	
		/// <summary>
		/// Sets the inventory item to be the inventory item that was last selected.
		/// Gets called from PlayMaker.
		/// </summary>
		public void RememberSourcePlayerIndex() {
			Debug.Log("PAUSE MENU: Remembering source player with index " + this.currentPlayerSelectionIndex);
			this.currentPlayerSourceIndex = this.currentPlayerSelectionIndex;
		}
		#endregion

		#region EVENT SYSTEM - SELECTIONS AS A RESULT OF THE STATE VARIABLES
		/// <summary>
		/// Tells the EventSystem to select the InventoryMenuListItem at the index of this.currentItemIndex.
		/// Gets called from PlayMaker.
		/// </summary>
		public void SelectCurrentItemGameObject() {
			EventSystem.current.SetSelectedGameObject(this.inventoryMenuList.MenuListItems[this.currentItemIndex].gameObject);
		}
		/// <summary>
		/// Tells the EventSystem to select the source player at the index of this.currentPlayerSourceIndex.
		/// Gets called from PlayMaker.
		/// </summary>
		public void SelectCurrentSourcePlayerGameObject() {
			EventSystem.current.SetSelectedGameObject(this.pauseMenuPlayerStatuses[this.currentPlayerSourceIndex].gameObject);
		}
		/// <summary>
		/// Tells the EventSystem to select the target player at the index of this.currentPlayerTargetIndex.
		/// Gets called from PlayMaker.
		/// </summary>
		public void SelectCurrentTargetPlayerGameObject() {
			// If the index is 4, it means I want to have all the players.
			// There's no real connection between the number 4 beyond its higher than 3; the highest possible index for a player status.
			if (this.currentPlayerTargetIndex == 4) {
				EventSystem.current.SetSelectedGameObject(allPlayersTargetGameObject);
			} else {
				EventSystem.current.SetSelectedGameObject(this.pauseMenuPlayerStatuses[this.currentPlayerTargetIndex].gameObject);
			}
			
		}
		/// <summary>
		/// Gets called as a UnityAction on the invisible fifth button when I want to fake the highlight/dehighlight.
		/// </summary>
		/// <param name="status"></param>
		public void SetHighlightOnAllPlayerStatuses(bool status) {
			// If true, call the Rebuild method and pass it all the relevant players.
			if (status == true) {
				this.RebuildPauseMenuPlayerStatuses(toKeepHighlighted: this.Variables.Players);
			} else {
				// If not, call this version, which effectively dehighlights everyone.
				this.RebuildPauseMenuPlayerStatuses();
			}
		}
		#endregion
		
		#region ANIMATIONS - GENERAL
		/// <summary>
		/// Sets the color of the top/bottom bars to a specific color.
		/// Good for making the different menus look nice.
		/// </summary>
		/// <param name="color">The color to set the bars to.</param>
		public void SetBarColor(Color color) {
			this.topBarImage.CrossFadeColor(targetColor: color, duration: 0f, ignoreTimeScale: true, useAlpha: true);
			this.bottomBarImage.CrossFadeColor(targetColor: color, duration: 0f, ignoreTimeScale: true, useAlpha: true);
		}
		/// <summary>
		/// Fades the top/bottom bars in and also blurs the screen.
		/// </summary>
		[HideInEditorMode, TabGroup("Debug", "Debug"), ShowIf("debugMode")]
		public void FadeInBorders() {
			// Tween the bar positions.
			this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 42f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -60f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);

			// Blur the camera.
			Camera.main.GetComponent<BlurOptimized>().enabled = true;
			DOTween.To(
				getter: () => Camera.main.GetComponent<BlurOptimized>().blurSize,
				setter: x => Camera.main.GetComponent<BlurOptimized>().blurSize = x,
				endValue: 3f,
				duration: this.blurFadeTime);

		}
		/// <summary>
		/// Fades the top/bottom bars out and also de-blurs the screen.
		/// </summary>
		[HideInEditorMode, TabGroup("Debug", "Debug"), ShowIf("debugMode")]
		public void FadeOutBorders() {
			// Tween the bar positions.
			this.topBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: 175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			this.bottomBarImage.GetComponent<RectTransform>().DOAnchorPosY(endValue: -175f, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);

			// Deblur the camera.
			DOTween.To(
				getter: () => Camera.main.GetComponent<BlurOptimized>().blurSize,
				setter: x => Camera.main.GetComponent<BlurOptimized>().blurSize = x,
				endValue: 0f,
				duration: this.blurFadeTime)
				.onComplete = new TweenCallback(delegate {
					Camera.main.GetComponent<BlurOptimized>().enabled = false;
				});
		}
		#endregion

		#region ANIMATIONS - TOP LEVEL
		/// <summary>
		/// Tweens the top level selections in or out. 
		/// Gets called from PlayMaker.
		/// </summary>
		/// <param name="status">Whether they should be tweened in or out.</param>
		public void TweenTopLevelSelections(bool status) {

			// Set the status of the top level selections's selectables to be whatever the tween status is. This is to help prevent errors and Shit.
			this.topLevelSelectionLabels.Select(t => t.GetComponent<Selectable>()).ToList().ForEach(s => s.enabled = status);

			// If tweening in the list, tween it to the initial position.
			if (status == true) {
				this.topLevelSelectionRectTransform.DOAnchorPos(endValue: this.topLevelSelectionRectTransformInitialPos, duration: this.menuListMoveTime, snapping: false).SetEase(ease: this.menuListEaseType);
				// Tween the pause label. Note that I'm using the INITIAL position here, because when bars are present, this shouldn't be.
				this.pauseLabelRectTransform.DOAnchorPos(endValue: this.pauseLabelRectTransformInitialPos, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			} else {
				// Otherwise, put it off screen.
				this.topLevelSelectionRectTransform.DOAnchorPos(endValue: this.topLevelSelectionRectTransformHidingPos, duration: this.menuListMoveTime, snapping: false).SetEase(ease: this.menuListEaseType);
				// Tween the pause label. Note that I'm using the HIDING position here, because when bars are present, this shouldn't be.
				this.pauseLabelRectTransform.DOAnchorPos(endValue: this.pauseLabelRectTransformHidingPos, duration: this.barMoveTime, snapping: true).SetEase(ease: this.barEaseType);
			}
		}
		#endregion

		#region ANIMATIONS - MENU LIST VIEW
		/// <summary>
		/// The animation for tweening the menu list in or out.
		/// </summary>
		public void TweenInventoryMenuList(bool status) {
			// If tweening in the list, tween it to the initial position.
			if (status == true) {
				this.inventoryMenuList.GetComponent<RectTransform>().DOAnchorPos(endValue: this.inventoryMenuListInitialPos, duration: this.menuListMoveTime, snapping: false).SetEase(ease: this.menuListEaseType);
			} else {
				// Otherwise, put it off screen.
				// this.inventoryMenuList.GetComponent<RectTransform>().DOAnchorPos(endValue: new Vector2(x: -1475f, y: -52f), duration: this.menuListMoveTime, snapping: false).SetEase(ease: this.menuListEaseType);
				this.inventoryMenuList.GetComponent<RectTransform>().DOAnchorPos(endValue: this.inventoryMenuListHidingPos, duration: this.menuListMoveTime, snapping: false).SetEase(ease: this.menuListEaseType);
			}
		}
		/// <summary>
		/// The animation for tweening the player statuses rect transform in or out.
		/// </summary>
		/// <param name="status"></param>
		public void TweenPlayerStatusesRectTransform(bool status) {
			// If tweening in the list, tween it to the initial position.
			if (status == true) {
				this.playerStatusesRectTransform.DOAnchorPos(endValue: this.playerStatusesRectTransformInitialPos, duration: this.menuListMoveTime, snapping: false).SetEase(ease: this.menuListEaseType);
			} else {
				// Otherwise, put it off screen.
				// this.playerStatusesRectTransform.DOAnchorPos(endValue: new Vector2(x: -2370f, y: 129f), duration: this.menuListMoveTime, snapping: false).SetEase(ease: this.menuListEaseType);
				this.playerStatusesRectTransform.DOAnchorPos(endValue: this.playerStatusesRectTransformHidingPos, duration: this.menuListMoveTime, snapping: false).SetEase(ease: this.menuListEaseType);
			}
		}
		#endregion

		#region ANIMATIONS - THINGS NOT PART OF THIS SCRIPT
		/// <summary>
		/// Tweens the calendar UI in or out.
		/// </summary>
		/// <param name="status"></param>
		public void TweenCalendarUI(bool status) {
			// Call the Instance of the calendar date UI if it exists.
			CalendarDateUI.instance?.Tween(status: status);
		}
		/// <summary>
		/// Tweens the statuses that are actually part of the dungeon controller.
		/// </summary>
		/// <param name="status"></param>
		public void TweenPlayerBattleStatuses(bool status) {
			// Go through all of the player statuses and tween them. If they exist, obviously.
			// Dungeon.DungeonHUDController.Instance?.PlayerDungeonStatuses.ForEach(ps => ps.Tween(status: status));
			// Dungeon.DungeonHUDController.Instance?.TweenPlayerStatuses(status: status);
			PlayerStatusDXController.instance?.TweenVisible(status: status);
		}
		#endregion

		#region OTHER
		/// <summary>
		/// A shortcut to loading the social link menu.
		/// </summary>
		public void LoadSocialLinkMenu() {
			
			// Tween the different UI elements out.
			this.TweenCalendarUI(status: false);
			this.TweenPlayerBattleStatuses(status: false);
			this.TweenTopLevelSelections(status: false);
			
			// Save the currently selected object.
			GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
			
			// Present the social link controller.
			SocialLinkMenuController.Instance.Present(closeCallback: delegate {
				
				// Tween the different UI elements in again.
				this.TweenCalendarUI(status: true);
				this.TweenPlayerBattleStatuses(status: true);
				this.TweenTopLevelSelections(status: true);
				
				EventSystem.current.SetSelectedGameObject(currentSelected);
			});
		}
		/// <summary>
		/// A shortcut to loading the badge board menu.
		/// </summary>
		public void LoadBadgeBoardMenu() {
			
			// Tween the different UI elements out.
			this.TweenCalendarUI(status: false);
			this.TweenPlayerBattleStatuses(status: false);
			this.TweenTopLevelSelections(status: false);
			
			// Save the currently selected object.
			GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
			
			// Open the badge board controller and also give it a callback to run when complete.
			BadgeBoardController.Instance.Open(
				entryType: BadgeBoardEntryType.AllPlayers,
				onComplete: () => {
					// Tween the different UI elements in again.
					this.TweenCalendarUI(status: true);
					this.TweenPlayerBattleStatuses(status: true);
					this.TweenTopLevelSelections(status: true);
				
					EventSystem.current.SetSelectedGameObject(currentSelected);
				});
			
		}
		#endregion

	}


}