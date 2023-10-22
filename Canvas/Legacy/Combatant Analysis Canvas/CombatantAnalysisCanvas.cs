using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using Grawly.Chat;
using Grawly.Dungeon;
using Grawly.Battle.BattleMenu;

namespace Grawly.UI.Legacy {

	public class CombatantAnalysisCanvas : MonoBehaviour {

		/// <summary>
		/// Depending on the context, different measures may need to be taken when interacting with the icons.
		/// </summary>
		public enum ContextType {
			ChangePersona,
			AnalyzeEnemy,
			PersonaLevelUp,
			SkillCardAdd,
		}

		public static CombatantAnalysisCanvas instance;

		#region FIELDS - STATE
		/// <summary>
		/// The GameObject that was being selected prior to building the analysis canvas. 
		/// It's june 28, 2018, and im an idiot but ill make this work so help me.
		/// </summary>
		private GameObject lastSelectedGameObject;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The object that holds the canvas for the combatant analysis.
		/// </summary>
		[TabGroup("Scene", "Objects"), SerializeField]
		private GameObject canvasObject;
		/// <summary>
		/// The object that holds the misc icons that represent combatants.
		/// </summary>
		[TabGroup("Scene", "Objects"), SerializeField]
		private GameObject combatantIconsObject;
		/// <summary>
		/// Holds the graphics that represent a potential move that a persona could learn.
		/// </summary>
		[TabGroup("Scene", "Objects"), SerializeField]
		private GameObject levelUpMoveIconsObject;


		/// <summary>
		/// The bust up for the combatant.
		/// </summary>
		[TabGroup("Graphics", "Images"), SerializeField]
		private Image combatantBustUp;
		/// <summary>
		/// The shadow for the combatant's bust up.
		/// </summary>
		[TabGroup("Graphics", "Images"), SerializeField]
		private Image combatantBustUpShadow;
		/// <summary>
		/// The white backing that appears behind the bust up.
		/// </summary>
		[TabGroup("Graphics", "Images"), SerializeField]
		private Image bustUpBackingWhite;
		/// <summary>
		/// The gradient that appears behind the white backing.
		/// </summary>
		[TabGroup("Graphics", "Images"), SerializeField]
		private Image bustUpBackingGradient;
		/// <summary>
		/// The icon to represent the standard attack's elemental affiliation.
		/// </summary>
		[TabGroup("Graphics", "Images"), SerializeField]
		private Image standardAttackElementalIcon;
		/// <summary>
		/// The graphic used to represent the info box's background.
		/// </summary>
		[TabGroup("Graphics", "Images"), SerializeField]
		private Image infoBoxBackground;
		/// <summary>
		/// The visuals that contain all the assets for the "current user" display.
		/// </summary>
		[TabGroup("Graphics", "Images"), SerializeField]
		private GameObject currentPersonaUserGameObject;
		/// <summary>
		/// The image for who is currently using this persona.
		/// </summary>
		[TabGroup("Graphics", "Images"), SerializeField]
		private Image currentPersonaUserBustUpImage;
		/// <summary>
		/// The dropshadow image for who is currently using this persona.
		/// </summary>
		[TabGroup("Graphics", "Images"), SerializeField]
		private Image currentPersonaUserBustUpDropshadowImage;

		/// <summary>
		/// The label used to describe the combatant's standard attack.
		/// </summary>
		[TabGroup("Graphics", "Text"), SerializeField]
		private SuperTextMesh standardAttackLabel;
		/// <summary>
		/// The label used to describe a given move highlighted.
		/// </summary>
		[TabGroup("Graphics", "Text"), SerializeField]
		private SuperTextMesh infoBoxLabel;
		/// <summary>
		/// The label that shows the combatatn's name.
		/// </summary>
		[TabGroup("Graphics", "Text"), SerializeField]
		private SuperTextMesh combatantNameLabel;
		/// <summary>
		/// The label that shows what level this combatant is.
		/// </summary>
		[TabGroup("Graphics", "Text"), SerializeField]
		private SuperTextMesh levelLabel;
		/// <summary>
		/// The label that shows how much EXP is needed for the next level.
		/// </summary>
		[TabGroup("Graphics", "Text"), SerializeField]
		private SuperTextMesh nextExpLabel;
		/// <summary>
		/// The label that says what the next level needs to be to unlock the next move.
		/// </summary>
		[TabGroup("Graphics", "Text"), SerializeField]
		private SuperTextMesh levelUpMoveLabel;

