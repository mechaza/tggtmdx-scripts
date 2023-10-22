using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using Grawly.Dungeon;

namespace Grawly.Chat {
	
	/// <summary>
	/// Plays a SFX in the chat.
	/// </summary>
	[Title("SFX")]
	public class SFXDirective : ChatDirective {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The type of SFX to play.
		/// </summary>
		[SerializeField, LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private SFXType SFXType = SFXType.None;
		/// <summary>
		/// The volume to play the sfx at.
		/// </summary>
		[SerializeField, LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private float volume = 0.6f;
		#endregion
		
		#region CONSTRUCTORS
		public SFXDirective() {
			
		}
		public SFXDirective(ChatDirectiveParams directiveParams) {
			
			this.SFXType = directiveParams.GetEnum<SFXType>(key: directiveParams.FirstLabel);
			
			this.volume = directiveParams.HasFloat(key: "volume")
				? directiveParams.GetFloat(key: "volume")
				: this.volume;
			
		}
		#endregion
		
		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			AudioController.instance?.PlaySFX(type: this.SFXType, scale: this.volume);
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