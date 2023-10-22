using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using Grawly.Battle.Equipment;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.Chat;
using Grawly.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Menus.BattleTerminal {
	
	/// <summary>
	/// The controller that manages the battle terminal screen.
	/// </summary>
	public class BattleTerminalController : MonoBehaviour {

		public static BattleTerminalController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The parameters currently in use for the Battle Terminal.
		/// </summary>
		public BattleTerminalParams CurrentTerminalParams { get; private set; }
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The menu list that allows selection of a template to play in battle.
		/// </summary>
		[SerializeField, TabGroup("Terminal", "Scene References")]
		private BattleTerminalMenuList battleTerminalMenuList;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Opens 
		/// </summary>
		/// <param name="battleTemplates"></param>
		public void Open(List<BattleTemplate> battleTemplates) {
			// Cascade down by making a new terminal params with the provided templates.
			this.Open(terminalParams: new BattleTerminalParams(battleTemplates: battleTemplates));
		}
		/// <summary>
		/// Opens the battle terminal with the parameters specified.
		/// </summary>
		/// <param name="terminalParams">The parameters to use for the battle terminal.</param>
		public void Open(BattleTerminalParams terminalParams) {
			
			// Reset the state of the terminal controller.
			this.ResetState();
			
			// Save the terminal parameters that were just passed in.
			this.CurrentTerminalParams = terminalParams;
			
			throw new NotImplementedException();
			
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the battle terminal controller.
		/// </summary>
		public void ResetState() {
			throw new NotImplementedException();
		}
		#endregion

		/*#region PRESENTATION
		/// <summary>
		/// Present the battle terminal with the parameters provided.
		/// </summary>
		/// <param name="terminalParams">The parameters that should be used in presenting the terminal.</param>
		private void Present(BattleTerminalParams terminalParams) {
			throw new NotImplementedException();
		}
		#endregion*/
		
	}
}