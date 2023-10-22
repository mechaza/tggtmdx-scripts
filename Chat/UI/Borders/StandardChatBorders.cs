using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

namespace Grawly.Chat {

	/// <summary>
	/// The standard way in which ChatBorders should be assembled.
	/// </summary>
	public class StandardChatBorders : ChatBorders {

		#region FIELDS - TOGGLES : FLAGS
		/// <summary>
		/// Should these borders be reset on start?
		/// </summary>
		[Title("Flags")]
		[SerializeField, TabGroup("Borders", "Toggles")]
		private bool resetOnStart = true;
		#endregion
		
		#region FIELDS - TOGGLES : ANIMATION
		/// <summary>
		/// The amount of time to take when tweening in the borders.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Borders", "Toggles")]
		private float borderTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening out the borders.
		/// </summary>
		[SerializeField, TabGroup("Borders", "Toggles")]
		private float borderTweenOutTime = 0.5f;
		/// <summary>
		/// The easing to use when tweening the borders in.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Borders", "Toggles")]
		private Ease borderEaseInType = Ease.OutQuint;
		/// <summary>
		/// The easing to use when tweening the borders out.
		/// </summary>
		[SerializeField, TabGroup("Borders", "Toggles")]
		private Ease borderEaseOutType = Ease.InQuint;
		#endregion

		#region FIELDS - INITIAL POSITIONS
		/// <summary>
		/// The position of the top border when its hiding.
		/// </summary>
		[SerializeField, TabGroup("Borders", "Positions")]
		private Vector2 topBorderHidingPosition = new Vector2();
		/// <summary>
		/// The position of the top border when its active.
		/// </summary>
		[SerializeField, TabGroup("Borders", "Positions")]
		private Vector2 topBorderActivePosition = new Vector2();
		/// <summary>
		/// The position of the bottom border when its hiding.
		/// </summary>
		[SerializeField, TabGroup("Borders", "Positions")]
		private Vector2 bottomBorderHidingPosition = new Vector2();
		/// <summary>
		/// The position of the bottom border when its active.
		/// </summary>
		[SerializeField, TabGroup("Borders", "Positions")]
		private Vector2 bottomBorderActivePosition = new Vector2();
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The pivot for the top border.
		/// </summary>
		[Title("Pivots")]
		[SerializeField, TabGroup("Borders", "Scene References")]
		private RectTransform topBorderMainPivot;
		/// <summary>
		/// The pivot for the bottom border.
		/// </summary>
		[SerializeField, TabGroup("Borders", "Scene References")]
		private RectTransform bottomBorderMainPivot;
		/// <summary>
		/// The image for the top border.
		/// </summary>
		[Title("Images")]
		[SerializeField, TabGroup("Borders", "Scene References")]
		private Image topBorderImage;
		/// <summary>
		/// The image for the bottom border.
		/// </summary>
		[SerializeField, TabGroup("Borders", "Scene References")]
		private Image bottomBorderImage;
		#endregion

		#region UNITY CALLS
		private void Start() {
			if (this.resetOnStart == true) {
				this.ResetState();
				
			}
			
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Totally resets the chat borders to its initial values.
		/// </summary>
		public override void ResetState() {

			this.topBorderMainPivot.DOKill(complete: true);
			this.bottomBorderMainPivot.DOKill(complete: true);

			this.topBorderMainPivot.anchoredPosition = this.topBorderHidingPosition;
			this.bottomBorderMainPivot.anchoredPosition = this.bottomBorderHidingPosition;
		}
		/// <summary>
		/// Presents the borders onto the screen.
		/// </summary>
		/// <param name="borderParams">The parameters used in presenting the borders onto the screen.</param>
		public override void PresentBorders(ChatBorderParams borderParams) {
			Debug.Log("Presenting borders.");
			this.topBorderMainPivot.DOAnchorPos(
				endValue: this.topBorderActivePosition,
				duration: this.borderTweenInTime,
				snapping: true)
				.SetEase(ease: this.borderEaseInType);

			this.bottomBorderMainPivot.DOAnchorPos(
				endValue: this.bottomBorderActivePosition,
				duration: this.borderTweenInTime,
				snapping: true)
				.SetEase(ease: this.borderEaseInType);
		}
		/// <summary>
		/// Dismisses the borders off the screen.
		/// </summary>
		/// <param name="borderParams">The parameters used in dismissing the borders off the screen.</param>
		public override void DismissBorders(ChatBorderParams borderParams) {

			this.topBorderMainPivot.DOKill(complete: true);
			this.bottomBorderMainPivot.DOKill(complete: true);

			this.topBorderMainPivot.DOAnchorPos(
				endValue: this.topBorderHidingPosition,
				duration: this.borderTweenOutTime,
				snapping: true)
				.SetEase(ease: this.borderEaseOutType);

			this.bottomBorderMainPivot.DOAnchorPos(
				endValue: this.bottomBorderHidingPosition,
				duration: this.borderTweenOutTime,
				snapping: true)
				.SetEase(ease: this.borderEaseOutType);

		}
		#endregion

	}


}