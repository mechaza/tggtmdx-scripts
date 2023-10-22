using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Grawly.Chat;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// A basic way to get a cutscene going. whomp whomp. This is for the clown demo and probably won't be used otherwise.
	/// </summary>
	public class PrototypeBattleIntroCutscene : BattleModifier {

		/*
		 #region FIELDS
		/// <summary>
		/// The chat script to open this shit up with.
		/// </summary>
		[SerializeField]
		private SerializedChatScript chatScript;
		/// <summary>
		/// A macro to send alongside the chat when its u.hgh
		/// </summary>
		[SerializeField]
		private Bolt.FlowMacro boltMacro;
		#endregion

		#region INTERFACE IMPLEMENTATION - IONBATTLESTART
		public Sequence OnBattleStart(Sequence seq, Combatant self) {
			// Basically, this simulataneously pushes the macro and opens a separate chat with the script specified.
			// When the chat is over, it will talk to the global flow machine. **I SHOULD MAKE SURE TO CALL THE FSM FROM THERE.**
			seq.OnComplete(new TweenCallback(delegate {
				GlobalFlowMachine.Instance.ExecuteFlowMacro(macro: this.boltMacro);
				/*Sequence seq2 = DOTween.Sequence();
				seq2.AppendCallback(new TweenCallback(delegate {
					GlobalFlowMachine.Instance.ExecuteFlowMacro(macro: this.boltMacro);
				}));
				seq2.AppendInterval(0.2f);
				seq2.AppendCallback(new TweenCallback(delegate {
					// ChatController.Open(script: this.chatScript, caller: GlobalFlowMachine.Instance.gameObject);
				}));
				seq2.Play();
	}));
			return seq;
		}
		#endregion
		 */
	}


}