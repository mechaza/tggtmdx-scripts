using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// This is what controls all of the other PlayerStatusDX's and is what helps animate them all at once. E.x., tweening, etc.
	/// </summary>
	public class PlayerStatusDXController : MonoBehaviour {

		public static PlayerStatusDXController instance;

		#region FIELDS - STATE
		/// <summary>
		/// A list of players who may be targets coming up.
		/// Mostly needed for when I'm making use of the hidden selectable.
		/// </summary>
		private List<Player> currentlySelectedPlayers;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// A list of the PlayerStatusDX's that are available.
		/// </summary>
		[SerializeField, TabGroup("PSDXC", "Scene References")]
		private List<PlayerStatusDX> playerStatuses = new List<PlayerStatusDX>();
		/// <summary>
		/// A list of PlayerStatusDXs where they are active in the hierarchy.
		/// </summary>
		private List<PlayerStatusDX> ActivePlayerStatuses {
			get {
				return this.playerStatuses.Where(ps => ps.gameObject.activeInHierarchy == true).ToList();
			}
		}
		/// <summary>
		/// An "invisible" gameobject that is selected in the event that the move that was chosen is one that targets all players.
		/// An event trigger should be hooked up to it which will events detailed below.
		/// </summary>
		[SerializeField, TabGroup("PSDXC", "Scene References")]
		private GameObject hiddenAllPlayersGameObject;
		/// <summary>
		/// The parent that holds all of the PlayerStatuses.
		/// I am deciding I want to manipulate this (RISKY!! But I'm so tired right now.)
		/// </summary>
		[SerializeField, TabGroup("PSDXC", "Scene References")]
		private RectTransform allPlayerStatusesParent;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				// DontDestroyOnLoad(this.gameObject);
			}
		}
		private void Start() {
			// Build the statuses from the GameController's variables.
			this.BuildPlayerStatuses(players: GameController.Instance.Variables.Players);
			// Make them small by default.
			this.TweenSize(big: false);
		}
		private void OnDestroy() {
			if (instance == this) {
				instance = null;
			}
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Builds the PlayerStatusDXs with the players it needs to get moving.
		/// </summary>
		/// <param name="players"></param>
		public void BuildPlayerStatuses(List<Player> players) {
			// Turn off all the player statuses.
			this.playerStatuses.ForEach(ps => ps.gameObject.SetActive(false));
			// Go through each player and assign it a status.
			for (int i = 0; i < players.Count; i++) {
				this.playerStatuses[i].gameObject.SetActive(true);
				this.playerStatuses[i].AssignPlayer(player: players[i]);
			}
		}
		/// <summary>
		/// Sets the active visuals on the given player.
		/// Turns off all the others.
		/// </summary>
		/// <param name="player"></param>
		public void SetActivePlayer(Player player) {
			// Turn off all the active players.
			this.TurnOffActivePlayer();
			// Set the active visuals appropriately.
			player.PlayerStatusDX.SetActiveVisuals(true);
		}
		/// <summary>
		/// Turns off the active visuals on all the players.
		/// </summary>
		public void TurnOffActivePlayer() {
			this.ActivePlayerStatuses.ForEach(ps => ps.SetActiveVisuals(false));
		}
		/// <summary>
		/// Calls QuickRebuild on all the player statuses.
		/// </summary>
		public void QuickRebuild() {
			this.ActivePlayerStatuses.ForEach(ps => ps.QuickRebuild());
			// this.playerStatuses.ForEach(ps => ps.QuickRebuild());
		}
		#endregion

		#region SETTING SELECTABLES
		/// <summary>
		/// Sets whether the player statuses should be selectable or not.
		/// </summary>
		/// <param name="status"></param>
		public void SetPlayerStatusSelectables(bool status) {
			// this.playerStatuses.ForEach(ps => ps.SetSelectable(status: status));
			this.ActivePlayerStatuses.ForEach(ps => ps.SetSelectable(status: status));
		}
		/// <summary>
		/// Enables the selectables on the player statuses and allows them to be used as buttons.
		/// </summary>
		/// <param name="players">The players who are allowed to be selected.</param>
		/// <param name="currentBattleBehavior">The behavior that is potentially going to be used.</param>
		public void SetSelectablesOnPlayerTargets(List<Player> players, BattleBehavior currentBattleBehavior) {

			// Turn off all the statuses real quick.
			this.SetPlayerStatusSelectables(status: false);

			if (currentBattleBehavior.targetType == TargetType.OneAliveAlly) {
				// Set the selectables on the players who are allowed to be selected.
				players.Select(p => p.PlayerStatusDX).ToList().ForEach(ps => ps.SetSelectable(status: true));
				// Select the first one.
				EventSystem.current.SetSelectedGameObject(players[0].PlayerStatusDX.gameObject);

			} else if (currentBattleBehavior.targetType == TargetType.OneDeadAlly) {
				throw new NotImplementedException("the code in the if block above may work but with dead allies it kept going to the else block below. fix this.");
			} else {
				// If all the players are to be selected, remember the targets.
				this.currentlySelectedPlayers = players;
				// Turn on the hidden object, as I may have turned it off before.
				this.hiddenAllPlayersGameObject.SetActive(true);
				// Now, just enable the hidden object.
				EventSystem.current.SetSelectedGameObject(this.hiddenAllPlayersGameObject);
				// Go through each of the player statuses and make them look like they're highlighted.
				// this.playerStatuses.ForEach(ps => ps.Highlight());
				this.ActivePlayerStatuses.ForEach(ps => ps.Highlight());
			}
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Tweens all the player statuses in/out.
		/// </summary>
		/// <param name="status">Whether or not the status should be visible.</param>
		public void TweenVisible(bool status, bool inBattle = false) {

			// Depending on if this is getting called when a bttle is happening, the x position may differ.
			float visibleXPos = (inBattle == true ? -167f : -100f);

			this.ActivePlayerStatuses.ForEach(
				ps => ps.GetComponent<RectTransform>().DOAnchorPosX(
					endValue: status == true ? visibleXPos : 130f,
					duration: 0.5f,
					snapping: true));

			/*this.playerStatuses.ForEach(
				ps => ps.GetComponent<RectTransform>().DOAnchorPosX(
					endValue: status == true ? visibleXPos : 130f, 
					duration: 0.5f, 
					snapping: true));*/
		}
		/// <summary>
		/// Tweens the statuses so they appear big/small. 
		/// </summary>
		/// <param name="big">Whether the statuses are big or smale.</param>
		public void TweenSize(bool big) {
			// If tweening to Big Size, run the coroutine that tweens them Big.
			if (big == true) {
				this.StartCoroutine(this.TweenSizeBigRoutine());

				/*// THIS IS NEW
				// I'm also manipulating the transform of the object that holds the statuses because I need to make room for the announcer.
				this.allPlayerStatusesParent.DOKill(complete: true);
				this.allPlayerStatusesParent.DOScale(endValue: 0.8f, duration: 0.0f).SetEase(ease: Ease.Linear);
				this.allPlayerStatusesParent.DOAnchorPosY(endValue: -267f, duration: 0.0f).SetEase(ease: Ease.Linear);*/

			} else {
				// If not tweening Big, just snap them to being small.
				// this.playerStatuses.ForEach(ps => ps.TweenSmallAnimation());
				this.ActivePlayerStatuses.ForEach(ps => ps.TweenSmallAnimation());
				// Go through each status and just snap it to the value.
				for (int i = 0; i < this.ActivePlayerStatuses.Count; i++) {
					this.ActivePlayerStatuses[i].TweenSmallAnimation();
					// The y-position starts at -260 from the top and decrements in amounts of -150 when small.
					this.ActivePlayerStatuses[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-100f, -434f + ((i + (4 - this.ActivePlayerStatuses.Count)) * -150f));
				}

				/*// THIS IS NEW
				// Also move the parent back to its normal positions. (the smaller number for the scale has to do with how the icons got bigger before on their own)
				this.allPlayerStatusesParent.DOKill(complete: true);
				this.allPlayerStatusesParent.DOScale(endValue: 1, duration: 0.0f).SetEase(ease: Ease.Linear);
				this.allPlayerStatusesParent.DOAnchorPosY(endValue: 0, duration: 0.0f).SetEase(ease: Ease.Linear);*/

				/*for (int i = 0; i < this.playerStatuses.Count; i++) {
					this.playerStatuses[i].TweenSmallAnimation();
					// The y-position starts at -260 from the top and decrements in amounts of -150 when small.
					this.playerStatuses[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-100f, -434f + (i * -150f));
				}*/
			}
		}
		/// <summary>
		/// The routine I use for tweening the statsues Big.
		/// </summary>
		/// <returns></returns>
		private IEnumerator TweenSizeBigRoutine() {
			// Go through each status but give pause between each tween.
			for (int i = 0; i < this.ActivePlayerStatuses.Count; i++) {
				this.ActivePlayerStatuses[i].TweenBigAnimation();
				// The y-position starts at -121 from the top and decrements in amounts of -232 when big.
				// this.ActivePlayerStatuses[i].GetComponent<RectTransform>().DOAnchorPos(endValue: new Vector2(x: -167f, y: -121f + (i * -232f)), duration: 0.1f, snapping: true).SetEase(ease: Ease.InOutBounce);
				// (THIS IS THE NEW ONE)
				this.ActivePlayerStatuses[i].GetComponent<RectTransform>().DOAnchorPos(endValue: new Vector2(x: -167f, y: -325.5f + (i * -196f)), duration: 0.1f, snapping: true).SetEase(ease: Ease.InOutBounce);
				yield return new WaitForSeconds(0.1f);
			}
			
		}
		#endregion

		#region EVENT TRIGGERS : HIDDEN SELECTABLE
		/// <summary>
		/// An event that is called from the 'hidden' object that is meant to be used when targeting all players.
		/// </summary>
		/// <param name="submit">Was the event a submit or a cancel? Yes I know this is kinda vague.</param>
		public void HiddenAllPlayersGameObjectEvent(bool submit) {

			// Dehighlight all the players. Whether cancelling or submitting does not matter.
			// this.playerStatuses.ForEach(ps => ps.Dehighlight());
			this.ActivePlayerStatuses.ForEach(ps => ps.Dehighlight());

			// Turn off the hidden object so i don't accidentally re-select it.
			this.hiddenAllPlayersGameObject.SetActive(false);

			if (submit == true) {
				// If submit was hit, tell the battle menu controller to proceed with the players
				BattleMenuControllerDX.instance.SetCurrentTargetCombatants(combatants: this.currentlySelectedPlayers.Cast<Combatant>().ToList());
				
			} else {
				AudioController.instance?.PlaySFX(SFXType.Close);
				BattleMenuControllerDX.instance.CancelCombatantSelection();
				// BattleMenuControllerDX.Instance.TriggerEvent(eventName: "Cancel Player Selection");
			}

			// Null out the reference to the list. I don't want to remember it.
			this.currentlySelectedPlayers = null;
			
		}
		#endregion

	}


}