		/// <summary>
		/// The fill for the ST attribute.
		/// </summary>
		[TabGroup("Scene", "Attributes"), SerializeField]
		private Image stBarFill;
		/// <summary>
		/// The fill for the MA attribute.
		/// </summary>
		[TabGroup("Scene", "Attributes"), SerializeField]
		private Image maBarFill;
		/// <summary>
		/// The fill for the EN attribute.
		/// </summary>
		[TabGroup("Scene", "Attributes"), SerializeField]
		private Image enBarFill;
		/// <summary>
		/// The fill for the AG attribute.
		/// </summary>
		[TabGroup("Scene", "Attributes"), SerializeField]
		private Image agBarFill;
		/// <summary>
		/// The fill for the LU attribute.
		/// </summary>
		[TabGroup("Scene", "Attributes"), SerializeField]
		private Image luBarFill;

		/// <summary>
		/// Contains the icons used for representing combatants.
		/// </summary>
		[TabGroup("Scene", "Icons"), SerializeField]
		private List<CombatantAnalysisProfileIcon> combatantIcons = new List<CombatantAnalysisProfileIcon>();
		/// <summary>
		/// Contains the icons used for representing a given combatant's resistances.
		/// </summary>
		[TabGroup("Scene", "Icons"), SerializeField]
		private List<CombatantAnalysisResistanceIcon> resistanceIcons = new List<CombatantAnalysisResistanceIcon>();
		/// <summary>
		/// Contains the different move items for a given combatant.
		/// </summary>
		[TabGroup("Scene", "Icons"), SerializeField]
		private List<CombatantAnalysisMoveItem> moveItems = new List<CombatantAnalysisMoveItem>();
		/// <summary>
		/// Just a quick way of referencing the move items at the top of the lists so that their selectables can be adjusted.
		/// I need to refactor all this later.
		/// </summary>
		public List<CombatantAnalysisMoveItem> TopMoveItems {
			get {
				return this.moveItems.Where(m => m.slot == 0 | m.slot == 4).ToList();
			}
		}
		/// <summary>
		/// This is the move item used to represent a move that may or may not exist within a persona, if it were to level up.
		/// </summary>
		[TabGroup("Scene", "Icons"), SerializeField]
		private CombatantAnalysisMoveItem levelUpMoveItem;
		/// <summary>
		/// Visual representations of level up moves the player can't know about yet.
		/// </summary>
		[TabGroup("Scene", "Icons"), SerializeField]
		private List<GameObject> lockedLevelUpMoveIcons = new List<GameObject>();

		#endregion

		#region FIELDS - MISC
		/// <summary>
		/// This is the combatant that should be saved if this canvas gives the option to the player to pick one.
		/// </summary>
		public Combatant selectedCombatant;
		/// <summary>
		/// The animation sequence for a bustup sliding into view.
		/// </summary>
		private Sequence bustUpSeq;
		/// <summary>
		/// The sequence for handling attribute fills.
		/// </summary>
		private Sequence attributeFillSeq;
		/// <summary>
		/// The current context of the canvas.
		/// </summary>
		private ContextType context;
		/// <summary>
		/// Saves the context of the behavior requested to be learned.
		/// </summary>
		private BattleBehavior savedBehavior;
		#endregion

