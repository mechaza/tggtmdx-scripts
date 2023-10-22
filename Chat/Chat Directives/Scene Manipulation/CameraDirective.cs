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
using CameraTransitions;

namespace Grawly.Chat {

	/// <summary>
	/// Makes contact with the Camera Controller to toggle any given camera on/off for a cutscene.
	/// </summary>
	[Title("Camera")]
	public class CameraDirective : ChatDirective {

		#region FIELDS
		/// <summary>
		/// The camera to swap to.
		/// </summary>
		[SerializeField, LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private CameraTagType cameraToSwap = CameraTagType.One;
		/// <summary>
		/// Should a transition effect be used?
		/// </summary>
		[SerializeField, LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private bool useTransitionEffect = false;
		/// <summary>
		/// The effect to use for the transition, if one is in use.
		/// </summary>
		[SerializeField, ShowIf("useTransitionEffect"), LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private CameraTransitionEffects transitionEffect = CameraTransitionEffects.LinearBlur;
		/// <summary>
		/// The duration for the transition effect, if one is in use.
		/// </summary>
		[SerializeField, ShowIf("useTransitionEffect"), LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private float transitionDuration = 0.2f;
		#endregion

		#region CONSTRUCTORS
		public CameraDirective() {

		}
		public CameraDirective(ChatDirectiveParams directiveParams) {
			
			// Get the tag for the camera to swap.
			this.cameraToSwap = directiveParams.GetEnum<CameraTagType>(key: "camera");
			
			// Check if an effect should be used for this directive.
			this.useTransitionEffect = directiveParams.HasEnum<CameraTransitionEffects>(key: "transition");
			
			// If true, get any other required parameters.
			if (this.useTransitionEffect == true) {
				this.transitionEffect = directiveParams.GetEnum<CameraTransitionEffects>(key: "transition");
				this.transitionDuration = directiveParams.HasFloat(key: "time")
					? directiveParams.GetFloat(key: "time")
					: this.transitionDuration;
			}
		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {

			// For shits and giggles, check if the director is actually in the scene or not.
			if (CameraDirector.Instance == null) {
				throw new System.Exception("There is no CameraDirector instance in the scene!");
			}

			// Depending on if this flag was set, use a transition effect.
			if (this.useTransitionEffect == true) {
				CameraDirector.Instance.SwapCamera(
					cameraType: this.cameraToSwap, 
					transitionEffect: this.transitionEffect, 
					time: this.transitionDuration);
			} else {
				CameraDirector.Instance.SwapCamera(cameraType: this.cameraToSwap);
			}

			
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