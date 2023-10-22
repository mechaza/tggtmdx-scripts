using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using Grawly.Battle;

namespace Grawly.Chat {
	
	[Title("Start Battle")]
	public class StartBattleDirective : ChatDirective {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The BattleTemplate to start a battle with.
		/// </summary>
		[LabelWidth(DEFAULT_LABEL_WIDTH)]
		public BattleTemplate battleTemplate;
		#endregion
		
		#region CONSTRUCTORS
		public StartBattleDirective() {
			
		}
		public StartBattleDirective(ChatDirectiveParams directiveParams) {
			// Get the actual key that identifies the battle.
			string battleKey = directiveParams.GetValue(key: directiveParams.FirstLabel);
			// Retrieve it from the LocalVariablesController.
			this.battleTemplate = LocalVariablesController.Instance.GetBattleTemplate(key: battleKey);
		}
		#endregion
		
		#region DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			BattleController.Instance.StartBattle(battleTemplate: this.battleTemplate);
			chatController.EvaluateNextDirective();
		}
		#endregion
		
		
		#region ODIN HELPERS
		protected override string FoldoutGroupTitle {
			get {
				return this.GetType().FullName;
			}
		}
		#endregion

	}
	
}