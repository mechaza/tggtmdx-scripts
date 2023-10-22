using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.UI.Legacy;
using Grawly.UI;
using System.Linq;
using Grawly.Battle.Analysis;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// Brings up the combatant analysis screen so that this combatant can learn a move.
	/// </summary>
	[System.Serializable]
	public class SkillCard : PauseBehaviorFunction {

		#region FIELDS - INHERITED
		/// <summary>
		/// Is this function asynchonous? I.e., healing probably isn't, but the menu to bring up learning a skill card is.
		/// </summary>
		public override bool IsAsynchronous {
			get {
				return true;
			}
		}
		#endregion

		#region FIELDS - BEHAVIOR TO LEARN
		/// <summary>
		/// The behavior that should be added to the combatant.
		/// </summary>
		[SerializeField]
		private BattleBehavior behaviorToLearn;
		#endregion

		#region FUNCTION
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior self) {

			
			
			// Save a ref to the persona.
			Persona persona = ((Player)targets[0]).ActivePersona;
			// Save a ref to the skill card just so I remember what's what.
			BattleBehavior skillCard = self;

			// Use the helper to present the screen.
			AnalysisSkillCardUseHelper.Instance.Present(
				persona: persona,
				cardBehavior: behaviorToLearn,
				onAnalysisCloseCallback: () => {
					
					// TODO: PLEASE MAKE THIS NOT SUCK
					Debug.LogWarning("PLEASE MAKE THIS NOT SUCK");
					
					// Determine if the skill card was used by checking the moves the persona has at the end of the evaluation.
					bool usedSkillCard = persona.AllBehaviors.SelectMany(d => d.Value).Contains(value: behaviorToLearn) ? true : false;

					if (usedSkillCard == true) {
						// If the skill card was used, pass the pause function complete param that says that 1 should be decremented from the count.
						PauseMenuEvaluator.instance.PauseFunctionComplete(num: 1);
					} else {
						// Otherwise, pass the param that says nothing should be decremented.
						PauseMenuEvaluator.instance.PauseFunctionComplete(num: 0);
					}
				});
			
			/*CombatantAnalysisControllerDX.Instance.Open(
				focusCombatant: persona, 
				analysisType: AnalysisScreenCategoryType.LearnSkillCard, 
				analysisDismissedCallback: () => {
						
				});*/
			
			/*Sequence seq = DOTween.Sequence();
			seq.AppendCallback(new TweenCallback(delegate {

				// Save the currently leveling persona.
				// currentLevelingPersona = GameController.Instance.players[0].GetPlayerPersona();
				
				// Build populates the combatant analysis canvas and makes it so the thing flashes and shit.
				// CombatantAnalysisCanvas.instance.Build(persona, CombatantAnalysisCanvas.ContextType.PersonaLevelUp);
				
				
				
			}));
			seq.AppendInterval(1f);
			seq.AppendCallback(new TweenCallback(delegate {
				CombatantAnalysisCanvas.instance.AddBehavior(
				persona: persona,
				behavior: behaviorToLearn,
				startCallback: new TweenCallback(delegate { }),
				finishCallback: new TweenCallback(delegate {

					CombatantAnalysisCanvas.instance.Clear();

					// Determine if the skill card was used by checking the moves the persona has at the end of the evaluation.
					bool usedSkillCard = persona.AllBehaviors.SelectMany(d => d.Value).Contains(value: behaviorToLearn) ? true : false;

					if (usedSkillCard == true) {
						// If the skill card was used, pass the pause function complete param that says that 1 should be decremented from the count.
						PauseMenuEvaluator.instance.PauseFunctionComplete(num: 1);
					} else {
						// Otherwise, pass the param that says nothing should be decremented.
						PauseMenuEvaluator.instance.PauseFunctionComplete(num: 0);
					}}),
				context: CombatantAnalysisCanvas.ContextType.SkillCardAdd);
			}));
			//
			//	Sequence has been assembled. Play it.
			//
			seq.Play();*/
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Brings up the combatant analysis screen so that this combatant can learn a move.";
		/// <summary>
		/// A string that details how this function works.
		/// </summary>
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		#endregion

	}


}