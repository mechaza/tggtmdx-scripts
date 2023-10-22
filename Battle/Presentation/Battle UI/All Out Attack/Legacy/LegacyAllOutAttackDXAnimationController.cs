using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Grawly.UI;

namespace Grawly.Battle.BattleMenu.Legacy {

	/// <summary>
	/// This is the class that should manage the assets of the all out attack. 
	/// The animation behavior is what should take care of the actuall function calls.
	/// All I do here is provide those functions that actually do get called.
	/// </summary>
	public class LegacyAllOutAttackDXAnimationController : MonoBehaviour {

		public static LegacyAllOutAttackDXAnimationController instance;

		#region FIELDS - RESOURCES
		/// <summary>
		/// The battle behavior that should be used for the all out attack itself.
		/// </summary>
		[SerializeField, TabGroup("All Out Attack", "Resources")]
		private BattleBehavior allOutAttackBehavior;
		/// <summary>
		/// The tutorial to use for the all out attacks on the first time.
		/// </summary>
		[SerializeField, TabGroup("All Out Attack", "Resources")]
		private TutorialTemplate allOutAttackTutorial;
		#endregion

		#region FIELDS - SCENE REFERENCES : GENERAL
		/// <summary>
		/// The GameObject that contains all visuals as children.
		/// </summary>
		[SerializeField, TabGroup("All Out Attack", "Scene References")]
		private GameObject allVisualObjects;
		/// <summary>
		/// The GameObject that contains all visuals as children.
		/// </summary>
		public GameObject AllVisualsObjects {
			get {
				return this.allVisualObjects;
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES : PROMPT
		/// <summary>
		/// The GameObject that contains all the other objects that make up the prompt.
		/// </summary>
		[SerializeField, TabGroup("All Out Attack", "Scene References")]
		private GameObject promptVisualsGameObject;
		/// <summary>
		/// The image that shows the bust up of who is giving the prompt.
		/// </summary>
		[SerializeField, TabGroup("All Out Attack", "Scene References")]
		private Image promptBustUpImage;
		/// <summary>
		/// The image that shows the dropshadow of who is giving the prompt.
		/// </summary>
		[SerializeField, TabGroup("All Out Attack", "Scene References")]
		private Image promptBustUpDropshadowImage;
		/// <summary>
		/// The GameObject that should be selected by default when this prompt is displayed. Usually is the "Yes" option.
		/// </summary>
		[SerializeField, TabGroup("All Out Attack", "Scene References")]
		private GameObject defaultPromptSelectableObject;
		#endregion

		#region FIELDS - SCENE REFERENCES : ANIMATION
		/// <summary>
		/// The GameObject that has all the visuals for the animation specifically.
		/// </summary>
		[SerializeField, TabGroup("All Out Attack", "Scene References")]
		private GameObject allOutAttackAnimationVisualsObject;
		/// <summary>
		/// The GameObject that has all the visuals for the animation specifically.
		/// </summary>
		public GameObject AllOutAttackAnimationVisualsObject {
			get {
				return this.allOutAttackAnimationVisualsObject;
			}
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region PROMPT CONTROLS
		/// <summary>
		/// Turns the all out attack prompt on and assembles the bust up from the players who are able to show up on it.
		/// </summary>
		/// <param name="activePlayers">The players who may have their bust up picked.</param>
		public void EnablePrompt(List<Player> activePlayers, bool haveSeenTutorial) {

			// Turn off the bust up in the battle menu controller.
			BattleMenuControllerDX.instance.PlayerBustUpGameObject.SetActive(false);

			// Turn on the entire prompt.
			this.promptVisualsGameObject.SetActive(true);

			// Make the bust up on the prompt be one of the players.
			Sprite randomPlayerSprite = activePlayers.Random().playerTemplate.bustUp;

			this.promptBustUpImage.overrideSprite = randomPlayerSprite;
			this.promptBustUpDropshadowImage.overrideSprite = randomPlayerSprite;


			// If its necessary to display the tutorial, do that. (This is usually checked via the story flags.)
			if (haveSeenTutorial == false) {

				// Tell the GameVariables that the all out attack has been seen.
				GameController.Instance.Variables.StoryFlags.SetFlag(flagType: StoryFlagType.SawAllOutAttackTutorial, value: true);

				// Null out the currently selected object.
				EventSystem.current.SetSelectedGameObject(null);

				// Give some time for the effect to settle in, then open the tutorial.
				GameController.Instance.WaitThenRun(timeToWait: 2f, action: delegate {

					// Turn off the prompt visuals to make room for the tutorial.
					this.promptVisualsGameObject.SetActive(false);

					// Open up the tutorial.
					TutorialScreenController.OpenTutorial(
						tutorialTemplate: this.allOutAttackTutorial,                    // Tutorial is the one for the all out attack, obv.		
						objectToReselectOnClose: this.defaultPromptSelectableObject,	// Upon closing the tutorial, reselect the default prompt.
						actionOnClose: delegate {
							this.promptVisualsGameObject.SetActive(true);				// Also make sure to display the visuals again.
						});   
				});


			} else {
				// If the tutorial has already been seen, just tell the event system to select the default prompt button.
				EventSystem.current.SetSelectedGameObject(this.defaultPromptSelectableObject);
			}

			

		}
		/// <summary>
		/// The function that gets called from an event trigger on a selectable game object
		/// </summary>
		/// <param name="status">Whether the player wants to engage in the all out attack or not.</param>
		public void PromptResponse(bool status) {
			// Regardless of what was chosen, turn the prompt visuals off.
			this.promptVisualsGameObject.SetActive(false);

			// Check what the player said they wanted to do.
			if (status == true) {
				// If they want to do the all out attack, Do That.
				this.allOutAttackBehavior.BattleFunction.Execute(
					source: BattleController.Instance.CurrentCombatant,
					targets: BattleController.Instance.AliveEnemies.Cast<Combatant>().ToList(),
					battleBehavior: this.allOutAttackBehavior);

			} else {
				// If they don't want to do the all out attack, just proceed to the next turn. The bust up gets turned on atuomatically.
				BattleController.Instance.FSM.SendEvent("All Out Attack Declined");

			}
		}
		#endregion

		#region BEHAVIOR EXECUTION
		/// <summary>
		/// dumbass me wants to provide access to running the all out attack when auto battle is on.
		/// there should only ever be one reference to this, ever.
		/// </summary>
		public void AutoAllOutAttack() {
			Debug.Log("EXECUTING ALL OUT ATTACK VIA THE SHOEHORNED IN FUNCTION THAT GETS CALLED WHEN AUTOBATTLE IS ON.");
			this.allOutAttackBehavior.BattleFunction.Execute(
					source: BattleController.Instance.CurrentCombatant,
					targets: BattleController.Instance.AliveEnemies.Cast<Combatant>().ToList(),
					battleBehavior: this.allOutAttackBehavior);
		}
		#endregion

	}


}