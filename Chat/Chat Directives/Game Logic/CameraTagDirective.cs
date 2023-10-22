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
	/// Sets the camera tag override stored in the GlobalFlagController.
	/// </summary>
	[Title("Camera Tag Override")]
	public class CameraTagDirective : ChatDirective {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The camera tag type to set as the override.
		/// </summary>
		[LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public CameraTagType cameraTagType = CameraTagType.None;
		#endregion

		#region CONSTRUCTORS
		public CameraTagDirective() {
			
		}
		public CameraTagDirective(ChatDirectiveParams directiveParams) {
			this.cameraTagType = directiveParams.GetEnum<CameraTagType>(key: "setcamtag");
		}
		#endregion
		
		#region DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			GlobalFlagController.Instance.SetCameraOverride(cameraTagType: this.cameraTagType);
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