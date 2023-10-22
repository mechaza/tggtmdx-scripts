using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// The graphic displaying the player whos weapon is currently being edited in the badge grid.
	/// </summary>
	public class BadgeGridCurrentPlayerHeadshot : MonoBehaviour {

		#region FIELDS - TWEENING : POSITIONS
		/// <summary>
		/// The starting pose for the player graphics rect transform.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField, Title("Positions")]
		private Vector2Int playerGraphicsStartPos = new Vector2Int();
		/// <summary>
		/// The starting pose for the persona graphics rect transform.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private Vector2Int personaGraphicsStartPos = new Vector2Int();
		/// <summary>
		/// The display pose for the player graphics rect transform.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private Vector2Int playerGraphicsDisplayPos = new Vector2Int();
		/// <summary>
		/// The display pose for the persona graphics rect transform.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private Vector2Int personaGraphicsDisplayPos = new Vector2Int();
		#endregion

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to wait before tweening the player graphics in.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField, Title("Timing")]
		private float playerGraphicsDelayInTime = 0.5f;
		/// <summary>
		/// The amount of time to wait before tweening the persona graphics in.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private float personaGraphicsDelayInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the player graphics in.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private float playerGraphicsTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the persona graphics in.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private float personaGraphicsTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the player graphics out.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private float playerGraphicsTweenOutTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the persona graphics out.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private float personaGraphicsTweenOutTime = 0.5f;
		#endregion

		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the player graphics in.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private Ease playerGraphicsEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the persona graphics in.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private Ease personaGraphicsEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the player graphics out.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private Ease playerGraphicsEaseOutType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the persona graphics out.
		/// </summary>
		[TabGroup("Headshot", "Tweening"), SerializeField]
		private Ease personaGraphicsEaseOutType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects that are children of this script.
		/// </summary>
		[TabGroup("Headshot", "Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The rect transform managing the position/rotation of all the player graphics.
		/// </summary>
		[TabGroup("Headshot", "Scene References"), SerializeField]
		private RectTransform playerGraphicsRectTransform;
		/// <summary>
		/// The RectTransform managing the position/rotation of all the persona graphics.
		/// </summary>
		[TabGroup("Headshot", "Scene References"), SerializeField]
		private RectTransform personaGraphicsRectTransform;
		/// <summary>
		/// The pivot that controls any movement for the player's headshot.
		/// </summary>
		[TabGroup("Headshot", "Scene References"), SerializeField]
		private RectTransform playerHeadshotMainPivot;
		/// <summary>
		/// The pivot that controls any movement for the persona's headshot.
		/// </summary>
		[TabGroup("Headshot", "Scene References"), SerializeField]
		private RectTransform personaHeadshotMainPivot;
		/// <summary>
		/// The image that displays the player's headshot graphic.
		/// </summary>
		[TabGroup("Headshot", "Scene References"), SerializeField]
		private Image playerHeadshotImage;
		/// <summary>
		/// The image that displays the persona's headshot graphic.
		/// </summary>
		[TabGroup("Headshot", "Scene References"), SerializeField]
		private Image personaHeadshotImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Kills all tweens and completes them.
		/// </summary>
		private void KillAllTweens() {
			// Kill Any outstanding tweens on the relevant objects.
			this.playerGraphicsRectTransform.DOKill(complete: true);
			this.personaGraphicsRectTransform.DOKill(complete: true);
			// this.personaHeadshotMainPivot.DOKill(complete: true);
			// this.playerHeadshotMainPivot.DOKill(complete: true);
			this.personaHeadshotImage.DOKill(complete: true);
			this.playerHeadshotImage.DOKill(complete: true);
		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			
			// Kill every single possible tween.
			this.KillAllTweens();
			
			// Snap the objects to their starting positions and state.
			this.playerGraphicsRectTransform.anchoredPosition = this.playerGraphicsStartPos;
			this.personaGraphicsRectTransform.anchoredPosition = this.personaGraphicsStartPos;
			
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the board controller using the data contained in the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters containing the information on how this board should be.</param>
		public void Present(BadgeBoardParams boardParams) {
			
			// Kill any tweens, just in case.
			this.KillAllTweens();
			
			// When preparing the headshot, just assign the sprites accordingly.
			this.playerHeadshotImage.overrideSprite = boardParams.CurrentPlayer.playerTemplate.badgeBoardHeadshotSprite;
			this.personaHeadshotImage.overrideSprite = boardParams.CurrentPlayer.ActivePersona.badgeBoardHeadshot;
			
			// Tween the graphics in appropriately.
			this.playerGraphicsRectTransform.DOAnchorPos(
					endValue: this.playerGraphicsDisplayPos, 
					duration: this.playerGraphicsTweenInTime, 
					snapping: true)
				.SetEase(ease: this.playerGraphicsEaseInType);
			this.personaGraphicsRectTransform.DOAnchorPos(
					endValue: this.personaGraphicsDisplayPos, 
					duration: this.personaGraphicsTweenInTime, 
					snapping: true)
				.SetEase(ease: this.personaGraphicsEaseInType);

		}
		/// <summary>
		/// Dismisses this element from the screen.
		/// </summary>
		/// <param name="boardParams">The board params that were used to create this object.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			
			// Kill any tweens, just in case.
			this.KillAllTweens();
			
			// Tween the graphics out appropriately.
			this.playerGraphicsRectTransform.DOAnchorPos(
					endValue: this.playerGraphicsStartPos, 
					duration: this.playerGraphicsTweenOutTime, 
					snapping: true)
				.SetEase(ease: this.playerGraphicsEaseOutType);
			this.personaGraphicsRectTransform.DOAnchorPos(
					endValue: this.personaGraphicsStartPos, 
					duration: this.personaGraphicsTweenOutTime, 
					snapping: true)
				.SetEase(ease: this.personaGraphicsEaseOutType);
			
		}
		#endregion
		
	}
}