		#region FIELDS - ADDING BEHAVIORS
		// I REALLY don't... enjoy how this is structured but hopefully I won't have to deal with it a lot.
		/// <summary>
		/// The callback to be run when adding a behavior.
		/// </summary>
		private TweenCallback finishCallback;
		/// <summary>
		/// The saved slot of the selected move item. I don't like this.
		/// </summary>
		public int slot;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				// Order the items to be safe.
				combatantIcons.OrderBy(c => c.slot);
				moveItems.OrderBy(c => c.slot);
				bustUpSeq = DOTween.Sequence();
				attributeFillSeq = DOTween.Sequence();
			}
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Builds up the analysis canvas with a list of combatants.
		/// </summary>
		/// <param name="combatants"></param>
		public void Build(List<Combatant> combatants, ContextType context) {

			// Remember the last gameobject being selected. I will need to reselect it when I'm done here.
			if (context == ContextType.AnalyzeEnemy || context == ContextType.ChangePersona) {
				this.lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			} else {
				// Setting it to null otherwise so I dont shoot myself in the foot.
				this.lastSelectedGameObject = null;
			}
			

			// Nullify the selected combatant to be on the safe side.
			selectedCombatant = null;
			// Turn on the canvas object.
			canvasObject.SetActive(true);

			if (context == ContextType.PersonaLevelUp) {
				// If this is the persona level up screen, hide the icons
				Flasher.instance.Flash();
				AudioController.instance.PlaySFX(SFXType.PlayerExploit);
				combatantIconsObject.SetActive(false);
				DisplayCombatantDetail(combatants[0], context);
			} else {
				combatantIconsObject.SetActive(true);
				// Go through each combatant icon and build it.
				for (int i = 0; i < combatants.Count; i++) {
					combatantIcons[i].Build(combatants[i], context);
				}
				// Go through the remaining icons and clear them out.
				for (int i = combatants.Count; i < combatantIcons.Count; i++) {
					combatantIcons[i].Clear();
				}
				// Turn on the the first combatant icon.
				EventSystem.current.SetSelectedGameObject(combatantIcons[0].gameObject);
			}

			// Make sure to turn off the move item selectables. But only if not analysing the enemy (AAA)
			if (context == ContextType.AnalyzeEnemy || context == ContextType.ChangePersona) {
				// make sure to. uh. set selectables accordingly. this sucks!
				SetMoveItemSelectables(status: true, context: context, combatant: combatants.First());
			} else {
				SetMoveItemSelectables(status: false, context: context);
			}
			
		}
		/// <summary>
		/// Builds up the analysis canvas with a combatant.
		/// </summary>
		/// <param name="combatant"></param>
		/// <param name="context"></param>
		public void Build(Combatant combatant, ContextType context) {
			List<Combatant> combatants = new List<Combatant>();
			combatants.Add(combatant);
			Build(combatants, context);
		}
		/// <summary>
		/// Clears out the canvas.
		/// </summary>
		public void Clear() {
			// Null out the last selected game object.
			this.lastSelectedGameObject = null;
			// Turn this canvas off.
			canvasObject.SetActive(false);
		}
		/// <summary>
		/// HAM FISTING THIS IN BECAUSE I HATE MYSELF
		/// </summary>
		public void ResetAndClose() {
			// Tell the battle menu controller to close this out.
			BattleMenuControllerDX.instance.CloseCombatantAnalysis(gameObjectToReselect: this.lastSelectedGameObject);
			// Clear out.
			this.Clear();
			/*	Debug.Log("This is where the battle canvas used to get turned back on.");
				// CanvasController.Instance.SetBattleCanvas(true);
				LegacyBattleMenuController.Instance.ChangeBustUpCombatant(BattleController.Instance.CurrentCombatant);
				Debug.Log("TODO: FIX THE SEND MESSAGE CALLS HERE");
				LegacyBattleMenuController.Instance.SendMessage("CancelPersonaSelect");*/
		}
		#endregion

		#region CANVAS DETAIL
		/// <summary>
		/// Displays the analysis of a given combatant.
		/// </summary>
		/// <param name="combatant"></param>
		public void DisplayCombatantDetail(Combatant combatant, ContextType context) {

			// Remember the combatant
			selectedCombatant = combatant;

			// Turn off the info box and its text. The only time display combatant detail gets called is
			// when a combatant icon is selected. I.e., the moves are not selectable right now! clever! wow!
			DisplayInfoBox("");

			// By default, turn off the current persona user visuals
			this.currentPersonaUserGameObject.SetActive(false);

			// Lerp the fill amounts of the bars
			stBarFill.fillAmount = Mathf.InverseLerp(-20f, 100f, combatant.BaseST);
			maBarFill.fillAmount = Mathf.InverseLerp(-20f, 100f, combatant.BaseMA);
			enBarFill.fillAmount = Mathf.InverseLerp(-20f, 100f, combatant.BaseEN);
			agBarFill.fillAmount = Mathf.InverseLerp(-20f, 100f, combatant.BaseAG);
			luBarFill.fillAmount = Mathf.InverseLerp(-20f, 100f, combatant.BaseLU);

			// Set the name.
			combatantNameLabel.Text = combatant.metaData.name;

			// Set the level
			levelLabel.Text = "Lv. " + combatant.Level;

			// Set up the bustups. I don't like using typeof but whatever lmao
			//
			//	Please make sure to fix this later.
			//
			//  Debug.LogWarning("Fix this so all combatants have a standard attack definition");
			if (combatant.GetType() == typeof(Persona)) {
				Persona p = (Persona)combatant;
				nextExpLabel.Text = "Next " + p.ExpForNextLevel();
				// Set the standard attack sprite to the element of the persona's standard attack.
				standardAttackElementalIcon.sprite = DataController.Instance.GetElementalIcon(p.standardAttackBehavior.elementType);
				// Set the standard attack label text.
				standardAttackLabel.Text = p.standardAttackBehavior.description;
				combatantBustUp.sprite = p.bustUp;
				combatantBustUpShadow.sprite = p.bustUp;
				combatantBustUp.preserveAspect = true;
				combatantBustUpShadow.preserveAspect = true;

				// Refresh the level up moves.
				RefreshLevelUpMoves(p, context);
				// Turn on the current persona user game object if the persona is active in the party somewhere.
				this.currentPersonaUserGameObject.SetActive(value: this.SetCurrentPersonaUserVisuals(persona: combatant as Persona));

			} else if (combatant.GetType() == typeof(Enemy)) {
				Enemy e = (Enemy)combatant;
				nextExpLabel.Text = "";
				standardAttackElementalIcon.sprite = DataController.Instance.GetElementalIcon(ElementType.Phys);
				standardAttackLabel.Text = "Basic attack.";
				combatantBustUp.sprite = e.EnemySprite;
				combatantBustUpShadow.sprite = e.EnemySprite;
				combatantBustUp.preserveAspect = true;
				combatantBustUpShadow.preserveAspect = true;
				// Enemies never have level up moves.
				levelUpMoveIconsObject.SetActive(false);
			} else {
				throw new System.Exception("Could not determine type of combatant!");
			}

			// Go through each resistance icon and build up the icon for the corresponding elemental affiliation.
			foreach (CombatantAnalysisResistanceIcon resistanceIcon in resistanceIcons) {
				resistanceIcon.Build(combatant);
			}

			// Loop through each special behavior and build a move item from it.
			for (int i = 0; i < combatant.AllBehaviors[BehaviorType.Special].Count; i++) {
				moveItems[i].Build(combatant.AllBehaviors[BehaviorType.Special][i], context);
			}
			// Now, clear out any move items that went unused.
			for (int i = combatant.AllBehaviors[BehaviorType.Special].Count; i < moveItems.Count; i++) {
				moveItems[i].Clear();
			}

			// Animate the bust up to slide in.
			AnimateBustUp(combatant);
			// Also animate the attribute fills
			AnimateAttributeFills(combatant);
		}
		/// <summary>
		/// Refreshes the level up moves for the given Persona.
		/// </summary>
		public void RefreshLevelUpMoves(Persona persona, ContextType context) {
			// Check if the player has level up moves.
			if (persona.levelUpMoves.Count > 0) {
				// Set up the level up moves if they do.
				levelUpMoveIconsObject.SetActive(true);
				levelUpMoveItem.Build(persona.levelUpMoves.Peek().behavior, context);
				levelUpMoveLabel.Text = "Lv " + persona.levelUpMoves.Peek().level.ToString();
				// Turn on relevant locked level up move icons.
				for (int i = 0; i < persona.levelUpMoves.Count - 1; i++) {
					// Do NOT do shit if there are too many moves to fill in the level up icons.
					if (i == lockedLevelUpMoveIcons.Count) { break; }
					// If you CAN toggle it, though, do so.
					lockedLevelUpMoveIcons[i].SetActive(true);
				}
				// Turn off the locked level up move icons that aren't in use.
				for (int i = persona.levelUpMoves.Count - 1; i < lockedLevelUpMoveIcons.Count; i++) {
					// Do NOT do shit if there are too many moves to fill in the level up icons.
					if (i == lockedLevelUpMoveIcons.Count) { break; }
					// If you CAN toggle it, though, do so.
					lockedLevelUpMoveIcons[i].SetActive(false);
				}
			} else {
				// Turn it off if they don't.
				levelUpMoveIconsObject.SetActive(false);
			}
		}
		/// <summary>
		/// Displays information about a given behavior inside of the info box.
		/// </summary>
		/// <param name="str"></param>
		public void DisplayInfoBox(string str) {
			if (str == "") {
				infoBoxLabel.Text = "";
				infoBoxBackground.gameObject.SetActive(false);
			} else {
				infoBoxLabel.Text = str;
				infoBoxBackground.gameObject.SetActive(true);
			}
		}
		#endregion

		#region HELPERS - CURRENT PERSONA USER
		/// <summary>
		/// Sets the visuals on the Current Persona user details if a player is using this persona.
		/// Returns true if a player is actually using this persona, false if not.
		/// I use that bool to turn the visuals themselves on/off.
		/// </summary>
		/// <param name="persona"></param>
		/// <returns></returns>
		private bool SetCurrentPersonaUserVisuals(Persona persona) {
			// Check if any of the players have this as their active persona.
			if (GameController.Instance.Variables.Players.Select(p => p.ActivePersona).Contains(value: persona)) {
				// If the player exists, grab their template and assign the bust up images.
				PlayerTemplate playerTemplate = GameController.Instance.Variables.Players.First(p => p.ActivePersona == persona).playerTemplate;
				// If the color was fading before as a result of the animation, be sure to reset it.
				this.currentPersonaUserBustUpImage.DOComplete();
				this.currentPersonaUserBustUpImage.sprite = playerTemplate.bustUp;
				this.currentPersonaUserBustUpDropshadowImage.sprite = playerTemplate.bustUp;
				// Return that the operation was successful.
				return true;
			} else {
				return false;
			}
		}
		/// <summary>
		/// A quick animation to be called from the profile icon select that flashes the current persona user's bust up to signal that the element kinda. exists.
		/// </summary>
		public void FlashCurrentPersonaUserVisuals() {
			this.currentPersonaUserBustUpImage.color = Color.red;
			this.currentPersonaUserBustUpImage.DOColor(endValue: Color.white, duration: 0.5f);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Makes the bust up slide into view.
		/// </summary>
		/// <param name="combatant"></param>
		public void AnimateBustUp(Combatant combatant) {
			// Complete the bust up sequence if it was in motion.
			bustUpSeq.Complete();
			// Start a new sequence.
			RectTransform srt = combatantBustUpShadow.GetComponent<RectTransform>();
			RectTransform crt = combatantBustUp.GetComponent<RectTransform>();
			bustUpSeq = DOTween.Sequence();
			bustUpSeq.AppendCallback(new TweenCallback(delegate {
				srt.anchoredPosition = new Vector2(250f, srt.anchoredPosition.y);
				crt.anchoredPosition = new Vector2(10f, crt.anchoredPosition.y);
			}));
			// bustUpSeq.Append(srt.DOAnchorPosX(-102.5f, 0.1f));
			bustUpSeq.Append(srt.DOAnchorPosX(15f, 0.1f));
			bustUpSeq.Append(crt.DOAnchorPosX(-9.83f, 0.1f));
			bustUpSeq.Play();
		}
		/// <summary>
		/// Animate the bars that fill up the attributes.
		/// </summary>
		/// <param name="combatant"></param>
		public void AnimateAttributeFills(Combatant combatant) {
			attributeFillSeq.Complete();
			attributeFillSeq = DOTween.Sequence();
			attributeFillSeq.AppendCallback(new TweenCallback(delegate {
				stBarFill.fillAmount = 0f;
				maBarFill.fillAmount = 0f;
				enBarFill.fillAmount = 0f;
				agBarFill.fillAmount = 0f;
				luBarFill.fillAmount = 0f;
			}));
			attributeFillSeq.AppendInterval(0.05f);
			attributeFillSeq.Append(stBarFill.DOFillAmount(Mathf.InverseLerp(-20f, 100f, combatant.BaseST), 0.05f));
			attributeFillSeq.Append(maBarFill.DOFillAmount(Mathf.InverseLerp(-20f, 100f, combatant.BaseMA), 0.05f));
			attributeFillSeq.Append(enBarFill.DOFillAmount(Mathf.InverseLerp(-20f, 100f, combatant.BaseEN), 0.05f));
			attributeFillSeq.Append(agBarFill.DOFillAmount(Mathf.InverseLerp(-20f, 100f, combatant.BaseAG), 0.05f));
			attributeFillSeq.Append(luBarFill.DOFillAmount(Mathf.InverseLerp(-20f, 100f, combatant.BaseLU), 0.05f));
			attributeFillSeq.Play();
		}
		/// <summary>
		/// Assuming the canvas has already been built, highlight the new move in the given slot.
		/// </summary>
		/// <param name="combatant"></param>
		/// <param name="behavior"></param>
		public void FlashNewMoveItem(Combatant combatant, int slot, BattleBehavior behavior) {
			if (slot == 8) {
				levelUpMoveItem.FlashNewMoveItem(behavior);
			} else {
				moveItems[slot].FlashNewMoveItem(behavior);
			}
		}
		#endregion

		#region ADDING MOVES
		/// <summary>
		/// Call this routine to start the routine which allows a standard way of adding a behavior to a Persona.
		/// </summary>
		/// <param name="persona">The Persona to potentially add a behavior to.</param>
		/// <param name="behavior">The behavior to be added to this Persona.</param>
		/// <param name="startCallback">A callback to be run at the beginning of this method.</param>
		/// <param name="finishCallback">A callback to be run when this method is finished.</param>
		/// <param name="context">The context for when this behavior is being added.</param>
		public void AddBehavior(Persona persona, BattleBehavior behavior, TweenCallback startCallback, TweenCallback finishCallback, ContextType context) {
			this.context = context;
			this.savedBehavior = behavior;
			this.finishCallback = finishCallback;
			// Start up a new sequence and append the start callback.
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(startCallback);

			//
			//	Determine if this Persona can add a behavior without having to worry about space.
			//
			if (persona.CanAddBehavior(behavior) == true) {
				//
				//	Persona can add move without worry. 
				//
				seq.AppendCallback(new TweenCallback(delegate {
					int slot = persona.AllBehaviors[BehaviorType.Special].Count;
					persona.AddBehavior(behavior: behavior, slot: slot);
					FlashNewMoveItem(persona, slot, behavior);
					if (context == ContextType.PersonaLevelUp) { persona.levelUpMoves.Dequeue(); }
					RefreshLevelUpMoves(persona, context);
				}));
				seq.AppendInterval(1f);
				seq.AppendCallback(new TweenCallback(delegate {
					ChatControllerDX.GlobalOpen(
						chatScript: new PlainChatScript(persona.metaData.name + " has learned " + behavior.behaviorName),
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							finishCallback();
						});
					/*Chat.Legacy.LegacyChatController.Open(
						script: new PlainChatScript(persona.metaData.name + " has learned " + behavior.behaviorName),
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							finishCallback();
					});*/
				}));
			} else {
				//
				//	Persona requires a bit of extra working with.
				//

				// If the context type is Leveling Up, flash the level up move.
				if (context == ContextType.PersonaLevelUp) {
					seq.AppendCallback(new TweenCallback(delegate {
						// This will flash slot 8; the new move.
						CombatantAnalysisCanvas.instance.FlashNewMoveItem(combatant: persona, slot: 8, behavior: behavior);
					}));
				}
				seq.AppendInterval(1f);
				seq.AppendCallback(new TweenCallback(delegate {
					ChatControllerDX.GlobalOpen(
						chatScript: new PlainChatScript("Please pick a move to replace."),
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							CombatantAnalysisCanvas.instance.SetMoveItemSelectables(status: true, context: context);
							EventSystem.current.SetSelectedGameObject(CombatantAnalysisCanvas.instance.GetMoveItemSelectable(7));
						});
					/*Chat.Legacy.LegacyChatController.Open(
						script: new PlainChatScript("Please pick a move to replace."),
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							CombatantAnalysisCanvas.Instance.SetMoveItemSelectables(status: true, context: context);
							EventSystem.current.SetSelectedGameObject(CombatantAnalysisCanvas.Instance.GetMoveItemSelectable(7));
						});*/
				}));
			}
		}
		/// <summary>
		/// This is the function that gets called from CombatantAnalysisMoveItem's chat controller callback when finishing up asking whether the player wants to add a move or not.
		/// </summary>
		/// <param name="str">The chatcontroller's callback variable</param>
		/// <param name="num">The chatcontroller's callback variable</param>
		/// <param name="toggle">The chatcontroller's callback variable</param>
		public void ConfirmMoveReplacement(string str, int num, bool toggle) {
			Sequence seq = DOTween.Sequence();
			if (toggle == true) {
				Persona persona = (Persona)selectedCombatant;
				seq.AppendCallback(new TweenCallback(delegate {
					persona.AddBehavior(behavior: savedBehavior, slot: slot);
					if (context == ContextType.PersonaLevelUp) { persona.levelUpMoves.Dequeue(); }
					// persona.levelUpMoves.Dequeue();
					// persona.AddBehavior(persona.levelUpMoves.Dequeue().behavior);
					FlashNewMoveItem(persona, slot, savedBehavior);
				}));
				seq.AppendInterval(1f);
				seq.AppendCallback(new TweenCallback(delegate {
					RefreshLevelUpMoves(persona, this.context);
					ChatControllerDX.GlobalOpen(
						chatScript: new PlainChatScript(persona.metaData.name + " has learned " + savedBehavior.behaviorName),
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							finishCallback();
						});
					/*Chat.Legacy.LegacyChatController.Open(
						script: new PlainChatScript(persona.metaData.name + " has learned " + savedBehavior.behaviorName),
						chatOpenedCallback: delegate { },
						chatClosedCallback: delegate {
							finishCallback();
						});*/
				}));
				// seq.AppendCallback(this.finishCallback);
				seq.Play();

			} else {
				SetMoveItemSelectables(true, context);
				EventSystem.current.SetSelectedGameObject(CombatantAnalysisCanvas.instance.GetMoveItemSelectable(this.slot));
			}
		}
		/// <summary>
		/// Gets called when a player decides to potentially cancel learning a new move.
		/// </summary>
		/// <param name="slot"></param>
		public void PromptForCancel(int slot) {
			List<ChatDirective> directives = new List<ChatDirective>();
			directives.Add(new DialogueDirective(speakerShorthand: "", dialogue: "Cancel learning " + this.savedBehavior.behaviorName + "?"));
			directives.Add(new OptionDirective(trueString: "Yes", falseString: "No"));
			ChatControllerDX.GlobalOpen(
				chatScript: new PlainChatScript(directives),
				chatOpenedCallback: delegate { },
				chatClosedCallback: delegate (string str, int num, bool toggle) {
					if (toggle == true) {
						if (context == ContextType.PersonaLevelUp) {
							((Persona)selectedCombatant).levelUpMoves.Dequeue();
						}
						this.finishCallback();
					} else {
						SetMoveItemSelectables(true, context);
						EventSystem.current.SetSelectedGameObject(GetMoveItemSelectable(slot));
					}
				});
			/*Chat.Legacy.LegacyChatController.Open(
				script: new PlainChatScript(directives),
				chatOpenedCallback: delegate { },
				chatClosedCallback: delegate(string str, int num, bool toggle) {
					if (toggle == true) {
						if (context == ContextType.PersonaLevelUp) {
							((Persona)selectedCombatant).levelUpMoves.Dequeue();
						}
						this.finishCallback();
					} else {
						SetMoveItemSelectables(true, context);
						EventSystem.current.SetSelectedGameObject(GetMoveItemSelectable(slot));
					}
				});*/
		}
		#endregion

		#region SELECTABLES
		/// <summary>
		/// Sets the combatant icons to be selectable or not.
		/// </summary>
		/// <param name="status"></param>
		public void SetCombatantIconSelectables(bool status) {
			foreach (CombatantAnalysisProfileIcon combatantIcon in combatantIcons) {
				combatantIcon.SetSelectable(status);
			}
		}
		/// <summary>
		/// Sets the move items to be selectable or not.
		/// </summary>
		/// <param name="status"></param>
		public void SetMoveItemSelectables(bool status, ContextType context, Combatant combatant = null) {
			foreach (CombatantAnalysisMoveItem moveItem in moveItems) {
				moveItem.SetSelectable(status);
			}

			// AHHHHHHHHHH I HATE THIS. DISABLE UNUSED SLOT SELECTABLES
			if (combatant != null) {
				for (int i = combatant.AllBehaviors[BehaviorType.Special].Count; i < 8; i++) {
					moveItems[i].SetSelectable(false);
				}
			}
			

			// Depending on the context, I may also want to enable the level up move item.
			if (context == ContextType.PersonaLevelUp) {
				levelUpMoveItem.SetSelectable(status);
			} else {
				levelUpMoveItem.SetSelectable(false);
			}
		}
		/// <summary>
		/// Returns a move item at the given slot. Mostly for the EventSystem and picking out a move item to select.
		/// </summary>
		/// <param name="slot"></param>
		/// <returns></returns>
		public GameObject GetMoveItemSelectable(int slot) {
			if (slot == 8) {
				return levelUpMoveItem.gameObject;
			} else {
				return moveItems[slot].gameObject;
			}
		}
		#endregion

	}

}